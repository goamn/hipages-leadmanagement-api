using System;

namespace Leadmanagement.Api.Features.Leads
{
    public class NewLeadDto
    {
        public long JobId { get; set; }
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string Suburb { get; set; } = "";
        public int Price { get; set; }
        public DateTime DateCreated { get; set; }
    }
}