using System;
using BitsmackGTAPI.Interfaces;
using BitsmackGTAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BitsmackGTAPI.Tests.Models
{
    [TestClass]
    public class DashboardServiceTest
    {
        private readonly DashboardService _service;
        private readonly Mock<IDAL> _dal;
        private readonly Mock<IPedometerService> _pedometerService;
        private readonly Mock<ITodoService> _todoService;

        public DashboardServiceTest()
        {
            _dal = new Mock<IDAL>();
            _pedometerService = new Mock<IPedometerService>();
            _todoService = new Mock<ITodoService>();
            _service = new DashboardService(_pedometerService.Object, _dal.Object, _todoService.Object);
        }

        [TestMethod]
        public void CalBurnedPerMinuteReturnsPositive()
        {
            //Arrange
            var startRec = new Pedometer
                {
                    trandate = new DateTime(2014, 8, 1),
                    weight = 170
                };
            var mostCurrent = new Pedometer
                {
                    trandate = new DateTime(2014, 8, 10),
                    weight = 160
                };
            const int calConsumed = (2000*10);


            //Act
            var result = _service.CalBurnedPerMinute(startRec, mostCurrent, calConsumed);

            //Assert
            Assert.AreEqual((55000d/12960d), result);

        }

    }
}
