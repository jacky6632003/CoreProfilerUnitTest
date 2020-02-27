using AutoFixture;
using CoreProfilerUnitTest.Repository.Implement;
using CoreProfilerUnitTest.Repository.Models;
using CoreProfilerUnitTest.Repository2.Helper;
using CoreProfilerUnitTest.RepositoryTests.Misc;
using Dapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreProfilerUnitTest.RepositoryTests.Implement
{
    [TestClass]
    public class GuaranteeNumberRepositoryTests
    {
        private IDatabaseHelper _databaseHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            this._databaseHelper = Substitute.For<IDatabaseHelper>();
            var conn = new SqlConnection(TestHook.SampleDbConnectionString);
            _databaseHelper.GetConnection("").ReturnsForAnyArgs(conn);
        }

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            CreateTable();
            PrepareData();
        }

        private static void CreateTable()
        {
            using var conn = new SqlConnection(TestHook.SampleDbConnectionString);
            conn.Open();
            using var trans = conn.BeginTransaction();
            var filePath = PathHelper.ReplacePathCharacters(@"TestData\Create.sql");
            var script = File.ReadAllText(filePath);
            conn.Execute(sql: script, transaction: trans);
            trans.Commit();
        }

        private static void PrepareData()
        {
            using var conn = new SqlConnection(TestHook.SampleDbConnectionString);
            conn.Open();
            using var trans = conn.BeginTransaction();
            var filePath = PathHelper.ReplacePathCharacters(@"TestData\Data.sql");
            var script = File.ReadAllText(filePath);
            conn.Execute(sql: script, transaction: trans);
            trans.Commit();
        }

        [ClassCleanup]
        public static void TestClassCleanup()
        {
            using var conn = new SqlConnection(TestHook.SampleDbConnectionString);
            conn.Open();
            string sqlCommand = TableCommands.DropTable("GuaranteeBankData");
            conn.Execute(sqlCommand);
        }

        private StationRepository GetSystemUnderTest()
        {
            var sut = new StationRepository(this._databaseHelper);
            return sut;
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "GetRangeAsync")]
        public void GetRangeAsync_From輸入負1_應拋出ArgumentOutOfRangeException()
        {
            // arrange
            var from = -1;
            var size = 10;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.GetRangeAsync(from, size);

            // assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                  .Which.ParamName.Should().Be("from");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "GetRangeAsync")]
        public void GetRangeAsync_From輸入0_應拋出ArgumentOutOfRangeException()
        {
            // arrange
            var from = 0;
            var size = 10;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.GetRangeAsync(from, size);

            // assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                  .Which.ParamName.Should().Be("from");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "GetRangeAsync")]
        public void GetRangeAsync_Size輸入負1_應拋出ArgumentOutOfRangeException()
        {
            // arrange
            var from = 1;
            var size = -1;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.GetRangeAsync(from, size);

            // assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                  .Which.ParamName.Should().Be("size");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "GetRangeAsync")]
        public void GetRangeAsync_Size輸入0_應拋出ArgumentOutOfRangeException()
        {
            // arrange
            var from = 1;
            var size = 0;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.GetRangeAsync(from, size);

            // assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                  .Which.ParamName.Should().Be("size");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "GetRangeAsync")]
        public async Task GetRangeAsync_From輸入1_Size輸入10_資料庫裡無資料_應回傳空的集合()
        {
            // arrange
            var from = 1;
            var size = 10;

            var sut = this.GetSystemUnderTest();

            // act
            var actual = await sut.GetRangeAsync(from, size);

            // assert
            actual.Should().NotBeNull();
            actual.Any().Should().BeFalse();
        }

        //-----------------------------------------------------------------------------------------
        // GetTotalCountAsync

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "GetTotalCountAsync")]
        public async Task GetTotalCountAsync_資料庫沒有資料_應回傳0()
        {
            // arrange
            var sut = this.GetSystemUnderTest();

            // act
            var actual = await sut.GetTotalCountAsync();

            // assert
            actual.Should().Be(1);
        }

        //-----------------------------------------------------------------------------------------
        // QueryByAreaAsync

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "QueryByAreaAsync")]
        public void QueryByAreaAsync_area為null_應拋出ArgumentNullException()
        {
            // arrange
            string area = null;
            var from = 1;
            var size = 10;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.QueryByAreaAsync(area, from, size);

            // assert
            action.Should().Throw<ArgumentNullException>()
                  .Which.ParamName.Should().Be("area");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "QueryByAreaAsync")]
        public void QueryByAreaAsync_area為空白字串_應拋出ArgumentException()
        {
            // arrange
            var area = string.Empty;
            var from = 1;
            var size = 10;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.QueryByAreaAsync(area, from, size);

            // assert
            action.Should().Throw<ArgumentException>()
                  .Which.ParamName.Should().Be("area");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "QueryByAreaAsync")]
        public void QueryByAreaAsync_area為半形空格_應拋出ArgumentException()
        {
            // arrange
            var area = " ";
            var from = 1;
            var size = 10;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.QueryByAreaAsync(area, from, size);

            // assert
            action.Should().Throw<ArgumentException>()
                  .Which.ParamName.Should().Be("area");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "QueryByAreaAsync")]
        public void QueryByAreaAsync_area輸入test_From輸入負1_應拋出ArgumentOutOfRangeException()
        {
            // arrange
            var area = "test";
            var from = -1;
            var size = 10;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.QueryByAreaAsync(area, from, size);

            // assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                  .Which.ParamName.Should().Be("from");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "QueryByAreaAsync")]
        public void QueryByAreaAsync_area輸入test_From輸入0_應拋出ArgumentOutOfRangeException()
        {
            // arrange
            var area = "test";
            var from = 0;
            var size = 10;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.QueryByAreaAsync(area, from, size);

            // assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                  .Which.ParamName.Should().Be("from");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "QueryByAreaAsync")]
        public void QueryByAreaAsync_area輸入test_From輸入1_Size輸入負1_應拋出ArgumentOutOfRangeException()
        {
            // arrange
            var area = "test";
            var from = 1;
            var size = -1;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.QueryByAreaAsync(area, from, size);

            // assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                  .Which.ParamName.Should().Be("size");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "QueryByAreaAsync")]
        public void QueryByAreaAsync_area輸入test_From輸入1_Size輸入0_應拋出ArgumentOutOfRangeException()
        {
            // arrange
            var area = "test";
            var from = 1;
            var size = 0;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.QueryByAreaAsync(area, from, size);

            // assert
            action.Should().Throw<ArgumentOutOfRangeException>()
                  .Which.ParamName.Should().Be("size");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "QueryByAreaAsync")]
        public async Task QueryByAreaAsync_area輸入test_From輸入1_Size輸入10_資料庫裡無資料_應回傳空的集合()
        {
            // arrange
            var area = "test";
            var from = 1;
            var size = 10;

            var sut = this.GetSystemUnderTest();

            // act
            var actual = await sut.QueryByAreaAsync(area, from, size);

            // assert
            actual.Should().NotBeNull();
            actual.Any().Should().BeFalse();
        }

        //-----------------------------------------------------------------------------------------
        // GetCountByAreaAsync

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "GetCountByAreaAsync")]
        public void GetCountByAreaAsync_area為null_應拋出ArgumentNullException()
        {
            // arrange
            string area = null;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.GetCountByAreaAsync(area);

            // assert
            action.Should().Throw<ArgumentNullException>()
                  .Which.ParamName.Should().Be("area");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "GetCountByAreaAsync")]
        public void GetCountByAreaAsync_area為空白字串_應拋出ArgumentException()
        {
            // arrange
            var area = string.Empty;

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.GetCountByAreaAsync(area);

            // assert
            action.Should().Throw<ArgumentException>()
                  .Which.ParamName.Should().Be("area");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "GetCountByAreaAsync")]
        public void GetCountByAreaAsync_area為半形空格_應拋出ArgumentException()
        {
            // arrange
            var area = " ";

            var sut = this.GetSystemUnderTest();

            // act
            Func<Task> action = async () => await sut.GetCountByAreaAsync(area);

            // assert
            action.Should().Throw<ArgumentException>()
                  .Which.ParamName.Should().Be("area");
        }

        [TestMethod]
        [Owner("jacky")]
        [TestCategory("StationRepository")]
        [TestProperty("StationRepository", "GetCountByAreaAsync")]
        public async Task GetCountByAreaAsync_area輸入test_資料庫裡無資料_應回傳0()
        {
            // arrange
            var area = "test";

            var sut = this.GetSystemUnderTest();

            // act
            var actual = await sut.GetCountByAreaAsync(area);

            // assert
            actual.Should().Be(0);
        }
    }
}