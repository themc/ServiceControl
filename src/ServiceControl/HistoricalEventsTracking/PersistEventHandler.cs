namespace ServiceControl.HistoricalEventsTracking
{
    using Contracts.Operations;
    using NServiceBus;

    public class PersistEventHandler : IHandleMessages<IPersistableEvent>
    {
        public void Handle(IPersistableEvent message)
        {
            // TODO: Store message in Raven and expose REST API for the UI to query the historical events.
        }
    }
}
