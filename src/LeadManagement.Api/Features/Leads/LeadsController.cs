using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.StringComparison;

namespace Leadmanagement.Api.Features.Leads
{
    [Route("[controller]")]
    [ApiController]
    public class LeadsController : ControllerBase
    {
        private readonly LeadsService _leadsService;

        public LeadsController(LeadsService leadsService)
        {
            _leadsService = leadsService;
        }

        /// <summary>
        /// Gets all available new leads (jobs) for tradie.
        /// </summary>
        [HttpGet("new-leads")]
        [ProducesResponseType(typeof(List<NewLeadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNewLeads()
        {
            return Ok(await _leadsService.GetNewLeads());
        }

        /// <summary>
        /// Gets all accepted new leads (jobs) for tradie.
        /// </summary>
        [HttpGet("accepted-leads")]
        [ProducesResponseType(typeof(List<AcceptedLeadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAcceptedLeads()
        {
            return Ok(await _leadsService.GetAcceptedLeads());
        }

        /// <summary>
        /// Populates database with test data.
        /// </summary>
        [HttpPost("test-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PopulateWithTestData()
        {
            _leadsService.PopulateWithTestData();
            return Ok();
        }

        /// <summary>
        /// Updates a deal depending on acceptance or declination.
        /// </summary>
        [HttpPut, HttpPost]      //TODO: remove HttpPost after fixing axios.Put(..)
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLead([FromRoute]int id, [FromQuery]string status)
        {
            try
            {
                if (status.Equals(LeadStatus.Accepted, OrdinalIgnoreCase))
                {
                    await _leadsService.AcceptLead(id);
                }
                else if (status.Equals(LeadStatus.Declined, OrdinalIgnoreCase))
                {
                    await _leadsService.DeclineLead(id);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}