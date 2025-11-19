using Moq;
using Todo.Api.Services;
using Microsoft.Extensions.Logging;
using Todo.Api.Models;

namespace Todo.Api.Tests;

[TestFixture]
internal class TodoServiceTests
{
    private TodoService underTest = null!;
    private Mock<ITodoRepository> mockRepository = new();
    private Mock<IUserContext> mockUserContext = new();
    private Mock<ILogger<TodoService>> mockLogger = new();

    [SetUp]
    public void Setup()
    {
        underTest = new TodoService(
            mockRepository.Object,
            mockUserContext.Object,
            mockLogger.Object);
    }

    [Test]
    public async Task GetAllAsync_CallsRepositoryWithCurrentUserId()
    {
        // Arrange
        var ownerId = "User-123";
        mockUserContext.Setup(uc => uc.GetCurrentUserId()).Returns(ownerId);
        var expectedItems = new List<TodoItem>
        {
            new() { Title = "Task 1", OwnerId = ownerId },
            new() { Title = "Task 2", OwnerId = ownerId }
        };
        mockRepository.Setup(r => r.GetAllAsync(ownerId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedItems);
        // Act
        var result = await underTest.GetAllAsync();
        // Assert
        Assert.That(result, Is.EqualTo(expectedItems));
        mockRepository.Verify(r => r.GetAllAsync(ownerId, It.IsAny<CancellationToken>()), Times.Once);
        mockUserContext.Verify(uc => uc.GetCurrentUserId(), Times.Once);
    }

    [Test]
    public async Task AddAsync_ReturnsNullAndLogsWarning_WhenTitleIsInvalid()
    {
        // Arrange
        var invalidTitle = "   ";
        // Act
        var result = await underTest.AddAsync(invalidTitle);
        // Assert
        Assert.That(result, Is.Null);
        mockLogger.Verify(
            log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Attempted to add a TodoItem with an invalid title.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

}
