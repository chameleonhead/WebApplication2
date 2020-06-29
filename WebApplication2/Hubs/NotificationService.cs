using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using WebApplication2.Data;

namespace WebApplication2.Hubs
{
    public class NotificationService
    {
        private IServiceScope serviceScope;
        private IHubContext<NotificationHub> hubContext;
        private IConfiguration configuration;

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
            Subscribe();
            Subscribe2();
        }

        private void Subscribe()
        {
            using (var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "select Id, Value from dbo.Messages";
                    command.Notification = null;
                    var dependency = new SqlDependency(command);
                    dependency.OnChange += Dependency_OnChange;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex) when (ex.Errors[0].Number == 9202)
                    {
                        // http://answers.flyppdevportal.com/MVC/Post/Thread/4c367faa-4723-4994-a28b-6b7d2d2e441b?category=sqlservicebroker
                        // Error 9202 は無視しても大丈夫
                    }
                }
            }
        }

        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            hubContext.Clients.All.SendAsync("DataUpdated");

            // OnChangeが呼ばれるごとに新たに購読する必要がある
            Subscribe();
        }

        private void Subscribe2()
        {
            using (var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "select Id, Value from dbo.Message2s";
                    command.Notification = null;
                    var dependency = new SqlDependency(command);
                    dependency.OnChange += Dependency_OnChange2;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex) when (ex.Errors[0].Number == 9202)
                    {
                        // http://answers.flyppdevportal.com/MVC/Post/Thread/4c367faa-4723-4994-a28b-6b7d2d2e441b?category=sqlservicebroker
                        // Error 9202 は無視しても大丈夫
                    }
                }
            }
        }

        private void Dependency_OnChange2(object sender, SqlNotificationEventArgs e)
        {
            hubContext.Clients.All.SendAsync("DataUpdated2");

            // OnChangeが呼ばれるごとに新たに購読する必要がある
            Subscribe2();
        }

        public void Shutdown()
        {
            serviceScope.Dispose();
            SqlDependency.Stop(configuration.GetConnectionString("DefaultConnection"));
        }

    }
}
