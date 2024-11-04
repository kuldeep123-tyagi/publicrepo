using APISolution.Data;
using APISolution.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;

namespace APISolution.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeDbContext _context;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public EmployeesController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            try
            {
                var employees = await _context.Employees.ToListAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while fetching employees.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var isEmployeeExist = await _context.Employees.FindAsync(employee.EmployeeID);
                if (isEmployeeExist != null)
                {
                    return BadRequest("Employee is already exist.");
                }
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeID }, employee);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while inserting an employee.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                    return NotFound();

                return employee;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while fetching an employee.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeID)
                return BadRequest("Employee ID mismatch.");

            try
            {
                var existingEmployee = await _context.Employees.FindAsync(id);
                if (existingEmployee == null)
                    return NotFound();

                existingEmployee.Name = employee.Name;
                existingEmployee.Department = employee.Department;
                existingEmployee.Salary = employee.Salary;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while updating the employee with ID {EmployeeID}", id);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                    return NotFound();

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while deleting an employee.");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }
    }
}
