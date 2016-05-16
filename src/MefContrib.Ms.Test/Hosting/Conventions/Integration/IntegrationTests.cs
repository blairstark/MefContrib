namespace MefContrib.Hosting.Conventions.Tests.Integration
{
    using MefContrib.Hosting.Conventions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Reflection;



    [TestClass]
    [Category("Integration")]
    public class IntegrationTests
    {
        [TestMethod]
        public void ConventionCatalog_should_support_constructor_injection()
        {
            var catalog =
                new ConventionCatalog(new CtorRegistry());

            var instance =
                new ConventionPart<InjectedHost>();

            var batch =
                new CompositionBatch();
            batch.AddPart(instance);

            var container =
                new CompositionContainer(catalog);

            container.Compose(batch);

            Assert.AreEqual(2, instance.Imports[0].Widgets.Count());
        }

        [TestMethod]
        public void ConventionCatalog_should_support_type_exports()
        {
            var registry = new PartRegistry();
            registry.TypeScanner = new AssemblyTypeScanner(Assembly.GetExecutingAssembly());

            registry
                .Part()
                .ForType<SampleExport>()
                .Export();

            var catalog =
               new ConventionCatalog(registry);

            var instance =
                new ConventionPart<SampleExport>();

            var batch =
                new CompositionBatch();
            batch.AddPart(instance);

            var container =
                new CompositionContainer(catalog);

            container.Compose(batch);
            Assert.AreEqual(1, instance.Imports.Count());
        }

        [TestMethod]
        public void ConventionCatalog_should_support_property_exports()
        {
            var registry = ();
            registry.TypeScanner = new AssemblyTypeScanner(Assembly.GetExecutingAssembly());

            registry
                .Part()
                .ForType<SampleExport>()
                .ExportProperty("TextValue", "V1");

            var catalog =
               new ConventionCatalog(registry);

            var container =
                new CompositionContainer(catalog);

            var exportedValue = container.GetExportedValue<string>("V1");
            Assert.AreEqual("this is some text", exportedValue);
        }

        [TestMethod]
        public void ConventionCatalog_should_support_field_exports()
        {
            var registry = new PartRegistry();
            registry.TypeScanner = new AssemblyTypeScanner(Assembly.GetExecutingAssembly());

            registry
                .Part()
                .ForType<SampleExport>()
                .ExportField("IntValue", "V1");

            var catalog =
               new ConventionCatalog(registry);

            var container =
                new CompositionContainer(catalog);

            var exportedValue = container.GetExportedValue<int>("V1");
            Assert.AreEqual(1234, exportedValue);
        }

        [TestMethod]
        public void ConventionCatalog_should_support_property_imports()
        {
            var registry = new PartRegistry();
            registry.TypeScanner = new AssemblyTypeScanner(Assembly.GetExecutingAssembly());

            registry
                .Part()
                .ForType<SampleExport>()
                .ExportProperty("TextValue", "V1");

            registry
                .Part()
                .ForType<SampleImport>()
                .Export()
                .ImportProperty("TextValue", "V1");

            var catalog =
               new ConventionCatalog(registry);

            var container =
                new CompositionContainer(catalog);

            var exportedValue = container.GetExportedValue<SampleImport>();
            Assert.IsNotNull(exportedValue);
            Assert.AreEqual("this is some text", exportedValue.TextValue);
        }

        [TestMethod]
        public void ConventionCatalog_should_support_field_imports()
        {
            var registry = new PartRegistry();
            registry.TypeScanner = new AssemblyTypeScanner(Assembly.GetExecutingAssembly());

            registry
                .Part()
                .ForType<SampleExport>()
                .ExportField("IntValue", "V1");

            registry
                .Part()
                .ForType<SampleImport>()
                .Export()
                .ImportField("IntValue", "V1");

            var catalog =
               new ConventionCatalog(registry);

            var container =
                new CompositionContainer(catalog);

            var exportedValue = container.GetExportedValue<SampleImport>();
            Assert.IsNotNull(exportedValue);
            Assert.AreEqual(1234, exportedValue.IntValue);
        }
    }

    public class InjectedHost
    {
        public InjectedHost(IEnumerable<IWidget> widgets)
        {
            this.Widgets = widgets;
        }

        public IEnumerable<IWidget> Widgets { get; set; }
    }

    public class CtorRegistry : MefContrib.Hosting.Conventions.Configuration.PartRegistry
    {
        public CtorRegistry()
        {
            Scan(x =>
            {
                x.Assembly(Assembly.GetExecutingAssembly());
            });

            Part()
                .ForType<InjectedHost>()
                .Export()
                .ImportConstructor()
                .MakeShared();

            Part()
                .ForTypesAssignableFrom<IWidget>()
                .ExportAs<IWidget>()
                .MakeShared();
        }
    }

    public interface IWidget
    {
    }

    public class FooWidget : IWidget
    {
    }

    public class BarWidget : IWidget
    {
    }
}
