using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Interception.Tests
{
    [TestClass]
    public class CompositeValueInterceptorTests
    {
        [TestMethod]
        public void Interceptors_are_evaluated_in_order_they_are_added()
        {
            var compositeValueInterceptor = new CompositeValueInterceptor();
            compositeValueInterceptor.Interceptors.Add(new Wrapper1Interceptor());
            compositeValueInterceptor.Interceptors.Add(new Wrapper2Interceptor());

            var val = "this is a value";
            var interceptedValue = (IWrapper) compositeValueInterceptor.Intercept(val);

            Assert.IsInstanceOfType(interceptedValue, typeof(Wrapper2));
            Assert.IsInstanceOfType(interceptedValue.Value, typeof(Wrapper1));
            Assert.AreEqual(val, ((IWrapper)interceptedValue.Value).Value);
        }

        public interface IWrapper
        {
            object Value { get; set; }
        }

        public class Wrapper1 : IWrapper
        {
            public Wrapper1(object value)
            {
                Value = value;
            }

            public object Value { get; set; }
        }

        public class Wrapper2 : IWrapper
        {
            public Wrapper2(object value)
            {
                Value = value;
            }

            public object Value { get; set; }
        }

        public class Wrapper1Interceptor : IExportedValueInterceptor
        {
            public object Intercept(object value)
            {
                return new Wrapper1(value);
            }
        }

        public class Wrapper2Interceptor : IExportedValueInterceptor
        {
            public object Intercept(object value)
            {
                return new Wrapper2(value);
            }
        }
    }
}