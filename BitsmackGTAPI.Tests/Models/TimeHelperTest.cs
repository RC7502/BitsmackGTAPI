using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitsmackGTAPI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitsmackGTAPI.Tests.Models
{
    [TestClass]
    public class TimeHelperTest
    {
        [TestMethod]
        public void MinutesToTimeReturnsTime()
        {
            //Arrange
            const string expected = "02:07";

            //Act
            var result = TimeHelper.MinutesToHours(127);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SecondsToTimeReturnsTime()
        {
            //Arrange
            const string expected = "02:07:04";

            //Act
            var result = TimeHelper.SecondsToTime(7624);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }
    }
}
