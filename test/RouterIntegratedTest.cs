using BibaViewEngine.Constants;
using BibaViewEngine.Exceptions;
using BibaViewEngine.Router;
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
        public async void GetMainComponent_Success()
        {
            // Act
            var response = await server.CreateRequest("c/main")
                .AddHeader("Referer", server.BaseAddress.AbsoluteUri)
                .GetAsync();
            response.EnsureSuccessStatusCode();

            var template = File.ReadAllText("FakeComponents/FakeMainComponent.html");
            var result = await GetRouterResult(response);

            // Assert
            Assert.Equal(template, result.Html);
        }

        [Fact]
        public async void GetComplexComponent_Success()
        {
            // Act
            var response = await server.CreateRequest("c/complex")
                .AddHeader("Referer", server.BaseAddress.AbsoluteUri)
                .GetAsync();
            response.EnsureSuccessStatusCode();

            var result = await GetRouterResult(response);

            // Assert
            Assert.Equal(complexTemplate, result.Html);
        }

        [Fact]
        public async void GetRouterData_Success() // TODO: Find better solution
        {
            // Act
            var response = await server.CreateRequest($"c/param/10")
                .AddHeader("Referer", server.BaseAddress.AbsoluteUri)
                .GetAsync();
            response.EnsureSuccessStatusCode();

            // Assert is not needed
        }

        [Fact]
        public void RootComponentNoContainer_Failed()
        {
            Assert.ThrowsAsync<RouterChildContainerNotExistsException>(async () =>
            {
                await server.CreateRequest($"c/bad-child/sub")
                    .AddHeader("Referer", server.BaseAddress.AbsoluteUri)
                    .GetAsync();
            });
        }

        [Fact]
        public async void FromRootToSubRoute_Success()
        {
            var response = await server.CreateRequest($"c/child/sub")
                .AddHeader("Referer", server.BaseAddress.AbsoluteUri)
                .GetAsync();
            response.EnsureSuccessStatusCode();

            var result = (await GetRouterResult(response)).Html;

            // Assert
            Assert.Equal(rootToSubTemplate, result);
        }

        private async Task<RouterResult> GetRouterResult(HttpResponseMessage response) =>
            JsonConvert.DeserializeObject<RouterResult>(await response.Content.ReadAsStringAsync());
    }
}
