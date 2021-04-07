
using IntelliTect.Coalesce;
using IntelliTect.Coalesce.Api;
using IntelliTect.Coalesce.Api.Controllers;
using IntelliTect.Coalesce.Api.DataSources;
using IntelliTect.Coalesce.Mapping;
using IntelliTect.Coalesce.Mapping.IncludeTrees;
using IntelliTect.Coalesce.Models;
using IntelliTect.Coalesce.TypeDefinition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TriviaGame.Web.Models;

namespace TriviaGame.Web.Api
{
    [Route("api/TriviaService")]
    [ServiceFilter(typeof(IApiActionFilter))]
    public partial class TriviaServiceController : Controller
    {
        protected TriviaGame.Data.Services.Impl.ITriviaService Service { get; }

        public TriviaServiceController(TriviaGame.Data.Services.Impl.ITriviaService service)
        {
            Service = service;
        }

        /// <summary>
        /// Method: GetRandomTriviaBoard
        /// </summary>
        [HttpPost("GetRandomTriviaBoard")]
        [Authorize]
        public virtual ItemResult<TriviaBoardDtoGen> GetRandomTriviaBoard()
        {
            IncludeTree includeTree = null;
            var methodResult = Service.GetRandomTriviaBoard();
            var result = new ItemResult<TriviaBoardDtoGen>();
            var mappingContext = new MappingContext(User, "");
            result.Object = Mapper.MapToDto<TriviaGame.Data.Models.TriviaBoard, TriviaBoardDtoGen>(methodResult, mappingContext, includeTree);
            return result;
        }

        /// <summary>
        /// Method: GetRandomTriviaBoardWithNoAnswers
        /// </summary>
        [HttpPost("GetRandomTriviaBoardWithNoAnswers")]
        [Authorize]
        public virtual ItemResult<TriviaBoardDtoGen> GetRandomTriviaBoardWithNoAnswers()
        {
            IncludeTree includeTree = null;
            var methodResult = Service.GetRandomTriviaBoardWithNoAnswers();
            var result = new ItemResult<TriviaBoardDtoGen>();
            var mappingContext = new MappingContext(User, "");
            result.Object = Mapper.MapToDto<TriviaGame.Data.Models.TriviaBoard, TriviaBoardDtoGen>(methodResult, mappingContext, includeTree);
            return result;
        }
    }
}
