using APISolution.Data;
using APISolution.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog.Config;
using System;
using Xunit;

namespace APISolution.Controllers.Testing
{
    public class EmployeeApiIntegrationTests : IClassFixture<WebApplicationFactory<EmployeesController>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<EmployeesController> _factory;

        public EmployeeApiIntegrationTests(WebApplicationFactory<EmployeesController> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EmployeeDbContext>));
                    services.Remove(descriptor);
                    services.AddDbContext<EmployeeDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryEmployeeTestDb");
                    });
                });
            }).CreateClient();
            _factory = factory;
        }

        [Fact]
        public async Task GetEmployees_ReturnsEmployeesList()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
                dbContext.Employees.Add(new Employee { EmployeeID = 1, Name = "John Doe", Department = "IT", Salary = 50000M });
                dbContext.Employees.Add(new Employee { EmployeeID = 2, Name = "Jane Smith", Department = "HR", Salary = 60000M });
                await dbContext.SaveChangesAsync(); // Use async SaveChanges for consistency
            }

            // Act
            var response = await _client.GetAsync("/api/employees");

            // Assert
            response.EnsureSuccessStatusCode();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(await response.Content.ReadAsStringAsync());
            Assert.Equal(2, employees.Count);
            Assert.Equal("John Doe", employees[0].Name);
        }
    }

}
