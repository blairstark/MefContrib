namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /*
    public static class ContextSpecificationAssertions
    {
        public static void ShouldMatch<T>(this T actual, Func<T, bool> condition)
        {
            Assert.IsTrue(condition.Invoke(actual));
        }

        public static void ShouldContainType<T>(this IList collection)
        {
            var found =
                collection.Contains(typeof(T));

            Assert.IsTrue(found);
        }

        public static void ShouldHaveCount<T>(this IList<T> list, int expected)
        {
            list.Count.ShouldEqual(expected);
        }

        public static void ShouldBeTrue(this bool actual)
        {
            Assert.IsTrue(actual);
        }

        public static void ShouldBeFalse(this bool actual)
        {
            Assert.False(actual);
        }

        public static void ShouldEqual(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldNotEqual(this object actual, object expected)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void ShouldNotBeSameAs(this object actual, object expected)
        {
            Assert.AreNotSame(expected, actual);
        }

        public static void ShouldBeSameAs(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
        }

        public static void ShouldBeNull(this object actual)
        {
            Assert.IsNull(actual);
        }

        public static void ShouldNotBeNull(this object actual)
        {
            Assert.IsNotNull(actual);
        }

        public static void ShouldBeOfType<T>(this Type asserted)
        {
            Assert.IsTrue(asserted == typeof(T));
        }

        public static void ShouldBeOfType<T>(this object asserted)
        {
            asserted.ShouldBeOfType(typeof(T));
        }

        public static void ShouldBeOfType(this object asserted, Type expected)
        {
            Assert.IsInstanceOf(expected, asserted);
        }

        public static void ShouldNotBeOfType<T>(this object assertedType)
        {
            if (assertedType != null)
            {
                Assert.IsInstanceOf(typeof(T), assertedType);
            }
        }

        public static void ShouldBeThrownBy(this Type expectedType, Action context)
        {
            Exception exception = null;

            try
            {
                context();
            }
            catch (Exception thrownException)
            {
                exception = thrownException;
                Assert.AreEqual(expectedType, thrownException.GetType());
            }
        }
    }
     * */
}