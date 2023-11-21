using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CinemaTicketHub.Startup))]
namespace CinemaTicketHub
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.Use((context, next) =>
            {
                if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                }

                if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Methods"))
                {
                    context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, OPTIONS" });
                }

                if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Headers"))
                {
                    context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Content-Type" });
                }

                return next.Invoke();
            });

        }
    }
}
