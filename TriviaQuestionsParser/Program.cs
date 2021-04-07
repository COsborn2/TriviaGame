using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TriviaGame.Data;
using TriviaGame.Data.Models;

namespace TriviaQuestionsParser
{
    public static class Program
    {
        public static void Main()
        {
            const string baseUrl = "";
            var seasonsNewFormat = new List<string> {"20", "19", "18"};
            var numberOfAnswers = new List<string> {"8", "7", "6", "5", "4"};

            foreach (var season in seasonsNewFormat)
            {
                Console.WriteLine($"Saving Season '{season}'");
                foreach (var numberOfAnswer in numberOfAnswers)
                {
                    Console.WriteLine($"Saving answer count '{numberOfAnswer}'");

                    var builtHtmlString = string.Join(
                        '/', 
                        new List<string> {baseUrl, $"feud{season}_{numberOfAnswer}.html"});
                    
                    DownloadPageAndSaveToDatabase(builtHtmlString);
                }
            }
        }

        private static void DownloadPageAndSaveToDatabase(string url)
        {
            HtmlWeb web = new HtmlWeb();

            Console.WriteLine($"Navigating to URL: '{url}'");
            var htmlDoc = web.Load(url);

            var nodes = htmlDoc.DocumentNode
                .SelectNodes("//body/center");

            var groupedBoards = nodes
                .Select((x, i) => new {x, i})
                .GroupBy(g => g.i / 3);

            var triviaBoards = new List<TriviaBoard>();

            foreach (var groupedBoard in groupedBoards)
            {
                var board = groupedBoard.ToList();

                if (board.Count != 3) continue;

                var boardSubject = GetTitle(board[0].x);

                var boardPoints = GetBoardPointTotal(board[1].x);

                var triviaAnswers = GetTriviaAnswers(board[2].x).ToList();
                
                triviaBoards.Add(new TriviaBoard
                {
                    Question = boardSubject,
                    TotalPoints = boardPoints,
                    Answers = triviaAnswers
                });
            }

            Console.WriteLine($"Total Count: {triviaBoards.Count}");

            var index = url.LastIndexOf('/') + 1;
            var fileName = $"{url.Substring(index, url.Length - index)}.json";
            
            File.WriteAllText(fileName, JsonSerializer.Serialize(triviaBoards));
            
            SubmitChangesToDatabase(triviaBoards);
        }

        private static void SubmitChangesToDatabase(ICollection<TriviaBoard> triviaBoards)
        {
            Console.WriteLine("Writing to Database");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>();
            var res = dbContextOptions.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            using var context = new AppDbContext(res.Options);

            var alreadySavedBoards = context.TriviaBoards
                .Select(x => x.Question)
                .Where(x => triviaBoards.Select(xi => xi.Question).Contains(x));
            Console.WriteLine($"Found '{alreadySavedBoards.Count()}' records already in database");
            var unsavedBoards = triviaBoards.Where(x => 
                !alreadySavedBoards.Contains(x.Question))
                .ToList();
            
            Console.WriteLine($"Saving '{unsavedBoards.Count}' records to database");
            context.AddRange(unsavedBoards);
            context.SaveChanges();
            
            Console.WriteLine("Records saved");
        }

        private static string GetTitle(HtmlNode node)
        {
            return node.Descendants("font")
                .First()
                .InnerText
                .Replace("&nbsp;", "");
        }

        private static int GetBoardPointTotal(HtmlNode node)
        {
            return int.Parse(node.InnerText.Trim());
        }

        private static ICollection<TriviaAnswer> GetTriviaAnswers(HtmlNode node)
        {
            var rows = node.Descendants("tr").Skip(1);

            var triviaAnswers = new List<TriviaAnswer>();

            foreach (var htmlNode in rows)
            {
                // get all tds from row
                var tds = htmlNode.Descendants("td")
                    .Select((x, y) => new {x, y})
                    .GroupBy(g => g.y / 2)
                    .Where(x => x.Count() == 2);

                var parsedAnswers = tds.Select(td => new TriviaAnswer
                {
                    Answer = td.First().x.InnerText.Trim(),
                    Points = int.Parse(td.Last().x.InnerText.Trim())
                });

                triviaAnswers.AddRange(parsedAnswers);
            }

            return triviaAnswers;
        }
    }
}