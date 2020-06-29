using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebApplication2.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task DataUpdated(string user, string message)
        {
            await Clients.All.SendAsync("DataUpdated", user, message);
        }
        public async Task DataUpdated2(string user, string message)
        {
            await Clients.All.SendAsync("DataUpdated2", user, message);
        }
    }
}
