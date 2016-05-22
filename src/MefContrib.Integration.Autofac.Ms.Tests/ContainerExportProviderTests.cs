namespace MefContrib.Integration.Autofac.Tests
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Reflection;
    using global::Autofac;
    using MefContrib.Containers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Ms.Tests;
    [TestClass]
    public class ContainerExportProviderTests
    {
        [TestMethod]
        public void ExportProviderResolvesServiceRegisteredByTypeTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            var autofacContainer = builder.Build();
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);

            var component = provider.GetExportedValue<IAutofacOnlyComponent>();
            Assert.IsNotNull(component);
            Assert.IsInstanceOfType(component, typeof(AutofacOnlyComponent1));
        }

        [TestMethod]
        public void ExportProviderResolvesServicesRegisteredByTypeTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            builder.RegisterType<AutofacOnlyComponent2>().Named<IAutofacOnlyComponent>("b");
            var autofacContainer = builder.Build();
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);

            var components = provider.GetExports<IAutofacOnlyComponent>();
            Assert.IsNotNull(components);
            Assert.AreEqual(components.Count(),2);

            Assert.AreEqual(components.Select(t => t.Value).OfType<AutofacOnlyComponent1>().Count(), 1);
            Assert.AreEqual(components.Select(t => t.Value).OfType<AutofacOnlyComponent2>().Count(),1);
        }

        [TestMethod]
        public void ExportProviderResolvesServiceRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            builder.RegisterType<AutofacOnlyComponent2>().Named<IAutofacOnlyComponent>("autofacComponent2");
            var autofacContainer = builder.Build();
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);

            var component = provider.GetExportedValue<IAutofacOnlyComponent>("autofacComponent2");
            Assert.IsNotNull(component);
            Assert.IsInstanceOfType(component, typeof(AutofacOnlyComponent2));
        }

        [TestMethod]
        public void MefCanResolveLazyTypeRegisteredInAutofacTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            var autofacContainer = builder.Build();
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            AutofacOnlyComponent1.InstanceCount = 0;

            var lazyAutofacComponent = container.GetExport<IAutofacOnlyComponent>();
            Assert.IsNotNull(lazyAutofacComponent);
            Assert.AreEqual(AutofacOnlyComponent1.InstanceCount, 0);

            Assert.IsNotNull(lazyAutofacComponent.Value);
            Assert.IsInstanceOfType(lazyAutofacComponent.Value, typeof(AutofacOnlyComponent1));
            Assert.AreEqual(AutofacOnlyComponent1.InstanceCount, 1);
        }

        [TestMethod]
        public void MefCanResolveLazyTypesRegisteredInAutofacTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            builder.RegisterType<AutofacOnlyComponent2>().Named<IAutofacOnlyComponent>("a");
            var autofacContainer = builder.Build();
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            AutofacOnlyComponent1.InstanceCount = 0;

            var lazyAutofacComponent = container.GetExports<IAutofacOnlyComponent>().ToList();
            Assert.IsNotNull(lazyAutofacComponent);
            Assert.AreEqual(AutofacOnlyComponent1.InstanceCount, 0);

            Assert.IsNotNull(lazyAutofacComponent);
            Assert.IsNotNull(lazyAutofacComponent[0].Value);
            Assert.IsNotNull(lazyAutofacComponent[1].Value);
            Assert.AreEqual(AutofacOnlyComponent1.InstanceCount, 1);
        }

        [TestMethod]
        public void MefCanResolveTypesRegisteredInAutofacAfterTrackingExtensionIsAddedTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.RegisterType<AutofacOnlyComponent2>().Named<IAutofacOnlyComponent>("autofacComponent2");
            var autofacContainer = builder.Build();

            // Further setup
            var adapter = new AutofacContainerAdapter(autofacContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            var component = container.GetExportedValue<IAutofacOnlyComponent>("autofacComponent2");
            Assert.IsNotNull(component);
            Assert.IsInstanceOfType(component, typeof(AutofacOnlyComponent2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CannotPassNullInstanceToTheContainerExportProviderConstructorTest()
        {
            new ContainerExportProvider(null);
            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CannotPassNullAutofacInstanceToTheAutofacContainerAdapterConstructorTest()
        {
            new AutofacContainerAdapter(null);
        }

        #region Composing with two providers

        class A { }

        class B { }

        [Export]
        class C
        {
            [ImportingConstructor]
            public C(A a, B b)
            {
                ThingA = a;
                ThingB = b;
            }
            public A ThingA { get; private set; }

            public B ThingB { get; private set; }
        }

        [TestMethod]
        public void ComposeWithTwoContainerExportProvidersTest()
        {
            var builder1 = new ContainerBuilder();
            var a = new A();
            builder1.RegisterInstance(a);
            var autofacContainer1 = builder1.Build();
            var exportProvider1 = new ContainerExportProvider(new AutofacContainerAdapter(autofacContainer1));

            var builder2 = new ContainerBuilder();
            var b = new B();
            builder2.RegisterInstance(b);
            var autofacContainer2 = builder2.Build();
            var exportProvider2 = new ContainerExportProvider(new AutofacContainerAdapter(autofacContainer2));

            var catalog = new TypeCatalog(typeof(C));
            var compositionContainer = new CompositionContainer(catalog, exportProvider1, exportProvider2);
            var instance = compositionContainer.GetExport<C>();
            Assert.IsNotNull(instance.Value);
            Assert.AreEqual(a, instance.Value.ThingA, "Instance of A is the same as that registered with the DI container.");
            Assert.AreEqual(b, instance.Value.ThingB, "Instance of B is the same as that registered with the DI container.");
        }

        #endregion
    }
}