using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Integration.Unity.Tests
{
    [TestClass]
    public class ResolutionOrderTests
    {
        [TestMethod]
        public void Unity_registered_components_take_precedence_over_MEF_registered_components_if_querying_for_a_single_component_registered_in_both_containers()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var typeCatalog = new TypeCatalog(typeof (Singleton));

            // Register catalog and types
            unityContainer.RegisterCatalog(typeCatalog);
            unityContainer.RegisterType<ISingleton, Singleton>(new ContainerControlledLifetimeManager());

            // Reset count
            Singleton.Count = 0;

            Assert.AreEqual(0, Singleton.Count);
            var singleton = unityContainer.Resolve<ISingleton>();

            Assert.IsNotNull(singleton);
            Assert.AreEqual(1, Singleton.Count);

            var mef = unityContainer.Resolve<CompositionContainer>();
            var mefSingleton = mef.GetExportedValue<ISingleton>();

            Assert.AreEqual(1, Singleton.Count);
            Assert.AreSame(singleton, mefSingleton);
        }

        [TestMethod]
        public void When_querying_MEF_for_a_multiple_components_registered_in_both_containers_all_instances_are_returned()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var typeCatalog = new TypeCatalog(typeof(Singleton));

            // Register catalog and types
            unityContainer.RegisterCatalog(typeCatalog);
            unityContainer.RegisterType<ISingleton, Singleton>(new ContainerControlledLifetimeManager());

            // Reset count
            Singleton.Count = 0;

            Assert.AreEqual(0, Singleton.Count);

            var mef = unityContainer.Resolve<CompositionContainer>();
            mef.GetExportedValues<ISingleton>();
            Assert.AreEqual(2, Singleton.Count);
        }

        
    }

    [Export(typeof(ISingleton))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Singleton : ISingleton
    {
        public static int Count;

        public Singleton()
        {
            Count++;
        }
    }

    public interface ISingleton { }
}