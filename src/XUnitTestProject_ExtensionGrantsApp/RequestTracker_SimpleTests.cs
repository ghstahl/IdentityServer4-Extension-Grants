using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4Extras.Extensions;
using IdentityServerRequestTracker.Models;
using IdentityServerRequestTracker.Services;
using Shouldly;
using Xunit;
using IdentityServer4.Extensions;
using P7Core.Extensions;

namespace XUnitTestProject_ExtensionGrantsApp
{
    public class RequestTracker_SimpleTests
    {
        [Fact]
        public void Validate_AllowRequestTrackerResult()
        {
            var allowRequestTrackerResult = new AllowRequestTrackerResult();
            allowRequestTrackerResult.Directive.ShouldBe(RequestTrackerEvaluatorDirective.AllowRequest);
            allowRequestTrackerResult.ProcessAsync(null).ShouldBe(Task.CompletedTask);
        }
        [Fact]
        public void Validate_IdentityServerRequestRecord()
        {
            var allowRequestTrackerResult = new IdentityServerRequestRecord()
            {
                Client = null,
                EndpointKey = null,
                HttpContext = null
            };
            allowRequestTrackerResult.Client.ShouldBeNull();
            allowRequestTrackerResult.EndpointKey.ShouldBeNull();
            allowRequestTrackerResult.HttpContext.ShouldBeNull();
            

        }
    }
}
