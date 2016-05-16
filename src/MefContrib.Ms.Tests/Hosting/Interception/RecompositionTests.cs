using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using MefContrib.Hosting.Interception.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Interception.Tests
{
    [TestClass]
    public class RecompositionTests
    {
        private CompositionContainer container;

        [TestMethod]
        public void When_adding_new_part_to_the_intercepted_catalog_intercepting_catalog_is_recomposed_and_intercepts_that_part()
        {
            var innerCatalog = new TypeCatalog(typeof(RecomposablePart1), typeof(RecomposablePartImporter));
            var cfg = new InterceptionConfiguration().AddInterceptor(new RecomposablePartInterceptor());
            var aggregateCatalog = new AggregateCatalog(innerCatalog);
            var catalog = new InterceptingCatalog(aggregateCatalog, cfg);
            container = new CompositionContainer(catalog);

            var importer = container.GetExportedValue<RecomposablePartImporter>();
            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.Parts);
            Assert.AreEqual(1, importer.Parts.Length);
            Assert.AreEqual(1, importer.Parts[0].Count);
            Assert.AreEqual(2, catalog.Parts.Count());

            // Recompose
            aggregateCatalog.Catalogs.Add(new TypeCatalog(typeof(RecomposablePart2)));

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.Parts);
            Assert.AreEqual(2, importer.Parts.Length);
            Assert.AreEqual(1, importer.Parts[0].Count);
            Assert.AreEqual(1, importer.Parts[1].Count);
            Assert.AreEqual(3, catalog.Parts.Count());
            Assert.AreEqual(3, catalog.Parts.OfType<InterceptingComposablePartDefinition>().Count());
        }

        [TestMethod]
        public void When_adding_new_part_to_the_intercepted_catalog_intercepting_catalog_raises_recomposition_events()
        {
            var innerCatalog = new TypeCatalog(typeof(RecomposablePart1));
            var cfg = new InterceptionConfiguration().AddInterceptor(new RecomposablePartInterceptor());
            var aggregateCatalog = new AggregateCatalog(innerCatalog);
            var catalog = new InterceptingCatalog(aggregateCatalog, cfg);
            container = new CompositionContainer(catalog);

            uint changingEventInvokeCount = 0;
            uint changedEventInvokeCount = 0;

            catalog.Changing += (s, e) => changingEventInvokeCount++;
            catalog.Changed += (s, e) => changedEventInvokeCount++;

            // Recompose
            aggregateCatalog.Catalogs.Add(new TypeCatalog(typeof(RecomposablePart2)));

            Assert.AreEqual(1U, changingEventInvokeCount);
            Assert.AreEqual(1U, changedEventInvokeCount);
        }

        [TestMethod]
        public void When_removing_a_part_from_the_intercepted_catalog_intercepting_catalog_is_recomposed_and_removes_that_part()
        {
            var innerCatalog1 = new TypeCatalog(typeof(RecomposablePart1), typeof(RecomposablePartImporter));
            var innerCatalog2 = new TypeCatalog(typeof(RecomposablePart2));
            var cfg = new InterceptionConfiguration().AddInterceptor(new RecomposablePartInterceptor());
            var aggregateCatalog = new AggregateCatalog(innerCatalog1, innerCatalog2);
            var catalog = new InterceptingCatalog(aggregateCatalog, cfg);
            container = new CompositionContainer(catalog);

            var importer = container.GetExportedValue<RecomposablePartImporter>();
            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.Parts);
            Assert.AreEqual(2, importer.Parts.Length);
            Assert.AreEqual(1, importer.Parts[0].Count);
            Assert.AreEqual(1, importer.Parts[1].Count);
            Assert.AreEqual(3, catalog.Parts.Count());

            // Recompose
            aggregateCatalog.Catalogs.Remove(innerCatalog2);

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.Parts);
            Assert.AreEqual(1, importer.Parts.Length);
            Assert.AreEqual(1, importer.Parts[0].Count);
            Assert.AreEqual(2, catalog.Parts.Count());
        }

        [TestMethod]
        public void When_removing_part_from_the_intercepted_catalog_intercepting_catalog_raises_recomposition_events()
        {
            var innerCatalog1 = new TypeCatalog(typeof(RecomposablePart1));
            var innerCatalog2 = new TypeCatalog(typeof(RecomposablePart2));
            var cfg = new InterceptionConfiguration().AddInterceptor(new RecomposablePartInterceptor());
            var aggregateCatalog = new AggregateCatalog(innerCatalog1, innerCatalog2);
            var catalog = new InterceptingCatalog(aggregateCatalog, cfg);
            container = new CompositionContainer(catalog);

            uint changingEventInvokeCount = 0;
            uint changedEventInvokeCount = 0;

            catalog.Changing += (s, e) => changingEventInvokeCount++;
            catalog.Changed += (s, e) => changedEventInvokeCount++;

            // Recompose
            aggregateCatalog.Catalogs.Remove(innerCatalog2);

            Assert.AreEqual(1U, changingEventInvokeCount);
            Assert.AreEqual(1U, changedEventInvokeCount);

           

        }

        [TestMethod]
        public void When_adding_new_part_to_the_part_handler_intercepting_catalog_is_recomposed_and_intercepts_that_part()
        {
            var partHandler = new RecomposablePartHandler();
            var innerCatalog = new TypeCatalog(typeof(RecomposablePartImporter));
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(new RecomposablePartInterceptor())
                .AddHandler(partHandler);
            var catalog = new InterceptingCatalog(innerCatalog, cfg);
            container = new CompositionContainer(catalog);

            var importer = container.GetExportedValue<RecomposablePartImporter>();
            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.Parts);
            Assert.AreEqual(0, importer.Parts.Length);
            Assert.AreEqual(1, catalog.Parts.Count());

            // Recompose
            partHandler.AddParts(new TypeCatalog(typeof(RecomposablePart1)));

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.Parts);
            Assert.AreEqual(1, importer.Parts.Length);
            Assert.AreEqual(1, importer.Parts[0].Count);
            Assert.AreEqual(2, catalog.Parts.Count());
            Assert.AreEqual(2, catalog.Parts.OfType<InterceptingComposablePartDefinition>().Count());

            // Recompose
            partHandler.AddParts(new TypeCatalog(typeof(RecomposablePart2)));

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.Parts);
            Assert.AreEqual(2, importer.Parts.Length);
            Assert.AreEqual(1, importer.Parts[1].Count);
            Assert.AreEqual(1, importer.Parts[0].Count);
            Assert.AreEqual(3, catalog.Parts.Count());
            Assert.AreEqual(3, catalog.Parts.OfType<InterceptingComposablePartDefinition>().Count());
        }

        [TestMethod]
        public void When_removing_existing_part_from_the_part_handler_intercepting_catalog_is_recomposed_and_removes_that_part()
        {
            var partHandler = new RecomposablePartHandler();
            var part2Catalog = new TypeCatalog(typeof(RecomposablePart2));
            var innerCatalog = new TypeCatalog(typeof(RecomposablePartImporter), typeof(RecomposablePart1));
            var aggregateCatalog = new AggregateCatalog(innerCatalog, part2Catalog);
            var cfg = new InterceptionConfiguration()
                .AddInterceptor(new RecomposablePartInterceptor())
                .AddHandler(partHandler);
            var catalog = new InterceptingCatalog(aggregateCatalog, cfg);
            container = new CompositionContainer(catalog);

            var importer = container.GetExportedValue<RecomposablePartImporter>();
            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.Parts);
            Assert.AreEqual(2, importer.Parts.Length);
            Assert.AreEqual(1, importer.Parts[0].Count);
            Assert.AreEqual(1, importer.Parts[1].Count);
            Assert.AreEqual(3, catalog.Parts.Count());
            Assert.AreEqual(3, catalog.Parts.OfType<InterceptingComposablePartDefinition>().Count());

            // Recompose
            partHandler.RemoveParts(part2Catalog);

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.Parts);
            Assert.AreEqual(1, importer.Parts.Length);
            Assert.AreEqual(1, importer.Parts[0].Count);
            Assert.IsInstanceOfType(importer.Parts[0], typeof(RecomposablePart1));
            Assert.AreEqual(2, catalog.Parts.Count());
            Assert.AreEqual(2, catalog.Parts.OfType<InterceptingComposablePartDefinition>().Count());
        }
    }
}