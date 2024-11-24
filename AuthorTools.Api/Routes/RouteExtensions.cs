namespace AuthorTools.Api.Routes;

public static class RouteExtensions
{
    public static IApplicationBuilder UseRoutes(this WebApplication app)
    {
        CharacterRoutes.Add(app);
        FileRoutes.Add(app);

        return app;
    }
}
