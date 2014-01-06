namespace ServiceControl.UnitTests.AAA
{
    using System;
    using System.Linq;
    using NServiceBus;
    using NServiceBus.Saga;
    using NUnit.Framework;

    [TestFixture]
    public class Sample
    {
        [Test]
        public void Should_start_saga()
        {
            var sagaToTest = Test.Saga<MySaga>();

            sagaToTest.When(new StartMessage());

            sagaToTest.AssertIsStarted();
        }
        [Test]
        public void Should_capture_bus_interactions()
        {
            var sagaToTest = Test.Saga<MySaga>();

            sagaToTest.When(new StartMessage());

            sagaToTest.AssertResultingMessages(messages => messages.Any(m=>m.OfType<StartMessage>()));
        }

        class MySaga : Saga<MySagaData>,IAmStartedByMessages<StartMessage>
        {
            public void Handle(StartMessage message)
            {
                Bus.Send(new MessageSentBySaga());
            }
        }

        class MySagaData : ContainSagaData
        {
            public Guid IdToCorrelateOn { get; set; }
        }
    }

    class MessageSentBySaga : IMessage
    {
    }

    public class StartMessage:IMessage
    {
        public Guid IdToCorrelateOn { get; set; }
    }

    public class Test
    {
        public static SagaSpecification<T> Saga<T>()
        {
            return new SagaSpecification<T>();
        }
    }
}