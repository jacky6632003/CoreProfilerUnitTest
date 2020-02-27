using System;
using System.Linq;
using System.Threading.Tasks;
using CoreProfilerUnitTest.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using CoreProfilerUnitTest.Models;
using CoreProfilerUnitTest.Infrastructure.ActionFilters;

namespace Sample.WebApplication.Controllers
{
    /// <summary>
    /// class HomeController
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("api/[controller]")]
    public class StationController : ControllerBase
    {
        private readonly IYoubikeService _youbikeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StationController"/> class.
        /// </summary>
        /// <param name="youbikeService">The youbike service.</param>
        public StationController(IYoubikeService youbikeService)
        {
            this._youbikeService = youbikeService;
        }

        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// 取得全部場站資料
        /// </summary>
        /// <returns></returns>
        [CoreProfilingAttrbute]
        [HttpGet("All")]
        [Consumes("application/json")]
        [Produces("application/json", "text/json")]
        [ProducesResponseType(200, Type = typeof(StationOutputModel))]
        public async Task<IActionResult> GetAllAsync()
        {
            var totalCount = await this._youbikeService.GetTotalCountASync();

            var stations = await this._youbikeService.GetAllStationsAsync();

            return this.Ok(new StationOutputModel
            {
                Total = totalCount,
                From = 1,
                Size = totalCount,
                Stations = stations.ToList().OrderByDescending(x => x.ModifyTime).AsEnumerable()
            });
        }

        /// <summary>
        /// 取得指定區域的場站資料
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="from">From.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        [CoreProfilingAttrbute]
        [HttpGet("ByArea")]
        [Consumes("application/json")]
        [Produces("application/json", "text/json")]
        [ProducesResponseType(200, Type = typeof(StationOutputModel))]
        public async Task<IActionResult> GetByAreaAsync(string area, int from = 1, int size = 10)
        {
            if (string.IsNullOrWhiteSpace(area))
            {
                area = "中正區";
            }

            from = from <= 0 ? 1 : from;
            size = size <= 0 ? 10 : size;

            var totalCount = await this._youbikeService.GetCountByAreaAsync(area);

            var stations = await this._youbikeService.GetStationsAsync(area, from, size);

            return this.Ok(new StationOutputModel
            {
                Total = totalCount,
                From = from,
                Size = size,
                Stations = stations.ToList().OrderByDescending(x => x.ModifyTime).AsEnumerable()
            });
        }
    }
}