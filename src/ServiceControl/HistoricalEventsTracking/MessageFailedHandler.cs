namespace ServiceControl.HistoricalEventsTracking
{
    using Contracts.MessageFailures;
    using NServiceBus;

    public class MessageFailedHandler : IHandleMessages<MessageFailed>
    {
        public void Handle(MessageFailed message)
        {
            // TODO: Create a new EndpointEvent from the event received and store this in raven.
        }
    }
}
