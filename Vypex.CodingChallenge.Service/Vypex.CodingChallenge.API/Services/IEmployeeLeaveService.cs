using Vypex.CodingChallenge.API.Dtos;

namespace Vypex.CodingChallenge.API.Services {

    public interface IEmployeeLeaveService {

       Task<EmployeeLeaveDto> UpsertEmployeeLeaveAsync(LeaveChangeRequestDto request);

       Task<EmployeeLeaveDto> DeleteEmployeeLeaveAsync(Guid LeaveId);

       Task<List<EmployeeLeaveDto>> GetEmployeesWithLeaveAsync();

       List<EmployeeLeaveDto> GetEmployeesWithLeave();

       Task<EmployeeLeaveDto> GetEmployeeWithLeaveAsync(Guid EmployeeId);

       Task<LeaveChangeResponseDto> CheckLeaveConstraints(LeaveChangeRequestDto leaveRequest );
    }
}