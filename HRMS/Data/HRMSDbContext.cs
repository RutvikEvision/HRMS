using Microsoft.EntityFrameworkCore;

namespace HRMS.Data
{
    public class HRMSDbContext : DbContext
    {
        public HRMSDbContext(DbContextOptions<HRMSDbContext> options) : base(options)
        {
        }
    }
}
