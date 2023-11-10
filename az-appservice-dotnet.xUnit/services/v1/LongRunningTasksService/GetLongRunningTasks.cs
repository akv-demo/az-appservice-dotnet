using az_appservice_dotnet.models;
using Moq;

namespace az_appservice_dotnet.xUnit.services.v1.LongRunningTasksService;

public class GetLongRunningTasks
{
    readonly Mock<ILongRunningWorkload> _fakeWorkload = new();
    readonly Mock<ILongRunningWorkloadFactory> _fakeFactory = new();
    
    public GetLongRunningTasks()
    {
        _fakeFactory.Setup(x =>
            x.Create(It.IsAny<uint>())).Returns(_fakeWorkload.Object);
    }
    
    [Fact]
    public void GetLongRunningTasks_shouldReturnEmptyI()
    {
        var stubFactory = new Mock<ILongRunningWorkloadFactory>();
        var sut = new az_appservice_dotnet.services.v1.LongRunningTasksService(stubFactory.Object);
        var actual = sut.GetLongRunningTasks();
        Assert.Empty(actual);
    }
    
    [Fact]
    public void GetLongRunningTasks_shouldReturnStartedTasks()
    {
        // Arrange
        var sut = new az_appservice_dotnet.services.v1.LongRunningTasksService(_fakeFactory.Object);
        var taskId1 = sut.StartLongRunningTasksAsync(2);
        var taskId2 = sut.StartLongRunningTasksAsync(2);
        // Act
        var actual = sut.GetLongRunningTasks();
        // Assert
        Assert.Equal(2, actual.Count);
        Assert.Contains(actual, x => x == taskId1);
        Assert.Contains(actual, x => x == taskId2);
    }
    
}