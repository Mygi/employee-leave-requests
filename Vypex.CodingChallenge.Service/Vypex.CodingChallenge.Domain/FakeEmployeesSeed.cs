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
    }
}
