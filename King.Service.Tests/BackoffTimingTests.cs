﻿namespace King.Azure.BackgroundWorker.Tests
{
    using King.Service.Timing;
    using NSubstitute;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class BackoffTimingTests
    {
        [Test]
        public void Constructor()
        {
            new BackoffTiming();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorTimingNull()
        {
            new BackoffTiming(null);
        }

        [Test]
        public void IsIDynamicTiming()
        {
            Assert.IsNotNull(new BackoffTiming() as IDynamicTiming);
        }

        [Test]
        public void Get()
        {
            var random = new Random();
            var min = random.Next();
            var max = random.Next();
            var expected = random.NextDouble();
            var timing = Substitute.For<ICalculateTiming>();
            timing.Exponential(0, max, min).Returns(expected);

            var t = new BackoffTiming(timing);
            var value = t.Get(true, max, min);

            Assert.AreEqual(expected, value);

            timing.Received().Exponential(0, max, min);
        }

        [Test]
        public void GetNoWork()
        {
            var random = new Random();
            var min = random.Next();
            var max = random.Next();
            var expected = random.NextDouble();
            var timing = Substitute.For<ICalculateTiming>();
            timing.Exponential(1, max, min).Returns(expected);

            var t = new BackoffTiming(timing);
            var value = t.Get(false, max, min);

            Assert.AreEqual(expected, value);

            timing.Received().Exponential(1, max, min);
        }

        [Test]
        public void GetWorkMultiple()
        {
            var random = new Random();
            var min = random.Next();
            var max = random.Next();
            var expected = random.NextDouble();
            var timing = Substitute.For<ICalculateTiming>();
            timing.Exponential(0, max, min).Returns(expected);

            var t = new BackoffTiming(timing);
            t.Get(true, max, min);
            t.Get(true, max, min);
            t.Get(true, max, min);
            t.Get(true, max, min);
            t.Get(true, max, min);
            var value = t.Get(true, max, min);

            Assert.AreEqual(expected, value);

            timing.Received(6).Exponential(0, max, min);
        }

        [Test]
        public void GetNoWorkMultiple()
        {
            var random = new Random();
            var min = random.Next();
            var max = random.Next();
            var expected = random.NextDouble();
            var timing = Substitute.For<ICalculateTiming>();
            timing.Exponential(6, max, min).Returns(expected);

            var t = new BackoffTiming(timing);
            t.Get(false, max, min);
            t.Get(false, max, min);
            t.Get(false, max, min);
            t.Get(false, max, min);
            t.Get(false, max, min);
            var value = t.Get(false, max, min);

            Assert.AreEqual(expected, value);

            timing.Received().Exponential(1, max, min);
            timing.Received().Exponential(2, max, min);
            timing.Received().Exponential(3, max, min);
            timing.Received().Exponential(4, max, min);
            timing.Received().Exponential(5, max, min);
            timing.Received().Exponential(6, max, min);
        }
    }
}