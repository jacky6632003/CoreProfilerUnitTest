using CoreProfiler;
using CoreProfilerUnitTest.Common.CoreProfile;
using CoreProfilerUnitTest.Common.Extensions;
using CoreProfilerUnitTest.Repository.Interface;
using CoreProfilerUnitTest.Repository.Models;
using CoreProfilerUnitTest.Repository2.Helper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreProfilerUnitTest.Repository.Implement
{
    public class StationRepository : IStationRepository
    {
        private readonly IDatabaseHelper _databaseHelper;

        public StationRepository(IDatabaseHelper databaseHelper)
        {
            this._databaseHelper = databaseHelper;
        }

        /// <summary>
        /// 取得指定範圍的 Station 資料.
        /// </summary>
        /// <param name="from">from</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        [CoreProfile("GetRangeAsync")]
        public async Task<IEnumerable<StationModel>> GetRangeAsync(int @from, int size)
        {
            var stepName = $"{nameof(StationRepository)}.{nameof(this.GetRangeAsync)}";
            using (ProfilingSession.Current.Step(stepName))
            {
                var sqlCommand = new StringBuilder();
                sqlCommand.AppendLine(@"SELECT");
                sqlCommand.AppendLine(@"	[Id]");
                sqlCommand.AppendLine(@"   ,[StationNo]");
                sqlCommand.AppendLine(@"   ,[StationName]");
                sqlCommand.AppendLine(@"   ,[Total]");
                sqlCommand.AppendLine(@"   ,[StationBikes]");
                sqlCommand.AppendLine(@"   ,[StationArea]");
                sqlCommand.AppendLine(@"   ,[ModifyTime]");
                sqlCommand.AppendLine(@"   ,[Latitude]");
                sqlCommand.AppendLine(@"   ,[Longitude]");
                sqlCommand.AppendLine(@"   ,[Address]");
                sqlCommand.AppendLine(@"   ,[StationAreaEnglish]");
                sqlCommand.AppendLine(@"   ,[StationNameEnglish]");
                sqlCommand.AppendLine(@"   ,[AddressEnglish]");
                sqlCommand.AppendLine(@"   ,[BikeEmpty]");
                sqlCommand.AppendLine(@"   ,[Active]");
                sqlCommand.AppendLine(@"FROM [dbo].[YoubikeStation]");
                sqlCommand.AppendLine(@"ORDER BY StationNo ASC ");
                sqlCommand.AppendLine(@"  OFFSET @OFFSET ROWS ");
                sqlCommand.AppendLine(@"  FETCH NEXT @FETCH ROWS only; ");

                var pageSize = size < 0 || size > 100 ? 100 : size;
                var start = @from <= 0 ? 1 : @from;

                using (var conn = (_databaseHelper.GetMySQLConnection(this._databaseHelper.WLDOConnectionString)))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("OFFSET", start - 1);
                    parameters.Add("FETCH", pageSize);
                    var query = await conn.QueryAsync<StationModel>(
                        sql: sqlCommand.ToString(),
                        param: parameters
                    );
                    var models = query.Any() ? query : Enumerable.Empty<StationModel>();
                    return models;
                }
            }
        }

        /// <summary>
        /// 取得全部 station 資料數量.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetTotalCountAsync()
        {
            var stepName = $"{nameof(StationRepository)}.{nameof(this.GetTotalCountAsync)}";
            using (ProfilingSession.Current.Step(stepName))
            {
                var sqlCommand = " SELECT count(p.Id) FROM [YoubikeStation] p WITH (NOLOCK) ";

                using (var conn = (_databaseHelper.GetMySQLConnection(this._databaseHelper.WLDOConnectionString)))
                {
                    var queryResult = await conn.QueryFirstOrDefaultAsync<int>(
                        sql: sqlCommand.ToString()

                    );

                    return queryResult;
                }
            }
        }

        /// <summary>
        /// 以 area 查詢並取得 staion 資料.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="from">From.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public async Task<IEnumerable<StationModel>> QueryByAreaAsync(string area, int @from, int size)
        {
            var stepName = $"{nameof(StationRepository)}.{nameof(this.QueryByAreaAsync)}";
            using (ProfilingSession.Current.Step(stepName))
            {
                var sqlCommand = new StringBuilder();
                sqlCommand.AppendLine(@"SELECT");
                sqlCommand.AppendLine(@"	[Id]");
                sqlCommand.AppendLine(@"   ,[StationNo]");
                sqlCommand.AppendLine(@"   ,[StationName]");
                sqlCommand.AppendLine(@"   ,[Total]");
                sqlCommand.AppendLine(@"   ,[StationBikes]");
                sqlCommand.AppendLine(@"   ,[StationArea]");
                sqlCommand.AppendLine(@"   ,[ModifyTime]");
                sqlCommand.AppendLine(@"   ,[Latitude]");
                sqlCommand.AppendLine(@"   ,[Longitude]");
                sqlCommand.AppendLine(@"   ,[Address]");
                sqlCommand.AppendLine(@"   ,[StationAreaEnglish]");
                sqlCommand.AppendLine(@"   ,[StationNameEnglish]");
                sqlCommand.AppendLine(@"   ,[AddressEnglish]");
                sqlCommand.AppendLine(@"   ,[BikeEmpty]");
                sqlCommand.AppendLine(@"   ,[Active]");
                sqlCommand.AppendLine(@"FROM [dbo].[YoubikeStation]");
                sqlCommand.AppendLine(@"where StationArea = @StationArea ");
                sqlCommand.AppendLine(@"ORDER BY StationNo ASC ");
                sqlCommand.AppendLine(@"  OFFSET @OFFSET ROWS ");
                sqlCommand.AppendLine(@"  FETCH NEXT @FETCH ROWS only; ");

                var pageSize = size < 0 || size > 100 ? 100 : size;
                var start = @from <= 0 ? 1 : @from;

                using (var conn = (_databaseHelper.GetMySQLConnection(this._databaseHelper.WLDOConnectionString)))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("StationArea", area.Truncate(10).ToNVarchar());
                    parameters.Add("OFFSET", start - 1);
                    parameters.Add("FETCH", pageSize);
                    var query = await conn.QueryAsync<StationModel>(
                        sql: sqlCommand.ToString(),
                        param: parameters
                    );
                    var models = query.Any() ? query : Enumerable.Empty<StationModel>();
                    return models;
                }
            }
        }

        /// <summary>
        /// 以 area 查詢並取得 station 資料筆數.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        public async Task<int> GetCountByAreaAsync(string area)
        {
            var stepName = $"{nameof(StationRepository)}.{nameof(this.GetCountByAreaAsync)}";
            using (ProfilingSession.Current.Step(stepName))
            {
                var sqlCommand = @"
                SELECT count(p.Id) FROM [YoubikeStation] p WITH (NOLOCK)
                Where p.StationArea = @StationArea
                ";

                using (var conn = (_databaseHelper.GetMySQLConnection(this._databaseHelper.WLDOConnectionString)))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("StationArea", area.Truncate(10).ToNVarchar());
                    var queryResult = await conn.QueryFirstOrDefaultAsync<int>(
                        sql: sqlCommand.ToString()

                    );

                    return queryResult;
                }
            }
        }
    }
}