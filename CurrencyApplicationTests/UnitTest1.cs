using CurrencyApplication.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CurrencyApplicationTests
{
    [TestClass]
    public class UnitTest1
    {
        private CurrencyController currency;

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var webHostEnvironment = serviceProvider.GetService<IWebHostEnvironment>();
            var logger = loggerFactory.CreateLogger<CurrencyController>();
            currency = new CurrencyController(logger, webHostEnvironment);
        }

        [TestMethod]
        public void IndexView()
        {
            ViewResult result = currency.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void OutputData()
        {
            try
            {
                var result = currency.GetOutputData();

                if (!(result == null || result.Count > 0))
                {
                    Assert.Fail();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [TestMethod]
        public void InputData()
        {
            try
            {
                var result = currency.GetInputData();

                if (!(result == null || result.Count > 0))
                {
                    Assert.Fail();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
