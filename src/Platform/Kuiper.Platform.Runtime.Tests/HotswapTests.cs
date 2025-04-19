// <copyright file="HotswapTests.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Tests
{
    using System;
    using System.Collections.Generic;

    using Shouldly;

    public class HotswapTests
    {
        private class TestClass
        {
            public Guid Value { get; set; }
        }

        public static T Replace<T>(List<T> values, int index, T newValue)
        {
            if (index < 0 || index >= values.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var originalValue = values[index];
            values[index] = newValue;

            return originalValue;
        }

        public static T Replace<T>(List<T> values, int index, T newValue, ReaderWriterLockSlim @lock, TimeSpan? timeout = null)
        {
            if (!@lock.TryEnterWriteLock(timeout ?? TimeSpan.FromMinutes(2)))
            {
                throw new TimeoutException();
            }

            try
            {
                if (index < 0 || index >= values.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                var originalValue = values[index];
                values[index] = newValue;

                return originalValue;
            }
            finally
            {
                @lock.ExitReadLock();
            }
        }

        [Fact]
        public void TestDictionaryValueHotswap()
        {
            // Arange
            Dictionary<string, List<TestClass>> pluginOperations = new Dictionary<string, List<TestClass>>()
            {
                ["test"] = new List<TestClass>()
                {
                    new TestClass() { Value = Guid.NewGuid() },
                    new TestClass() { Value = Guid.NewGuid() },
                },
            };

            Dictionary<string, List<TestClass>> targetOperations = new Dictionary<string, List<TestClass>>();

            // Act
            Dictionary<string, List<TestClass>> originalOpertions = Interlocked.Exchange(ref targetOperations, pluginOperations);

            // Assert - Proove that the target operations passed by reference and therefore not safe to use
            // You must copy the dictionary to a new instance in order to use it safely.
            pluginOperations.Add("test2", new List<TestClass>());
            pluginOperations.Count.ShouldBe(2);
            targetOperations.Count.ShouldBe(2);

            targetOperations.Add("test3", new List<TestClass>());
            pluginOperations.Count.ShouldBe(3);
            targetOperations.Count.ShouldBe(3);

            originalOpertions.Count.ShouldBe(0);
        }

        [Fact]
        public void TestListValueHotswap()
        {
            // Arange
            var values = new List<TestClass>()
            {
                new TestClass() { Value = Guid.NewGuid() },
                new TestClass() { Value = Guid.NewGuid() },
            };

            var firstValue = values[0];
            var secondValue = values[1];

            // Act
            var originalValue = Interlocked.Exchange(ref firstValue, new TestClass() { Value = Guid.NewGuid() });

            // Assert
            // Proves that list values are not affected by the exchange.
            values[0].Value.ShouldBe(originalValue.Value);
        }

        [Fact]
        public void TestCorrectSwap()
        {
            // Arange
            var values = new List<TestClass>()
            {
                new TestClass() { Value = Guid.NewGuid() },
                new TestClass() { Value = Guid.NewGuid() },
            };

            var originalValue = Replace(values, 0, new TestClass() { Value = Guid.NewGuid() });

            values[0].Value.ShouldNotBe(originalValue.Value);
        }

        [Fact]
        public void TestRecursiveError()
        {
            // Arange
            var values = new List<TestClass>()
            {
                new TestClass() { Value = Guid.NewGuid() },
                new TestClass() { Value = Guid.NewGuid() },
            };

            ReaderWriterLockSlim @lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            @lock.EnterWriteLock();

            Assert.Throws<LockRecursionException>(() => Replace(values, 0, new TestClass() { Value = Guid.NewGuid() }, @lock, TimeSpan.FromMilliseconds(50)));
        }

        [Fact]
        public void TestReadLockWithTimeout()
        {
            // Arange
            var values = new List<TestClass>()
            {
                new TestClass() { Value = Guid.NewGuid() },
                new TestClass() { Value = Guid.NewGuid() },
            };

            ReaderWriterLockSlim @lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            bool hasLock = false;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                Task task = Task.Run(() =>
                {
                    @lock.EnterReadLock();
                    hasLock = true;
                    while (cancellationTokenSource.IsCancellationRequested == false)
                    {
                        Task.Delay(25);
                    }
                });

                while (!hasLock)
                {
                    Thread.Sleep(10);
                }

                Assert.Throws<TimeoutException>(() => Replace(values, 0, new TestClass() { Value = Guid.NewGuid() }, @lock, TimeSpan.FromMilliseconds(100)));
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }
        }
    }
}
