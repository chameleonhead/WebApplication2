using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace WebApplication2.Hubs
{
    public static class NotificationExtension
    {
        private static NotificationService service;

        public static void UseNotification(this IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            service = new NotificationService(app);
            service.Start();
            applicationLifetime.ApplicationStopping.Register(() => service.Shutdown());
        }
    }
}
