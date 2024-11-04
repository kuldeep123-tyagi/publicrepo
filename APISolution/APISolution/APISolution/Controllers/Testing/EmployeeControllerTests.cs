using APISolution.Data;
using APISolution.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Xunit;

namespace APISolution.Controllers
{
    public class EmployeeControllerTests : ControllerBase
    {
        private readonly Mock<EmployeeDbContext> _mockContext;
        private readonly EmployeesController _controller;

        public EmployeeControllerTests()
        {
            _mockContext = new Mock<EmployeeDbContext>();
            _controller = new EmployeesController(_mockContext.Object);
        }

        [Fact]
        public async Task GetEmployee_EmployeeExists_ReturnsEmployee()
        {
            // Arrange
            var employee = new Employee { EmployeeID = 1, Name = "John Doe", Department = "IT", Salary = 50000M };
            _mockContext.Setup(db => db.Employees.FindAsync(1))
                        .ReturnsAsync(employee);

            // Act
            var result = await _controller.GetEmployee(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Employee>>(result);
            var returnValue = Assert.IsType<Employee>(actionResult.Value);
            Assert.Equal("John Doe", returnValue.Name);
        }

        [Fact]
        public async Task GetEmployee_EmployeeNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockContext.Setup(db => db.Employees.FindAsync(1)).ReturnsAsync((Employee)null);

            // Act
            var result = await _controller.GetEmployee(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
