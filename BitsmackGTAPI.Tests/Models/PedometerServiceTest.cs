using System;
using BitsmackGTAPI.Interfaces;
using BitsmackGTAPI.Models;
using BitsmackGTAPI.Tests.Helpers;
using Fitbit.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BitsmackGTAPI.Tests.Models
{
    [TestClass]
    public class PedometerServiceTest
    {
        private readonly PedometerService _service;
        private readonly Mock<IDAL> _dal;
        private readonly Mock<ICommonService> _commonService;
        private readonly Mock<IFitbitClient> _fitbitClient;

        public PedometerServiceTest()
        {
            _dal = new Mock<IDAL>();
            _commonService = new Mock<ICommonService>();
            _fitbitClient = new Mock<IFitbitClient>();
            _service = new PedometerService(_commonService.Object, _dal.Object, 
                _fitbitClient.Object);
        }

        [TestMethod]
        public void GetFitbitDataReturnsList()
        {
            //Arrange
            var key = CommonHelper.CreateAPIKey();
            var start = DateTime.UtcNow.AddDays(-14);
            var end = DateTime.UtcNow.Date;

            _fitbitClient.Setup(x=>x.GetWeight(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(PedometerHelper.CreateWeight());

            //Act

            //Assert
        }
    }
}
