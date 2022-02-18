using System;

namespace SignalrDemo.EFModels
{
    public class Person
    {
        public Guid Id { get; set; }
        public string personName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
