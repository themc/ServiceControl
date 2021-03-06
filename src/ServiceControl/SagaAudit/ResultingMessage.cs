namespace ServiceControl.SagaAudit
{
    using System;

    public class ResultingMessage
    {
        public TimeSpan? DeliveryDelay { get; set; }
        public string Destination { get; set; }
        public DateTime? DeliverAt { get; set; }
        public string MessageId { get; set; }
        public DateTime TimeSent { get; set; }
        public string MessageType { get; set; }
    }
}