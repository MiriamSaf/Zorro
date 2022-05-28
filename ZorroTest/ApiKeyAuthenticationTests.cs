using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Zorro.Api.Services;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace ZorroTest
{
    [TestClass]
    public class ApiKeyAuthenticationTests
    {
        private readonly Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _options;
        private readonly Mock<ILoggerFactory> _loggerFactory;
        private readonly Mock<UrlEncoder> _encoder;
        private readonly Mock<ISystemClock> _clock;
        private readonly ApplicationDbContext _context;
        private readonly ApiKeyAuthenticationHandler _handler;

        // initialize required components to test ApiKeyAuthenticationHandler
        public ApiKeyAuthenticationTests()
        {
            _options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();

            // This Setup is required for .NET Core 3.1 onwards.
            _options
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(new AuthenticationSchemeOptions());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "FakeDB")
                .Options;

            var logger = new Mock<ILogger<ApiKeyAuthenticationHandler>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();
            _context = new ApplicationDbContext(options);
            var user = new ApplicationUser()
            {
                FirstName = "Tommy",
                Surname = "Wiseau",
                NormalizedEmail = "TOMMY.WISEAU@GMAIL.COM",
                NormalizedUserName = "TOMMY.WISEAU@GMAIL.COM",
                APIKey = "d0a484f7-fbd6-45bd-b505-57f54b330acf"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            _handler = new ApiKeyAuthenticationHandler(_options.Object, _loggerFactory.Object, _encoder.Object, _clock.Object, _context);
        }

        // Test that authentication fails if an API key that doesn't belong to a user/merchant is supplied
        [TestMethod]
        public async Task TestInvalidApiKey()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("X-API-KEY", "46224798-ce29-4a45-b824-e70fec9b88ab");

            await _handler.InitializeAsync(new AuthenticationScheme("ApiKeyAuthentication", null, typeof(ApiKeyAuthenticationHandler)), context);
            var result = await _handler.AuthenticateAsync();

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Invalid Authorization Header", result.Failure.Message);
        }

        // Test that authentication success if a valid API key is supplied
        [TestMethod]
        public async Task TestValidApikey()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("X-API-KEY", "d0a484f7-fbd6-45bd-b505-57f54b330acf");

            await _handler.InitializeAsync(new AuthenticationScheme("ApiKeyAuthentication", null, typeof(ApiKeyAuthenticationHandler)), context);
            var result = await _handler.AuthenticateAsync();

            Assert.IsTrue(result.Succeeded);
        }
    }
}
