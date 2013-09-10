namespace ServiceControl.HistoricalEventsTracking
{
    using Contracts.HeartbeatMonitoring;
    using NServiceBus;

    class EndpointFailedHeartbeatHandler : IHandleMessages<EndpointFailedToHeartbeat>
    {
        public IBus Bus { get; set; }
        public void Handle(EndpointFailedToHeartbeat message)
        {
            // TODO: Create a new EndpointEvent from the event received and store this in raven.
        }
    }
}
