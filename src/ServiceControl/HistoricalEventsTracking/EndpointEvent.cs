namespace ServiceControl.HistoricalEventsTracking
{
    using System;
    using Contracts.HistoricalEvents;

    public class EndpointEvent : IEventReceived
    {
        public string Id { get; set; }
        public DateTime RaisedAt { get; set; }
        public string Endpoint { get; set; }
        public string Machine { get; set; }
        public string Type { get; set; } // actual type name -- EndpointFailedToHearbeat or failedmessages
        public string AssociatedMessageId { get; set; } // Id of the document either failed message in errq or hearbeat failed. 
    }
}
