using System;
using System.Linq;
using MefContrib.Hosting.Interception.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Interception.Tests.Configuration
{
    [TestClass]
    public class InterceptionConfigurationTests
    {
        [TestMethod]
        public void Adding_export_handlers_is_reflected_in_the_ExportHandlers_collection()
        {
            var cfg = new InterceptionConfiguration()
                .AddHandler(new FakeExportHandler1())
                .AddHandler(new FakeExportHandler2());

            Assert.AreEqual(2, cfg.ExportHandlers.Count());
            Assert.IsTrue(cfg.ExportHandlers.OfType<FakeExportHandler1>().Any());
            Assert.IsTrue(cfg.ExportHandlers.OfType<FakeExportHandler2>().Any());
        }

        [TestMethod]
        public void Adding_part_handlers_is_reflected_in_the_PartHandlers_collection()
        {
            var cfg = new InterceptionConfiguration()
                .AddHandler(new FakePartHandler1())
                .AddHandler(new FakePartHandler1());

            Assert.AreEqual(2, cfg.PartHandlers.Count());
            Assert.IsTrue(cfg.PartHandlers.OfType<FakePartHandler1>().Any());
            Assert.IsTrue(cfg.PartHandlers.OfType<FakePartHandler1>().Any());
        }

        [TestMethod]
        public void Adding_interceptors_is_reflected_in_the_Interceptors_collection()
        {
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(new CompositeValueInterceptor())
                .AddInterceptor(new FakeInterceptor());

            Assert.AreEqual(2, cfg.Interceptors.Count());
            Assert.IsTrue(cfg.Interceptors.OfType<CompositeValueInterceptor>().Any());
            Assert.IsTrue(cfg.Interceptors.OfType<FakeInterceptor>().Any());
        }

        [TestMethod]
        public void Adding_interception_criteria_is_reflected_in_the_InterceptionCriteria_collection()
        {
            var cfg = new InterceptionConfiguration()
                .AddInterceptionCriteria(new PredicateInterceptionCriteria(new FakeInterceptor(), part => true));

            Assert.AreEqual(1, cfg.InterceptionCriteria.Count());
            Assert.IsTrue(cfg.InterceptionCriteria.OfType<PredicateInterceptionCriteria>().Any());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Adding_null_export_handler_causes_argument_null_exception_to_be_thrown()
        {
            new InterceptionConfiguration().AddHandler((IExportHandler)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Adding_null_part_handler_causes_argument_null_exception_to_be_thrown()
        {
            new InterceptionConfiguration().AddHandler((IPartHandler)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Adding_null_interceptor_causes_argument_null_exception_to_be_thrown()
        {

            new InterceptionConfiguration().AddInterceptor(null);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Adding_null_interception_criteria_causes_argument_null_exception_to_be_thrown()
        {
            new InterceptionConfiguration().AddInterceptionCriteria(null);

        }
    }
}