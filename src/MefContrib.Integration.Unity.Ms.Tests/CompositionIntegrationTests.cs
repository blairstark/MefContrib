using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Integration.Unity.Ms.Tests
{
    [TestClass]
    public class CompositionIntegrationTests
    {
        [TestMethod]
        public void UnityCanResolveMefComponentRegisteredByTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var mefComponent = unityContainer.Resolve<IMefComponent>();
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent, typeof(MefComponent1));

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.IsNotNull(unityComponent);
            Assert.IsInstanceOfType(unityComponent.MefComponent, typeof(MefComponent1));
        }

        [TestMethod]
        public void UnityCanResolveMefComponentRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var mefComponent = unityContainer.Resolve<IMefComponent>("component2");
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent, typeof(MefComponent2));

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.IsNotNull(unityComponent);
            Assert.IsInstanceOfType(unityComponent.MefComponent, typeof(MefComponent2));
        }

        [TestMethod]
        public void UnityCanResolveMefComponentRegisteredByTypeUsingConstructorInjectionTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.IsNotNull(unityComponent);
            Assert.IsInstanceOfType(unityComponent.MefComponent, typeof(MefComponent1));
        }

        [TestMethod]
        public void UnityCanResolveMefComponentRegisteredByTypeAndRegistrationNameUsingConstructorInjectionTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.IsNotNull(unityComponent);
            Assert.IsInstanceOfType(unityComponent.MefComponent, typeof(MefComponent2));
        }

        [TestMethod]
        public void UnitySatisfiesMefImportsByTypeOnUnityComponentsTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.IsNotNull(unityComponent);
            Assert.IsInstanceOfType(unityComponent.ImportedMefComponent, typeof(MefComponent1));
        }

        [TestMethod]
        public void UnityLazySatisfiesMefImportsByTypeOnUnityComponentsTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent11>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.IsNotNull(unityComponent);
            Assert.IsInstanceOfType(unityComponent, typeof(UnityComponent11));
            Assert.IsInstanceOfType(unityComponent.ImportedMefComponent, typeof(MefComponent1));
            Assert.IsInstanceOfType(unityComponent.MefComponent, typeof(MefComponent1));

            var unityComponent11 = (UnityComponent11)unityComponent;
            var mefComponent = unityComponent11.MefComponentFactory();
            Assert.AreSame(mefComponent, unityComponent.MefComponent);
        }

        [TestMethod]
        public void UnitySatisfiesMefImportsByTypeAndRegistrationNameOnUnityComponentsTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.IsNotNull(unityComponent);
            Assert.IsInstanceOfType(unityComponent.ImportedMefComponent, typeof(MefComponent2));
        }

        [TestMethod]
        public void UnityDoesNotSatisfyMefImportsOnUnityComponentsWhenMarkedWithPartNotComposableAttributeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IUnityComponent, UnityComponent3>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();
            Assert.IsNotNull(unityComponent);
            Assert.IsNull(unityComponent.ImportedMefComponent);
        }

        [TestMethod]
        public void UnityCanResolveCompositionContainerTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var compositionContainer = unityContainer.Resolve<CompositionContainer>();
            Assert.IsNotNull(compositionContainer);
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void UnityCannotResolveCompositionContainerWhenExplicitlyDisallowedTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(false));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var internalCompositionContainer = unityContainer.Configure<CompositionIntegration>().CompositionContainer;
            Assert.IsNotNull(internalCompositionContainer);
            Assert.IsFalse(unityContainer.Configure<CompositionIntegration>().Register);
            unityContainer.Resolve<CompositionContainer>();
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void UnityCannotResolveMultipleMefInstancesTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.Resolve<IMultipleMefComponent>();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void DisposingUnityDisposesCompositionContainerTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddNewExtension<CompositionIntegration>();
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var compositionContainer = unityContainer.Resolve<CompositionContainer>();
            unityContainer.Dispose();
            compositionContainer.GetExport<IMefComponent>();
        }

    }
}