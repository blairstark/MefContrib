using System.ComponentModel.Composition.Hosting;
using MefContrib.Hosting.Conventions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Conventions.Tests.Integration
{
     [TestClass]
    public class ConfigurationPartRegistryIntegrationTests
    {
        [TestMethod]
        public void SimpleExport_part_is_properly_exported()
        {
            var registry = new ConfigurationPartRegistry("mef.configuration");
            var catalog = new ConventionCatalog(registry);
            var container = new CompositionContainer(catalog);

            var simpleExport = container.GetExportedValue<SimpleExport>();
            Assert.IsNotNull(simpleExport);
        }

        [TestMethod]
        public void ExportWithPropertyImport_part_is_properly_exported_and_its_imports_are_satisfied()
        {
            var registry = new ConfigurationPartRegistry("mef.configuration");
            var catalog = new ConventionCatalog(registry);
            var container = new CompositionContainer(catalog);

            var exportWithPropertyImport = container.GetExportedValue<ExportWithPropertyImport>();
            Assert.IsNotNull(exportWithPropertyImport);
            Assert.IsNotNull(exportWithPropertyImport.SimpleImport);
        }

        [TestMethod]
        public void SimpleExport_part_with_metadata_is_properly_exported()
        {
            var registry = new ConfigurationPartRegistry("mef.configuration");
            var catalog = new ConventionCatalog(registry);
            var container = new CompositionContainer(catalog);
            var simpleExport = container.GetExport<SimpleExportWithMetadata, ISimpleMetadata>("simple-export");
            Assert.IsNotNull(simpleExport);
            Assert.AreEqual(1234, simpleExport.Metadata.IntValue);
            Assert.AreEqual("some string",simpleExport.Metadata.StrValue);
        }

        [TestMethod]
        public void SimpleContract_part_with_metadata_is_properly_imported()
        {
            var registry = new ConfigurationPartRegistry("mef.configuration");
            var catalog = new ConventionCatalog(registry);
            var container = new CompositionContainer(catalog);

            var simpleExport = container.GetExport<SimpleContractImporter>();
            Assert.IsNotNull(simpleExport);
            Assert.IsNotNull(simpleExport.Value.SimpleContracts);
            Assert.AreEqual(1, simpleExport.Value.SimpleContracts.Length);
            Assert.AreEqual(1234, simpleExport.Value.SimpleContracts[0].Metadata.IntValue);
            Assert.AreEqual("some string", simpleExport.Value.SimpleContracts[0].Metadata.StrValue);

            Assert.IsNotNull(simpleExport.Value.SimpleContractsNoMetadataInterface);
            Assert.AreEqual(1, simpleExport.Value.SimpleContractsNoMetadataInterface.Length);
            Assert.IsInstanceOfType(simpleExport.Value.SimpleContractsNoMetadataInterface[0], typeof(SimpleContract1));
        }
    }
}