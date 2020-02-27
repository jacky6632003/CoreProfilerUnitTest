using CoreProfilerUnitTest.Service.DTOs;
using System;
using System.Collections.Generic;

namespace CoreProfilerUnitTest.Models
{
    /// <summary>
    /// class StationOutputModel
    /// </summary>
    public class StationOutputModel
    {
        /// <summary>
        /// 資料總數
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 資料起始
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// 取得數量
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 場站資料
        /// </summary>
        public IEnumerable<StationDto> Stations { get; set; }
    }
}