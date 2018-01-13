using BibaViewEngine.Constants;
using BibaViewEngine.Router;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace test
{
    public partial class IntegratedTest
    {
        private readonly IWebHost host;

        [Fact]
        public async void GetFirstEntryComponent_Success()
        {
            // Act
            var response = await client.GetAsync("app/start");
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<RouterResult>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(File.ReadAllText("FakeComponents/FaceAppComponent.html"), result.Html);
        }

        [Fact]
        public async void Get_NotExists_Route_Fail()
        {
            // Act
            var response = await client.GetAsync("c/");

            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(Constants.UNKNOWN_ROUTE, result);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void GetMainComponent()
        {
            // Act
            var response = await server.CreateRequest("c/main")
                .AddHeader("Referer", server.BaseAddress + "")
                .GetAsync();
            response.EnsureSuccessStatusCode();

            var template = File.ReadAllText("FakeComponents/FakeMainComponent.html");
            var result = await GetRouterResult(response);

            // Assert
            Assert.Equal(template, result.Html);
        }

        private async Task<RouterResult> GetRouterResult(HttpResponseMessage response) =>
            JsonConvert.DeserializeObject<RouterResult>(await response.Content.ReadAsStringAsync());
    }
}
