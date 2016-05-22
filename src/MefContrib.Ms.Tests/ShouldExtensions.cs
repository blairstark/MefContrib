namespace MefContrib.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class ShouldAssertExtensions
    {
        public static void ShouldMatch<T>(this T actual, Func<T, bool> condition)
        {
            Assert.IsTrue(condition.Invoke(actual));
        }

        public static void ShouldImplementInterface<T>(this Type actual)
        {
            var found =
                actual.GetInterfaces().Contains(typeof(T));

            Assert.IsTrue(found);
        }

        public static void ShouldContainType<T>(this IEnumerable collection)
        {
            var selection =
                from c in collection.Cast<object>()
                where c.GetType().IsAssignableFrom(typeof(T))
                select c;

            Assert.IsTrue(selection.Count() > 0);
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
            Assert.IsFalse(actual);
        }

        public static void ShouldEqual(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldBeGreaterThan(this int actual, int smallestValueNotAccepted)
        {
            Assert.IsTrue(actual > smallestValueNotAccepted);
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
            Assert.AreEqual(typeof(T), asserted);
        }

        public static void ShouldBeOfType<T>(this object asserted)
        {
            Assert.IsInstanceOfType(asserted, typeof(T));
        }

        public static void ShouldBeOfType(this object asserted, Type expected)
        {
            Assert.IsInstanceOfType(asserted, expected);
        }

        public static void ShouldNotBeOfType<T>(this object assertedType)
        {
            if (assertedType != null)
            {
                Assert.IsNotInstanceOfType(assertedType, typeof(T));
            }
        }

        //public static void ShouldBeThrownBy(this Type expectedType, Action context)
        //{
        //    try
        //    {
        //        context();
        //    }
        //    catch (Exception thrownException)
        //    {
        //        Assert.AreEqual(expectedType, thrownException.GetType());
        //    }
        //}
    }
}
