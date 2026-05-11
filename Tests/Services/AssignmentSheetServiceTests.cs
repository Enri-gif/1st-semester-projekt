using api.Data;
using api.Services;
using FluentAssertions;
using Moq;
using api.Interfaces;

namespace Tests.ServiceTests;

public class AssignmentSheetServiceTests
{
    [Fact]
    public async Task DeleteAssignmentSheet_ShouldReturnTrue_WhenDeleteIsSuccessful()
    {
        // Arrange
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);
        var service = new AssignmentSheetService(mockRepo.Object, Mock.Of<IAssignmentRepository>());
        var id = Guid.NewGuid();

        // Act
        var result = await service.DeleteAssignmentSheet(id);

        // Assert
        result.Should().BeTrue();
        mockRepo.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    public static IEnumerable<object[]> AssignmentSheetIds =>
        new List<object[]>
        {
            new object[] { Guid.NewGuid() },
            new object[] { Guid.NewGuid() },
            new object[] { Guid.NewGuid() }
        };

    [Theory]
    [MemberData(nameof(AssignmentSheetIds))]
    public async Task DeleteAssignmentSheet_ShouldSucceed_WithVariousIds(Guid assignmentSheetId)
    {
        // Arrange
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.DeleteAsync(assignmentSheetId)).ReturnsAsync(true);
        var service = new AssignmentSheetService(mockRepo.Object, Mock.Of<IAssignmentRepository>());

        // Act
        var result = await service.DeleteAssignmentSheet(assignmentSheetId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAssignmentSheet_ShouldReturnFalse_WhenDeleteFails()
    {
        // Arrange
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);
        var service = new AssignmentSheetService(mockRepo.Object, Mock.Of<IAssignmentRepository>());

        // Act
        var result = await service.DeleteAssignmentSheet(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    // Classic xUnit-style variant kept for comparison with FluentAssertions.
    [Fact]
    public async Task DeleteAssignmentSheet_ShouldReturnFalse_WhenDeleteFails_ClassicXunit()
    {
        // Arrange
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);
        var service = new AssignmentSheetService(mockRepo.Object, Mock.Of<IAssignmentRepository>());

        // Act
        var result = await service.DeleteAssignmentSheet(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetPointsBreakdown_ShouldReturnNull_WhenSheetDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((api.Models.AssignmentSheet?)null);
        var service = new AssignmentSheetService(mockRepo.Object, Mock.Of<IAssignmentRepository>());

        // Act
        var result = await service.GetPointsBreakdown(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPointsBreakdown_ShouldSumPoints_WhenSheetHasAssignments()
    {
        // Arrange
        var sheet = new api.Models.AssignmentSheet
        {
            Id = Guid.NewGuid(),
            Assignments = new List<api.Models.Assignment>
            {
                new api.Models.Assignment { Id = Guid.NewGuid(), Number = 1, Points = 10 },
                new api.Models.Assignment { Id = Guid.NewGuid(), Number = 2, Points = 25 },
                new api.Models.Assignment { Id = Guid.NewGuid(), Number = 3, Points = 5 }
            }
        };
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(sheet.Id)).ReturnsAsync(sheet);
        var service = new AssignmentSheetService(mockRepo.Object, Mock.Of<IAssignmentRepository>());

        // Act
        var result = await service.GetPointsBreakdown(sheet.Id);

        // Assert
        result.Should().NotBeNull();
        result!.TotalPoints.Should().Be(40);
        result.PerAssignment.Should().HaveCount(3);
    }
}
