using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using BitsmackGTAPI.Interfaces;
using BitsmackGTAPI.Models;
using BitsmackGTAPI.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitsmackGTAPI;
using BitsmackGTAPI.Controllers;
using Moq;

namespace BitsmackGTAPI.Tests.Controllers
{
    [TestClass]
    public class PedometerServiceTest
    {
        private readonly PedometerService _service; 
        private readonly Mock<IGTRepository<Pedometer>> _repo;
        private readonly Mock<ICommonService> _commonService;

        public PedometerServiceTest()
        {
            _repo = new Mock<IGTRepository<Pedometer>>();
            _commonService = new Mock<ICommonService>();
            _service = new PedometerService(_repo.Object, _commonService.Object);
        }

        [TestMethod]
        public void GetSummaryReturnsValidModel()
        {
            //Arrange
            var steps = new List<int>()
                {
                    2548, 2039, 2319, 4592, 1029
                };
            var expectedAvg = (int)Math.Round(steps.Average(), 0);
            var pedList = steps.Select(step => PedometerHelper.Create(steps: step)).ToList();
            _repo.Setup(x => x.AllForRead()).Returns(pedList.AsQueryable);

            //Act
            var result = _service.GetSummary();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedAvg, result.AverageSteps);
            Assert.AreEqual(steps.Count, result.NumOfDays);
        }
    }
}
