﻿using NUnit.Framework;
using PubnubApi;
using PubnubApi.EventEngine.Core;
using PubnubApi.EventEngine.Subscribe.Common;
using PubnubApi.EventEngine.Subscribe.Context;
using PubnubApi.EventEngine.Subscribe.Events;
using PubnubApi.EventEngine.Subscribe.States;
using System.Linq;

namespace PubNubMessaging.Tests
{
    [TestFixture]
    public class EventEngineTests
    {
        [SetUp]
        public void Init()
        {
            // Method intentionally left empty.
        }

        //Create unit tests for each state transition and event 
        //to make sure that the transition is correct
        [Test]
        public void TestUnsubscribedStateTransitionWithSubscriptionChangedEvent()
        {
            //Arrange
            State unsubscribeState = new UnsubscribedState() { Channels = new string[] { "ch1", "ch2" }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };   
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = unsubscribeState.Transition(new SubscriptionChangedEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(PNReconnectionPolicy.LINEAR, ((HandshakingState)(result.State)).ReconnectionConfiguration.ReconnectionPolicy);
            Assert.AreEqual(50, ((HandshakingState)(result.State)).ReconnectionConfiguration.MaximumReconnectionRetries);
        }
        
        [Test]
        public void TestUnsubscribedStateTransitionWithSubscriptionRestoreEvent()
        {
            //Arrange
            State unsubscribeState = new UnsubscribedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receivingState = new ReceivingState();
            //Act
            TransitionResult result = unsubscribeState.Transition(new SubscriptionRestoredEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receivingState.GetType()));
            Assert.AreEqual("ch1", ((ReceivingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceivingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceivingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceivingState)(result.State)).Cursor.Timetoken);
            Assert.AreEqual(PNReconnectionPolicy.LINEAR, ((ReceivingState)(result.State)).ReconnectionConfiguration.ReconnectionPolicy);
            Assert.AreEqual(50, ((ReceivingState)(result.State)).ReconnectionConfiguration.MaximumReconnectionRetries);
        }

        [Test]
        public void TestHandshakingStateTransitionWithSubscriptionRestoredEvent()
        {
            //Arrange
            State handshakingState = new HandshakingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State handshakingState2 = new HandshakingState();
            //Act
            TransitionResult result = handshakingState.Transition(new SubscriptionRestoredEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState2.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((HandshakingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((HandshakingState)(result.State)).Cursor.Timetoken);
            Assert.AreEqual(PNReconnectionPolicy.LINEAR, ((HandshakingState)(result.State)).ReconnectionConfiguration.ReconnectionPolicy);
            Assert.AreEqual(50, ((HandshakingState)(result.State)).ReconnectionConfiguration.MaximumReconnectionRetries);
        }

        [Test]
        public void TestHandshakingStateTransitionWithWithSubscriptionChangedEvent()
        {
            //Arrange
            State handshakingState = new HandshakingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State handshakingState2 = new HandshakingState();
            //Act
            TransitionResult result = handshakingState.Transition(new SubscriptionChangedEvent()
            {
                Channels = new string[] { "ch1", "ch2", "ch3" },
                ChannelGroups = new string[] { "cg1", "cg2", "cg3" }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState2.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("ch3", ((HandshakingState)(result.State)).Channels.ElementAt(2));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual("cg3", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(2));
            Assert.AreEqual(PNReconnectionPolicy.LINEAR, ((HandshakingState)(result.State)).ReconnectionConfiguration.ReconnectionPolicy);
            Assert.AreEqual(50, ((HandshakingState)(result.State)).ReconnectionConfiguration.MaximumReconnectionRetries);
        }

        [Test]
        public void TestHandshakingStateTransitionWithHandshakeFailureEvent()
        {
            //Arrange
            State handshakingState = new HandshakingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State handshakeReconnectingState = new HandshakeReconnectingState();
            //Act
            TransitionResult result = handshakingState.Transition(new HandshakeFailureEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakeReconnectingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakeReconnectingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakeReconnectingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakeReconnectingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakeReconnectingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(PNReconnectionPolicy.LINEAR, ((HandshakeReconnectingState)(result.State)).ReconnectionConfiguration.ReconnectionPolicy);
            Assert.AreEqual(50, ((HandshakeReconnectingState)(result.State)).ReconnectionConfiguration.MaximumReconnectionRetries);
        }

        [Test]
        public void TestHandshakingStateTransitionWithDisconnectEvent() 
        {
            //Arrange
            State handshakingState = new HandshakingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State handshakeStoppedState = new HandshakeStoppedState();
            //Act
            TransitionResult result = handshakingState.Transition(new DisconnectEvent() 
            { 
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakeStoppedState.GetType()));
            Assert.AreEqual("ch1", ((HandshakeStoppedState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakeStoppedState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakeStoppedState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakeStoppedState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(PNReconnectionPolicy.LINEAR, ((HandshakeStoppedState)(result.State)).ReconnectionConfiguration.ReconnectionPolicy);
            Assert.AreEqual(50, ((HandshakeStoppedState)(result.State)).ReconnectionConfiguration.MaximumReconnectionRetries);
        }

        [Test]
        public void TestHandshakingStateTransitionWithHandshakeSuccessEvent()
        {
            //Arrange
            State handshakingState = new HandshakingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receivingState = new ReceivingState();
            //Act
            TransitionResult result = handshakingState.Transition(new HandshakeSuccessEvent() 
            { 
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receivingState.GetType()));
            Assert.AreEqual("ch1", ((ReceivingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceivingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(PNReconnectionPolicy.LINEAR, ((ReceivingState)(result.State)).ReconnectionConfiguration.ReconnectionPolicy);
            Assert.AreEqual(50, ((ReceivingState)(result.State)).ReconnectionConfiguration.MaximumReconnectionRetries);
        }

        [Test]
        public void TestHandshakingStateTransitionWithUnsubscribeEvent()
        {
            //Arrange
            State handshakingState = new HandshakingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State unsubscribedState = new UnsubscribedState();
            //Act
            TransitionResult result = handshakingState.Transition(new UnsubscribeAllEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(unsubscribedState.GetType()));
        }

        [Test]
        public void TestHandshakeReconnectingStateTransitionWithSubscriptionChangedEvent() 
        {
            //Arrange
            State handshakeReconnectingState = new HandshakeReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = handshakeReconnectingState.Transition(new SubscriptionChangedEvent() 
            { 
                Channels = new string[] { "ch1", "ch2", "ch3" },
                ChannelGroups = new string[] { "cg1", "cg2", "cg3" }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("ch3", ((HandshakingState)(result.State)).Channels.ElementAt(2));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual("cg3", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(2));
        }

        [Test]
        public void TestHandshakeReconnectingStateTransitionWithSubscriptionRestoredEvent() 
        {
            //Arrange
            State handshakeReconnectingState = new HandshakeReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = handshakeReconnectingState.Transition(new SubscriptionRestoredEvent() 
            { 
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
        }

        [Test]
        public void TestHandshakeReconnectingStateTransitionWithHandshakeReconnectFailureEvent()
        {
            //Arrange
            State handshakeReconnectingState = new HandshakeReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakeReconnectingState2 = new HandshakeReconnectingState();
            //Act
            TransitionResult result = handshakeReconnectingState.Transition(new HandshakeReconnectFailureEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakeReconnectingState2.GetType()));
            Assert.AreEqual("ch1", ((HandshakeReconnectingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakeReconnectingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakeReconnectingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakeReconnectingState)(result.State)).ChannelGroups.ElementAt(1));
        }

        [Test]
        public void TestHandshakeReconnectingStateTransitionWithDisconnectEvent()
        {
            //Arrange
            State handshakeReconnectingState = new HandshakeReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakeStoppedState = new HandshakeStoppedState();
            //Act
            TransitionResult result = handshakeReconnectingState.Transition(new DisconnectEvent() 
            { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } } );
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakeStoppedState.GetType()));
            Assert.AreEqual("ch1", ((HandshakeStoppedState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakeStoppedState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakeStoppedState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakeStoppedState)(result.State)).ChannelGroups.ElementAt(1));
        }

        [Test]
        public void TestHandshakeReconnectingStateTransitionWithHandshakeReconnectGiveupEvent() 
        {
            //Arrange
            State handshakeReconnectingState = new HandshakeReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakeFailedState = new HandshakeFailedState();
            //Act
            TransitionResult result = handshakeReconnectingState.Transition(new HandshakeReconnectGiveUpEvent() { } );
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakeFailedState.GetType()));
            Assert.AreEqual("ch1", ((HandshakeFailedState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakeFailedState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakeFailedState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakeFailedState)(result.State)).ChannelGroups.ElementAt(1));
        }

        [Test]
        public void TestHandshakeReconnectingStateTransitionWithHandshakeReconnectSuccessEvent()
        {
            //Arrange
            State handshakeReconnectingState = new HandshakeReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receivingState = new ReceivingState();
            //Act
            TransitionResult result = handshakeReconnectingState.Transition(new HandshakeReconnectSuccessEvent() 
            {
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 },
            } );
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receivingState.GetType()));
            Assert.AreEqual("ch1", ((ReceivingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceivingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceivingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceivingState)(result.State)).Cursor.Timetoken);
            Assert.AreEqual(PNReconnectionPolicy.LINEAR, ((ReceivingState)(result.State)).ReconnectionConfiguration.ReconnectionPolicy);
            Assert.AreEqual(50, ((ReceivingState)(result.State)).ReconnectionConfiguration.MaximumReconnectionRetries);
        }

        [Test]
        public void TestHandshakeReconnectingStateTransitionWithUnsubscribeEvent()
        {
            //Arrange
            State handshakeReconnectingState = new HandshakeReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State unsubscribedState = new UnsubscribedState();
            //Act
            TransitionResult result = handshakeReconnectingState.Transition(new UnsubscribeAllEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(unsubscribedState.GetType()));
        }

        [Test]
        public void TestHandshakeFailedStateTransitionWithSubscriptionChangedEvent()
        {
            //Arrange
            State handshakeFailedState = new HandshakeFailedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = handshakeFailedState.Transition(new SubscriptionChangedEvent()
            {
                Channels = new string[] { "ch1", "ch2", "ch3" },
                ChannelGroups = new string[] { "cg1", "cg2", "cg3" }
            } );
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("ch3", ((HandshakingState)(result.State)).Channels.ElementAt(2));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual("cg3", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(2));
        }

        [Test]
        public void TestHandshakeFailedStateTransitionWithSubscriptionRestoredEvent()
        {
            //Arrange
            State handshakeFailedState = new HandshakeFailedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = handshakeFailedState.Transition(new SubscriptionRestoredEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" }
            } );
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
        }

        [Test]
        public void TestHandshakeFailedStateTransitionWithReconnectEvent()
        {
            //Arrange
            State handshakeFailedState = new HandshakeFailedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = handshakeFailedState.Transition(new ReconnectEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            } );
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((HandshakingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((HandshakingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestHandshakeFailedStateTransitionWithUnsubscribeEvent()
        {
            //Arrange
            State handshakeFailedState = new HandshakeFailedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State unsubscribedState = new UnsubscribedState();
            //Act
            TransitionResult result = handshakeFailedState.Transition(new UnsubscribeAllEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(unsubscribedState.GetType()));
        }

        [Test]
        public void TestHandshakeStoppedStateTransitionWithSubscriptionChangedEvent()
        {
            //Arrange
            State handshakeStoppedState = new HandshakeStoppedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = handshakeStoppedState.Transition(new SubscriptionChangedEvent()
            {
                Channels = new string[] { "ch1", "ch2", "ch3" },
                ChannelGroups = new string[] { "cg1", "cg2", "cg3" }
            } );
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("ch3", ((HandshakingState)(result.State)).Channels.ElementAt(2));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual("cg3", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(2));
        }

        [Test]
        public void TestHandshakeStoppedStateTransitionWithSubscriptionRestoredEvent()
        {
            //Arrange
            State handshakeStoppedState = new HandshakeStoppedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = handshakeStoppedState.Transition(new SubscriptionRestoredEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" }
            } );
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
        }

        [Test]
        public void TestHandshakeStoppedStateTransitionWithReconnectEvent()
        {
            //Arrange
            State handshakeStoppedState = new HandshakeStoppedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = handshakeStoppedState.Transition(new ReconnectEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            } );
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((HandshakingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((HandshakingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestHandshakeStoppedStateWithUnsubscribeEvent()
        {
            //Arrange
            State handshakeStoppedState = new HandshakeStoppedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" } };
            State unsubscribedState = new UnsubscribedState();
            //Act
            TransitionResult result = handshakeStoppedState.Transition(new UnsubscribeAllEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(unsubscribedState.GetType()));
        }

        [Test]
        public void TestReceivingStateTransitionWithSubscriptionChangedEvent()
        {
            //Arrange
            State receivingState = new ReceivingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receivingState2 = new ReceivingState();
            //Act
            TransitionResult result = receivingState.Transition(new SubscriptionChangedEvent()
            {
                Channels = new string[] { "ch1", "ch2", "ch3" },
                ChannelGroups = new string[] { "cg1", "cg2", "cg3" }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receivingState2.GetType()));
            Assert.AreEqual("ch1", ((ReceivingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceivingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("ch3", ((ReceivingState)(result.State)).Channels.ElementAt(2));
            Assert.AreEqual("cg1", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual("cg3", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(2));
            Assert.AreEqual(1, ((ReceivingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceivingState)(result.State)).Cursor.Timetoken);
            Assert.AreEqual(PNReconnectionPolicy.LINEAR, ((ReceivingState)(result.State)).ReconnectionConfiguration.ReconnectionPolicy);
            Assert.AreEqual(50, ((ReceivingState)(result.State)).ReconnectionConfiguration.MaximumReconnectionRetries);
        }

        [Test]
        public void TestReceivingStateTransitionWithSubscriptionRestoredEvent()
        {
            //Arrange
            State receivingState = new ReceivingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receivingState2 = new ReceivingState();
            //Act
            TransitionResult result = receivingState.Transition(new SubscriptionChangedEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receivingState2.GetType()));
            Assert.AreEqual("ch1", ((ReceivingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceivingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceivingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceivingState)(result.State)).Cursor.Timetoken);
            Assert.AreEqual(PNReconnectionPolicy.LINEAR, ((ReceivingState)(result.State)).ReconnectionConfiguration.ReconnectionPolicy);
            Assert.AreEqual(50, ((ReceivingState)(result.State)).ReconnectionConfiguration.MaximumReconnectionRetries);
        }

        [Test]
        public void TestReceivingStateTransitionWithDisconnectEvent()
        {
            //Arrange
            State receivingState = new ReceivingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receiveStoppedState = new ReceiveStoppedState();
            //Act
            TransitionResult result = receivingState.Transition(new DisconnectEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receiveStoppedState.GetType()));
            Assert.AreEqual("ch1", ((ReceiveStoppedState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceiveStoppedState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceiveStoppedState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceiveStoppedState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceiveStoppedState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceiveStoppedState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceivingStateTransitionWithReceiveFailureEvent()
        {
            //Arrange
            State receivingState = new ReceivingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receiveReconnectingState = new ReceiveReconnectingState();
            //Act
            TransitionResult result = receivingState.Transition(new ReceiveFailureEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receiveReconnectingState.GetType()));
            Assert.AreEqual("ch1", ((ReceiveReconnectingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceiveReconnectingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceiveReconnectingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceiveReconnectingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceiveReconnectingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceiveReconnectingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceivingStateTransitionWithReceiveSuccessEvent()
        {
            //Arrange
            State receivingState = new ReceivingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receivingState2 = new ReceivingState();
            //Act
            TransitionResult result = receivingState.Transition(new ReceiveSuccessEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receivingState2.GetType()));
            Assert.AreEqual("ch1", ((ReceivingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceivingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceivingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceivingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceivingStateTransitionWithUnsubscribeEvent()
        {
            //Arrange
            State receivingState = new ReceivingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State unsubscribedState = new UnsubscribedState();
            //Act
            TransitionResult result = receivingState.Transition(new UnsubscribeAllEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(unsubscribedState.GetType()));
        }

        [Test]
        public void TestReceiveReconnectingStateTransitionWithSubscriptionChangedEvent()
        {
            //Arrange
            State receiveReconnectingState = new ReceiveReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receivingState = new ReceivingState();
            //Act
            TransitionResult result = receiveReconnectingState.Transition(new SubscriptionChangedEvent()
            {
                Channels = new string[] { "ch1", "ch2", "ch3" },
                ChannelGroups = new string[] { "cg1", "cg2", "cg3" }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receivingState.GetType()));
            Assert.AreEqual("ch1", ((ReceivingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceivingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("ch3", ((ReceivingState)(result.State)).Channels.ElementAt(2));
            Assert.AreEqual("cg1", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual("cg3", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(2));
            Assert.AreEqual(1, ((ReceivingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceivingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveReconnectingStateTransitionWithSubscriptionRestoredEvent()
        {
            //Arrange
            State receiveReconnectingState = new ReceiveReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receivingState = new ReceivingState();
            //Act
            TransitionResult result = receiveReconnectingState.Transition(new SubscriptionRestoredEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receivingState.GetType()));
            Assert.AreEqual("ch1", ((ReceivingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceivingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceivingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceivingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveReconnectingStateTransitionWithReceiveReconnectFailureEvent()
        {
            //Arrange
            State receiveReconnectingState = new ReceiveReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receiveReconnectingState2 = new ReceiveReconnectingState();
            //Act
            TransitionResult result = receiveReconnectingState.Transition(new ReceiveReconnectFailureEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receiveReconnectingState2.GetType()));
            Assert.AreEqual("ch1", ((ReceiveReconnectingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceiveReconnectingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceiveReconnectingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceiveReconnectingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceiveReconnectingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceiveReconnectingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveReconnectingStateTransitionWithReceiveReconnectSuccessEvent()
        {
            //Arrange
            State receiveReconnectingState = new ReceiveReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receivingState = new ReceivingState();
            //Act
            TransitionResult result = receiveReconnectingState.Transition(new ReceiveReconnectSuccessEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receivingState.GetType()));
            Assert.AreEqual("ch1", ((ReceivingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceivingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceivingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceivingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceivingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveReconnectingStateTransitionWithDisconnectEvent()
        {
            //Arrange
            State receiveReconnectingState = new ReceiveReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receiveStoppedState = new ReceiveStoppedState();
            //Act
            TransitionResult result = receiveReconnectingState.Transition(new DisconnectEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receiveStoppedState.GetType()));
            Assert.AreEqual("ch1", ((ReceiveStoppedState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceiveStoppedState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceiveStoppedState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceiveStoppedState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceiveStoppedState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceiveStoppedState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveReconnectingStateTransitionWithReceiveReconnectGiveup()
        {
            //Arrange
            State receiveReconnectingState = new ReceiveReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State receiveFailedState = new ReceiveFailedState();
            //Act
            TransitionResult result = receiveReconnectingState.Transition(new ReceiveReconnectGiveUpEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receiveFailedState.GetType()));
            Assert.AreEqual("ch1", ((ReceiveFailedState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceiveFailedState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceiveFailedState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceiveFailedState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceiveFailedState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceiveFailedState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveReconnectingStateTransitionWithUnsubscribeEvent()
        {
            //Arrange
            State receiveReconnectingState = new ReceiveReconnectingState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State unsubscribedState = new UnsubscribedState();
            //Act
            TransitionResult result = receiveReconnectingState.Transition(new UnsubscribeAllEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(unsubscribedState.GetType()));
        }

        [Test]
        public void TestReceiveFailedStateTransitionWithSubscriptionChangedEvent()
        {
            //Arrange
            State receiveFailedState = new ReceiveFailedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = receiveFailedState.Transition(new SubscriptionChangedEvent()
            {
                Channels = new string[] { "ch1", "ch2", "ch3" },
                ChannelGroups = new string[] { "cg1", "cg2", "cg3" }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("ch3", ((HandshakingState)(result.State)).Channels.ElementAt(2));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual("cg3", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(2));
            Assert.AreEqual(1, ((HandshakingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((HandshakingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveFailedStateTransitionWithSubscriptionRestoredEvent()
        {
            //Arrange
            State receiveFailedState = new ReceiveFailedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = receiveFailedState.Transition(new SubscriptionRestoredEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((HandshakingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((HandshakingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveFailedStateTransitionWithReconnectEvent()
        {
            //Arrange
            State receiveFailedState = new ReceiveFailedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = receiveFailedState.Transition(new ReconnectEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((HandshakingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((HandshakingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveFailedStateTransitionWithUnsubscribeEvent()
        {
            //Arrange
            State receiveFailedState = new ReceiveFailedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }, ReconnectionConfiguration = new ReconnectionConfiguration(PNReconnectionPolicy.LINEAR, 50) };
            State unsubscribedState = new UnsubscribedState();
            //Act
            TransitionResult result = receiveFailedState.Transition(new UnsubscribeAllEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(unsubscribedState.GetType()));
        }

        [Test]
        public void TestReceiveStoppedStateTransitionWithReconnectEvent()
        {
            //Arrange
            State receiveStoppedState = new ReceiveStoppedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 } };
            State handshakingState = new HandshakingState();
            //Act
            TransitionResult result = receiveStoppedState.Transition(new ReconnectEvent()
            {
                Channels = new string[] { "ch1", "ch2", "ch3" },
                ChannelGroups = new string[] { "cg1", "cg2", "cg3" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(handshakingState.GetType()));
            Assert.AreEqual("ch1", ((HandshakingState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((HandshakingState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("ch3", ((HandshakingState)(result.State)).Channels.ElementAt(2));
            Assert.AreEqual("cg1", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual("cg3", ((HandshakingState)(result.State)).ChannelGroups.ElementAt(2));
            Assert.AreEqual(1, ((HandshakingState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((HandshakingState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveStoppedStateTransitionWithSubscriptionChangedEvent()
        {
            //Arrange
            State receiveStoppedState = new ReceiveStoppedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 } };
            State receiveStoppedState2 = new ReceiveStoppedState();
            //Act
            TransitionResult result = receiveStoppedState.Transition(new SubscriptionChangedEvent()
            {
                Channels = new string[] { "ch1", "ch2", "ch3" },
                ChannelGroups = new string[] { "cg1", "cg2", "cg3" }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receiveStoppedState2.GetType()));
            Assert.AreEqual("ch1", ((ReceiveStoppedState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceiveStoppedState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("ch3", ((ReceiveStoppedState)(result.State)).Channels.ElementAt(2));
            Assert.AreEqual("cg1", ((ReceiveStoppedState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceiveStoppedState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual("cg3", ((ReceiveStoppedState)(result.State)).ChannelGroups.ElementAt(2));
            Assert.AreEqual(1, ((ReceiveStoppedState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceiveStoppedState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveStoppedStateTransitionWithSubscriptionRestoredEvent()
        {
            //Arrange
            State receiveStoppedState = new ReceiveStoppedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 } };
            State receiveStoppedState2 = new ReceiveStoppedState();
            //Act
            TransitionResult result = receiveStoppedState.Transition(new SubscriptionRestoredEvent()
            {
                Channels = new string[] { "ch1", "ch2" },
                ChannelGroups = new string[] { "cg1", "cg2" },
                Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 }
            });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(receiveStoppedState2.GetType()));
            Assert.AreEqual("ch1", ((ReceiveStoppedState)(result.State)).Channels.ElementAt(0));
            Assert.AreEqual("ch2", ((ReceiveStoppedState)(result.State)).Channels.ElementAt(1));
            Assert.AreEqual("cg1", ((ReceiveStoppedState)(result.State)).ChannelGroups.ElementAt(0));
            Assert.AreEqual("cg2", ((ReceiveStoppedState)(result.State)).ChannelGroups.ElementAt(1));
            Assert.AreEqual(1, ((ReceiveStoppedState)(result.State)).Cursor.Region);
            Assert.AreEqual(1234567890, ((ReceiveStoppedState)(result.State)).Cursor.Timetoken);
        }

        [Test]
        public void TestReceiveStoppedStateTransitionWithUnsubscribeEvent()
        {
            //Arrange
            State receiveStoppedState = new ReceiveStoppedState() { Channels = new string[] { "ch1", "ch2" }, ChannelGroups = new string[] { "cg1", "cg2" }, Cursor = new SubscriptionCursor() { Region = 1, Timetoken = 1234567890 } };
            State unsubscribedState = new UnsubscribedState();
            //Act
            TransitionResult result = receiveStoppedState.Transition(new UnsubscribeAllEvent() { });
            //Assert
            Assert.IsTrue(result.State.GetType().Equals(unsubscribedState.GetType()));
        }

    }
}
