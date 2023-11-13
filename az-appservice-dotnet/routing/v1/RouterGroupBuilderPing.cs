namespace az_appservice_dotnet.routing.v1;

public static class RouterGroupBuilderPing
{
    public static RouteGroupBuilder MapPing(this RouteGroupBuilder group)
    {
        group
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            .MapGet("/ping", (HttpContext context) =>
            {
                var workload = context.Request.Query["workload"].ToString();
                return workload;
            })
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