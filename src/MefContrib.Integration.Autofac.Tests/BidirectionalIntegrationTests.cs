#region Header

// -----------------------------------------------------------------------------
//  Copyright (c) Edenred (Incentives & Motivation) Ltd.  All rights reserved.
// -----------------------------------------------------------------------------

#endregion

namespace MefContrib.Integration.Autofac.Tests
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Reflection;
    using global::Autofac;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BidirectionalIntegrationTests
    {
        public class MefSingletonComponent
        {
            #region Fields

            public static int Counter;

            #endregion

            #region Constructors

            public MefSingletonComponent()
            {
                Counter++;
            }

            #endregion
        }

        [TestMethod]
        public void AutofacCanResolveAutofacComponentThatHasAutofacAndMefDependenciesTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            builder.RegisterType<AutofacMixedComponent>();

            // Test
            var container = builder.Build();
            var autofacMixedComponent = container.Resolve<AutofacMixedComponent>();
            Assert.IsNotNull(autofacMixedComponent);
            Assert.IsInstanceOfType(autofacMixedComponent, typeof (AutofacMixedComponent));
            Assert.IsInstanceOfType(autofacMixedComponent.MefComponent, typeof (MefComponent1));
            Assert.IsInstanceOfType(autofacMixedComponent.AutofacComponent, typeof (AutofacOnlyComponent1));
        }

        [TestMethod]
        public void AutofacCanResolveMefComponentRegisteredUsingAddExportedValueTest()
        {
            MefSingletonComponent.Counter = 0;

            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            var autofacContainer = builder.Build();
            var compositionContainer = autofacContainer.Resolve<CompositionContainer>();
            var batch = new CompositionBatch();
            var singletonComponent = new MefSingletonComponent();

            batch.AddExportedValue(singletonComponent);
            compositionContainer.Compose(batch);

            var singletonComponent1 = compositionContainer.GetExport<MefSingletonComponent>().Value;
            Assert.AreEqual(1, MefSingletonComponent.Counter);
            Assert.AreSame(singletonComponent1, singletonComponent);

            var singletonComponent2 = autofacContainer.Resolve<MefSingletonComponent>();
            Assert.AreEqual(1, MefSingletonComponent.Counter);
            Assert.AreSame(singletonComponent2, singletonComponent);
        }

        [TestMethod]
        public void AutofacCanResolveMefComponentThatHasAutofacDependenciesTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();

            // Test
            var container = builder.Build();
            var mefComponent = container.Resolve<IMefComponentWithAutofacDependencies>();
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent.MefOnlyComponent, typeof (MefComponent1));
            Assert.IsInstanceOfType(mefComponent.AutofacOnlyComponent, typeof (AutofacOnlyComponent1));
        }

        [TestMethod]
        public void AutofacCanResolveMefComponentThatHasAutofacDependenciesThatHaveMefDependenciesTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();
            builder.RegisterType<AutofacComponent1>().As<IAutofacComponent>();

            // Test
            var container = builder.Build();
            var mefComponent = container.ResolveNamed<IMefComponentWithAutofacDependencies>("component2");
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent, typeof (MefComponentWithAutofacDependencies2));
            Assert.IsInstanceOfType(mefComponent.MefOnlyComponent, typeof (MefComponent1));
            Assert.IsInstanceOfType(mefComponent.AutofacOnlyComponent,typeof (AutofacOnlyComponent1));

            var mefComponentWithAutofacDependencies2 = (MefComponentWithAutofacDependencies2) mefComponent;
            Assert.IsInstanceOfType(mefComponentWithAutofacDependencies2.MixedAutofacMefComponent,
                        typeof (AutofacComponent1));
            Assert.IsInstanceOfType(mefComponentWithAutofacDependencies2.MixedAutofacMefComponent.MefComponent,
                        typeof (MefComponent1));
        }

        [TestMethod]
        public void AutofacCircularDependencyIsDetectedTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacOnlyComponent1>();

            // Test
            var container = builder.Build();
            var autofacOnlyComponent1 = container.Resolve<AutofacOnlyComponent1>();
            Assert.IsNotNull(autofacOnlyComponent1);
        }

        [TestMethod]
        public void AutofacContainerCanBeResolvedByMefTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);

            // Test
            var container = builder.Build();
            var compositionContainer1 = container.Resolve<CompositionContainer>();
            var compositionContainer2 = container.Resolve<CompositionContainer>();
            Assert.IsNotNull(compositionContainer1);
            Assert.IsNotNull(compositionContainer2);
            Assert.AreSame(compositionContainer1, compositionContainer2);

            var autofacContainerFromMef1 = compositionContainer1.GetExportedValue<ILifetimeScope>();
            var autofacContainerFromMef2 = compositionContainer1.GetExportedValue<ILifetimeScope>();

            Assert.IsNotNull(autofacContainerFromMef1);
            Assert.IsNotNull(autofacContainerFromMef2);
            Assert.AreSame(autofacContainerFromMef1, autofacContainerFromMef2);
        }

        [TestMethod]
        public void AutofacResolvesAutofacComponentRegisteredWithoutInterfaceTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);

            // Registration
            builder.Register((c, p) => new AutofacComponent2(c.ResolveNamed<IMefComponent>("component2")));
            builder.Register((c, p) => new AutofacComponent3(c.ResolveNamed<IMefComponent>("component2")));
            var container = builder.Build();
            var component2 = container.Resolve<AutofacComponent2>();
            Assert.IsNotNull(component2);
            Assert.IsNotNull(component2.ImportedMefComponent);
            Assert.IsInstanceOfType(component2.ImportedMefComponent, typeof (MefComponent2));
            Assert.IsInstanceOfType(component2.MefComponent, typeof (MefComponent2));
        }

        [TestMethod]
        public void MefCanResolveMefComponentThatHasAutofacAndMefDependenciesTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>();

            // Test
            var autofacContainer = builder.Build();
            var compositionContainer = autofacContainer.Resolve<CompositionContainer>();
            var mefMixedComponent = compositionContainer.GetExportedValue<MefMixedComponent>();
            Assert.IsNotNull(mefMixedComponent);
            Assert.IsInstanceOfType(mefMixedComponent, typeof (MefMixedComponent));
            Assert.IsInstanceOfType(mefMixedComponent.MefComponent, typeof (MefComponent1));
            Assert.IsInstanceOfType(mefMixedComponent.AutofacComponent, typeof (AutofacOnlyComponent1));
        }

        [TestMethod]
        public void MefResolvesServiceRegisteredInAutofacByTypeTest()
        {
            // Setup
            var builder = new ContainerBuilder();
            builder.EnableCompositionIntegration();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Register catalog and types
            builder.RegisterCatalog(assemblyCatalog);

            // Registration
            builder.RegisterType<AutofacOnlyComponent1>().As<IAutofacOnlyComponent>().InstancePerLifetimeScope();

            var autofacContainer = builder.Build();
            var compositionContainer = autofacContainer.Resolve<CompositionContainer>();
            var fromMef = compositionContainer.GetExportedValue<IAutofacOnlyComponent>();
            var fromAutofac = autofacContainer.Resolve<IAutofacOnlyComponent>();
            Assert.IsNotNull(fromMef);
            Assert.IsInstanceOfType(fromMef, typeof (AutofacOnlyComponent1));
            Assert.IsNotNull(fromAutofac);
            Assert.IsInstanceOfType(fromAutofac, typeof (AutofacOnlyComponent1));
            Assert.AreEqual(fromMef,fromAutofac);
        }
    }
}