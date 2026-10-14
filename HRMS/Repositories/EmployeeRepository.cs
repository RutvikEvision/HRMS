using Dapper;
using HRMS.Data;
using HRMS.Data.Entities;
using HRMS.DTOs;
using HRMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly HRMSDbContext _context;
        private readonly IDbConnectionFactory _connectionFactory;

        public EmployeeRepository(HRMSDbContext context, IDbConnectionFactory connectionFactory)
        {
            _context = context;
            _connectionFactory = connectionFactory;
        }

        //Used By HR
        public async Task<List<Employee>> GetEmployeesCreatedByHrAsync(Guid organizationId, Guid hrUserId)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e =>
                    e.OrganizationId == organizationId &&
                    e.CreatedByUserId == hrUserId)
                .OrderBy(e => e.CreatedAtUtc)
                .ToListAsync();
        }

        //To Get One Employee
        public async Task<Employee?> GetEmployeeCreatedByHrAsync(Guid organizationId, Guid hrUserId, Guid employeeId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.Id == employeeId &&
                    e.OrganizationId == organizationId &&
                    e.CreatedByUserId == hrUserId);
        }

        //Used By Admin
        public async Task<List<Employee>> GetEmployeesByOrganizationAsync(Guid organizationId)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.OrganizationId == organizationId)
                .OrderBy(e => e.CreatedAtUtc)
                .ToListAsync();
        }

        //HR can Add
        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();
        }

        //Using Dapper Employee Detail
        public async Task<EmployeeDetailResponse?> GetDetailsAsync(Guid organizationId, Guid employeeId)
        {
            await using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT *
                FROM get_employee_details(
                    @OrganizationId,
                    @EmployeeId
                );
                """;

            var command = new CommandDefinition(
                sql,
                new
                {
                    OrganizationId = organizationId,
                    EmployeeId = employeeId
                });

            return await connection.QuerySingleOrDefaultAsync<EmployeeDetailResponse>(command);
        }
    }
}
