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
    public partial class TriviaBoardDtoGen : GeneratedDto<TriviaGame.Data.Models.TriviaBoard>
    {
        public TriviaBoardDtoGen() { }

        private int? _TriviaBoardId;
        private string _Question;
        private int? _TotalPoints;
        private System.Collections.Generic.List<TriviaGame.Web.Models.TriviaAnswerDtoGen> _Answers;

        public int? TriviaBoardId
        {
            get => _TriviaBoardId;
            set { _TriviaBoardId = value; Changed(nameof(TriviaBoardId)); }
        }
        public string Question
        {
            get => _Question;
            set { _Question = value; Changed(nameof(Question)); }
        }
        public int? TotalPoints
        {
            get => _TotalPoints;
            set { _TotalPoints = value; Changed(nameof(TotalPoints)); }
        }
        public System.Collections.Generic.List<TriviaGame.Web.Models.TriviaAnswerDtoGen> Answers
        {
            get => _Answers;
            set { _Answers = value; Changed(nameof(Answers)); }
        }

        /// <summary>
        /// Map from the domain object to the properties of the current DTO instance.
        /// </summary>
        public override void MapFrom(TriviaGame.Data.Models.TriviaBoard obj, IMappingContext context, IncludeTree tree = null)
        {
            if (obj == null) return;
            var includes = context.Includes;

            // Fill the properties of the object.

            this.TriviaBoardId = obj.TriviaBoardId;
            this.Question = obj.Question;
            this.TotalPoints = obj.TotalPoints;
            var propValAnswers = obj.Answers;
            if (propValAnswers != null)
            {
                this.Answers = propValAnswers
                    .Select(f => f.MapToDto<TriviaGame.Data.Models.TriviaAnswer, TriviaAnswerDtoGen>(context, tree?[nameof(this.Answers)])).ToList();
            }

        }

        /// <summary>
        /// Map from the current DTO instance to the domain object.
        /// </summary>
        public override void MapTo(TriviaGame.Data.Models.TriviaBoard entity, IMappingContext context)
        {
            var includes = context.Includes;

            if (OnUpdate(entity, context)) return;

            if (ShouldMapTo(nameof(TriviaBoardId))) entity.TriviaBoardId = (TriviaBoardId ?? entity.TriviaBoardId);
            if (ShouldMapTo(nameof(Question))) entity.Question = Question;
            if (ShouldMapTo(nameof(TotalPoints))) entity.TotalPoints = (TotalPoints ?? entity.TotalPoints);
        }
    }
}
