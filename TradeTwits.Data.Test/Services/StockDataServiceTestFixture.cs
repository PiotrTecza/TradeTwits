using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System.Collections.Generic;
using System.Threading;
using TradeTwits.Data.Models;
using TradeTwits.Data.Services;

namespace TradeTwits.Data.Test.Services
{
    [TestFixture]
    public class StockDataServiceTestFixture
    {
        private IFixture fixture;
        private Mock<IMongoCollection<StockDataModel>> stockDataMock;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            fixture = new Fixture().Customize(new AutoMoqCustomization());

            stockDataMock = fixture.Freeze<Mock<IMongoCollection<StockDataModel>>>();
        }

        [TestFixture]
        public class InsertManyTestFixture : StockDataServiceTestFixture
        {
            private List<StockDataModel> stockDataList;

            [SetUp]
            public void SetUp()
            {
                stockDataList = fixture.Create<List<StockDataModel>>();
            }

            [TearDown]
            public void TearDown()
            {
                stockDataMock.Reset();
            }

            [Test]
            public void InsertMany_WhenCalled_InsertsData()
            {
                //Arrange
                var subject = fixture.Create<StockDataService>();

                //Act
                subject.InsertMany(stockDataList);

                //Assert
                stockDataMock.Verify(x => x.InsertMany(stockDataList, null, It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
