using az_appservice_dotnet.models;
using az_appservice_dotnet.services;
using Moq;

namespace az_appservice_dotnet.xUnit.services.v1.LongRunningTasksService;

public class StartLongRunningTasksAsyncTest
{
    readonly Mock<ILongRunningWorkload> _fakeWorkload = new();
    readonly Mock<ILongRunningWorkloadFactory> _fakeFactory = new();

    public StartLongRunningTasksAsyncTest()
    {
        _fakeFactory.Setup(x =>
            x.Create(It.IsAny<uint>())).Returns(_fakeWorkload.Object);
    }

    [Fact]
    public void StartLongRunningTasksAsync_shouldReturnTaskId()
    {
        // Arrange
        var sut = new az_appservice_dotnet.services.v1.LongRunningTasksService(_fakeFactory.Object);
        // Act
        var actual = sut.StartLongRunningTasksAsync(2);
        // Assert
        Assert.IsType<TaskId>(actual);
    }

    [Fact]
    public void StartLongRunningTasksAsync_shouldReturnTaskIdWhatExists()
    {
        // Arrange
        var sut = new az_appservice_dotnet.services.v1.LongRunningTasksService(_fakeFactory.Object);
        // Act
        var taskId = sut.StartLongRunningTasksAsync(2);
        var foundTaskId = sut.GetLongRunningTaskProgress(taskId);
        // Assert
        Assert.True(foundTaskId >= 0);
    }

    [Fact]
    public void StartLongRunningTasksAsync_shouldCallWorkloadNext2Times()
    {
        // Arrange
        var sut = new az_appservice_dotnet.services.v1.LongRunningTasksService(_fakeFactory.Object);
        // Act
        sut.StartLongRunningTasksAsync(2);
        Thread.Sleep(3000);
        // Assert
        _fakeWorkload.Verify(wl => wl.Next(It.IsAny<uint>()),
            Times.Exactly(2));
    }

    [Fact]
    public void StartLongRunningTasksAsync_shouldStartTaskAsIncomplete()
    {
        // Arrange
        var sut = new az_appservice_dotnet.services.v1.LongRunningTasksService(_fakeFactory.Object);
        // Act
        var actual = sut.StartLongRunningTasksAsync(1);
        // Assert
        sut.GetTaskDictionaryTest().TryGetValue(actual, out var task);
        Assert.NotNull(task);
        Assert.False(task.IsCompleted);
    }
    
    [Fact]
    public void StartLongRunningTasksAsync_shouldStartTaskHaveCompleted()
    {
        // Arrange
        var sut = new az_appservice_dotnet.services.v1.LongRunningTasksService(_fakeFactory.Object);
        // Act
        var actual = sut.StartLongRunningTasksAsync(1);
        Thread.Sleep(2000);
        // Assert
        sut.GetTaskDictionaryTest().TryGetValue(actual, out var task);
        Assert.NotNull(task);
        Assert.True(task.IsCompleted);
    }
}