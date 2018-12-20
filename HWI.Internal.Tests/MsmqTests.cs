using System;
using HWI.Internal.Queueing;
using HWI.Internal.Queueing.Msmq;
using My.Other.Namespace;
using NSubstitute;
using Xunit;

namespace HWI.Internal.Tests
{
    public class MsmqTests
    {
        [Fact]
        public void When_building_queue_path_name_with_appsettings()
        {
            var settings = Substitute.For<IPersistenceSettings>();

            settings.QueueNamePrefix.Returns("qnp");
            settings.QueueLocation.Returns("location\\");

            var subject = new AppSettingsQueuePathBuilder(settings);

            var expected = "location\\qnp.msmqtests";
            var actual = subject.BuildMsmqPath(GetType());

            Assert.Equal(expected, actual);

        }

        [Fact]
        public void When_building_queue_path_name_with_simple_and_same_namespace()
        {
            var subject = new SimpleQueuePathBuilder();

            var expected = "hwi.internal.tests.msmqtests";
            var actual = subject.BuildMsmqPath(GetType());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void When_building_queue_path_name_with_simple_and_different_namespace()
        {
            var subject = new SimpleQueuePathBuilder();

            var expected = "hwi.internal.tests.testmessage";
            var actual = subject.BuildMsmqPath(typeof(TestMessage));

            Assert.Equal(expected, actual);
        }
    }
}

namespace My.Other.Namespace
{
    public class TestMessage { }
}