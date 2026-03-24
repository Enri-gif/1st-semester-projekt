using System;
using System.Threading.Tasks;
using api.Controllers;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xunit;

namespace api.Tests
{
    public class AssignmentControllerTests
    {
        private AssignmentController GetControllerWithInMemoryDb(out ApplicationDbContext context)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            context = new ApplicationDbContext(options);
            var service = new AssignmentService(context);
            return new AssignmentController(service);
        }

        [Fact]
        public async Task DeleteAssignment_ShouldReturnDeletedAssignment_WhenAssignmentExists()
        {
            // Arrange
            var controller = GetControllerWithInMemoryDb(out var context);

            var assignment = new Assignment
            {
                Id = Guid.NewGuid(),
                Answer = "Test Answer",
                Topic = "Test Topic",
                Subject = "Test Subject",
                Level = "A",
                Subtest = 1,
                Subquestion = "1",
                Points = 10,
                Number = 1
            };

            context.Assignments.Add(assignment);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.DeleteAssignment(assignment.Id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Assignment>>(result);
            var deletedAssignment = Assert.IsType<Assignment>(actionResult.Value);

            Assert.Equal(assignment.Id, deletedAssignment.Id);

            var dbAssignment = await context.Assignments.FindAsync(assignment.Id);
            Assert.Null(dbAssignment);
        }

        [Fact]
        public async Task DeleteAssignment_ShouldReturnNotFound_WhenAssignmentDoesNotExist()
        {
            // Arrange
            var controller = GetControllerWithInMemoryDb(out var _);

            // Act
            var result = await controller.DeleteAssignment(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
