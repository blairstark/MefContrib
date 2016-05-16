

namespace MefContrib.Hosting.Tests
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FactoryExportProviderTests
    {
        #region Fake External Components

        public interface IExternalComponent
        {
            void Foo();
        }

        public class ExternalComponent1 : IExternalComponent
        {
            public void Foo()
            {
            }
        }

        public class ExternalComponent2 : IExternalComponent
        {
            public void Foo()
            {
            }
        }

        public class ExternalComponent3 : IExternalComponent
        {
            public IExternalComponent ExternalComponent { get; set; }

            public ExternalComponent3(IExternalComponent externalComponent)
            {
                ExternalComponent = externalComponent;
            }

            public void Foo()
            {
            }
        }

        public class ExternalComponent4 : IExternalComponent
        {
            public IExternalComponent ExternalComponent { get; set; }

            public IMefComponent MefComponent { get; set; }

            public ExternalComponent4(IExternalComponent externalComponent, IMefComponent mefComponent)
            {
                ExternalComponent = externalComponent;
                MefComponent = mefComponent;
            }

            public void Foo()
            {
            }
        }

        #endregion

        #region Fake MEF Components

        public interface IMefComponent
        {
            void Foo();

            IExternalComponent Component1 { get; }

            IExternalComponent Component2 { get; }
        }

        [Export(typeof(IMefComponent))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class MefComponent1 : IMefComponent
        {
            private readonly IExternalComponent m_Component1;

            [ImportingConstructor]
            public MefComponent1(IExternalComponent component1)
            {
                m_Component1 = component1;
            }

            public void Foo()
            {
            }

            public IExternalComponent Component1
            {
                get { return m_Component1; }
            }

            [Import]
            public IExternalComponent Component2 { get; set; }
        }

        [Export("component2", typeof(IMefComponent))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class MefComponent2 : IMefComponent
        {
            private readonly IExternalComponent m_Component1;

            [ImportingConstructor]
            public MefComponent2([Import("external2")] IExternalComponent component1)
            {
                m_Component1 = component1;
            }

            public void Foo()
            {
            }

            public IExternalComponent Component1
            {
                get { return m_Component1; }
            }

            [Import("external2")]
            public IExternalComponent Component2 { get; set; }
        }

        [Export("component3", typeof(IMefComponent))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class MefComponent3 : IMefComponent
        {
            public void Foo()
            {
            }

            [Import]
            public IExternalComponent Component1 { get; set; }

            [Import("external2")]
            public IExternalComponent Component2 { get; set; }
        }

        [Export("component4", typeof(IMefComponent))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class MefComponent4 : IMefComponent
        {
            [ImportingConstructor]
            public MefComponent4(IExternalComponent component1, [Import("external2")] IExternalComponent component2)
            {
                Component1 = component1;
                Component2 = component2;
            }

            public void Foo()
            {
            }

            public IExternalComponent Component1 { get; set; }

            public IExternalComponent Component2 { get; set; }
        }

        #endregion

        [TestMethod]
        public void Export_provider_can_resolve_service_registered_by_type()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider(FactoryMethod1);
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            provider.Register(typeof(IExternalComponent));

            var externalComponent = container.GetExportedValue<IExternalComponent>();
            Assert.IsNotNull(externalComponent);
            Assert.IsInstanceOfType(externalComponent, typeof(ExternalComponent1));

            var mefComponent = container.GetExportedValue<IMefComponent>();
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent.Component1, typeof(ExternalComponent1));
            Assert.IsInstanceOfType(mefComponent.Component2, typeof(ExternalComponent1));
        }

        [TestMethod]
        public void Export_provider_can_resolve_service_registered_by_type_and_registration_name()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider(FactoryMethod1);
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            provider.Register(typeof(IExternalComponent), "external2");

            var externalComponent = container.GetExportedValue<IExternalComponent>("external2");
            Assert.IsNotNull(externalComponent);
            Assert.IsInstanceOfType(externalComponent, typeof(ExternalComponent2));

            var mefComponent = container.GetExportedValue<IMefComponent>("component2");
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent.Component1, typeof(ExternalComponent2));
            Assert.IsInstanceOfType(mefComponent.Component2, typeof(ExternalComponent2));
        }

        [TestMethod]
        public void Export_provider_can_resolve_service_registered_by_type_and_or_registration_name()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider(FactoryMethod1);
            var container = new CompositionContainer(assemblyCatalog, provider);

            // Registration
            provider.Register(typeof(IExternalComponent));
            provider.Register(typeof(IExternalComponent), "external2");

            var mefComponent = container.GetExportedValue<IMefComponent>("component3");
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent.Component1, typeof(ExternalComponent1));
            Assert.IsInstanceOfType(mefComponent.Component2, typeof(ExternalComponent2));
        }

        [TestMethod]
        public void Container_can_resolve_services_from_two_factory_export_providers()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider1 = new FactoryExportProvider(FactoryMethod2_1);
            var provider2 = new FactoryExportProvider(FactoryMethod2_2);
            var container = new CompositionContainer(assemblyCatalog, provider1, provider2);

            // Registration
            provider1.Register(typeof(IExternalComponent));
            provider2.Register(typeof(IExternalComponent), "external2");

            var mefComponent = container.GetExportedValue<IMefComponent>("component3");
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent.Component1, typeof(ExternalComponent1));
            Assert.IsInstanceOfType(mefComponent.Component2, typeof(ExternalComponent2));

            mefComponent = container.GetExportedValue<IMefComponent>("component4");
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent.Component1, typeof(ExternalComponent1));
            Assert.IsInstanceOfType(mefComponent.Component2, typeof(ExternalComponent2));
        }

        private static object FactoryMethod1(Type type, string registrationName)
        {
            if (type == typeof(IExternalComponent) && registrationName == null)
                return new ExternalComponent1();

            if (type == typeof(IExternalComponent) && registrationName == "external2")
                return new ExternalComponent2();

            return null;
        }

        private static object FactoryMethod2_1(Type type, string registrationName)
        {
            if (type == typeof(IExternalComponent) && registrationName == null)
                return new ExternalComponent1();

            return null;
        }

        private static object FactoryMethod2_2(Type type, string registrationName)
        {
            if (type == typeof(IExternalComponent) && registrationName == "external2")
                return new ExternalComponent2();

            return null;
        }

        [TestMethod]
        public void Factory_export_provider_can_resolve_service_registered_using_factory_method()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider()
                .Register(typeof(IExternalComponent), ep => new ExternalComponent1())
                .Register(typeof(ExternalComponent2), ep => new ExternalComponent2());
            var container = new CompositionContainer(assemblyCatalog, provider);

            var externalComponent = container.GetExportedValue<IExternalComponent>();
            Assert.IsNotNull(externalComponent);
            Assert.IsInstanceOfType(externalComponent, typeof(ExternalComponent1));

            var externalComponent2 = container.GetExportedValue<ExternalComponent2>();
            Assert.IsNotNull(externalComponent2);
            Assert.IsInstanceOfType(externalComponent2, typeof(ExternalComponent2));

            var mefComponent = container.GetExportedValue<IMefComponent>();
            Assert.IsNotNull(mefComponent);
            Assert.IsInstanceOfType(mefComponent.Component1, typeof(ExternalComponent1));
            Assert.IsInstanceOfType(mefComponent.Component2, typeof(ExternalComponent1));
        }

        [TestMethod]
        public void Factory_export_provider_executes_the_factory_each_time_the_instance_is_requested()
        {
            var count = 0;

            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider(typeof(ExternalComponent2), ep =>
           {
               count++;
               return new ExternalComponent2();
           });
            var container = new CompositionContainer(assemblyCatalog, provider);

            var externalComponent1 = container.GetExportedValue<ExternalComponent2>();
            Assert.IsNotNull(externalComponent1);
            Assert.IsInstanceOfType(externalComponent1, typeof(ExternalComponent2));

            var externalComponent2 = container.GetExportedValue<ExternalComponent2>();
            Assert.IsNotNull(externalComponent2);
            Assert.IsInstanceOfType(externalComponent2, typeof(ExternalComponent2));

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void Factory_export_provider_can_resolve_single_instance()
        {
            var count = 0;

            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider();
            var container = new CompositionContainer(assemblyCatalog, provider);

            provider.RegisterInstance(typeof(IExternalComponent), ep =>
            {
                count++;
                return new ExternalComponent1();
            });

            var externalComponent1 = container.GetExportedValue<IExternalComponent>();
            Assert.IsNotNull(externalComponent1);
            Assert.IsInstanceOfType(externalComponent1, typeof(ExternalComponent1));

            var externalComponent2 = container.GetExportedValue<IExternalComponent>();
            Assert.IsNotNull(externalComponent2);
            Assert.IsInstanceOfType(externalComponent2, typeof(ExternalComponent1));

            Assert.AreEqual(1, count);
            Assert.AreEqual(externalComponent2, externalComponent1);
        }

        [TestMethod]
        public void Factory_export_provider_can_resolve_single_instance_registered_using_generic_overload()
        {
            var count = 0;

            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider();
            var container = new CompositionContainer(assemblyCatalog, provider);

            provider.RegisterInstance<IExternalComponent>(ep =>
            {
                count++;
                return new ExternalComponent1();
            });

            var externalComponent1 = container.GetExportedValue<IExternalComponent>();
            Assert.IsNotNull(externalComponent1);
            Assert.AreEqual(typeof(ExternalComponent1), externalComponent1.GetType());

            var externalComponent2 = container.GetExportedValue<IExternalComponent>();
            Assert.IsNotNull(externalComponent2);
            Assert.IsInstanceOfType(externalComponent2, typeof(ExternalComponent1));

            Assert.AreEqual(1, count);
            Assert.AreEqual(externalComponent2, externalComponent1);
        }

        [TestMethod]
        public void Factory_export_provider_can_resolve_single_instance_given_by_registration_name()
        {
            var count = 0;

            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider();
            var container = new CompositionContainer(assemblyCatalog, provider);

            provider.RegisterInstance(typeof(IExternalComponent), "ext2", ep =>
            {
                count++;
                return new ExternalComponent2();
            });

            var externalComponent1 = container.GetExportedValue<IExternalComponent>("ext2");
            Assert.IsNotNull(externalComponent1);
            Assert.IsInstanceOfType(externalComponent1, typeof(ExternalComponent2));

            var externalComponent2 = container.GetExportedValue<IExternalComponent>("ext2");
            Assert.IsNotNull(externalComponent2);
            Assert.IsInstanceOfType(externalComponent2, typeof(ExternalComponent2));

            Assert.AreEqual(1, count);
            Assert.AreEqual(externalComponent2, externalComponent1);
        }

        [TestMethod]
        public void Factory_export_provider_can_resolve_single_instance_given_by_registration_name_registered_using_generic_overload()
        {
            var count = 0;

            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider();
            var container = new CompositionContainer(assemblyCatalog, provider);
            provider.SourceProvider = container;

            provider.RegisterInstance<IExternalComponent>("ext2", ep =>
            {
                count++;
                return new ExternalComponent2();
            });

            var externalComponent1 = container.GetExportedValue<IExternalComponent>("ext2");
            Assert.IsNotNull(externalComponent1);
            Assert.IsInstanceOfType(externalComponent1, typeof(ExternalComponent2));

            var externalComponent2 = container.GetExportedValue<IExternalComponent>("ext2");
            Assert.IsNotNull(externalComponent2);
            Assert.IsInstanceOfType(externalComponent2, typeof(ExternalComponent2));

            Assert.AreEqual(1, count);
            Assert.AreEqual(externalComponent1, externalComponent2);
        }

        [TestMethod]
        public void Factory_export_provider_can_resolve_additional_exports_from_the_factory()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider();
            var container = new CompositionContainer(assemblyCatalog, provider);
            provider.SourceProvider = provider;

            provider.RegisterInstance<IExternalComponent>(ep => new ExternalComponent2());
            provider.Register<IExternalComponent>("ext3", ep => new ExternalComponent3(ep.GetExportedValue<IExternalComponent>()));

            var externalComponent1 = container.GetExportedValue<IExternalComponent>("ext3");
            Assert.IsNotNull(externalComponent1);
            Assert.IsInstanceOfType(externalComponent1, typeof(ExternalComponent3));
            var externalComponent13 = (ExternalComponent3)externalComponent1;
            Assert.IsNotNull(externalComponent13.ExternalComponent);
            Assert.IsInstanceOfType(externalComponent13.ExternalComponent, typeof(ExternalComponent2));

            var externalComponent2 = container.GetExportedValue<IExternalComponent>("ext3");
            Assert.IsNotNull(externalComponent2);
            Assert.IsInstanceOfType(externalComponent2, typeof(ExternalComponent3));
            var externalComponent23 = (ExternalComponent3)externalComponent1;
            Assert.IsNotNull(externalComponent23.ExternalComponent);
            Assert.IsInstanceOfType(externalComponent23.ExternalComponent, typeof(ExternalComponent2));

            Assert.AreSame(externalComponent13.ExternalComponent, externalComponent23.ExternalComponent);
        }

        [TestMethod]
        public void Factory_export_provider_can_resolve_additional_exports_from_the_container()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider()
                .RegisterInstance<IExternalComponent>(ep => new ExternalComponent2())
                .Register<IExternalComponent>("ext", ep => new ExternalComponent4(
                                                               ep.GetExportedValue<IExternalComponent>(),
                                                               ep.GetExportedValue<IMefComponent>()));

            var container = new CompositionContainer(assemblyCatalog, provider);
            provider.SourceProvider = container;

            var externalComponent1 = container.GetExportedValue<IExternalComponent>("ext");
            Assert.IsNotNull(externalComponent1);
            Assert.IsInstanceOfType(externalComponent1, typeof(ExternalComponent4));
            var externalComponent14 = (ExternalComponent4)externalComponent1;
            Assert.IsNotNull(externalComponent14.ExternalComponent);
            Assert.IsNotNull(externalComponent14.MefComponent);
            Assert.IsInstanceOfType(externalComponent14.ExternalComponent, typeof(ExternalComponent2));
            Assert.IsInstanceOfType(externalComponent14.MefComponent, typeof(MefComponent1));
        }

        [TestMethod]
        public void Factory_export_provider_throws_exception_when_resolving_unknown_parts()
        {
            // Setup
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var provider = new FactoryExportProvider();
            var container = new CompositionContainer(assemblyCatalog, provider);
            provider.SourceProvider = provider;

            provider.RegisterInstance<IExternalComponent>("ext", ep => new ExternalComponent2());

            Exception ex = null;
            try
            {
                container.GetExportedValue<IExternalComponent>();
            }
            catch (Exception e)
            {
                ex = e;
            }
            Assert.IsInstanceOfType(ex, typeof(ImportCardinalityMismatchException));

            var externalComponent2 = container.GetExportedValue<IExternalComponent>("ext");
            Assert.IsNotNull(externalComponent2);
            Assert.IsInstanceOfType(externalComponent2, typeof(ExternalComponent2));
        }
    }
}