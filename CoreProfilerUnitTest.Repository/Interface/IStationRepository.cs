using CoreProfilerUnitTest.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreProfilerUnitTest.Repository.Interface
{
    public interface IStationRepository
    {
        /// <summary>
        /// 取得指定範圍的 Station 資料.
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        Task<IEnumerable<StationModel>> GetRangeAsync(int from, int size);

        /// <summary>
        /// 取得全部 station 資料數量.
        /// </summary>
        /// <returns></returns>
        Task<int> GetTotalCountAsync();

        /// <summary>
        /// 以 area 查詢並取得 staion 資料.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="from">From.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        Task<IEnumerable<StationModel>> QueryByAreaAsync(string area, int from, int size);

        /// <summary>
        /// 以 area 查詢並取得 station 資料筆數.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        Task<int> GetCountByAreaAsync(string area);
    }
}