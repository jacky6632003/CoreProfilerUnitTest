using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreProfiler;
using CoreProfilerUnitTest.Repository.Interface;
using CoreProfilerUnitTest.Repository.Models;
using CoreProfilerUnitTest.Service.DTOs;
using CoreProfilerUnitTest.Service.Interface;

namespace CoreProfilerUnitTest.Service.Implement
{
    /// <summary>
    /// class YoubikeService
    /// </summary>
    /// <seealso cref="Sample.Service.Interface.IYoubikeService" />
    public class YoubikeService : IYoubikeService
    {
        private readonly IMapper _mapper;

        private readonly IStationRepository _stationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="YoubikeService"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="stationRepository">The station repository.</param>
        public YoubikeService(IMapper mapper,
                              IStationRepository stationRepository)
        {
            this._mapper = mapper;
            this._stationRepository = stationRepository;
        }

        /// <summary>
        /// 取得所有的 YouBike 場站數量.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetTotalCountASync()
        {
            var stepName = $"{nameof(YoubikeService)}.{nameof(this.GetTotalCountASync)}";
            using (ProfilingSession.Current.Step(stepName))
            {
                var stationAmount = await this._stationRepository.GetTotalCountAsync();
                return stationAmount;
            }
        }

        /// <summary>
        /// 取得所有的 YouBike 場站資料.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<StationDto>> GetAllStationsAsync()
        {
            var stepName = $"{nameof(YoubikeService)}.{nameof(this.GetAllStationsAsync)}";
            using (ProfilingSession.Current.Step(stepName))
            {
                var stationAmount = await this._stationRepository.GetTotalCountAsync();

                if (stationAmount.Equals(0))
                {
                    return Enumerable.Empty<StationDto>();
                }

                var from = 1;
                var size = 100;

                var result = new List<StationDto>();

                while (from < stationAmount)
                {
                    var source = await this._stationRepository.GetRangeAsync(from, size);
                    var stations = this._mapper.Map<IEnumerable<StationModel>, IEnumerable<StationDto>>(source);
                    result.AddRange(stations);
                    from += size;
                }

                return result;
            }
        }

        /// <summary>
        /// 取得指定區域的場站數量.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        public async Task<int> GetCountByAreaAsync(string area)
        {
            var stepName = $"{nameof(YoubikeService)}.{nameof(this.GetCountByAreaAsync)}";
            using (ProfilingSession.Current.Step(stepName))
            {
                if (string.IsNullOrWhiteSpace(area))
                {
                    return 0;
                }

                var amount = await this._stationRepository.GetCountByAreaAsync(area);
                return amount;
            }
        }

        /// <summary>
        /// 取得指定區域、數量、範圍的 YouBike 場站資料.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="from">From.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public async Task<IEnumerable<StationDto>> GetStationsAsync(string area, int @from, int size)
        {
            var stepName = $"{nameof(YoubikeService)}.{nameof(this.GetStationsAsync)}";
            using (ProfilingSession.Current.Step(stepName))
            {
                if (string.IsNullOrWhiteSpace(area))
                {
                    return Enumerable.Empty<StationDto>();
                }

                var amount = await this._stationRepository.GetCountByAreaAsync(area);

                if (amount.Equals(0))
                {
                    return Enumerable.Empty<StationDto>();
                }

                var source = await this._stationRepository.QueryByAreaAsync(area, from, size);

                var stations = this._mapper.Map<IEnumerable<StationModel>, IEnumerable<StationDto>>(source);
                return stations;
            }
        }
    }
}