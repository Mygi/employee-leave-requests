using Bogus;
using Vypex.CodingChallenge.Domain.Models;
namespace Vypex.CodingChallenge.Domain
{
    public static class FakeEmployeesSeed
    {
        public static IEnumerable<Employee> Generate(int count) =>
            new Faker<Employee>()
                .UseSeed(8675309)
                .StrictMode(false)
                .RuleFor(e => e.Id, _ => Guid.NewGuid())
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .Generate(count)
                .Select(e => new Employee
                {
                    Id = e.Id,
                    Name = e.Name
                });

        public static EmployeeLeave GenerateLeave(Employee employee, int maxLeaveDuration= 10 ) =>
             new Faker<EmployeeLeave>()
                .UseSeed(8675310)
                .StrictMode(false)
                .RuleFor(a => a.Id, _ => Guid.NewGuid())
                .RuleFor(a => a.EmployeeId, _ => employee.Id)
                .RuleFor(a => a.StartDate, f => f.Date.Recent( 365  ) )
                .RuleFor(a => a.CalculatedLeaveDays, f => f.Random.Int(0, maxLeaveDuration))
                .RuleFor(a => a.EndDate, (_, a) => DateTimeExtensions.AddWorkingDays(a.StartDate, a.CalculatedLeaveDays));
            
        
    }
}
