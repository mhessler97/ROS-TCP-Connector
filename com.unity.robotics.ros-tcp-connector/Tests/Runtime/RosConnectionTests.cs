using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Geometry;
using RosMessageTypes.Tf2;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnitTests
{
    public class RosConnectionTests
    {
        [Test]
        public void GetOrCreateInstance_CallOnce_ReturnsValidInstance()
        {
            ROSConnection ros = ROSConnection.GetOrCreateInstance();
            ros.ConnectOnStart = false;
            Assert.NotNull(ros);
        }

        [Test]
        public void GetOrCreateInstance_CallTwice_ReturnsSameInstance()
        {
            ROSConnection ros = ROSConnection.GetOrCreateInstance();
            Assert.NotNull(ros);
            ros.ConnectOnStart = false;
            ROSConnection ros2 = ROSConnection.GetOrCreateInstance();
            Assert.AreEqual(ros, ros2);
        }

        [Test]
        public void Subscribe_WithLatch_StoresTransientLocalIntentOnTopic()
        {
            ROSConnection ros = ROSConnection.GetOrCreateInstance();
            ros.ConnectOnStart = false;
            const string topic = "/unit_test_latched_subscription";

            ros.Subscribe<TimeMsg>(topic, _ => { }, latch: true);

            Assert.IsTrue(ros.GetTopic(topic).IsSubscriberLatched);
        }

        [Test]
        public void Subscribe_LatchedCallback_UpgradesSharedTopicAndCannotBeDowngraded()
        {
            ROSConnection ros = ROSConnection.GetOrCreateInstance();
            ros.ConnectOnStart = false;
            const string topic = "/unit_test_latched_subscription_upgrade";

            ros.Subscribe<TimeMsg>(topic, _ => { });
            Assert.IsFalse(ros.GetTopic(topic).IsSubscriberLatched);

            ros.Subscribe<TimeMsg>(topic, _ => { }, latch: true);
            Assert.IsTrue(ros.GetTopic(topic).IsSubscriberLatched);

            ros.Subscribe<TimeMsg>(topic, _ => { }, latch: false);
            Assert.IsTrue(ros.GetTopic(topic).IsSubscriberLatched);
        }

        [Test]
        public void Unsubscribe_ClearsLatchedIntentForFutureSubscription()
        {
            ROSConnection ros = ROSConnection.GetOrCreateInstance();
            ros.ConnectOnStart = false;
            const string topic = "/unit_test_latched_subscription_reset";

            ros.Subscribe<TimeMsg>(topic, _ => { }, latch: true);
            ros.Unsubscribe(topic);
            ros.Subscribe<TimeMsg>(topic, _ => { });

            Assert.IsFalse(ros.GetTopic(topic).IsSubscriberLatched);
        }
    }
}
