using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using MefContrib.Containers;
using MefContrib.Integration.Unity.Extensions;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Integration.Unity.Tests
{
    [TestClass]
    public class ContainerExportProviderTests
    {
        [TestMethod]
        public void ExportProviderResolvesServiceRegisteredByTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            var component = provider.GetExportedValue<IUnityOnlyComponent>();
            Assert.IsNotNull(component);
            Assert.IsInstanceOfType(component, typeof(UnityOnlyComponent1));
        }

        [TestMethod]
        public void ExportProviderResolvesServicesRegisteredByTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("b");

            var components = provider.GetExports<IUnityOnlyComponent>();
            Assert.IsNotNull(components);
            Assert.AreEqual(2, components.Count());

            Assert.AreEqual(1, components.Select(t => t.Value).OfType<UnityOnlyComponent1>().Count());
            Assert.AreEqual(1, components.Select(t => t.Value).OfType<UnityOnlyComponent2>().Count());
        }

        [TestMethod]
        public void ExportProviderResolvesServiceRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("unityComponent2");

            var component = provider.GetExportedValue<IUnityOnlyComponent>("unityComponent2");
            Assert.IsNotNull(component);
            Assert.IsInstanceOfType(component, typeof(UnityOnlyComponent2));
        }

        [TestMethod]
        public void MefCanResolveLazyTypeRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            UnityOnlyComponent1.InstanceCount = 0;
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            var lazyUnityComponent = container.GetExport<IUnityOnlyComponent>();
            Assert.IsNotNull(lazyUnityComponent);
            Assert.AreEqual(0, UnityOnlyComponent1.InstanceCount);

            Assert.IsNotNull(lazyUnityComponent.Value);
            Assert.IsInstanceOfType(lazyUnityComponent.Value, typeof(UnityOnlyComponent1));
            Assert.AreEqual(1, UnityOnlyComponent1.InstanceCount);
        }

        [TestMethod]
        public void MefCanResolveLazyTypesRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            UnityOnlyComponent1.InstanceCount = 0;
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("a");

            var lazyUnityComponent = container.GetExports<IUnityOnlyComponent>().ToList();
            Assert.IsNotNull(lazyUnityComponent);
            Assert.AreEqual(0, UnityOnlyComponent1.InstanceCount);

            Assert.IsNotNull(lazyUnityComponent);
            Assert.IsNotNull(lazyUnityComponent[0].Value);
            Assert.IsNotNull(lazyUnityComponent[1].Value);
            Assert.AreEqual(1, UnityOnlyComponent1.InstanceCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ImportCardinalityMismatchException))]
        public void MefCannotResolveTypesRegisteredInUnityBeforeTrackingExtensionIsAddedTest()
        {
            // Setup
            var unityContainer = new UnityContainer();

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("unityComponent2");

            // Further setup
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            container.GetExportedValue<IUnityOnlyComponent>("unityComponent2");
         
        }

        [TestMethod]
        public void MefCanResolveTypesRegisteredInUnityAfterTrackingExtensionIsAddedTest()
        {
            // Setup
            var unityContainer = new UnityContainer();

            // Enable tracking
            TypeRegistrationTrackerExtension.RegisterIfMissing(unityContainer);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent2>("unityComponent2");

            // Further setup
            var adapter = new UnityContainerAdapter(unityContainer);
            var provider = new ContainerExportProvider(adapter);
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog, provider);

            var component = container.GetExportedValue<IUnityOnlyComponent>("unityComponent2");
            Assert.IsNotNull(component);
            Assert.IsInstanceOfType(component, typeof(UnityOnlyComponent2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]

        public void CannotPassNullInstanceToTheContainerExportProviderConstructorTest()
        {

            new ContainerExportProvider(null);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]

        public void CannotPassNullUnityInstanceToTheUnityContainerAdapterConstructorTest()
        {

            new UnityContainerAdapter(null);

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
            var unityContainer1 = new UnityContainer();
            var exportProvider1 = new ContainerExportProvider(new UnityContainerAdapter(unityContainer1));

            var a = new A();
            unityContainer1.RegisterInstance<A>(a);

            var unityContainer2 = new UnityContainer();
            var exportProvider2 = new ContainerExportProvider(new UnityContainerAdapter(unityContainer2));

            var b = new B();
            unityContainer2.RegisterInstance<B>(b);

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