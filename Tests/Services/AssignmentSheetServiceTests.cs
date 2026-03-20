using Moq;
using System.Collections;

public class AssignmentSheetServiceTests
{
    // -------------------------
    // BASIC DELETE TEST
    // -------------------------
    [Fact]
    public async Task DeleteAssignmentSheet_ShouldReturnTrue_WhenDeleteIsSuccessful()
    {
        // Arrange
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

        var service = new AssignmentSheetService(mockRepo.Object);
        int assignmentSheetId = 1;

        // Act
        var result = await service.DeleteAssignmentSheet(assignmentSheetId);

        // Assert
        Assert.True(result);
        mockRepo.Verify(r => r.DeleteAsync(assignmentSheetId), Times.Once);
    }

    // -------------------------
    // INLINE DATA
    // -------------------------
    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(999)]
    public async Task DeleteAssignmentSheet_ShouldCallRepository_WithCorrectId(int assignmentSheetId)
    {
        // Arrange
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.DeleteAsync(assignmentSheetId))
                .ReturnsAsync(true);

        var service = new AssignmentSheetService(mockRepo.Object);

        // Act
        var result = await service.DeleteAssignmentSheet(assignmentSheetId);

        // Assert
        Assert.True(result);
        mockRepo.Verify(r => r.DeleteAsync(assignmentSheetId), Times.Once);
    }

    // -------------------------
    // MEMBER DATA
    // -------------------------
    public static IEnumerable<object[]> AssignmentSheetIds =>
        new List<object[]>
        {
            new object[] { 1 },
            new object[] { 2 },
            new object[] { 3 }
        };

    [Theory]
    [MemberData(nameof(AssignmentSheetIds))]
    public async Task DeleteAssignmentSheet_ShouldSucceed_WithVariousIds(int assignmentSheetId)
    {
        // Arrange
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.DeleteAsync(assignmentSheetId))
                .ReturnsAsync(true);

        var service = new AssignmentSheetService(mockRepo.Object);

        // Act
        var result = await service.DeleteAssignmentSheet(assignmentSheetId);

        // Assert
        Assert.True(result);
    }

    // -------------------------
    // CLASS DATA
    // -------------------------
    public class AssignmentSheetIdTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 10 };
            yield return new object[] { 20 };
            yield return new object[] { 30 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(AssignmentSheetIdTestData))]
    public async Task DeleteAssignmentSheet_ShouldWork_WithClassData(int assignmentSheetId)
    {
        // Arrange
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.DeleteAsync(assignmentSheetId))
                .ReturnsAsync(true);

        var service = new AssignmentSheetService(mockRepo.Object);

        // Act
        var result = await service.DeleteAssignmentSheet(assignmentSheetId);

        // Assert
        Assert.True(result);
    }

    // -------------------------
    // FAILURE CASE
    // -------------------------
    [Fact]
    public async Task DeleteAssignmentSheet_ShouldReturnFalse_WhenDeleteFails()
    {
        // Arrange
        var mockRepo = new Mock<IAssignmentSheetRepository>();
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

        var service = new AssignmentSheetService(mockRepo.Object);

        // Act
        var result = await service.DeleteAssignmentSheet(1);

        // Assert
        Assert.False(result);
    }
}