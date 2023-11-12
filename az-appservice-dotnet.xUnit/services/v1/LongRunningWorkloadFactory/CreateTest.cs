namespace az_appservice_dotnet.xUnit.services.v1.LongRunningWorkloadFactory;

public class CreateTest
{
    [Fact]
    public void Create_shouldReturnLongRunningWorkload()
    {
        // Arrange
        var sut = new az_appservice_dotnet.models.v1.LongRunningWorkloadFactory();
        // Act
        var actual = sut.Create(10);
        // Assert
        Assert.IsType<az_appservice_dotnet.models.v1.LongRunningWorkloads>(actual);
    }
    
    [Fact]
    public void Create_shouldPassDurationToWorkload()
    {
        // Arrange
        var sut = new az_appservice_dotnet.models.v1.LongRunningWorkloadFactory();
        // Act
        var actual = sut.Create(10);
        actual.Next(9);
        var actualProgress1 = actual.Progress;
        actual.Next(10);
        var actualProgress2 = actual.Progress;
        // Assert
        Assert.Equal(90U, actualProgress1);
        Assert.Equal(100U, actualProgress2);
    }
}