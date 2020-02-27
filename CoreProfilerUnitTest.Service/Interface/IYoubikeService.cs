using CoreProfilerUnitTest.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreProfilerUnitTest.Service.Interface
{
    public interface IYoubikeService
    {
        /// <summary>
        /// 取得所有的 YouBike 場站數量.
        /// </summary>
        /// <returns></returns>
        Task<int> GetTotalCountASync();

        /// <summary>
        /// 取得所有的 YouBike 場站資料.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<StationDto>> GetAllStationsAsync();

        /// <summary>
        /// 取得指定區域的場站數量.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        Task<int> GetCountByAreaAsync(string area);

        /// <summary>
        /// 取得指定區域、數量、範圍的 YouBike 場站資料.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="from">From.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        Task<IEnumerable<StationDto>> GetStationsAsync(string area, int from, int size);
    }
}