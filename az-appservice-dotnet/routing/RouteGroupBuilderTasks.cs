namespace az_appservice_dotnet;

public static class RouteGroupBuilderTasks
{
    public static RouteGroupBuilder MapTasks(this RouteGroupBuilder group)
    {
        group.MapPost("/long-running-taks", (int size) =>
        {
            
        });
        return group;
    }
    
}