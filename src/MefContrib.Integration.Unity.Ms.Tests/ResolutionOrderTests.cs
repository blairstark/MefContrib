using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Practices.Unity; 
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Integration.Unity.Ms.Tests
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

            Assert.AreEqual(Singleton.Count, 0);
            var singleton = unityContainer.Resolve<ISingleton>();

            Assert.IsNotNull(singleton);
            Assert.AreEqual(Singleton.Count, 1);

            var mef = unityContainer.Resolve<CompositionContainer>();
            var mefSingleton = mef.GetExportedValue<ISingleton>();

            Assert.AreEqual(Singleton.Count, 1);
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

            Assert.AreEqual(Singleton.Count, 0);

            var mef = unityContainer.Resolve<CompositionContainer>();
            mef.GetExportedValues<ISingleton>();
            Assert.AreEqual(Singleton.Count, 2);
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