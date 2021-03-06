namespace ServiceControl.CompositeViews.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts.Operations;
    using Raven.Client.Indexes;

    public class MessagesViewTransformer : AbstractTransformerCreationTask<MessagesViewIndex.Result>
    {
        public MessagesViewTransformer()
        {
            TransformResults = messages => from message in messages
                let metadata = message.ProcessingAttempts != null ? message.ProcessingAttempts.Last().MessageMetadata : message.MessageMetadata
                let headers = message.ProcessingAttempts != null ? message.ProcessingAttempts.Last().Headers : message.Headers
                select new
                {
                    Id = message.UniqueMessageId,
                    MessageId = metadata["MessageId"],
                    MessageType = metadata["MessageType"],
                    SendingEndpoint = metadata["SendingEndpoint"],
                    ReceivingEndpoint = metadata["ReceivingEndpoint"],
                    TimeSent = (DateTime)metadata["TimeSent"],
                    ProcessedAt = message.ProcessingAttempts != null ? message.ProcessingAttempts.Last().AttemptedAt : message.ProcessedAt,
                    CriticalTime = (TimeSpan)metadata["CriticalTime"],
                    ProcessingTime = (TimeSpan)metadata["ProcessingTime"],
                    DeliveryTime = (TimeSpan)metadata["DeliveryTime"],
                    IsSystemMessage = (bool)metadata["IsSystemMessage"],
                    ConversationId = metadata["ConversationId"],
                    //the reason the we need to use a KeyValuePair<string, object> is that raven seems to interpret the values and convert them
                    // to real types. In this case it was the NServiceBus.Temporary.DelayDeliveryWith header to was converted to a timespan
                    Headers = headers.Select(header => new KeyValuePair<string, object>(header.Key, header.Value)),
                    Status = message.ProcessingAttempts == null ? MessageStatus.Successful : (message.ProcessingAttempts.Count() == 1 ? MessageStatus.Failed : MessageStatus.RepeatedFailure),
                    MessageIntent = metadata["MessageIntent"],
                    BodyUrl = metadata["BodyUrl"],
                    BodySize = (int)metadata["BodySize"],
                    InvokedSagas = metadata["InvokedSagas"],
                    OriginatesFromSaga = metadata["OriginatesFromSaga"]   
                };
        }
    }
}