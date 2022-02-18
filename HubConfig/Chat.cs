using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalrDemo.HubModels;

namespace SignalrDemo.HubConfig
{
    public partial class MyHub
    {
        public async Task getOnlineUsers()
        {
            var currUserId = _dbContext.Connections.Where(c => c.SignalrId == Context.ConnectionId).Select(c => c.personId).SingleOrDefault();
            var onlineUsers = _dbContext.Connections
                 .Where(c => c.personId != currUserId)
                 .Select(c =>
                     new User(c.personId, _dbContext.Persons
                         .Where(p => p.Id == c.personId)
                         .Select(p => p.Username)
                         .SingleOrDefault(), c.SignalrId)
                 ).ToList();
            await Clients.Caller.SendAsync("getOnlineUsersResponse", onlineUsers);
        }

        public async Task sendMsg(string connId, string msg)
        {
            await Clients.Client(connId).SendAsync("sendMsgResponse", Context.ConnectionId, msg);
        }
    }
}
