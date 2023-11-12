using az_appservice_dotnet.models;
using az_appservice_dotnet.services;
using Moq;

namespace az_appservice_dotnet.xUnit.services.v1.LongRunningTasksService;

public class GetLongRunningTaskProgressTest
{
    readonly Mock<ILongRunningWorkload> _fakeWorkload = new();
    readonly Mock<ILongRunningWorkloadFactory> _fakeFactory = new();
    
    public GetLongRunningTaskProgressTest()
    {
        _fakeFactory.Setup(x =>
            x.Create(It.IsAny<uint>())).Returns(_fakeWorkload.Object);
    }
    
    [Fact]
    public void GetLongRunningTaskProgress_shouldReturnMinus1()
    {
        var stubFactory = new Mock<ILongRunningWorkloadFactory>();
        var sut = new az_appservice_dotnet.services.v1.LongRunningTasksService(stubFactory.Object);
        var actual = sut.GetLongRunningTaskProgress(new TaskId());
        Assert.Equal(-1, actual);
    }
    
    [Fact]
    public void GetLongRunningTaskProgress_shouldReturnForStartedTask()
    {
        // Arrange
        var sut = new az_appservice_dotnet.services.v1.LongRunningTasksService(_fakeFactory.Object);
        var taskId = sut.StartLongRunningTasksAsync(2);
        // Act
        var actual = sut.GetLongRunningTaskProgress(taskId);
        // Assert
        Assert.True(actual >= 0);
    }
}