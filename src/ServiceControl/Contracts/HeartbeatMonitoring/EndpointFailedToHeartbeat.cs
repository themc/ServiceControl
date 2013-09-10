namespace ServiceControl.Contracts.HeartbeatMonitoring
{
    using System;
    using NServiceBus;
    using Operations;

    public class EndpointFailedToHeartbeat : IEvent, IPersistableEvent
    {
        public DateTime LastReceivedAt { get; set; }

        // IPersistableEvent properties
        public string Id { get; set; }
        public string Endpoint { get; set; }
        public string Machine { get; set; }
        public DateTime RaisedAt { get; set; }
        public string Type { get; set; }
        public string AssociatedMessageId { get; set; }
    }
}