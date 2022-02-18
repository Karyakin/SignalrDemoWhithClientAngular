using System;

namespace SignalrDemo.EFModels
{
    public class Connections
    {
        public Guid Id { get; set; }
        public Guid personId { get; set; }
        public Person Person { get; set; }
        public string SignalrId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
