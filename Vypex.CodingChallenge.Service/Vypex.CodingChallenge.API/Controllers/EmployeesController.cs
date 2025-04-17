using Microsoft.AspNetCore.Mvc;
using Vypex.CodingChallenge.Domain;
using Vypex.CodingChallenge.Domain.Models;

namespace Vypex.CodingChallenge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        [HttpGet(Name = "GetEmployees")]
        public IEnumerable<Employee> Get() => FakeEmployeesSeed.Generate(5);

        [HttpGet("{id}", Name = "GetEmployeeById")]
        public Employee GetById(Guid id) => FakeEmployeesSeed.Generate(1).First();
    }
}
