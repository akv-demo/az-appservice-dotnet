namespace az_appservice_dotnet.routing.v1;

public static class RouterGroupBuilderPing
{
    public static RouteGroupBuilder MapPing(this RouteGroupBuilder group)
    {
        group
            .MapGet("/ping", () => "pong")
            .AddEndpointFilter(async (context, next) =>
            {
                if (!context.HttpContext.Request.Query.ContainsKey("workload"))
                {
                    return TypedResults.UnprocessableEntity("Missing workload query parameter");
                }

                return await next(context);
            });
        return group;
    }
}