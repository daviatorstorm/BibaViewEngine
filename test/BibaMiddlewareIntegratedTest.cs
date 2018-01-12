using BibaViewEngine.Exceptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.IO;
using System.Net.Http;
using test.FaceMiscs;
using Xunit;

namespace test
{
    public class BibaMiddlewareIntegratedTest
    {
        TestServer server;
        HttpClient client;
        string path;
        string template = "<html><head><base href=\"/\"><script src=\"biba.min.js\"></script></head><body></body></html>\r\n";

        public BibaMiddlewareIntegratedTest()
        {
            server = new TestServer(WebHost.CreateDefaultBuilder()
                    .UseStartup<FakeEmptyStartup>());
            client = server.CreateClient();

            path = "wwwroot/index.html";
        }

        [Fact]
        public async void Returns_IndexHtml_Succsess()
        {
            // Prepare
            Directory.CreateDirectory("wwwroot");
            using (var file = new StreamWriter(File.Create(path)))
            {
                file.WriteLine("<html><body></body></html>");
            }

            // Act
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(template, responseString);

            File.Delete(path);
            Directory.Delete("wwwroot", true);
        }

        [Fact]
        public void IndexHtml_NotExists_Fail()
        {
            // Act
            Assert.Throws<IndexHtmlNotExistsException>(() =>
            {
                client.GetAsync("/").GetAwaiter().GetResult();
            });
        }
    }
}
