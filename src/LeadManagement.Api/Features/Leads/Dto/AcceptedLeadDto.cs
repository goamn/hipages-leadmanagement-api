using System;

namespace Leadmanagement.Api.Features.Leads
{
    public class AcceptedLeadDto
    {
        public long JobId { get; set; }
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Suburb { get; set; } = "";
        public int Price { get; set; }
        public DateTime DateCreated { get; set; }
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
    }
}