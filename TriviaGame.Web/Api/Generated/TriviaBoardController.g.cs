
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
    [Route("api/TriviaBoard")]
    [Authorize]
    [ServiceFilter(typeof(IApiActionFilter))]
    public partial class TriviaBoardController
        : BaseApiController<TriviaGame.Data.Models.TriviaBoard, TriviaBoardDtoGen, TriviaGame.Data.AppDbContext>
    {
        public TriviaBoardController(TriviaGame.Data.AppDbContext db) : base(db)
        {
            GeneratedForClassViewModel = ReflectionRepository.Global.GetClassViewModel<TriviaGame.Data.Models.TriviaBoard>();
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public virtual Task<ItemResult<TriviaBoardDtoGen>> Get(
            int id,
            DataSourceParameters parameters,
            IDataSource<TriviaGame.Data.Models.TriviaBoard> dataSource)
            => GetImplementation(id, parameters, dataSource);

        [HttpGet("list")]
        [Authorize]
        public virtual Task<ListResult<TriviaBoardDtoGen>> List(
            ListParameters parameters,
            IDataSource<TriviaGame.Data.Models.TriviaBoard> dataSource)
            => ListImplementation(parameters, dataSource);

        [HttpGet("count")]
        [Authorize]
        public virtual Task<ItemResult<int>> Count(
            FilterParameters parameters,
            IDataSource<TriviaGame.Data.Models.TriviaBoard> dataSource)
            => CountImplementation(parameters, dataSource);

        /// <summary>
        /// Downloads CSV of TriviaBoardDtoGen
        /// </summary>
        [HttpGet("csvDownload")]
        [Authorize]
        public virtual Task<FileResult> CsvDownload(
            ListParameters parameters,
            IDataSource<TriviaGame.Data.Models.TriviaBoard> dataSource)
            => CsvDownloadImplementation(parameters, dataSource);

        /// <summary>
        /// Returns CSV text of TriviaBoardDtoGen
        /// </summary>
        [HttpGet("csvText")]
        [Authorize]
        public virtual Task<string> CsvText(
            ListParameters parameters,
            IDataSource<TriviaGame.Data.Models.TriviaBoard> dataSource)
            => CsvTextImplementation(parameters, dataSource);

        // Methods from data class exposed through API Controller.
    }
}
