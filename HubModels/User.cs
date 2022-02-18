using System;

namespace SignalrDemo.HubModels
{
    public class User
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string connId { get; set; } //signalrId

        public User(Guid someId, string someName, string someConnId)
        {
            id = someId;
            name = someName;
            connId = someConnId;
        }
    }
}
