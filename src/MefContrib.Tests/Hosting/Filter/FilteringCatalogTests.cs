using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Filter.Tests
{
    [TestClass]
    public class FilteringCatalogTests
    {
        public interface IMetadataComponent { }

        [Export(typeof(IMetadataComponent))]
        [PartMetadata("key", "value")]
        public class Component1
        {
        }

        [Export(typeof(IMetadataComponent))]
        public class Component2
        {
        }

        [TestMethod]
        public void Parts_are_filtered_based_on_metadata_using_ContainsMetadata_filter()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var filteredCatalog = new FilteringCatalog(catalog, new ContainsMetadata("key", "value"));
            var container = new CompositionContainer(filteredCatalog);
            var components = container.GetExports<IMetadataComponent>();

            Assert.IsNotNull(components);
            Assert.AreEqual(1, components.Count());
        }

        [TestMethod]
        public void Parts_are_filtered_using_lambda_expression_filter()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var filteredCatalog = new FilteringCatalog(catalog,
                                                      partDefinition => partDefinition.Metadata.ContainsKey("key") &&
                                                                        partDefinition.Metadata["key"].Equals("value"));
            var container = new CompositionContainer(filteredCatalog);
            var components = container.GetExports<IMetadataComponent>();

            Assert.IsNotNull(components);
            Assert.AreEqual(1, components.Count());
        }

        public interface ILifetimeComponent
        {
        }

        [Export(typeof(ILifetimeComponent))]
        public class LifetimeComponent1 : ILifetimeComponent
        {
        }

        [Export(typeof(ILifetimeComponent))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class LifetimeComponent2 : ILifetimeComponent
        {
        }

        [Export(typeof(ILifetimeComponent))]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class LifetimeComponent3 : ILifetimeComponent
        {
        }

        [Export(typeof(ILifetimeComponent))]
        [PartCreationPolicy(CreationPolicy.Any)]
        public class LifetimeComponent4 : ILifetimeComponent
        {
        }

        [TestMethod]
        public void Parts_are_filtered_based_on_shared_lifetime_using_HasCreationPolicy_filter()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var filteredCatalog = new FilteringCatalog(catalog, new HasCreationPolicy(CreationPolicy.Shared));
            var container = new CompositionContainer(filteredCatalog);
            var components = container.GetExports<ILifetimeComponent>();

            Assert.IsNotNull(components);
            Assert.AreEqual(1, components.Count());
            Assert.IsInstanceOfType(components.First().Value, typeof(LifetimeComponent2));
        }

        [TestMethod]
        public void Parts_are_filtered_based_on_nonshared_lifetime_using_HasCreationPolicy_filter()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var filteredCatalog = new FilteringCatalog(catalog, new HasCreationPolicy(CreationPolicy.NonShared));
            var container = new CompositionContainer(filteredCatalog);
            var components = container.GetExports<ILifetimeComponent>();

            Assert.IsNotNull(components);
            Assert.AreEqual(1, components.Count());
            Assert.IsInstanceOfType(components.First().Value, typeof(LifetimeComponent3));
        }

        [TestMethod]
        public void Parts_are_filtered_based_on_any_lifetime_using_HasCreationPolicy_filter()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var filteredCatalog = new FilteringCatalog(catalog, new HasCreationPolicy(CreationPolicy.Any));
            var container = new CompositionContainer(filteredCatalog);
            var components = container.GetExports<ILifetimeComponent>();

            Assert.IsNotNull(components);
            Assert.AreEqual(2, components.Count());
            Assert.IsInstanceOfType(components.Select(t => t.Value).OfType<LifetimeComponent1>().First(), typeof(LifetimeComponent1));
            Assert.IsInstanceOfType(components.Select(t => t.Value).OfType<LifetimeComponent4>().First(), typeof(LifetimeComponent4));
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class Root : IDisposable
        {
            [Import]
            public Dep1 Dep { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class Dep1 : IDisposable
        {
            [Import]
            public Dep2 Dep { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class Dep2 : IDisposable
        {
            [Import]
            public Dep3 Dep { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class Dep3 : IDisposable
        {
            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        [TestMethod]
        public void Parts_created_by_the_child_container_are_disposed()
        {
            var catalog = new AssemblyCatalog(typeof(FilteringCatalogTests).Assembly);
            var parent = new CompositionContainer(catalog);

            var filteredCat = new FilteringCatalog(catalog, new HasCreationPolicy(CreationPolicy.NonShared));
            var child = new CompositionContainer(filteredCat, parent);

            var root = child.GetExportedValue<Root>();
            var dep1 = root.Dep;
            var dep2 = dep1.Dep;
            var dep3 = dep2.Dep;

            Assert.IsFalse(root.Disposed);
            Assert.IsFalse(dep1.Disposed);
            Assert.IsFalse(dep2.Disposed);
            Assert.IsFalse(dep3.Disposed);

            child.Dispose();

            Assert.IsTrue(root.Disposed); // Disposed as it was created by the child container
            Assert.IsTrue(dep1.Disposed); // Disposed as it was created by the child container
            Assert.IsFalse(dep2.Disposed);
            Assert.IsFalse(dep3.Disposed);
        }
    }
}