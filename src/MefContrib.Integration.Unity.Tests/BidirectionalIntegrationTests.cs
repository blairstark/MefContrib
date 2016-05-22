using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Integration.Unity.Tests
{
    [TestClass]
    public class BidirectionalIntegrationTests
    {
        [TestMethod]
        public void UnityCanResolveMefComponentThatHasUnityDependenciesTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            // Test
            var mefComponent = unityContainer.Resolve<IMefComponentWithUnityDependencies>();
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent.MefOnlyComponent, typeof(MefComponent1));
            Assert.IsInstanceOfType(mefComponent.UnityOnlyComponent, typeof(UnityOnlyComponent1));
        }

        [TestMethod]
        public void UnityCanResolveMefComponentThatHasUnityDependenciesThatHaveMefDependenciesTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();
            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            // Test
            var mefComponent = unityContainer.Resolve<IMefComponentWithUnityDependencies>("component2");
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent,typeof(MefComponentWithUnityDependencies2));
            Assert.IsInstanceOfType(mefComponent.MefOnlyComponent,  typeof(MefComponent1));
            Assert.IsInstanceOfType(mefComponent.UnityOnlyComponent, typeof(UnityOnlyComponent1));

            var mefComponentWithUnityDependencies2 = (MefComponentWithUnityDependencies2) mefComponent;
            Assert.IsInstanceOfType(mefComponentWithUnityDependencies2.MixedUnityMefComponent, typeof(UnityComponent1));
            Assert.IsInstanceOfType(mefComponentWithUnityDependencies2.MixedUnityMefComponent.MefComponent, typeof(MefComponent1));
        }

        [TestMethod]
        public void UnityCircularDependencyIsDetectedTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            unityContainer.RegisterType<UnityOnlyComponent1>();
            
            // Test
            var unityOnlyComponent1 = unityContainer.Resolve<UnityOnlyComponent1>();
            Assert.IsNotNull(unityOnlyComponent1);
        }

        [TestMethod]
        public void UnityCanResolveUnityComponentThatHasUnityAndMefDependenciesTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();
            unityContainer.RegisterType<UnityMixedComponent>();

            // Test
            var unityMixedComponent = unityContainer.Resolve<UnityMixedComponent>();
            Assert.IsNotNull(unityMixedComponent);
            Assert.IsInstanceOfType(unityMixedComponent, typeof(UnityMixedComponent));
            Assert.IsInstanceOfType(unityMixedComponent.MefComponent, typeof(MefComponent1));
            Assert.IsInstanceOfType(unityMixedComponent.UnityComponent, typeof(UnityOnlyComponent1));
        }

        [TestMethod]
        public void UnityContainerCanBeResolvedByMefTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);

            var compositionContainer1 = unityContainer.Resolve<CompositionContainer>();
            var compositionContainer2 = unityContainer.Resolve<CompositionContainer>();
            Assert.IsNotNull(compositionContainer1);
            Assert.IsNotNull(compositionContainer2);
            Assert.AreSame(compositionContainer1, compositionContainer2);

            var unityContainerFromMef1 = compositionContainer1.GetExportedValue<IUnityContainer>();
            var unityContainerFromMef2 = compositionContainer1.GetExportedValue<IUnityContainer>();
            
            Assert.IsNotNull(unityContainerFromMef1);
            Assert.IsNotNull(unityContainerFromMef2);
            Assert.AreSame(unityContainerFromMef1, unityContainerFromMef2);
            Assert.AreSame(unityContainer, unityContainerFromMef1);
        }

        [TestMethod]
        public void MefResolvesServiceRegisteredInUnityByTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);

            // Registration
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>(new ContainerControlledLifetimeManager());

            var container = unityContainer.Resolve<CompositionContainer>();
            var unityOnlyComponent = container.GetExportedValue<IUnityOnlyComponent>();
            var unityOnlyComponent2 = unityContainer.Resolve<IUnityOnlyComponent>();
            Assert.IsNotNull(unityOnlyComponent);
            Assert.IsInstanceOfType(unityOnlyComponent, typeof(UnityOnlyComponent1));
            Assert.IsNotNull(unityOnlyComponent2);
            Assert.IsInstanceOfType(unityOnlyComponent2, typeof(UnityOnlyComponent1));
            Assert.AreEqual(unityOnlyComponent, unityOnlyComponent2);
        }

        [TestMethod]
        public void MefCanResolveMefComponentThatHasUnityAndMefDependenciesTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            // Test
            var container = unityContainer.Resolve<CompositionContainer>();
            var mefMixedComponent = container.GetExportedValue<MefMixedComponent>();
            Assert.IsNotNull(mefMixedComponent);
            Assert.IsInstanceOfType(mefMixedComponent, typeof(MefMixedComponent));
            Assert.IsInstanceOfType(mefMixedComponent.MefComponent, typeof(MefComponent1));
            Assert.IsInstanceOfType(mefMixedComponent.UnityComponent, typeof(UnityOnlyComponent1));
        }

        [TestMethod]
        public void UnityResolvesUnityComponentRegisteredWithoutInterfaceTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);

            // Registration
            unityContainer.RegisterType<UnityComponent3>();

            var component2 = unityContainer.Resolve<UnityComponent2>();
            Assert.IsNotNull(component2);
            Assert.IsNotNull(component2.ImportedMefComponent);
            Assert.IsInstanceOfType(component2.ImportedMefComponent, typeof(MefComponent2));
            Assert.IsInstanceOfType(component2.MefComponent, typeof(MefComponent2));
        }

        public class MefSingletonComponent
        {
            public static int Counter;

            public MefSingletonComponent()
            {
                Counter++;
            }
        }

        [TestMethod]
        public void UnityCanResolveMefComponentRegisteredUsingAddExportedValueTest()
        {
            MefSingletonComponent.Counter = 0;

            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            unityContainer.RegisterCatalog(assemblyCatalog);
            var compositionContainer = unityContainer.Configure<CompositionIntegration>().CompositionContainer;
            var batch = new CompositionBatch();
            var singletonComponent = new MefSingletonComponent();

            batch.AddExportedValue(singletonComponent);
            compositionContainer.Compose(batch);

            var singletonComponent1 = compositionContainer.GetExport<MefSingletonComponent>().Value;
            Assert.AreEqual(MefSingletonComponent.Counter, 1);
            Assert.AreSame(singletonComponent1, singletonComponent);

            var singletonComponent2 = unityContainer.Resolve<MefSingletonComponent>();
            Assert.AreEqual(1, MefSingletonComponent.Counter);
            Assert.AreSame(singletonComponent2, singletonComponent);
        }
    }
}