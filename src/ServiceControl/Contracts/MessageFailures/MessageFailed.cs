namespace ServiceControl.Contracts.MessageFailures
{
    using System;
    using NServiceBus;
    using Operations;

    public class MessageFailed : IEvent, IPersistableEvent
    {
        public DateTime FailedAt { get; set; }
        
        // IPersistableEvent properties
        public string Id { get; set; }
        public string Endpoint { get; set; }
        public string Machine { get; set; }
        public DateTime RaisedAt { get; set; }
        public string Type { get; set; }
        public string AssociatedMessageId { get; set; }
    }
}
