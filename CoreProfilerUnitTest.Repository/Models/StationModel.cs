using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreProfilerUnitTest.Repository.Models
{
    /// <summary>
    /// class StationModel
    /// </summary>
    public class StationModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StationModel"/> class.
        /// </summary>
        public StationModel()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// The id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// sno 站點代號
        /// </summary>
        [MaxLength(10)]
        public string StationNo { get; set; }

        /// <summary>
        /// sna 場站名稱(中文)
        /// </summary>
        [MaxLength(50)]
        public string StationName { get; set; }

        /// <summary>
        /// tot 場站總停車格
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// sbi 場站目前車輛數量
        /// </summary>
        public int StationBikes { get; set; }

        /// <summary>
        /// sarea 場站區域(中文)
        /// </summary>
        [MaxLength(10)]
        public string StationArea { get; set; }

        /// <summary>
        /// mday 資料更新時間
        /// </summary>
        [MaxLength(20)]
        public string ModifyTime { get; set; }

        /// <summary>
        /// latitude 緯度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// longitude 經度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// ar 地址(中文)
        /// </summary>
        [MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// sareaen 場站區域(英文)
        /// </summary>
        [MaxLength(50)]
        public string StationAreaEnglish { get; set; }

        /// <summary>
        /// snaen 場站名稱(英文)
        /// </summary>
        [MaxLength(100)]
        public string StationNameEnglish { get; set; }

        /// <summary>
        /// aren 地址(英文)
        /// </summary>
        [MaxLength(500)]
        public string AddressEnglish { get; set; }

        /// <summary>
        /// bemp 空位數量
        /// </summary>
        public int BikeEmpty { get; set; }

        /// <summary>
        /// act 禁用狀態 (false:禁用, true:正常)
        /// </summary>
        public bool Active { get; set; }
    }
}