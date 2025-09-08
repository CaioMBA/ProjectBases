using App.Controllers;
using Bogus;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;

namespace Tests.App
{
    public class ExampleControllerTests
    {
        private static ExampleController CreateController(Domain.Utils utils, Mock<IExampleService> serviceMock, string ip, string userAgent)
        {
            var controller = new ExampleController(utils, serviceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Connection = { RemoteIpAddress = IPAddress.Parse(ip) }
                    }
                }
            };
            controller.HttpContext.Request.Headers["User-Agent"] = userAgent;
            return controller;
        }

        [Fact]
        public async Task Get_ForwardsParametersToService()
        {
            var faker = new Faker();
            var ip = faker.Internet.Ip();
            var ua = faker.Internet.UserAgent();

            var settings = new AppSettingsModel
            {
                DataBaseConnections = [],
                ApiConnections = []
            };

            var optionsMonitor = Mock.Of<IOptionsMonitor<AppSettingsModel>>(m => m.CurrentValue == settings);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var logger = NullLogger<Domain.Utils>.Instance;

            var utils = new Domain.Utils(optionsMonitor, cache, logger);

            var expected = new DefaultResponseModel { StatusCode = 200 };
            var service = new Mock<IExampleService>();

            service.Setup(s => s.Example()).Returns(Task.FromResult(expected));

            var controller = CreateController(utils, service, ip, ua);

            var result = await controller.Get();

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(expected.StatusCode, obj.StatusCode);
            Assert.Equal(expected, obj.Value);
            service.Verify(s => s.Example(), Times.Once);
        }
    }
}
