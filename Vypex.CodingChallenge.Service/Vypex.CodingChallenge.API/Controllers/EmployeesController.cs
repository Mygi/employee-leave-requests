using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vypex.CodingChallenge.API.Dtos;
using Vypex.CodingChallenge.API.Services;
using Vypex.CodingChallenge.Domain.Models;

namespace Vypex.CodingChallenge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController(IEmployeeLeaveService employeeLeaveService, ILoggerFactory logger)  : ControllerBase
    {
        private readonly ILogger _logger = logger.CreateLogger("Vypex.CodingChallenge.API.Controllers.EmployeesController");
        

        [HttpGet(Name = "GetEmployees")]
        [ProducesResponseType<List<Employee>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<EmployeeLeaveDto>>> GetEmployees()
        {
       
            try {
                var employees = await employeeLeaveService.GetEmployeesWithLeaveAsync();
                if (employees == null) {
                    return NotFound();
                }

                return Ok(employees);
            }
            catch(Exception) {
                // Returnong bad request in case secure messages appear in response
                return BadRequest();
            }
            
            
        }

        [HttpGet("{employeeId}")]
        [ProducesResponseType<EmployeeLeaveDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeLeaveDto>> GetEmployee(Guid employeeId)
        {
       
            try {
                var employee = await employeeLeaveService.GetEmployeeWithLeaveAsync(employeeId);
                return Ok(employee);
            }
            catch(KeyNotFoundException ex) {
                return NotFound(ex.Message);
            } 
            catch(Exception) {
                // Returnong bad request in case secure messages appear in response
                return BadRequest();
            }
            
            
        }

        [HttpPut("UpsertLeave")]
        [ProducesResponseType<LeaveChangeResponseDto>(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LeaveChangeResponseDto>> UpsertLeaveAsync(LeaveChangeRequestDto leaveRequest)
        {            
            try 
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await employeeLeaveService.UpsertEmployeeLeaveAsync(leaveRequest);
                return Accepted(result);
            }
            catch(InvalidDataException ex) {
                return Conflict(ex);
            } 
            catch(KeyNotFoundException ex) {
                return NotFound(ex.Message);
            } 
            catch
            {
                return BadRequest();
            }
            

            
        }


        [HttpDelete("leave/{leaveId:guid}")]
        [ProducesResponseType<LeaveChangeResponseDto>(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteLeave(Guid leaveId)
        {
            try 
            {
                var result = await employeeLeaveService.DeleteEmployeeLeaveAsync(leaveId);
                return Accepted(result);
            }
            catch(KeyNotFoundException ex) {
                return NotFound(ex.Message);
            } 
            catch
            {
                return BadRequest();
            }
                
        }
    }
}
