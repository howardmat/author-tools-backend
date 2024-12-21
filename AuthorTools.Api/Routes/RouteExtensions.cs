namespace AuthorTools.Api.Routes;

public static class RouteExtensions
{
    public static IApplicationBuilder UseRoutes(this WebApplication app)
    {
        app.MapCharacterRoutes();
        app.MapLocationRoutes();
        app.MapCreatureRoutes();
        app.MapFileRoutes();
        app.MapUserSettingRoutes();

        return app;
    }
}
