namespace AuthorTools.Api.Routes;

public static class RouteExtensions
{
    public static IApplicationBuilder UseRoutes(this WebApplication app)
    {
        app.MapCharacterRoutes();
        app.MapFileRoutes();

        return app;
    }
}
