using System;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using MefContrib.Hosting.Interception.Configuration;
using MefContrib.Tests;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Interception.Tests
{
    [TestClass]
    public class InterceptingCatalogTests
    {
        private CompositionContainer container;

        [TestInitialize]
        public void TestSetUp()
        {
            var innerCatalog = new TypeCatalog(typeof(Part1), typeof(Part2), typeof(Part3));
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(new GeneralInterceptor())
                .AddInterceptionCriteria(
                    new PredicateInterceptionCriteria(
                        new PartInterceptor(), d => d.Metadata.ContainsKey("metadata1")));
            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            container = new CompositionContainer(catalog);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_should_throw_argument_null_exception_if_called_with_null_catalog()
        {
            new InterceptingCatalog(null, new InterceptionConfiguration());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_should_throw_argument_null_exception_if_called_with_null_configuration()
        {
            new InterceptingCatalog(new TypeCatalog(typeof(Part1)), null);
        }

        [TestMethod]
        public void When_querying_for_a_part_it_should_return_an_intercepting_part_definition()
        {
            var innerCatalog = new TypeCatalog(typeof(Logger));
            var mockInterceptor = new Mock<IExportedValueInterceptor>();
            var cfg = new InterceptionConfiguration().AddInterceptor(mockInterceptor.Object);
            var catalog = new InterceptingCatalog(innerCatalog, cfg);

            var partDefinition = catalog.Parts.First();
            partDefinition.ShouldBeOfType<InterceptingComposablePartDefinition>();
        }

        [TestMethod]
        public void When_querying_for_a_part_not_being_intercepted_it_should_return_original_part_definition()
        {
            var innerCatalog = new TypeCatalog(typeof(Logger));
            var cfg = new InterceptionConfiguration();
            var catalog = new InterceptingCatalog(innerCatalog, cfg);

            var partDefinition = catalog.Parts.First();
            partDefinition.ShouldNotBeOfType<InterceptingComposablePartDefinition>();
        }

        [TestMethod]
        public void Catalog_should_return_non_intercepted_value_for_part1()
        {
            var part = container.GetExportedValue<IPart>();
            Assert.IsInstanceOfType(part, typeof(Part1));
        }

        [TestMethod]
        public void Catalog_should_return_an_intercepted_value_for_part2()
        {
            var part = container.GetExportedValue<IPart>("part2");
            Assert.IsInstanceOfType(part, typeof(PartWrapper));
            Assert.IsInstanceOfType(((PartWrapper)part).Inner, typeof(Part2));
        }

        [TestMethod]
        public void Catalog_should_return_a_part_with_properly_set_name()
        {
            var part1 = container.GetExportedValue<IPart>();
            var part2 = container.GetExportedValue<IPart>("part2");

            Assert.AreEqual("Name property is set be the interceptor.", part1.Name);
            Assert.AreEqual("Name property is set be the interceptor.", part2.Name);
        }

        [TestMethod]
        public void Catalog_should_return_a_part_with_respect_to_its_creation_policy()
        {
            Part1.InstanceCount = 0;
            Part3.InstanceCount = 0;

            var part11 = container.GetExportedValue<IPart>();
            var part12 = container.GetExportedValue<IPart>();
            var part31 = container.GetExportedValue<IPart>("part3");
            var part32 = container.GetExportedValue<IPart>("part3");

            Assert.IsNotNull(part11);
            Assert.IsNotNull(part12);
            Assert.IsNotNull(part31);
            Assert.IsNotNull(part32);

            Assert.AreSame(part11, part12);
            Assert.AreNotSame(part31, part32);

            Assert.AreEqual(1, Part1.InstanceCount);
            Assert.AreEqual(2, Part3.InstanceCount);
        }

        [TestMethod]
        public void Catalog_should_filter_out_parts()
        {
            var innerCatalog = new TypeCatalog(typeof(Part0), typeof(Part1), typeof(Part2), typeof(Part3));
            var cfg = new InterceptionConfiguration()
                .AddHandler(new PartFilter());
            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            container = new CompositionContainer(catalog);

            var parts = container.GetExportedValues<IPart>();
            Assert.AreEqual(1, parts.Count());
            Assert.IsInstanceOfType(parts.First(), typeof(Part0));
        }

        [TestMethod]
        public void Catalog_should_call_Initialize_on_a_given_export_handlers()
        {
            var innerCatalog = new TypeCatalog();
            var exportHandlerMock = new Mock<IExportHandler>(MockBehavior.Strict);
            exportHandlerMock.Setup(p => p.Initialize(innerCatalog)).Verifiable();

            var cfg = new InterceptionConfiguration()
                .AddHandler(exportHandlerMock.Object);
            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            Assert.IsNotNull(catalog);
            exportHandlerMock.Verify();
        }

        [TestMethod]
        public void Catalog_should_call_Initialize_on_a_given_part_handlers()
        {
            var innerCatalog = new TypeCatalog();
            var partHandlerMock = new Mock<IPartHandler>(MockBehavior.Strict);
            partHandlerMock.Setup(p => p.Initialize(innerCatalog)).Verifiable();

            var cfg = new InterceptionConfiguration()
                .AddHandler(partHandlerMock.Object);
            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            Assert.IsNotNull(catalog);
            partHandlerMock.Verify();
        }

        [TestMethod]
        public void Disposing_catalog_should_dispose_parts_implementing_dispose_pattern()
        {
            var innerCatalog = new TypeCatalog(typeof(DisposablePart));
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(new FakeInterceptor());
            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            var partDefinition = catalog.Parts.First();
            container = new CompositionContainer(catalog);

            partDefinition.ShouldBeOfType<InterceptingComposablePartDefinition>();

            var part = container.GetExportedValueOrDefault<DisposablePart>();
            Assert.IsFalse(part.IsDisposed);
            container.Dispose();
            Assert.IsTrue(part.IsDisposed);
        }
    }
}