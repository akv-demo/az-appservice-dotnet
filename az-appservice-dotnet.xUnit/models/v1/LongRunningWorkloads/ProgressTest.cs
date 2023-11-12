namespace az_appservice_dotnet.xUnit.models.v1.LongRunningWorkloads;

public class ProgressTest
{
    [Fact]
    public void Progress_shouldReturnZeroOnInit()
    {
        // Arrange
        var sut = new az_appservice_dotnet.models.v1.LongRunningWorkloads(10);
        // Act
        var actual = sut.Progress;
        // Assert
        Assert.Equal(0U, actual);
    }
    
    [Fact]
    public void Progress_shouldReturn10OnNext()
    {
        // Arrange
        var sut = new az_appservice_dotnet.models.v1.LongRunningWorkloads(10);
        // Act
        sut.Next(1);
        var actual = sut.Progress;
        // Assert
        Assert.Equal(10U, actual);
    }
    
    [Fact]
    public void Progress_shouldReturn100OnOverflow()
    {
        // Arrange
        var sut = new az_appservice_dotnet.models.v1.LongRunningWorkloads(10);
        // Act
        sut.Next(11);
        var actual = sut.Progress;
        // Assert
        Assert.Equal(100U, actual);
    }
}