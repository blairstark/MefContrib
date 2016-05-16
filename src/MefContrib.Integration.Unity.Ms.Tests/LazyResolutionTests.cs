using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity; 
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Integration.Unity.Ms.Tests
{
    [TestClass]
    public class LazyResolutionTests
    {
        #region Fake components

        public interface IMixedComponent { }

        public class MixedComponent1 : IMixedComponent
        {
            public static int InstanceCount;

            public MixedComponent1()
            {
                InstanceCount++;
            }
        }

        public class MixedComponent2 : IMixedComponent
        {
            public static int InstanceCount;

            public MixedComponent2()
            {
                InstanceCount++;
            }
        }

        public class MixedComponent3 : IMixedComponent { }

        [Export(typeof(IMixedComponent))]
        public class MixedComponent4 : IMixedComponent { }

        [Export(typeof(IMixedComponent))]
        public class MixedComponent5 : IMixedComponent
        {
            public static int InstanceCount;

            public MixedComponent5()
            {
                InstanceCount++;
            }
        }

        #endregion

        [TestMethod]
        public void UnityCanResolveLazyTypeRegisteredInMefTest()
        {
            MefComponent1.InstanceCount = 0;

            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(false));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            Assert.AreEqual(MefComponent1.InstanceCount, 0);

            var lazyMefComponent = unityContainer.Resolve<Lazy<IMefComponent>>();
            Assert.AreEqual(MefComponent1.InstanceCount, 0);
            Assert.IsNotNull(lazyMefComponent);
            Assert.IsNotNull(lazyMefComponent.Value);
            Assert.AreEqual(MefComponent1.InstanceCount, 1);
            Assert.IsInstanceOfType(lazyMefComponent.Value, typeof(MefComponent1));
        }

        [TestMethod]
        public void UnityCanResolveLazyTypeRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(false));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            UnityComponent1.InstanceCount = 0;
            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var lazyUnityComponent = unityContainer.Resolve<Lazy<IUnityComponent>>();
            Assert.IsNotNull(lazyUnityComponent);
            Assert.AreEqual(UnityComponent1.InstanceCount, 0);

            Assert.IsNotNull(lazyUnityComponent.Value);
            Assert.IsInstanceOfType(lazyUnityComponent.Value, typeof(UnityComponent1));
            Assert.AreEqual(UnityComponent1.InstanceCount,1);
        }
        
        [TestMethod]
        public void UnityCanResolveLazyEnumerableOfTypesRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(false));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            UnityComponent1.InstanceCount = 0;
            
            unityContainer.RegisterType<IUnityComponent, UnityComponent1>("component1");
            unityContainer.RegisterType<IUnityComponent, UnityComponent2>("component2");

            var collectionOfLazyUnityComponents = unityContainer.Resolve<Lazy<IEnumerable<IUnityComponent>>>();
            Assert.IsNotNull(collectionOfLazyUnityComponents);

            Assert.AreEqual(UnityComponent1.InstanceCount,0);
            var list = new List<IUnityComponent>(collectionOfLazyUnityComponents.Value);
            Assert.AreEqual(UnityComponent1.InstanceCount, 1);
            Assert.AreEqual(list.Count, 2);
        }

        [TestMethod]
        public void UnityCanResolveEnumerableOfLazyTypesRegisteredInUnityAndMefTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            MixedComponent1.InstanceCount = 0;
            MixedComponent2.InstanceCount = 0;
            MixedComponent5.InstanceCount = 0;

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(true));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IMixedComponent, MixedComponent1>("component1");
            unityContainer.RegisterType<IMixedComponent, MixedComponent2>("component2");
            unityContainer.RegisterType<IMixedComponent, MixedComponent3>();

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<Lazy<IMixedComponent>>>();
            Assert.IsNotNull(collectionOfLazyUnityComponents);

            Assert.AreEqual(MixedComponent1.InstanceCount, 0);
            Assert.AreEqual(MixedComponent2.InstanceCount,0);
            Assert.AreEqual(MixedComponent5.InstanceCount,0);

            var list = new List<Lazy<IMixedComponent>>(collectionOfLazyUnityComponents);

            Assert.AreEqual(MixedComponent1.InstanceCount, 0);
            Assert.AreEqual(MixedComponent2.InstanceCount, 0);
            Assert.AreEqual(MixedComponent5.InstanceCount, 0);

            Assert.IsNotNull(list[0].Value);
            Assert.IsNotNull(list[1].Value);
            Assert.IsNotNull(list[2].Value);
            Assert.IsNotNull(list[3].Value);
            Assert.IsNotNull(list[4].Value);

            Assert.AreEqual(MixedComponent1.InstanceCount, 1);
            Assert.AreEqual(MixedComponent2.InstanceCount, 1);
            Assert.AreEqual(MixedComponent5.InstanceCount, 1);

            Assert.AreEqual(list.Count, 5);

            Assert.AreEqual(list.Select(t => t.Value).OfType<MixedComponent1>().Count(), 1);
            Assert.AreEqual(list.Select(t => t.Value).OfType<MixedComponent2>().Count(), 1);
            Assert.AreEqual(list.Select(t => t.Value).OfType<MixedComponent3>().Count(), 1);
            Assert.AreEqual(list.Select(t => t.Value).OfType<MixedComponent4>().Count(), 1);
            Assert.AreEqual(list.Select(t => t.Value).OfType<MixedComponent5>().Count(), 1);
        }

        [TestMethod]
        public void UnityCanResolveEnumerableOfTypesRegisteredInUnityAndMefTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            MixedComponent1.InstanceCount = 0;
            MixedComponent2.InstanceCount = 0;
            MixedComponent5.InstanceCount = 0;

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(true));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IMixedComponent, MixedComponent1>("component1");
            unityContainer.RegisterType<IMixedComponent, MixedComponent2>("component2");
            unityContainer.RegisterType<IMixedComponent, MixedComponent3>();

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<IMixedComponent>>();
            Assert.IsNotNull(collectionOfLazyUnityComponents);

            Assert.AreEqual(MixedComponent1.InstanceCount, 1);
            Assert.AreEqual(MixedComponent2.InstanceCount, 1);
            Assert.AreEqual(MixedComponent5.InstanceCount, 1);

            var list = new List<IMixedComponent>(collectionOfLazyUnityComponents);
            Assert.AreEqual(list.Count, 5);
            Assert.AreEqual(list.OfType<MixedComponent1>().Count(), 1);
            Assert.AreEqual(list.OfType<MixedComponent2>().Count(), 1);
            Assert.AreEqual(list.OfType<MixedComponent3>().Count(), 1);
            Assert.AreEqual(list.OfType<MixedComponent4>().Count(), 1);
            Assert.AreEqual(list.OfType<MixedComponent5>().Count(), 1);
        }

        [TestMethod]
        public void UnityCanResolveEnumerableOfLazyTypesRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(true));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            UnityComponent1.InstanceCount = 0;
            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();
            unityContainer.RegisterType<IUnityComponent, UnityComponent2>("component2");

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<Lazy<IUnityComponent>>>();
            Assert.IsNotNull(collectionOfLazyUnityComponents);

            Assert.AreEqual(UnityComponent1.InstanceCount, 0);
            var list = new List<Lazy<IUnityComponent>>(collectionOfLazyUnityComponents);
            Assert.AreEqual(UnityComponent1.InstanceCount, 0);
            Assert.IsNotNull(list[0].Value);
            Assert.IsNotNull(list[1].Value);
            Assert.AreEqual(UnityComponent1.InstanceCount, 1);
            Assert.AreEqual(list.Count, 2);
        }

        [TestMethod]
        public void UnityCanResolveEnumerableOfTypesRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(true));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            UnityComponent1.InstanceCount = 0;
            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();
            unityContainer.RegisterType<IUnityComponent, UnityComponent2>("component2");

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<IUnityComponent>>();
            Assert.IsNotNull(collectionOfLazyUnityComponents);
            Assert.AreEqual(UnityComponent1.InstanceCount, 1);

            var list = new List<IUnityComponent>(collectionOfLazyUnityComponents);
            Assert.AreEqual(list.Count, 2);
        }

        public interface IModule { }

        [Export(typeof(IModule))]
        public class Module1 : IModule { }

        public class Module2 : IModule { }

        [TestMethod]
        public void UnityCanResolveEnumerableOfTypesRegisteredInUnityEvenIfOnOfTheTypesIsExportedViaMefTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            unityContainer.AddNewExtension<CompositionIntegration>();

            unityContainer.RegisterType<IModule, Module1>();
            unityContainer.RegisterType<IModule, Module2>("mod2");

            var modules1 = unityContainer.Resolve<IEnumerable<IModule>>();
            Assert.AreEqual(modules1.Count(), 2);

            var modules2 = unityContainer.Resolve<IEnumerable<IModule>>();
            Assert.AreEqual(modules2.Count(), 2);
        }
        
        [TestMethod]
        public void UnityCanResolveEnumerableOfTypesRegisteredInUnityAndMefEvenIfBothMefAndUnityRegisterTheSameTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            unityContainer.RegisterCatalog(new AssemblyCatalog(typeof(IModule).Assembly));

            unityContainer.RegisterType<IModule, Module1>();
            unityContainer.RegisterType<IModule, Module2>("module2");

            var modules1 = unityContainer.Resolve<IEnumerable<IModule>>();
            Assert.AreEqual(modules1.Count(), 3);
            Assert.AreEqual(modules1.OfType<Module1>().Count(), 2);

            var modules2 = unityContainer.Resolve<IEnumerable<IModule>>();
            Assert.AreEqual(modules2.Count(),3);
            Assert.AreEqual(modules1.OfType<Module1>().Count(), 2);
        }
    }
}