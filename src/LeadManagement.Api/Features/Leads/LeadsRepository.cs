using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Leadmanagement.Api.Infrastructure.Database;

namespace Leadmanagement.Api.Features.Leads
{
    public class LeadsRepository
    {
        private readonly LeadManagementDatabase _db;

        public LeadsRepository(LeadManagementDatabase db)
        {
            _db = db;
        }
        
        public async Task<List<Lead>> GetLeads(string status)
        {
            await using var conn = await _db.CreateAndOpenConnection();

            var leads = await conn.QueryAsync<Lead>(@"
                SELECT j.id, j.description, j.contact_name, j.contact_phone, j.contact_email, j.price, j.created_at, 
                    c.name AS categoryName, s.name AS suburbName, s.postcode
	            FROM jobs j
                LEFT JOIN categories c ON c.id=j.category_id
                LEFT JOIN suburbs s ON s.id=j.suburb_id
                WHERE status like @Status", new { status });

            return leads.ToList();
        }

        public async Task<Job> GetJob(int id)
        {
            await using var conn = await _db.CreateAndOpenConnection();

            var job = await conn.QuerySingleOrDefaultAsync<Job>(@"
                SELECT id, status, suburb_id, category_id, contact_name, contact_phone, contact_email, price, description, 
                    created_at, updated_at                    
	            FROM jobs
                WHERE id = @Id", new { id });

            return job;
        }

        public async Task UpdateJob(Job job)
        {
            await using var conn = await _db.CreateAndOpenConnection();

            await conn.QueryFirstOrDefaultAsync<int>(@"
                UPDATE jobs
                SET status = @Status,
                    suburb_id = @SuburbId,
                    category_id = @CategoryId,
                    contact_name = @ContactName,
                    contact_phone = @ContactPhone,
                    contact_email = @ContactEmail,
                    price = @Price,
                    description = @Description,
                    updated_at = @UpdatedAt
                WHERE id=@Id", job);

        }
    }
}