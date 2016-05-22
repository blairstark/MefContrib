using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using MefContrib.Integration.Unity.Extensions;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Integration.Unity.Tests
{
    [TestClass]
    public class UnityContainerExtensionsTests
    {
        public interface IMefExposedComponent
        {
        }

        [Export(typeof(IMefExposedComponent))]
        public class MefComponent : IMefExposedComponent
        {
        }

        public interface IUnityComponent
        {
        }

        public class UnityComponent : IUnityComponent
        {
        }

        public interface IMefDependingOnUnity
        {
            IUnityComponent UnityComponent { get; }
        }

        public interface IMefDependingOnUnity2
        {
            IUnityComponent UnityComponent { get; }
        }

        [Export(typeof(IMefDependingOnUnity))]
        public class MefComponentDependingOnUnity : IMefDependingOnUnity
        {
            private readonly IUnityComponent m_UnityComponent;

            [ImportingConstructor]
            public MefComponentDependingOnUnity(IUnityComponent unityComponent)
            {
                m_UnityComponent = unityComponent;
            }

            public IUnityComponent UnityComponent { get { return m_UnityComponent; } }
        }

        [Export(typeof(IMefDependingOnUnity2))]
        public class MefComponentDependingOnUnityByProperty : IMefDependingOnUnity2
        {
            [Import]
            public IUnityComponent UnityComponent { get; set; }
        }
        
        private static UnityContainer ConfigureUnityThenMef()
        {
            var container = new UnityContainer();
            TypeRegistrationTrackerExtension.RegisterIfMissing(container);

            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            container.RegisterType<IUnityComponent, UnityComponent>();
            container.RegisterType<ICountableUnityComponent, CountableUnityComponent>();
            container.RegisterCatalog(catalog);

            return container;
        }

        private static UnityContainer ConfigureMefThenUnity()
        {
            var container = new UnityContainer();
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            container.RegisterCatalog(catalog);
            container.RegisterType<IUnityComponent, UnityComponent>();
            container.RegisterType<ICountableUnityComponent, CountableUnityComponent>();

            return container;
        }

        [TestMethod]
        public void ResolveUnityFromUnityTest()
        {
            var container = ConfigureUnityThenMef();

            var component = container.Resolve<IUnityComponent>();
            Assert.IsNotNull(component);
        }

        [TestMethod]
        public void ResolveUnityFromUnity2Test()
        {
            var container = ConfigureMefThenUnity();

            var component = container.Resolve<IUnityComponent>();
            Assert.IsNotNull(component);
        }

        [TestMethod]
        public void ResolveMefFromUnityTest()
        {
            var container = ConfigureUnityThenMef();

            var component = container.Resolve<IMefExposedComponent>();
            Assert.IsNotNull(component);
        }
        [TestMethod]
        public void ResolveMefFromUnity2Test()
        {
            var container = ConfigureMefThenUnity();

            var component = container.Resolve<IMefExposedComponent>();
            Assert.IsNotNull(component);
        }

        [TestMethod]
        public void ResolveUnityFromMefByCtorTest()
        {
            var container = ConfigureUnityThenMef();

            var unityComponent = container.Resolve<IUnityComponent>();

            var component = container.Resolve<IMefDependingOnUnity>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreNotEqual(unityComponent, component.UnityComponent);
        }
        
        [TestMethod]
        public void ResolveUnityFromMefByCtor2Test()
        {
            var container = ConfigureMefThenUnity();

            var unityComponent = container.Resolve<IUnityComponent>();

            var component = container.Resolve<IMefDependingOnUnity>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreNotEqual(unityComponent, component.UnityComponent);
        }

        [TestMethod]
        public void ResolveUnityFromMefByPropTest()
        {
            var container = ConfigureUnityThenMef();

            var unityComponent = container.Resolve<IUnityComponent>();

            var component = container.Resolve<IMefDependingOnUnity2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreNotEqual(unityComponent, component.UnityComponent);
        }

        [TestMethod]
        public void ResolveUnityFromMefByProp2Test()
        {
            var container = ConfigureMefThenUnity();

            var unityComponent = container.Resolve<IUnityComponent>();

            var component = container.Resolve<IMefDependingOnUnity2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreNotEqual(unityComponent, component.UnityComponent);
        }

        [TestMethod]
        public void ResolveUnityFromMefByCtorWithScopeTest()
        {
            IUnityContainer container = ConfigureUnityThenMef();

            var unityComponent = container.Resolve<IUnityComponent>();
            container = container.CreateChildContainer(true);
            container.RegisterInstance(unityComponent);

            var component = container.Resolve<IMefDependingOnUnity>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreEqual(unityComponent, component.UnityComponent);
        }

        [TestMethod]
        public void ResolveUnityFromMefByCtorWithScope2Test()
        {
            IUnityContainer container = ConfigureMefThenUnity();

            var unityComponent = container.Resolve<IUnityComponent>();
            container = container.CreateChildContainer();
            container.EnableCompositionIntegration();
            container.RegisterInstance(unityComponent);

            var component = container.Resolve<IMefDependingOnUnity>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreEqual(unityComponent, component.UnityComponent);
        }

        [TestMethod]
        public void ResolveUnityFromMefByPropWithScopeTest()
        {
            IUnityContainer container = ConfigureUnityThenMef();

            var unityComponent = container.Resolve<IUnityComponent>();
            container = container.CreateChildContainer();
            container.EnableCompositionIntegration();
            container.RegisterInstance(unityComponent);

            var component = container.Resolve<IMefDependingOnUnity2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreEqual(unityComponent, component.UnityComponent);
        }

        [TestMethod]
        public void ResolveUnityFromMefByPropWithScope2Test()
        {
            IUnityContainer container = ConfigureMefThenUnity();

            var unityComponent = container.Resolve<IUnityComponent>();
            container = container.CreateChildContainer();
            container.EnableCompositionIntegration();
            container.RegisterInstance(unityComponent);

            var component = container.Resolve<IMefDependingOnUnity2>();
            Assert.IsNotNull(component);
            Assert.IsNotNull(component.UnityComponent);
            Assert.AreEqual(unityComponent, component.UnityComponent);
        }


        public interface ICountableUnityComponent
        {
            int InstanceCount { get; }
        }

        public interface ICountableMefComponent
        {
            int InstanceCount { get; }
        }

        public class CountableUnityComponent : ICountableUnityComponent
        {
            private static int m_InstanceCount;

            public CountableUnityComponent()
            {
                ++m_InstanceCount;
            }

            public static void ResetInstanceCount()
            {
                m_InstanceCount = 0;
            }

            public int InstanceCount { get { return m_InstanceCount; } }
        }

        [Export(typeof(ICountableMefComponent))]
        public class CountableMefComponent : ICountableMefComponent
        {
            private static int m_InstanceCount;

            public CountableMefComponent()
            {
                ++m_InstanceCount;
            }

            public static void ResetInstanceCount()
            {
                m_InstanceCount = 0;
            }

            public int InstanceCount { get { return m_InstanceCount; } }
        }

        [TestMethod]
        public void UnityInstanceCountTest()
        {
            CountableUnityComponent.ResetInstanceCount();
            IUnityContainer container = ConfigureUnityThenMef();
            var countable = container.Resolve<ICountableUnityComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        [TestMethod]
        public void UnityInstanceCount2Test()
        {
            CountableUnityComponent.ResetInstanceCount();
            IUnityContainer container = ConfigureMefThenUnity();
            var countable = container.Resolve<ICountableUnityComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        [TestMethod]
        public void MefInstanceCountTest()
        {
            CountableMefComponent.ResetInstanceCount();
            IUnityContainer container = ConfigureUnityThenMef();
            var countable = container.Resolve<ICountableMefComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        [TestMethod]
        public void MefInstanceCount2Test()
        {
            CountableMefComponent.ResetInstanceCount();
            IUnityContainer container = ConfigureMefThenUnity();
            var countable = container.Resolve<ICountableMefComponent>();
            Assert.AreEqual(1, countable.InstanceCount);
        }

        public interface IDependOnCountableUnity
        {
            ICountableUnityComponent Component { get; set; }
        }

        [Export(typeof(IDependOnCountableUnity))]
        public class DependOnCountableUnity : IDependOnCountableUnity
        {
            [Import]
            public ICountableUnityComponent Component { get; set; }
        }

        [TestMethod]
        public void UnityInstanceCountDepScopedTest()
        {
            CountableUnityComponent.ResetInstanceCount();
            IUnityContainer container = ConfigureUnityThenMef();
            var childContainer = container.CreateChildContainer(true);
            var countableDep = childContainer.Resolve<IDependOnCountableUnity>();
            Assert.AreEqual(1, countableDep.Component.InstanceCount);
        }
    }
}