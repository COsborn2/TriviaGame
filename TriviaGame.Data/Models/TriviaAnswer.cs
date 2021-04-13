using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using IntelliTect.Coalesce.DataAnnotations;
using TriviaGame.Data.Services.Impl;

namespace TriviaGame.Data.Models
{
    [Create(PermissionLevel = SecurityPermissionLevels.DenyAll)]
    [Delete(PermissionLevel = SecurityPermissionLevels.DenyAll)]
    [Edit(PermissionLevel = SecurityPermissionLevels.DenyAll)]
    [Read(PermissionLevel = SecurityPermissionLevels.AllowAuthorized)]
    public class TriviaAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TriviaAnswerId { get; set; }

        public string Answer { get; set; }

        public int Points { get; set; }

        [JsonIgnore]
        public TriviaBoard TriviaBoard { get; set; }

        [NotMapped]
        public int Position { get; set; }

        [NotMapped]
        public Team WonBy { get; set; }
    }
}
