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
    using NServiceBus.Sagas.Finders;
    using NServiceBus.Unicast;
    using NServiceBus.Unicast.Behaviors;
    using NServiceBus.Unicast.Messages;
    using NUnit.Framework;

    public class SagaSpecification<T> where T : ISaga,new()
    {
        public SagaSpecification()
        {
            Sagas.Clear();

            sagaPersister = PersisterFactory();

            builder.Register<ISagaPersister>(() => sagaPersister);

            var sagaEntityType = GetSagaEntityType<T>();

            var sagaHeaderIdFinder = typeof(HeaderSagaIdFinder<>).MakeGenericType(sagaEntityType);
            builder.Register(sagaHeaderIdFinder);

            Sagas.ConfigureSaga(typeof(T));
            Sagas.ConfigureFinder(sagaHeaderIdFinder);

            if (Sagas.SagaEntityToMessageToPropertyLookup.ContainsKey(sagaEntityType))
            {
                foreach (var entityLookups in Sagas.SagaEntityToMessageToPropertyLookup[sagaEntityType])
                {
                    var propertyFinder = typeof(PropertySagaFinder<,>).MakeGenericType(sagaEntityType, entityLookups.Key);

                    Sagas.ConfigureFinder(propertyFinder);

                    var propertyLookups = entityLookups.Value;

                    var finder = Activator.CreateInstance(propertyFinder);
                    propertyFinder.GetProperty("SagaProperty").SetValue(finder, propertyLookups.Key,null);
                    propertyFinder.GetProperty("MessageProperty").SetValue(finder, propertyLookups.Value,null);
                    builder.Register(propertyFinder, () => finder);
                }
            }

        }

        static Type GetSagaEntityType<T>() where T : new()
        {
            var sagaType = typeof(T);


            var args = sagaType.BaseType.GetGenericArguments();
            foreach (var type in args)
            {
                if (typeof(IContainSagaData).IsAssignableFrom(type))
                {
                    return type;
                }
            }
            return null;
        }


        public void Given(object message)
        {
            ApplyMessageToSaga(message);
        }

        public void When<TMessage>(TMessage message)
        {
            ApplyMessageToSaga(message);
        }

        void ApplyMessageToSaga(object message)
        {
            HandlerInvocationCache.CacheMethodForHandler(typeof(T), message.GetType());
            var metadata = new MessageMetadata { MessageType = message.GetType() };

            var instance = CreateSagaInstance();

            var logicalMessage = new LogicalMessage(metadata, message, new Dictionary<string, string>());
            var messageHandler = new MessageHandler
            {
                Instance = instance,

                //todo removed when the core has been refactored
                Invocation = (handlerInstance, m) => HandlerInvocationCache.InvokeHandle(handlerInstance, m)
            };

            context = new HandlerInvocationContext(new ReceiveLogicalMessageContext(new RootContext(builder), logicalMessage), messageHandler);

            var sagaPersistenceBehavior = new SagaPersistenceBehavior
            {
                SagaPersister = sagaPersister
            };

            //todo: removed when core has been patched to not use Headers.GetMessageHeader
            ExtensionMethods.GetHeaderAction = (m, key) =>
            {
                string result;

                logicalMessage.Headers.TryGetValue(key, out result);

                return result;
            };

            //load saga
            sagaPersistenceBehavior.Invoke(context, () =>
            {
                var handlerInvocation = new InvokeHandlersBehavior();

                sagaActivated = context.TryGet(out sagaInstance);

                //only pass the message to the saga if it was actually found
                if (sagaInstance != null && !sagaInstance.NotFound)
                {
                    handlerInvocation.Invoke(new HandlerInvocationContext(context, messageHandler), () => { });
                }
            });

            
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

        ISagaPersister sagaPersister;

        public Func<ISagaPersister> PersisterFactory = () => new InMemorySagaPersister();

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