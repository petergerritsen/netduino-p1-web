using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Web {
    public class UsageHub : Hub {
        public Task JoinHub(string apikey) {
            return Groups.Add(Context.ConnectionId, apikey);
        }

        public void SendCurrentUsage(string apikey, DateTime timestamp, decimal currentUsage) {
            Clients.Group(apikey).currentUsage(timestamp, currentUsage);
        }
    }
}