namespace ServiceControl.HistoricalEventsTracking
{
    using System;
    using NServiceBus;
    using ServiceControl.Contracts.BusinessMonitoring;

    public class EndpointSLABreachedHandler : IHandleMessages<EndpointSLABreached>
    {
        public void Handle(EndpointSLABreached message)
        {
            // TODO: Create a new EndpointEvent from the event received and store this in raven.
            Console.Out.WriteLine("Demo - SLA breached for endpoint {0}", message.Endpoint);
        }
    }
}