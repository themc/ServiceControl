namespace ServiceControl.Contracts.HistoricalEvents
{
    using System;
    using NServiceBus;

    // Not sure if we need this.. The UI can simply call the Rest API to clear the alert. Does it need an event back to
    // remove it from the UI?
    public class EventCleared : IEvent
    {
        public string Id { get; set; }
        public DateTime ClearedAt { get; set; }
    }
}
