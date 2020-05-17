using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Leadmanagement.Api.Features.Leads;
using Leadmanagement.Tests.Infrastructure;
using Serilog;
using Xunit;

namespace Leadmanagement.Tests
{
    public class LeadsTests
    {
        [Fact]
        public async Task WhenCallingGetNewLeads_ShouldReturnLeads()
        {
            await using var server = new Server();
            server.Start();

            var response = await server.Client.GetAsync("/leads/new-leads");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var leads = await response.Content.ReadAsAsync<List<NewLeadDto>>();
            leads.Count.Should().Be(7);
        }

        [Fact]
        public async Task WhenCallingGetNewLeads_ShouldHaveCorrectData()
        {
            await using var server = new Server();
            server.Start();

            var response = await server.Client.GetAsync("/leads/new-leads");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var leads = await response.Content.ReadAsAsync<List<NewLeadDto>>();
            var firstLead = leads.FirstOrDefault(x => x.FirstName == "Luke");
            firstLead.Should().NotBeNull();
            firstLead.Description.Should().Be("Integer aliquam pulvinar odio et convallis. Integer id tristique ex. Aenean scelerisque massa vel est sollicitudin vulputate. Suspendisse quis ex eu ligula elementum suscipit nec a est. Aliquam a gravida diam. Donec placerat magna posuere massa maximus vehicula. Cras nisl ipsum, fermentum nec odio in, ultricies dapibus risus. Vivamus neque.");
            firstLead.Category.Should().Be("Plumbing");
            firstLead.Suburb.Should().Be("Sydney 2000");
            firstLead.Price.Should().Be(20);
        }
        [Fact]
        public async Task WhenCallingGetAcceptedLeads_ShouldReturnLeads()
        {
            await using var server = new Server();
            server.Start();

            var response = await server.Client.GetAsync("/leads/accepted-leads");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var leads = await response.Content.ReadAsAsync<List<AcceptedLeadDto>>();
            leads.Count.Should().Be(0);
        }

        [Fact]
        public async Task WhenDecliningLead_JobShouldNotBeAcceptedOrNew()
        {
            await using var server = new Server();
            server.Start();
            var jobId = 1;

            var response = await server.Client.PutAsync($"/leads/{jobId}?status={LeadStatus.Declined}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await server.Client.GetAsync("/leads/accepted-leads");
            var acceptedLeads = await response.Content.ReadAsAsync<List<AcceptedLeadDto>>();
            acceptedLeads.FirstOrDefault(x => x.JobId == jobId).Should().BeNull();

            response = await server.Client.GetAsync("/leads/new-leads");
            var newLeads = await response.Content.ReadAsAsync<List<NewLeadDto>>();
            newLeads.FirstOrDefault(x => x.JobId == jobId).Should().BeNull();
        }

        [Fact]
        public async Task WhenAcceptingLead_DatabaseShouldBeUpdatedAccordingly()
        {
            await using var server = new Server();
            server.Start();
            var jobId = 2;

            var response = await server.Client.PutAsync($"/leads/{jobId}?status={LeadStatus.Accepted}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await server.Client.GetAsync("/leads/accepted-leads");
            var acceptedLeads = await response.Content.ReadAsAsync<List<AcceptedLeadDto>>();
            var acceptedLeadDto = acceptedLeads.FirstOrDefault(x => x.JobId == jobId);
            acceptedLeadDto.Should().NotBeNull();
            acceptedLeadDto.Price.Should().Be(30);
        }

        [Fact]
        public async Task WhenAcceptingLeadWithPriceOver500_DatabaseShouldBeUpdatedAccordingly()
        {
            await using var server = new Server();
            server.Start();
            var jobId = 7;

            var response = await server.Client.PutAsync($"/leads/{jobId}?status={LeadStatus.Accepted}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await server.Client.GetAsync("/leads/accepted-leads");
            var acceptedLeads = await response.Content.ReadAsAsync<List<AcceptedLeadDto>>();
            var acceptedLeadDto = acceptedLeads.FirstOrDefault(x => x.JobId == jobId);
            acceptedLeadDto.Should().NotBeNull();
            acceptedLeadDto.Price.Should().Be(509);
        }

        [Fact]
        public async Task WhenAcceptingLead_EmailShouldBeSent()
        {
            var fakeLogger = A.Fake<ILogger>();
            await using var server = new Server(fakeLogger);
            server.Start();
            var leadsService = server.GetLeadsService();

            await leadsService.AcceptLead(1);

            A.CallTo(() => fakeLogger.Information("Email has been sent to {recipientEmails} for Job {JobId}", A<string>._, A<int>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}