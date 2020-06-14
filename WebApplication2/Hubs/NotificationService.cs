using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Sql;
using System.Data.SqlClient;
using WebApplication2.Data;

namespace WebApplication2.Hubs
{
    public class NotificationService
    {
        private SqlDependency dependency;
        private IServiceScope serviceScope;
        private IHubContext<NotificationHub> hubContext;
        private IConfiguration configuration;
        private SqlConnection conn;

        public NotificationService(IApplicationBuilder app)
        {
            this.serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
        }

        public void Start()
        {
            hubContext = serviceScope.ServiceProvider.GetService<IHubContext<NotificationHub>>();
            configuration = serviceScope.ServiceProvider.GetService<IConfiguration>();
            using (var context = serviceScope.ServiceProvider.GetService<MessageContext>())
            {
                context.Database.EnsureCreated();
            }

            SqlDependency.Start(configuration.GetConnectionString("DefaultConnection"));
            conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = conn.CreateCommand();
            this.dependency = new SqlDependency(cmd);
            cmd.CommandText = "select * from Messages";
            cmd.ExecuteNonQuery();
            dependency.AddCommandDependency(cmd);
            dependency.OnChange += Dependency_OnChange;
        }

        public void Shutdown()
        {
            dependency.OnChange -= Dependency_OnChange;
            conn.Dispose();
            serviceScope.Dispose();
            SqlDependency.Stop(configuration.GetConnectionString("DefaultConnection"));
        }

        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            hubContext.Clients.All.SendAsync("DataUpdated");
        }

    }
}
