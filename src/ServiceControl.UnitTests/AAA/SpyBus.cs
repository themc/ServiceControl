namespace ServiceControl.UnitTests.AAA
{
    using System;
    using System.Collections.Generic;
    using NServiceBus;
    using NServiceBus.Unicast;
    using NServiceBus.Unicast.Messages;

    class SpyBus : IBus
    {
        readonly List<CapturedMessage> capturedMessages;

        public SpyBus(List<CapturedMessage> capturedMessages)
        {
            this.capturedMessages = capturedMessages;
        }

        public T CreateInstance<T>()
        {
            throw new NotImplementedException();
        }

        public T CreateInstance<T>(Action<T> action)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(Type messageType)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(params T[] messages)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(T message)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>()
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(Action<T> messageConstructor)
        {
            //todo: support interfaces
            var instance = Activator.CreateInstance<T>();

            Capture(instance,typeof(T))
            ;
        }

        public void Subscribe(Type messageType)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Type messageType, Predicate<object> condition)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>(Predicate<T> condition)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Type messageType)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T>()
        {
            throw new NotImplementedException();
        }

        public ICallback SendLocal(params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback SendLocal(object message)
        {
            return Capture(message, message.GetType());
        }

        public ICallback SendLocal<T>(Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(object message)
        {
            return Capture(message,message.GetType());
        }

        ICallback Capture(object message,Type messageType,MessageIntentEnum intent = MessageIntentEnum.Send)
        {
            var metadata = new MessageMetadata { MessageType = messageType };

            capturedMessages.Add(new CapturedMessage(metadata, message, intent));

            return new Callback(Guid.NewGuid().ToString());
        }

        public ICallback Send<T>(Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(string destination, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(string destination, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(Address address, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(Address address, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(string destination, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(Address address, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(string destination, string correlationId, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(string destination, string correlationId, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(Address address, string correlationId, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(Address address, string correlationId, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(string destination, string correlationId, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(Address address, string correlationId, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback SendToSites(IEnumerable<string> siteKeys, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback SendToSites(IEnumerable<string> siteKeys, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Defer(TimeSpan delay, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Defer(TimeSpan delay, object message)
        {
            throw new NotImplementedException();
        }

        public ICallback Defer(DateTime processAt, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Defer(DateTime processAt, object message)
        {
            throw new NotImplementedException();
        }

        public void Reply(params object[] messages)
        {
            throw new NotImplementedException();
        }

        public void Reply(object message)
        {
            throw new NotImplementedException();
        }

        public void Reply<T>(Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public void Return<T>(T errorEnum)
        {
            throw new NotImplementedException();
        }

        public void HandleCurrentMessageLater()
        {
            throw new NotImplementedException();
        }

        public void ForwardCurrentMessageTo(string destination)
        {
            throw new NotImplementedException();
        }

        public void DoNotContinueDispatchingCurrentMessageToHandlers()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> OutgoingHeaders { get; private set; }
        public IMessageContext CurrentMessageContext { get; private set; }
        public IInMemoryOperations InMemory { get; private set; }
    }
}