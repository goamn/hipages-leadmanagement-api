using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leadmanagement.Api.Features.Common
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public EmailService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void SendEmail(int id)
        {
            var recipientEmails = _configuration["acceptanceRecipients"];
            //Send email here
            _logger.Information("Email has been sent to {recipientEmails} for Job {JobId}", recipientEmails, id);
        }
    }
}