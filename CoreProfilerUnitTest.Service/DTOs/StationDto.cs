using System;
using System.Collections.Generic;
using System.Text;

namespace CoreProfilerUnitTest.Service.DTOs
{
    /// <summary>
    /// class StationDto
    /// </summary>
    public class StationDto
    {
        /// <summary>
        /// 站點代號
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 場站名稱(中文)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 場站總停車格
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 場站目前車輛數量
        /// </summary>
        public int Bikes { get; set; }

        /// <summary>
        /// 場站區域(中文)
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 資料更新時間
        /// </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 緯度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 經度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 地址(中文)
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 場站區域(英文)
        /// </summary>
        public string AreaEnglish { get; set; }

        /// <summary>
        /// 場站名稱(英文)
        /// </summary>
        public string NameEnglish { get; set; }

        /// <summary>
        /// 地址(英文)
        /// </summary>
        public string AddressEnglish { get; set; }

        /// <summary>
        /// 空位數量
        /// </summary>
        public int BikeEmpty { get; set; }

        /// <summary>
        /// 禁用狀態 (false:禁用, true:正常)
        /// </summary>
        public bool Active { get; set; }
    }
}