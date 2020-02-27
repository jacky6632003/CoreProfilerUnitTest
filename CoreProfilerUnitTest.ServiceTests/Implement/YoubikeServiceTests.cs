using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CoreProfilerUnitTest.Repository.Interface;
using CoreProfilerUnitTest.Repository.Models;
using CoreProfilerUnitTest.Service.Implement;
using CoreProfilerUnitTest.ServiceTests;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CoreProfilerUnitTest.Service.Implement.Tests
{
    [TestClass]
    public class YoubikeServiceTests
    {
        private IMapper _mapper;

        private IStationRepository _stationRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            this._mapper = TestHook.MapperConfigurationProvider.CreateMapper();
            this._stationRepository = Substitute.For<IStationRepository>();
        }

        private YoubikeService GetSystemUnderTest()
        {
            var sut = new YoubikeService(this._mapper, this._stationRepository);
            return sut;
        }

        //-----------------------------------------------------------------------------------------
        // GetTotalCountASync

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetTotalCountASync")]
        public async Task GetTotalCountASync_無資料_應回傳0()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            this._stationRepository.GetTotalCountAsync().Returns(0);

            // act
            var actual = await sut.GetTotalCountASync();

            // assert
            actual.Should().Be(0);
        }

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetTotalCountASync")]
        public async Task GetTotalCountASync_有100筆資料_應回傳100()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            this._stationRepository.GetTotalCountAsync().Returns(100);

            // act
            var actual = await sut.GetTotalCountASync();

            // assert
            actual.Should().Be(100);
        }

        //-----------------------------------------------------------------------------------------
        // GetAllStationsAsync

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetAllStationsAsync")]
        public async Task GetAllStationsAsync_有資料_無快取_取得全部的場站資料()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            var fixture = new Fixture();
            var stations = fixture.Build<StationModel>()
                                  .With(x => x.ModifyTime, "")
                                  .CreateMany(100)
                                  .ToList();

            this._stationRepository.GetTotalCountAsync().Returns(100);

            this._stationRepository
                .GetRangeAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(stations);

            // act
            var actual = await sut.GetAllStationsAsync();

            // assert
            actual.Should().NotBeNull();
            actual.Any().Should().BeTrue();
            actual.Should().HaveCount(100);
        }

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetAllStationsAsync")]
        public async Task GetAllStationsAsync_無資料_應回傳空的資料集合()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            this._stationRepository.GetTotalCountAsync().Returns(0);

            // act
            var actual = await sut.GetAllStationsAsync();

            // assert
            actual.Should().NotBeNull();
            actual.Any().Should().BeFalse();
            actual.Should().HaveCount(0);

            await this._stationRepository.DidNotReceive().GetRangeAsync(Arg.Any<int>(), Arg.Any<int>());
        }

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetAllStationsAsync")]
        public async Task GetAllStationsAsync_有200筆資料_StationRepository的Gets方法會呼叫兩次_應回傳資料集合()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            this._stationRepository.GetTotalCountAsync().Returns(200);

            var fixture = new Fixture();

            var stationCollecion1 = fixture.Build<StationModel>()
                                           .With(x => x.ModifyTime, "")
                                           .CreateMany(100)
                                           .ToList();

            var stationCollecion2 = fixture.Build<StationModel>()
                                           .With(x => x.ModifyTime, "")
                                           .CreateMany(100)
                                           .ToList();

            this._stationRepository
                .GetRangeAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(stationCollecion1, stationCollecion2);

            // act
            var actual = await sut.GetAllStationsAsync();

            // assert
            actual.Should().NotBeNull();
            actual.Any().Should().BeTrue();
            actual.Should().HaveCount(200);

            await this._stationRepository.Received(2).GetRangeAsync(Arg.Any<int>(), Arg.Any<int>());
        }

        //-----------------------------------------------------------------------------------------
        // GetCountByAreaAsync

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetCountByAreaAsync")]
        public async Task GetCountByAreaAsync_area為null_應回傳0()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            string area = null;

            // act
            var actual = await sut.GetCountByAreaAsync(area);

            // assert
            actual.Should().Be(0);
        }

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetCountByAreaAsync")]
        public async Task GetCountByAreaAsync_area為空白_應回傳0()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            string area = string.Empty;

            // act
            var actual = await sut.GetCountByAreaAsync(area);

            // assert
            actual.Should().Be(0);
        }

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetCountByAreaAsync")]
        public async Task GetCountByAreaAsync_area輸入楠梓區_沒有符合的資料_應回傳0()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            string area = "楠梓區";

            this._stationRepository
                .GetCountByAreaAsync(Arg.Any<string>())
                .Returns(0);

            // act
            var actual = await sut.GetCountByAreaAsync(area);

            // assert
            actual.Should().Be(0);
        }

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetCountByAreaAsync")]
        public async Task GetCountByAreaAsync_area輸入北投區_符合資料筆數為40筆_應回傳40()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            string area = "北投區";

            this._stationRepository
                .GetCountByAreaAsync(Arg.Any<string>())
                .Returns(40);

            // act
            var actual = await sut.GetCountByAreaAsync(area);

            // assert
            actual.Should().Be(40);
        }

        //-----------------------------------------------------------------------------------------
        // GetStationsAsync

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetStationsAsync")]
        public async Task GetStationsAsync_area為null_應回傳空的資料集合()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            string area = null;
            var from = 0;
            var size = 100;

            // act
            var actual = await sut.GetStationsAsync(area, from, size);

            // assert
            actual.Should().NotBeNull();
            actual.Any().Should().BeFalse();
            actual.Should().HaveCount(0);
        }

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetStationsAsync")]
        public async Task GetStationsAsync_area為空白_應回傳空的資料集合()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            var area = "";
            var from = 0;
            var size = 100;

            // act
            var actual = await sut.GetStationsAsync(area, from, size);

            // assert
            actual.Should().NotBeNull();
            actual.Any().Should().BeFalse();
            actual.Should().HaveCount(0);
        }

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetStationsAsync")]
        public async Task GetStationsAsync_area為test_符合area為test的資料數量為0_應回傳空的資料集合()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            this._stationRepository.GetCountByAreaAsync(Arg.Any<string>())
                .Returns(0);

            var area = "test";
            var from = 0;
            var size = 100;

            // act
            var actual = await sut.GetStationsAsync(area, from, size);

            // assert
            actual.Should().NotBeNull();
            actual.Any().Should().BeFalse();
            actual.Should().HaveCount(0);

            await this._stationRepository.DidNotReceive()
                      .QueryByAreaAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>());
        }

        [TestMethod]
        [Owner("Kevin")]
        [TestCategory("YoubikeService")]
        [TestProperty("YoubikeService", "GetStationsAsync")]
        public async Task GetStationsAsync_area為test_應回傳area為test的資料集合()
        {
            // arrange
            var fixture = new Fixture();

            var stations = fixture.Build<StationModel>()
                                  .With(x => x.ModifyTime, "")
                                  .With(x => x.StationArea, "test")
                                  .CreateMany(10)
                                  .ToList();

            this._stationRepository.QueryByAreaAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(stations);

            this._stationRepository.GetCountByAreaAsync(Arg.Any<string>())
                .Returns(10);

            var sut = this.GetSystemUnderTest();

            var area = "test";
            var from = 0;
            var size = 100;

            // act
            var actual = await sut.GetStationsAsync(area, from, size);

            // assert
            actual.Should().NotBeNull();
            actual.Any().Should().BeTrue();
            actual.Should().HaveCount(10);

            actual.All(x => x.Area.Equals("test", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }
    }
}