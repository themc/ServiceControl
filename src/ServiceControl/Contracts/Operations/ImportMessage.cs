﻿namespace ServiceControl.Contracts.Operations
{
    using System.Collections.Generic;
    using System.Linq;
    using NServiceBus;

    public class ImportMessage : IMessage
    {
        protected ImportMessage(TransportMessage message)
        {
            UniqueMessageId = message.UniqueId();
            MessageId = message.Id;
            
            PhysicalMessage = new PhysicalMessage(message);
        
            Metadata = new Dictionary<string, object>
            {
                {"MessageId", message.Id},
                {"MessageIntent", message.MessageIntent},
                {"HeadersForSearching", message.Headers.Select(_ => _.Value).ToArray()}
            };

            //add basic message metadata
        }

        public string UniqueMessageId { get; set; }
        public string MessageId { get; set; }

        public PhysicalMessage PhysicalMessage { get; set; }

        public Dictionary<string, object> Metadata { get; set; }
    }
}