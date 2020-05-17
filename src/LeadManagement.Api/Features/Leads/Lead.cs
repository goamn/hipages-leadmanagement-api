using System;

namespace Leadmanagement.Api.Features.Leads
{
    public class Lead
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = "";
        public string Description { get; set; } = "";
        public string ContactName { get; set; } = "";
        public string ContactPhone { get; set; } = "";
        public string ContactEmail { get; set; } = "";
        public string SuburbName { get; set; } = "";
        public string Postcode { get; set; } = "";
        public int Price { get; set; }      //TODO: change to decimal
        public DateTime CreatedAt { get; set; }
    }
}