using Vypex.CodingChallenge.API.Dtos;
using Vypex.CodingChallenge.Infrastructure.Data;
﻿using Microsoft.EntityFrameworkCore;
using Vypex.CodingChallenge.Domain.Models;
using Vypex.CodingChallenge.Domain.DateExtensions;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Vypex.CodingChallenge.API.Services
{

    public class EmployeeLeaveService(CodingChallengeContext context, ILoggerFactory logger) : IEmployeeLeaveService
    {
        private readonly CodingChallengeContext _context = context;
        private readonly ILogger _logger = logger.CreateLogger("Vypex.CodingChallenge.API.Services.EmployeeLeaveService");

        public async Task<LeaveChangeResponseDto> PatchEmployeeLeaveAsync(LeaveChangeRequestDto request)
        {
            var employee = await _context.Employees.Include(e => e.AllocatedLeave).FirstOrDefaultAsync(e => e.Id == request.EmployeeId) ?? throw new ArgumentException("Can not find an employee by that ID ");
            if (employee.AllocatedLeave == null) {
                employee.AllocatedLeave = [];
                
            } 
            if(employee.AllocatedLeave.Any(x => DateTimeExtensions.DateRangesOvelap(new Tuple<DateTime, DateTime>(request.StartDate, request.EndDate), new Tuple<DateTime, DateTime>(x.StartDate, x.EndDate))) ) {
                throw new InvalidDataException("Requested leave date ovelaps with existing leave");                            
            } 
            EmployeeLeave? leaveUnderWork = default;
            if (request.Id == default)
            {
                leaveUnderWork = new EmployeeLeave() {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    EmployeeId = request.EmployeeId,
                    CalculatedLeaveDays = DateTimeExtensions.BusinessDaysUntil(request.StartDate, request.EndDate)
                };
                employee.AllocatedLeave.Add(leaveUnderWork );
               
            }
            else
            {
                leaveUnderWork = employee.AllocatedLeave.FirstOrDefault(a => a.Id == request.Id) ?? throw new KeyNotFoundException("Referenced Allocated Leave not found for employee");
                leaveUnderWork.StartDate = request.StartDate;
                leaveUnderWork.EndDate = request.EndDate;
                leaveUnderWork.CalculatedLeaveDays = DateTimeExtensions.BusinessDaysUntil(request.StartDate, request.EndDate);
            }
            try
            {
                await _context.SaveChangesAsync();
                if(leaveUnderWork == default) {
                    throw new Exception("Leave is no longer in context");
                }
                return new LeaveChangeResponseDto() {
                    LeaveId = leaveUnderWork.Id,
                    StartDate = leaveUnderWork.StartDate,
                    EndDate = leaveUnderWork.EndDate,
                    LeaveDaysTaken = leaveUnderWork.CalculatedLeaveDays,
                    UpdatedEmployeAnnualLeave = employee.AllocatedLeave.Sum(x => x.CalculatedLeaveDays),
                    EmployeeId = employee.Id
                    
                };
            }
            catch (DbException ex)
            {
                _logger.LogError("Database exception Caught", [ex.Message]);
                throw;
            }
        }

        public Task<bool> DeleteEmployeeLeaveAsync(Guid LeaveId)
        {
            throw new NotImplementedException("");
        }

        public Task<List<EmployeeLeaveDto>> GetEmployeesWithLeaveAsync()
        {
            throw new NotImplementedException("");
        }

        public List<EmployeeLeaveDto> GetEmployeesWithLeave()
        {
            throw new NotImplementedException("");
        }

        public Task<EmployeeLeaveDto> GetEmployeeWithLeaveAsync(Guid EmployeeId)
        {
            throw new NotImplementedException("");
        }
    }
}