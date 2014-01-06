namespace ServiceControl.UnitTests.MessageFailureTests
{
    using System.Linq;
    using AAA;
    using Contracts.Operations;
    using NServiceBus;
    using NServiceBus.Faults;
    using NUnit.Framework;
    using MessageFailures;
    using MessageFailures.InternalMessages;

    [TestFixture]
    public class When_the_retries_a_failure_that_is_already_resolved
    {
        [Test]
        public void Should_reject_the_retry()
        {
            var policy = Test.Saga<FailedMessagePolicy>();
            var failedMessage = CreateFailedMessage();

            policy.Given(failedMessage);
            var retryAttempt = policy.Given(new RetryMessage{FailedMessageId = failedMessage.UniqueMessageId}).Single();
            
            policy.Given(new RegisterSuccessfulRetry
            {
                FailedMessageId = failedMessage.UniqueMessageId,
                RetryId = retryAttempt.Instance<PerformRetry>().RetryId
            });


            var result = policy.When(new RetryMessage { FailedMessageId = failedMessage.UniqueMessageId });

            Assert.False(result.Any());
        }

        ImportFailedMessage CreateFailedMessage()
        {
            var transportMessage = new TransportMessage();

            transportMessage.Headers[Headers.ProcessingEndpoint] = "server1";

            transportMessage.Headers[FaultsHeaderKeys.FailedQ] = "endpointX@server1";
            
            return new ImportFailedMessage(transportMessage);

        }
    }
}