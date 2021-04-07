using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using IntelliTect.Coalesce.DataAnnotations;

namespace TriviaGame.Data.Models
{
    [Create(PermissionLevel = SecurityPermissionLevels.DenyAll)]
    [Delete(PermissionLevel = SecurityPermissionLevels.DenyAll)]
    [Edit(PermissionLevel = SecurityPermissionLevels.DenyAll)]
    [Read(PermissionLevel = SecurityPermissionLevels.AllowAuthorized)]
    public class TriviaBoard
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TriviaBoardId { get; set; }

        public string Question { get; set; }

        public int TotalPoints { get; set; }

        public List<TriviaAnswer> Answers { get; set; }
    }
}