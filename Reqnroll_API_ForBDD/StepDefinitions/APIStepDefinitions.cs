using api.Reqnroll;
using Reqnroll;
using Shared.Reqnroll;
using System;
using System.Net;
using Xunit;

namespace Reqnroll_API_ForBDD.StepDefinitions
{
    [Binding]
    public class APIStepDefinitions
    {
        private readonly HttpClient _client;
        private HttpResponseMessage _response = default!;

        public APIStepDefinitions ()
        {
            var factory = new ApiFactory ();
            _client = factory.CreateClient ();
        }

        [When("I call GET \\/req-test")]
        public async Task WhenICallGETReq_Test()
        {
            //throw new PendingStepException();
            _response = await _client.GetAsync ("api/account/req-test");
        }

        [Then("the response is Hello from ReqTest.")]
        public async Task ThenTheResponseIsHelloFromReqTest_()
        {
            //throw new PendingStepException();
            Assert.Equal (HttpStatusCode.OK, _response.StatusCode);
        }

    }
}
