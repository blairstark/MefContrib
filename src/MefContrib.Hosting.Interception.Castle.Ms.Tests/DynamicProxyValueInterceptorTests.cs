using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using MefContrib.Hosting.Interception.Configuration;
using Castle.DynamicProxy;
using System.ComponentModel.Composition;
using static MefContrib.Hosting.Interception.Castle.Ms.Tests.FreezableInterceptor;

namespace MefContrib.Hosting.Interception.Castle.Ms.Tests
{
    /// <summary>
    /// Summary description for DynamicProxyValueInterceptorTests
    /// </summary>
    [TestClass]
    public class DynamicProxyValueInterceptorTests
    {


        private CompositionContainer container;

        public DynamicProxyValueInterceptorTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void TestSetUp()
        {
            var innerCatalog = new TypeCatalog(typeof(Customer));
            var interceptor = new FreezableInterceptor();
            interceptor.Freeze();

            var valueInterceptor = new DynamicProxyInterceptor(interceptor);
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(valueInterceptor);

            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            container = new CompositionContainer(catalog);
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void When_setting_name_on_the_customer_it_should_error()
        {
            var customer = container.GetExportedValue<ICustomer>();
            customer.Name = "John Doe";
        }
    }

    [Export(typeof(ICustomer))]
    public class Customer : ICustomer
    {
        public string Name { get; set; }
    }

    public interface ICustomer
    {
        string Name { get; set; }
    }

    // Freezable interceptor taken from Krzysztof Kozmic
    internal class FreezableInterceptor : IInterceptor, IFreezable
    {
        private bool _isFrozen;

        public void Freeze()
        {
            _isFrozen = true;
        }

        public bool IsFrozen
        {
            get { return _isFrozen; }
        }

        public void Intercept(IInvocation invocation)
        {
            if (_isFrozen && invocation.Method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException();
            }
            invocation.Proceed();
        }


        internal interface IFreezable { bool IsFrozen { get; } void Freeze(); }
    }
}