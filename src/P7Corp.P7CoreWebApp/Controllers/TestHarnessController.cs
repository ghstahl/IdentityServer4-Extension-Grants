using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModelExtras;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P7Corp.P7CoreWebApp.Services;

namespace P7Corp.P7CoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestHarnessController : ControllerBase
    {
        private IDefaultHttpClientFactory _defaultHttpClientFactory;
        private HttpClient _httpClient;
        private HttpMessageHandler _httpMessageHandler;
        private ISomeTransient _someTransient;
        private ISomeScoped _someScoped;
        private ISomeSingleton _someSingleton;
        private Lazy<ISomeLazyTransient> _someLazyTransient;
        private Lazy<ISomeLazyScoped> _someLazyScoped;
        private Lazy<ISomeLazySingleton> _someLazySingleton;

        public TestHarnessController(
            ISomeTransient someTransient,
            Lazy<ISomeLazyTransient> someLazyTransient,
            ISomeScoped someScoped,
            Lazy<ISomeLazyScoped> someLazyScoped,
            ISomeSingleton someSingleton,
            Lazy<ISomeLazySingleton> someLazySingleton,
            IDefaultHttpClientFactory defaultHttpClientFactory)
        {
            _someTransient = someTransient;
            _someLazyTransient = someLazyTransient;
            _someScoped = someScoped;
            _someLazyScoped = someLazyScoped;
            _someSingleton = someSingleton;
            _someLazySingleton = someLazySingleton;

            _defaultHttpClientFactory = defaultHttpClientFactory;
            _httpClient = _defaultHttpClientFactory.HttpClient;
            _httpMessageHandler = _defaultHttpClientFactory.HttpMessageHandler;
        }


        // GET: api/TestHarness
        [HttpGet]
        [Route("index")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("LazyServicesNames")]
        public async Task<IEnumerable<string>> GetLazyServicesNamesAsync()
        {
            return new string[] {
                _someTransient.Name,
                _someLazyTransient.Value.Name,
                _someScoped.Name,
                _someLazyScoped.Value.Name,
                _someSingleton.Name,
                _someLazySingleton.Value.Name};
        }
    }
}
