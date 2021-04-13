using IntelliTect.Coalesce;
using IntelliTect.Coalesce.Mapping;
using IntelliTect.Coalesce.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TriviaGame.Web.Models
{
    public partial class TriviaAnswerDtoGen : GeneratedDto<TriviaGame.Data.Models.TriviaAnswer>
    {
        public TriviaAnswerDtoGen() { }

        private int? _TriviaAnswerId;
        private string _Answer;
        private int? _Points;
        private TriviaGame.Web.Models.TriviaBoardDtoGen _TriviaBoard;
        private int? _Position;
        private TriviaGame.Data.Services.Impl.Team? _WonBy;

        public int? TriviaAnswerId
        {
            get => _TriviaAnswerId;
            set { _TriviaAnswerId = value; Changed(nameof(TriviaAnswerId)); }
        }
        public string Answer
        {
            get => _Answer;
            set { _Answer = value; Changed(nameof(Answer)); }
        }
        public int? Points
        {
            get => _Points;
            set { _Points = value; Changed(nameof(Points)); }
        }
        public TriviaGame.Web.Models.TriviaBoardDtoGen TriviaBoard
        {
            get => _TriviaBoard;
            set { _TriviaBoard = value; Changed(nameof(TriviaBoard)); }
        }
        public int? Position
        {
            get => _Position;
            set { _Position = value; Changed(nameof(Position)); }
        }
        public TriviaGame.Data.Services.Impl.Team? WonBy
        {
            get => _WonBy;
            set { _WonBy = value; Changed(nameof(WonBy)); }
        }

        /// <summary>
        /// Map from the domain object to the properties of the current DTO instance.
        /// </summary>
        public override void MapFrom(TriviaGame.Data.Models.TriviaAnswer obj, IMappingContext context, IncludeTree tree = null)
        {
            if (obj == null) return;
            var includes = context.Includes;

            // Fill the properties of the object.

            this.TriviaAnswerId = obj.TriviaAnswerId;
            this.Answer = obj.Answer;
            this.Points = obj.Points;
            this.Position = obj.Position;
            this.WonBy = obj.WonBy;
            if (tree == null || tree[nameof(this.TriviaBoard)] != null)
                this.TriviaBoard = obj.TriviaBoard.MapToDto<TriviaGame.Data.Models.TriviaBoard, TriviaBoardDtoGen>(context, tree?[nameof(this.TriviaBoard)]);

        }

        /// <summary>
        /// Map from the current DTO instance to the domain object.
        /// </summary>
        public override void MapTo(TriviaGame.Data.Models.TriviaAnswer entity, IMappingContext context)
        {
            var includes = context.Includes;

            if (OnUpdate(entity, context)) return;

            if (ShouldMapTo(nameof(TriviaAnswerId))) entity.TriviaAnswerId = (TriviaAnswerId ?? entity.TriviaAnswerId);
            if (ShouldMapTo(nameof(Answer))) entity.Answer = Answer;
            if (ShouldMapTo(nameof(Points))) entity.Points = (Points ?? entity.Points);
            if (ShouldMapTo(nameof(Position))) entity.Position = (Position ?? entity.Position);
            if (ShouldMapTo(nameof(WonBy))) entity.WonBy = (WonBy ?? entity.WonBy);
        }
    }
}
