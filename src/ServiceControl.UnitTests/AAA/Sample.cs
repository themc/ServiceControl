namespace ServiceControl.UnitTests.AAA
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Saga;
    using NServiceBus.Sagas;
    using NServiceBus.Unicast.Behaviors;
    using NServiceBus.Unicast.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class Sample
    {
        [Test]
        public void CheckStartMessage()
        {
            var sagaToTest = Test.Saga<MySaga>();

            sagaToTest.When(new StartedMessage());

            sagaToTest.AssertIsStarted();
        }

        class MySaga : Saga<MySagaData>
        {
            
        }

        class MySagaData : ContainSagaData
        {
        }
    }

    public class StartedMessage
    {
    }

    public class Test
    {
        public static SagaSpecification<T> Saga<T>()
        {
            return new SagaSpecification<T>();
        }
    }

    public class SagaSpecification<T>
    {
     
        public void Given(object message)
        {
            
        }

        public void When<TMessage>(TMessage message)
        {
            ApplyMessageToSaga(message);
        }

        void ApplyMessageToSaga(object message)
        {
            var metadata = new MessageMetadata();
            metadata.MessageType = message.GetType();

            var logicalMessage = new LogicalMessage(metadata, message, new Dictionary<string, string>());

            context = new HandlerInvocationContext(new ReceiveLogicalMessageContext(new RootContext(builder), logicalMessage), new MessageHandler
            {
                Instance = Activator.CreateInstance<T>()
            });

            var sagaPersitenceBehavior = new SagaPersistenceBehavior();

            sagaPersitenceBehavior.Invoke(context, () => { });

            sagaActivated = context.TryGet(out sagaInstance);
        }

        public void AssertIsStarted()
        {
            Assert.True(sagaActivated && !sagaInstance.NotFound);
        }

        HandlerInvocationContext context;

        ActiveSagaInstance sagaInstance;

        FuncBuilder builder = new FuncBuilder();

        bool sagaActivated;
    }
}