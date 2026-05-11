using System;
using System.Threading.Tasks;
using Moq;
using Reqnroll;
using FluentAssertions;
using api.Data;
using api.Models;
using api.Services;
using api.Data;

namespace Tests.ServiceTests;

[Binding]
public class DeleteAssignmentServiceSteps
{
    private readonly Mock<IAssignmentRepository> _repoMock = new();
    private readonly Mock<IMongoImageService> _imageServiceMock = new();
    private readonly Mock<IMongoVideoService> _videoServiceMock = new();

    private AssignmentService _service;
    private Guid _assignmentId;
    private bool _result;
    private Assignment _assignment;

    public DeleteAssignmentServiceSteps()
    {
        _service = new AssignmentService(
            _repoMock.Object,
            _imageServiceMock.Object,
            _videoServiceMock.Object
        );
    }

    [Given(@"an assignment id")]
    public void GivenAnAssignmentId()
    {
        _assignmentId = Guid.NewGuid();
    }

    [Given(@"the repository returns null for that id")]
    public void GivenRepositoryReturnsNull()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(_assignmentId))
            .ReturnsAsync((Assignment)null);
    }

    [Given(@"the repository returns an assignment for that id")]
    public void GivenRepositoryReturnsAssignment()
    {
        _assignment = new Assignment { Id = _assignmentId };

        _repoMock
            .Setup(r => r.GetByIdAsync(_assignmentId))
            .ReturnsAsync(_assignment);
    }

    [Given(@"removing the assignment succeeds")]
    public void GivenRemovingSucceeds()
    {
        _repoMock
            .Setup(r => r.RemoveAsync(It.IsAny<Assignment>()))
            .ReturnsAsync(true);
    }

    [When(@"I delete the assignment")]
    public async Task WhenIDeleteTheAssignment()
    {
        _result = await _service.DeleteAssignment(_assignmentId);
    }

    [Then(@"the result should be false")]
    public void ThenResultShouldBeFalse()
    {
        _result.Should().BeFalse();

        _imageServiceMock.Verify(
            s => s.DeleteImagesByAssignmentIdAsync(It.IsAny<string>()),
            Times.Never
        );

        _videoServiceMock.Verify(
            s => s.DeleteVideosByAssignmentIdAsync(It.IsAny<string>()),
            Times.Never
        );
    }

    [Then(@"the result should be true")]
    public void ThenResultShouldBeTrue()
    {
        _result.Should().BeTrue();
    }

    [Then(@"images are deleted for the assignment")]
    public void ThenImagesAreDeleted()
    {
        _imageServiceMock.Verify(
            s => s.DeleteImagesByAssignmentIdAsync(_assignmentId.ToString()),
            Times.Once
        );
    }

    [Then(@"videos are deleted for the assignment")]
    public void ThenVideosAreDeleted()
    {
        _videoServiceMock.Verify(
            s => s.DeleteVideosByAssignmentIdAsync(_assignmentId.ToString()),
            Times.Once
        );
    }
}
