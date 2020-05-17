using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Leadmanagement.Tests.Infrastructure;
using Xunit;

namespace Leadmanagement.Tests
{
    public class SwaggerTests
    {
        [Fact]
        public async Task Can_Generate_Swagger()
        {
            await using var server = new Server();
            server.Start();

            var response = await server.Client.GetAsync("/swagger/v1/swagger.json");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}