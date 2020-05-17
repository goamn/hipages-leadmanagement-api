using Leadmanagement.Api.Features.Common;
using Leadmanagement.Api.Infrastructure.Database;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.StringComparison;

namespace Leadmanagement.Api.Features.Leads
{
    public class LeadsService
    {
        private readonly LeadsRepository _leadsRepo;
        private readonly EmailService _emailService;
        private readonly ILogger _logger;
        private readonly LeadManagementDatabase _db;

        public LeadsService(LeadsRepository leadsRepository, EmailService emailService, ILogger logger, LeadManagementDatabase leadManagementDatabase)
        {
            _leadsRepo = leadsRepository;
            _emailService = emailService;
            _logger = logger;
            _db = leadManagementDatabase;
        }

        public async Task<List<NewLeadDto>> GetNewLeads()
        {
            var leads = await _leadsRepo.GetLeads(LeadStatus.New);
            var newLeads = leads.Select(s => new NewLeadDto
            {
                FirstName = s.ContactName.Split(" ").FirstOrDefault() ?? "",
                DateCreated = s.CreatedAt,
                Suburb = $"{s.SuburbName} {s.Postcode}",
                Category = s.CategoryName,
                JobId = s.Id,
                Description = s.Description,
                Price = s.Price
            }).ToList();
            return newLeads;
        }

        public async Task<List<AcceptedLeadDto>> GetAcceptedLeads()
        {
            var leads = await _leadsRepo.GetLeads(LeadStatus.Accepted);
            var acceptedLeads = leads.Select(s => new AcceptedLeadDto
            {
                FullName = s.ContactName,
                DateCreated = s.CreatedAt,
                Suburb = $"{s.SuburbName} {s.Postcode}",
                Category = s.CategoryName,
                JobId = s.Id,
                Description = s.Description,
                Price = s.Price,
                Phone = s.ContactPhone,
                Email = s.ContactEmail
            }).ToList();
            return acceptedLeads;
        }

        public void PopulateWithTestData()
        {
            _db.UpgradeWithTestScripts();
        }

        public async Task AcceptLead(int id)
        {
            var job = await _leadsRepo.GetJob(id);
            if (job.Status.Equals(LeadStatus.Accepted, OrdinalIgnoreCase))
            {
                _logger.Warning("Job with {jobId} could not be accepted as it was in an invalid state.", id);
                throw new InvalidOperationException();
            }
            job.Status = LeadStatus.Accepted;
            job.UpdatedAt = DateTime.UtcNow;
            if (job.Price > 500)
            {
                job.Price = Convert.ToInt32(Math.Ceiling(job.Price * 0.9));
            }
            await _leadsRepo.UpdateJob(job);
            _emailService.SendEmail(job.Id);
        }

        public async Task DeclineLead(int id)
        {
            var job = await _leadsRepo.GetJob(id);
            if (job.Status.Equals(LeadStatus.Declined, OrdinalIgnoreCase))
            {
                _logger.Warning("Job with {jobId} could not be declined as it was in an invalid state.", id);
                throw new InvalidOperationException();
            }
            job.Status = LeadStatus.Declined;
            job.UpdatedAt = DateTime.UtcNow;
            await _leadsRepo.UpdateJob(job);
        }
    }

    public class LeadStatus
    {
        public static string New = "new";
        public static string Accepted = "accepted";
        public static string Declined = "declined";
    }
}