using Vypex.CodingChallenge.API.Dtos;

namespace Vypex.CodingChallenge.API.Services {

    public interface IEmployeeLeaveService {

       Task<LeaveChangeResponseDto> PatchEmployeeLeaveAsync(LeaveChangeRequestDto request);

       Task<bool> DeleteEmployeeLeaveAsync(Guid LeaveId);

       Task<List<EmployeeLeaveDto>> GetEmployeesWithLeaveAsync();

       List<EmployeeLeaveDto> GetEmployeesWithLeave();

       Task<EmployeeLeaveDto> GetEmployeeWithLeaveAsync(Guid EmployeeId);
    }
}