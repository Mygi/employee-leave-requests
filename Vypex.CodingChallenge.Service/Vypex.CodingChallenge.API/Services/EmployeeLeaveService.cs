using Vypex.CodingChallenge.API.Dtos;
using Vypex.CodingChallenge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Vypex.CodingChallenge.Domain.Models;
using Vypex.CodingChallenge.Domain.DateExtensions;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Vypex.CodingChallenge.API.Services
{

    public class EmployeeLeaveService(CodingChallengeContext context, ILoggerFactory logger) : IEmployeeLeaveService
    {
        private readonly ILogger _logger = logger.CreateLogger("Vypex.CodingChallenge.API.Services.EmployeeLeaveService");

        /// <summary>
        /// Peforms insertion or update depending on the request made
        /// Will check for overlaps between proposed leave and existing
        /// Will add business days for leave days and will update the total loeave days for Employee in response
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<EmployeeLeaveDto> UpsertEmployeeLeaveAsync(LeaveChangeRequestDto request)
        {
             if(request.StartDate > request.EndDate) {
                throw new InvalidDataException("Proposed Leave would end before it starts");                             
            }
            var calculatedLeaveDays = DateTimeExtensions.BusinessDaysUntil(request.StartDate, request.EndDate);
            if(calculatedLeaveDays == 0) {
                    throw new InvalidDataException("No Leave Days allocated");
            }
            var employee = await context.Employees.Include(e => e.AllocatedLeave).FirstOrDefaultAsync(e => e.Id == request.EmployeeId) ?? throw new KeyNotFoundException("Can not find an employee by that ID ");
            employee.AllocatedLeave ??= [];
            if (employee.AllocatedLeave.Any(x => (x.Id != request.Id) && DateTimeExtensions.DateRangesOvelap(new Tuple<DateTime, DateTime>(request.StartDate, request.EndDate), new Tuple<DateTime, DateTime>(x.StartDate, x.EndDate))))
            {
                throw new InvalidDataException("Requested leave date ovelaps with existing leave");
            }
            EmployeeLeave? leaveUnderWork = default;
            if (request.Id == default)
            {
                
                leaveUnderWork = new EmployeeLeave()
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    EmployeeId = request.EmployeeId,
                    CalculatedLeaveDays = calculatedLeaveDays
                };
                
                employee.AllocatedLeave.Add(leaveUnderWork);

            }
            else
            {
                leaveUnderWork = employee.AllocatedLeave.FirstOrDefault(a => a.Id == request.Id) ?? throw new KeyNotFoundException("Referenced Allocated Leave not found for employee");
                leaveUnderWork.StartDate = request.StartDate;
                leaveUnderWork.EndDate = request.EndDate;
                leaveUnderWork.CalculatedLeaveDays = calculatedLeaveDays;
            }
            try
            {
                await context.SaveChangesAsync();
                if (leaveUnderWork == default)
                {
                    throw new Exception("Leave is no longer in context");
                }
                return await GetEmployeeWithLeaveAsync(request.EmployeeId);
            }
            catch (DbException ex)
            {
                _logger.LogError("Database exception Caught", [ex.Message]);
                throw;
            }
        }

        /// <summary>
        /// Used as a preflight check before modifying data
        /// </summary>
        /// <param name="leaveRequest"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<LeaveChangeResponseDto> CheckLeaveConstraints(LeaveChangeRequestDto leaveRequest ) {
            if(leaveRequest.StartDate > leaveRequest.EndDate) {
                return new LeaveChangeResponseDto() {
                    Reason = "Proposed Leave would end before it starts",
                    Allowed = false,
                    LeaveDaysTaken = 0
                };
            }
            
            var leaveDays = DateTimeExtensions.BusinessDaysUntil(leaveRequest.StartDate, leaveRequest.EndDate);
            if(leaveDays == 0) {
                return new LeaveChangeResponseDto() {
                    Reason = "No Business Days Taken in Leave",
                    Allowed = false,
                    LeaveDaysTaken = 0
                };
            }

            var employee = await context.Employees.Include(e => e.AllocatedLeave).FirstOrDefaultAsync(e => e.Id == leaveRequest.EmployeeId) ?? throw new KeyNotFoundException("Can not find an employee by that ID ");
            employee.AllocatedLeave ??= [];
            
            if (employee.AllocatedLeave.Any(x => (x.Id != leaveRequest.Id) && DateTimeExtensions.DateRangesOvelap(new Tuple<DateTime, DateTime>(leaveRequest.StartDate, leaveRequest.EndDate), new Tuple<DateTime, DateTime>(x.StartDate, x.EndDate))))
            {
                return new LeaveChangeResponseDto() {
                    Reason = "Proposed Leave would overlap with existing leave",
                    Allowed = false,
                    LeaveDaysTaken = 0
                };
            }
            var existingLeave = employee.AllocatedLeave.FirstOrDefault(x => x.Id == leaveRequest.Id);
            return new LeaveChangeResponseDto() {
                Reason = "Success",
                Allowed = true,
                LeaveDaysTaken = existingLeave != default ? leaveDays - existingLeave.CalculatedLeaveDays  + employee.AllocatedLeave.Sum(x => x.CalculatedLeaveDays)
                                                          : leaveDays + employee.AllocatedLeave.Sum(x => x.CalculatedLeaveDays)
            };
        }

        /// <summary>
        /// Delete Leave if found
        /// </summary>
        /// <param name="leaveId"></param>
        /// <returns>The updated count for leave taken</returns>
        public async Task<EmployeeLeaveDto> DeleteEmployeeLeaveAsync(Guid leaveId)
        {
            var leave = await context.EmployeeLeave.FirstOrDefaultAsync(l => l.Id == leaveId);
            if (leave != null)
            {
                try {
                    var employee = await GetEmployeeWithLeaveAsync(leave.EmployeeId);
                    context.EmployeeLeave.Remove(leave);
                    // Update Leave Count
                    
                    await context.SaveChangesAsync();
                    return await GetEmployeeWithLeaveAsync(leave.EmployeeId);
                }
                catch (DbException ex)
                {
                    _logger.LogError("Database exception Caught", [ex.Message]);
                    throw;
                }
                
            }   
            else {
                throw new KeyNotFoundException("No leave found by that Id");
            }
        }

        /// <summary>
        /// Async call to get full list of employees with leave
        /// </summary>
        /// <returns></returns>
        public async Task<List<EmployeeLeaveDto>> GetEmployeesWithLeaveAsync()
        {
            var query = context.Employees.Include(e => e.AllocatedLeave).AsQueryable();
            try
            {
                return await query.Select(e => new EmployeeLeaveDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    AccumulatedLeaveDays = e.AllocatedLeave.Sum(x => x.CalculatedLeaveDays),
                    LeaveTaken = e.AllocatedLeave.Select(a => new AnnualLeaveDto
                    {
                        Id = a.Id,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        LeaveDaysAllocated = a.CalculatedLeaveDays

                    }).ToList()
                }).ToListAsync();
            }
            catch (DbException ex)
            {
                _logger.LogError("Database exception Caught", [ex.Message]);
                throw;
            }

        }

        /// <summary>
        /// Sync call to get Employees. Use if you can't cache but need performant response
        /// that can lock threads
        /// </summary>
        /// <returns></returns>
        public List<EmployeeLeaveDto> GetEmployeesWithLeave()
        {
            var query = context.Employees.Include(e => e.AllocatedLeave).AsQueryable();
            try
            {
                return [.. query.Select(e => new EmployeeLeaveDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    AccumulatedLeaveDays = e.AllocatedLeave.Sum(x => x.CalculatedLeaveDays),
                    LeaveTaken = e.AllocatedLeave.Select(a => new AnnualLeaveDto
                    {
                        Id = a.Id,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        LeaveDaysAllocated = a.CalculatedLeaveDays

                    }).ToList()
                })];
            }
            catch (DbException ex)
            {
                _logger.LogError("Database exception Caught", [ex.Message]);
                throw;
            }
        }

        /// <summary>
        /// Get a single employee by their ID
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<EmployeeLeaveDto> GetEmployeeWithLeaveAsync(Guid EmployeeId)
        {
            var employee = await context.Employees.Include(e => e.AllocatedLeave)
                                            .AsQueryable()
                                            .FirstOrDefaultAsync(x => x.Id == EmployeeId);
            if(employee == default) {
                throw new KeyNotFoundException("No Employee found by that ID");
            } 
            return new EmployeeLeaveDto
            {
                    Id = employee.Id,
                    Name = employee.Name,
                    AccumulatedLeaveDays = employee.AllocatedLeave.Sum(x => x.CalculatedLeaveDays),
                    LeaveTaken = [.. employee.AllocatedLeave.Select(a => new AnnualLeaveDto
                    {
                        Id = a.Id,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        LeaveDaysAllocated = a.CalculatedLeaveDays

                    })]

            };
        }
    }
}