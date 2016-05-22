using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using Autofac;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace MefContrib.Integration.Autofac.Ms.Tests
{
    [TestClass]
    public class CompositionIntegrationTests
      {

        public static class CompositionContainerExporter
        {
            static CompositionContainerExporter()
            {
                Container = new CompositionContainer();
            }

            [Export]
            public static CompositionContainer Container { get; set; }
        }
        [TestMethod]
        public void AutofacCanResolveMefComponentRegisteredByTypeTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);

            builder.RegisterType<AutofacComponent1>().As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var mefComponent = autofacContainer.Resolve<IMefComponent>();
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent, typeof(MefComponent1));

            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.IsNotNull(autofacComponent);
            Assert.IsInstanceOfType(autofacComponent.MefComponent, typeof(MefComponent1));
        }

        [TestMethod]
        public void AutofacCanResolveMefComponentRegisteredByTypeAndRegistrationNameTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.Register((c, p) => new AutofacComponent2(c.ResolveNamed<IMefComponent>("component2"))).As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var mefComponent = autofacContainer.ResolveNamed<IMefComponent>("component2");
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent, typeof(MefComponent2));

            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.IsNotNull(autofacComponent);
            Assert.IsInstanceOfType(autofacComponent.MefComponent, typeof(MefComponent2));
        }

        [TestMethod]
        public void AutofacCanResolveMefComponentRegisteredByTypeUsingConstructorInjectionTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacComponent1>().As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.IsNotNull(autofacComponent);
            Assert.IsInstanceOfType(autofacComponent.MefComponent, typeof(MefComponent1));
        }

        [TestMethod]
        public void AutofacCanResolveMefComponentRegisteredByTypeAndRegistrationNameUsingConstructorInjectionTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.Register((c, p) => new AutofacComponent2(c.ResolveNamed<IMefComponent>("component2"))).As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.IsNotNull(autofacComponent);
            Assert.IsInstanceOfType(autofacComponent.MefComponent, typeof(MefComponent2));
        }

        [TestMethod]
        public void AutofacSatisfiesMefImportsByTypeOnAutofacComponentsTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacComponent1>().As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.IsNotNull(autofacComponent);
            Assert.IsInstanceOfType(autofacComponent.ImportedMefComponent, typeof(MefComponent1));
        }

        [TestMethod]
        public void AutofacLazySatisfiesMefImportsByTypeOnAutofacComponentsTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacComponent1>().As<IAutofacComponent>();

            builder.RegisterType<AutofacComponent11>().As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.IsNotNull(autofacComponent);
            Assert.IsInstanceOfType(autofacComponent, typeof(AutofacComponent11));
            Assert.IsInstanceOfType(autofacComponent.ImportedMefComponent, typeof(MefComponent1));
            Assert.IsInstanceOfType(autofacComponent.MefComponent, typeof(MefComponent1));

            var autofacComponent11 = (AutofacComponent11)autofacComponent;
            var mefComponent = autofacComponent11.MefComponentFactory();
            Assert.AreSame(mefComponent, autofacComponent.MefComponent);
        }

        [TestMethod]
        public void AutofacSatisfiesMefImportsByTypeAndRegistrationNameOnAutofacComponentsTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.Register((c, p) => new AutofacComponent2(c.ResolveNamed<IMefComponent>("component2"))).As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.IsNotNull(autofacComponent);
            Assert.IsInstanceOfType(autofacComponent.ImportedMefComponent, typeof(MefComponent2));
        }

        [TestMethod]
        public void AutofacDoesNotSatisfyMefImportsOnAutofacComponentsWhenMarkedWithPartNotComposableAttributeTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);
            builder.Register((c, p) => new AutofacComponent3(c.ResolveNamed<IMefComponent>("component2"))).As<IAutofacComponent>();

            var autofacContainer = builder.Build();
            var autofacComponent = autofacContainer.Resolve<IAutofacComponent>();
            Assert.IsNotNull(autofacComponent);
            Assert.IsNull(autofacComponent.ImportedMefComponent);
        }

        [TestMethod]
        public void AutofacCanResolveCompositionContainerTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);

            var autofacContainer = builder.Build();
            var compositionContainer = autofacContainer.Resolve<CompositionContainer>();
            Assert.IsNotNull(compositionContainer);
        }

        [TestMethod]
        public void AutofacCanResolveMultipleMefInstancesTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);

            var autofacContainer = builder.Build();
            try
            {
                var defaultInstance = autofacContainer.Resolve<IMultipleMefComponent>();
                Debug.WriteLine("Default Instance -> {0}", defaultInstance);
                var all = autofacContainer.Resolve<IEnumerable<IMultipleMefComponent>>().ToArray();
                Debug.WriteLine("All instances -> {0}, {1}", all);
                return;
            }
            catch
            {
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void DisposingAutofacDisposesCompositionContainerTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for autofac
            builder.EnableCompositionIntegration();
            builder.RegisterCatalog(assemblyCatalog);

            var autofacContainer = builder.Build();
            var compositionContainer = autofacContainer.Resolve<CompositionContainer>();
            autofacContainer.Dispose();

            var temp = compositionContainer.GetExport<IMefComponent>();
        }
    }
}
