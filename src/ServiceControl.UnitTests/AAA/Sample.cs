namespace ServiceControl.UnitTests.AAA
{
    using System;
    using System.Linq;
    using NServiceBus;
    using NServiceBus.Persistence.InMemory.SagaPersister;
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

            sagaToTest.AssertResultingMessages(messages => messages.Any(m=>m.OfType<MessageSentBySaga>()));
        }

        [Test]
        public void Should_be_strict_about_correlation()
        {
            var correlationId = Guid.NewGuid();

            var sagaToTest = Test.Saga<MySaga>();

            sagaToTest.Given(new StartMessage
            {
                IdToCorrelateOn = correlationId
            });

            sagaToTest.When(new MessageThatCorrelatesToSaga
            {
                IdToCorrelateOn = correlationId
            });

            sagaToTest.AssertResultingMessages(messages => messages.Any(m => m.OfType<MessageSentAsAResultOfTheMessageBeingCorrelated>()));
        }



        class MySaga : Saga<MySaga.MySagaData>,
            IAmStartedByMessages<StartMessage>,
            IHandleMessages<MessageThatCorrelatesToSaga>
        {
            public void Handle(StartMessage message)
            {
                Data.IdToCorrelateOn = message.IdToCorrelateOn;
                Bus.Send(new MessageSentBySaga());
            }

            public void Handle(MessageThatCorrelatesToSaga message)
            {
                Assert.AreEqual(Data.IdToCorrelateOn,message.IdToCorrelateOn);
                Bus.Send(new MessageSentAsAResultOfTheMessageBeingCorrelated());
            }

            public override void ConfigureHowToFindSaga()
            {
                ConfigureMapping<MessageThatCorrelatesToSaga>(m=>m.IdToCorrelateOn).ToSaga(s=>s.IdToCorrelateOn);
            }

            internal class MySagaData : ContainSagaData
            {
                [Unique]
                public Guid IdToCorrelateOn { get; set; }
            }
        }
    }



    class MessageSentAsAResultOfTheMessageBeingCorrelated : IMessage
    {
    }

    public class MessageThatCorrelatesToSaga:IMessage
    {
        public Guid IdToCorrelateOn { get; set; }
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
        public static SagaSpecification<T> Saga<T>() where T:ISaga,new()
        {
            return new SagaSpecification<T>();
        }
    }
}