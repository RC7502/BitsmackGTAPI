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


    }
}
