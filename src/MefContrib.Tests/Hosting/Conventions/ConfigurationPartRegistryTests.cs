using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using MefContrib.Hosting.Conventions.Configuration;
using MefContrib.Hosting.Conventions.Configuration.Section;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Conventions.Tests
{
    [TestClass]
    public class ConfigurationPartRegistryTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Invoking_ctor_with_null_string_causes_an_exception()
        {
            new ConfigurationPartRegistry((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Invoking_ctor_with_null_section_causes_an_exception()
        {
            new ConfigurationPartRegistry((ConventionConfigurationSection)null);
        }

        [TestMethod]
        public void FakePart_is_exported_using_xml_configuration()
        {
            var registry = new ConfigurationPartRegistry("mef.configuration");
            var catalog = new ConventionCatalog(registry);

            var parts = new List<ComposablePartDefinition>(catalog.Parts);
            Assert.AreNotEqual(0, parts.Count);

            var exports = new List<ExportDefinition>(parts[0].ExportDefinitions);
            Assert.AreEqual(1, exports.Count);

            var imports = new List<ImportDefinition>(parts[0].ImportDefinitions);
            Assert.AreEqual(1, imports.Count);
            Assert.AreEqual("somestring", imports[0].ContractName);
            Assert.IsFalse(imports[0].IsRecomposable);
            Assert.IsFalse(imports[0].IsPrerequisite);
        }
    }
}