using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModelExtras;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace P7Corp.P7CoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestHarnessController : ControllerBase
    {
        private IDefaultHttpClientFactory _defaultHttpClientFactory;
        private HttpClient _httpClient;
        private HttpMessageHandler _httpMessageHandler;

        public TestHarnessController(IDefaultHttpClientFactory defaultHttpClientFactory)
        {
            _defaultHttpClientFactory = defaultHttpClientFactory;
            _httpClient = _defaultHttpClientFactory.HttpClient;
            _httpMessageHandler = _defaultHttpClientFactory.HttpMessageHandler;
        }


        // GET: api/TestHarness
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

    }
}
