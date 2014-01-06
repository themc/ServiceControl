namespace ServiceControl.UnitTests.AAA
{
    using System;
    using System.Collections.Generic;
    using NServiceBus;
    using NServiceBus.Features;
    using NServiceBus.Persistence.InMemory.SagaPersister;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Saga;
    using NServiceBus.Sagas;
    using NServiceBus.Unicast;
    using NServiceBus.Unicast.Behaviors;
    using NServiceBus.Unicast.Messages;
    using NUnit.Framework;

    public class SagaSpecification<T> where T:ISaga
    {
        public SagaSpecification()
        {
            Sagas.ConfigureSaga(typeof(T));
        }

        public void Given(object message)
        {

        }

        public void When<TMessage>(TMessage message)
        {
            ApplyMessageToSaga(message);
        }

        void ApplyMessageToSaga(object message)
        {
            HandlerInvocationCache.CacheMethodForHandler(typeof(T),message.GetType());
            var metadata = new MessageMetadata {MessageType = message.GetType()};

            var instance = CreateSagaInstance();

            var logicalMessage = new LogicalMessage(metadata, message, new Dictionary<string, string>());
            var messageHandler = new MessageHandler
            {
                Instance = instance,

                //todo removed when the core has been refactored
                Invocation = (handlerInstance, m) => HandlerInvocationCache.InvokeHandle(handlerInstance, m)
            };

            context = new HandlerInvocationContext(new ReceiveLogicalMessageContext(new RootContext(builder), logicalMessage),messageHandler );

            var sagaPersistenceBehavior = new SagaPersistenceBehavior
            {
                SagaPersister = new InMemorySagaPersister()
            };

            //todo: removed when core has been patched to not use Headers.GetMessageHeader
            ExtensionMethods.GetHeaderAction = (m, key) =>
            {
                string result;

                logicalMessage.Headers.TryGetValue(key, out result);

                return result;
            };
            
            //load saga
            sagaPersistenceBehavior.Invoke(context, () => { });

            var handlerInvocation = new InvokeHandlersBehavior();

            handlerInvocation.Invoke(new HandlerInvocationContext(context,messageHandler),()=>{});


            sagaActivated = context.TryGet(out sagaInstance);
        }

        T CreateSagaInstance()
        {
            var instance = Activator.CreateInstance<T>();

            instance.Bus = new SpyBus(capturedMessages);
            return instance;
        }

        public void AssertIsStarted()
        {
            Assert.True(sagaActivated && !sagaInstance.NotFound);
        }

        HandlerInvocationContext context;

        ActiveSagaInstance sagaInstance;

        FuncBuilder builder = new FuncBuilder();

        bool sagaActivated;

        public void AssertResultingMessages(Predicate<List<CapturedMessage>> predicate)
        {
            Assert.True(predicate(capturedMessages));
        }

        List<CapturedMessage> capturedMessages = new List<CapturedMessage>();
    }

    public class CapturedMessage
    {
        public bool OfType<T>()
        {
            return typeof(T) == metadata.MessageType;
        }

        MessageMetadata metadata;
        readonly object message;

        public CapturedMessage(MessageMetadata metadata, object message)
        {
            this.metadata = metadata;
            this.message = message;
        }
    }
}