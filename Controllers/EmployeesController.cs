using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Repositories;
using Newtonsoft.Json;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        // private readonly TodoContext _context;
        private readonly IRepositoryWrapper _repositoryWrapper;

        // public EmployeesController(TodoContext context)
        // {
        //     _context = context;
        // }
        public EmployeesController(IRepositoryWrapper RW)
        {
            _repositoryWrapper = RW;
        }

        // GET: api/Employees
        [HttpGet]
        // public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        // {
        //   if (_context.Employee == null)
        //   {
        //       return NotFound();
        //   }
        //     return await _context.Employee.ToListAsync();
        // }
         public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var empList = await _repositoryWrapper.Employee.FindAllAsync();
            return Ok(empList);
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
         public async Task<ActionResult<Employee>> GetEmployee(long id)
        {
            var employee = await _repositoryWrapper.Employee.FindByIDAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }
        // public async Task<ActionResult<Employee>> GetEmployee(long id)
        // {
        //   if (_context.Employee == null)
        //   {
        //       return NotFound();
        //   }
        //     var employee = await _context.Employee.FindAsync(id);

        //     if (employee == null)
        //     {
        //         return NotFound();
        //     }

        //     return employee;
        // }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
         public async Task<IActionResult> PutEmployee(long id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            Employee? objEmp;
            try
            {
                objEmp = _repositoryWrapper.Employee.FindByID(id);
                if (objEmp == null) 
                    throw new Exception("Invalid Employee ID");
                
                objEmp.empName = employee.empName;
                objEmp.empAddress = employee.empAddress;
                await _repositoryWrapper.Employee.UpdateAsync(objEmp);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        // public async Task<IActionResult> PutEmployee(long id, Employee employee)
        // {
        //     if (id != employee.Id)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(employee).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!EmployeeExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
         public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            await _repositoryWrapper.Employee.CreateAsync(employee, true);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(long id)
        {
            var employee = await _repositoryWrapper.Employee.FindByIDAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            await _repositoryWrapper.Employee.DeleteAsync(employee, true);
            
            return NoContent();
        }

        // public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        // {
        //   if (_context.Employee == null)
        //   {
        //       return Problem("Entity set 'TodoContext.Employee'  is null.");
        //   }
        //     _context.Employee.Add(employee);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        // }

        // DELETE: api/Employees/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteEmployee(long id)
        // {
        //     if (_context.Employee == null)
        //     {
        //         return NotFound();
        //     }
        //     var employee = await _context.Employee.FindAsync(id);
        //     if (employee == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Employee.Remove(employee);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        // private bool EmployeeExists(long id)
        // {
        //     return (_context.Employee?.Any(e => e.Id == id)).GetValueOrDefault();
        // }
         [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<Employee>>>  SearchEmployee(dynamic param)
        {
            dynamic filterObj = JsonConvert.DeserializeObject<dynamic>(param.ToString());
            string nameFilter = filterObj.term;
            var empList = await _repositoryWrapper.Employee.SearchEmployee(nameFilter);
            return Ok(empList);
            
        }

        private bool EmployeeExists(long id)
        {
            return _repositoryWrapper.Employee.IsExists(id);
        }
    }
}
