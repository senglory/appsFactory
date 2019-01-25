using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using svc.Controllers;
using NetCoreVueTypeScript.Dto;



namespace svc.MSUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private WeatherDataController _controller;
        private IMemoryCache _cache;

        [TestInitialize]
        public void TestSetup()
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            var services = new ServiceCollection();
            //services.AddLogging();
            services.AddMvc().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            });
            services.AddOptions();

            //services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.AddSingleton<IConfiguration>(configuration);
            ////services.AddTransient<IEmailSender, EmailSender>();

            //var options = Options.Create(configuration.GetSection("AppSettings").Get<AppSettings>());
            //var serviceProvider = services.BuildServiceProvider();
            //var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            //loggerFactory.AddConsole(configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            //loggerFactory.AddTestsLogger(_testLogsStore);

            //var userManagerLogger = loggerFactory.CreateLogger<UserManager<ApplicationUser>>();

            var cache = new MemoryCache(new MemoryCacheOptions());
            _cache = cache;

            var mockHostingEnvironment = new Mock<IHostingEnvironment>(MockBehavior.Strict);


            var mockHttpContext = new Mock<HttpContext>(MockBehavior.Strict);
            //mockHttpContext.SetupGet(hc => hc.User).Returns(validPrincipal);
            //mockHttpContext.SetupGet(c => c.Items).Returns(_httpContextItems);
            mockHttpContext.SetupGet(ctx => ctx.RequestServices)
                //.Returns(serviceProvider)
                ;

            var collection = Mock.Of<IFormCollection>();
            var request = new Mock<HttpRequest>();
            request.Setup(f => f.ReadFormAsync(CancellationToken.None)).Returns(Task.FromResult(collection));

            var mockHeader = new Mock<IHeaderDictionary>();
            mockHeader.Setup(h => h["X-Requested-With"]).Returns("XMLHttpRequest");
            request.SetupGet(r => r.Headers).Returns(mockHeader.Object);

            mockHttpContext.SetupGet(c => c.Request).Returns(request.Object);

            var response = new Mock<HttpResponse>();
            response.SetupProperty(it => it.StatusCode);

            mockHttpContext.Setup(c => c.Response).Returns(response.Object);

            _controller = new WeatherDataController(_cache);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };
        }


        [TestMethod]
        public async Task TestWebApiAccessibility()
        {
            var cityName = "Berlin";
            var res1 = await _controller.WeatherForecasts(cityName);
            Assert.IsInstanceOfType(res1, typeof(Microsoft.AspNetCore.Mvc.JsonResult));
            var resval = res1 as Microsoft.AspNetCore.Mvc.JsonResult;
            Assert.IsInstanceOfType(resval.Value , typeof(WeatherDto));
            var weatherDto = resval.Value as WeatherDto;
            Assert.IsTrue(weatherDto.City == cityName);
            Assert.IsTrue(weatherDto.Readings.Count > 0);
        }

        [TestMethod]
        public async Task TestNumberOfDays()
        {
            var cityName = "Berlin";
            var res1 = await _controller.WeatherForecasts(cityName);
            var resval = res1 as Microsoft.AspNetCore.Mvc.JsonResult;
            var weatherDto = resval.Value as WeatherDto;

            var diffDates = new HashSet<DateTime>();
            foreach( var reading in weatherDto.Readings)
            {
                var dt = DateTime.ParseExact(reading.DateFormatted,"dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                if (!diffDates.Contains(dt.Date))
                    diffDates.Add(dt.Date);
            }
            Assert.IsTrue(diffDates.Count == 6 || diffDates.Count == 5);
        }

        [TestMethod]
        public async Task TestWebApi_BadCityName()
        {
            var cityName = "qqqq";
            var res1 = await _controller.WeatherForecasts(cityName);
            Assert.IsInstanceOfType(res1, typeof(Microsoft.AspNetCore.Mvc.BadRequestObjectResult));
        }
    }
}
