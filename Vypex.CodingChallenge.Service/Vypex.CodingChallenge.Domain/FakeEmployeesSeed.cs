using System.Collections.Generic;
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
                .RuleFor(e => e.AllocatedLeave, (_, e) => GenerateLeave(e).ToList())
                .Generate(count)
                .Select(e => new Employee
                {
                    Id = e.Id,
                    Name = e.Name,
                    AllocatedLeave = e.AllocatedLeave 
                });

        public static IEnumerable<EmployeeLeave> GenerateLeave(Employee employee, int maxLeaveDuration= 10 ) =>
             new Faker<EmployeeLeave>()
                .StrictMode(false)
                .RuleFor(a => a.Id, _ => Guid.NewGuid())
                .RuleFor(a => a.EmployeeId, _ => employee.Id)
                .RuleFor(a => a.StartDate, f => f.Date.Recent( 365  ) )
                .RuleFor(a => a.CalculatedLeaveDays, f => f.Random.Int(1, maxLeaveDuration))
                .RuleFor(a => a.EndDate, (_, a) => DateTimeExtensions.AddWorkingDays(a.StartDate, a.CalculatedLeaveDays))
                .Generate(1)
                .Select( a => new EmployeeLeave 
                {
                    Id = a.Id, 
                    EmployeeId = a.EmployeeId,
                    CalculatedLeaveDays = a.CalculatedLeaveDays,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate 
                });
            
        
    }
}
