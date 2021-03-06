﻿namespace ServiceControl.Plugin.SagaAudit
{
    using System;
    using System.Linq;
    using EndpointPlugin.Messages.SagaState;
    using NServiceBus;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;

    // ReSharper disable CSharpWarnings::CS0618
    class CaptureSagaResultingMessagesBehavior : IBehavior<SendPhysicalMessageContext>
    {
        SagaUpdatedMessage sagaUpdatedMessage;

        public void Invoke(SendPhysicalMessageContext context, Action next)
        {
            AppendMessageToState(context);
            next();
        }

        void AppendMessageToState(SendPhysicalMessageContext context)
        {
            if (!context.TryGet(out sagaUpdatedMessage))
            {
                return;
            }
            var messages = context.LogicalMessages.ToList();
            if (messages.Count > 1)
            {
                throw new Exception("The SagaAuditing plugin does not support batch messages.");
            }
            if (messages.Count == 0)
            {
                //this can happen on control messages
                return;
            }
            var logicalMessage = messages.First();

            var sagaResultingMessage = new SagaChangeOutput
                {
                    ResultingMessageId = context.MessageToSend.Id,
                    TimeSent = DateTimeExtensions.ToUtcDateTime(context.MessageToSend.Headers[Headers.TimeSent]),
                    MessageType = logicalMessage.MessageType.ToString(),
                    DeliveryDelay = context.SendOptions.DelayDeliveryWith,
                    DeliveryAt = context.SendOptions.DeliverAt,
                    Destination = GetDestination(context)
                };
            sagaUpdatedMessage.ResultingMessages.Add(sagaResultingMessage);
        }

        static string GetDestination(SendPhysicalMessageContext context)
        {
            // Destination can be null for publish events
            if (context.SendOptions.Destination != null)
            {
                return context.SendOptions.Destination.ToString();
            }
            return null;
        }
    }
}