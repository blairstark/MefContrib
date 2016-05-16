using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Tests
{
     [TestClass]
    public class FactoryExportDefinitionTests
    {
        public interface IComponent {}

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cannot_pass_null_type_to_the_ctor()
        {
                new FactoryExportDefinition(null, null, ep => null);
        }

        [TestMethod]
        public void Contract_type_and_names_are_properly_set()
        {
            var exportDefinition = new FactoryExportDefinition(typeof(IComponent), "ContractName", ep => null);
            Assert.AreEqual(typeof(IComponent), exportDefinition.ContractType );
            Assert.AreEqual("ContractName", exportDefinition.RegistrationName);
            Assert.AreEqual("ContractName", exportDefinition.ContractName);
        }

        [TestMethod]
        public void When_passing_null_registration_name_the_contract_name_is_properly_set()
        {
            var exportDefinition = new FactoryExportDefinition(typeof(IComponent), null, ep => null);
            Assert.AreEqual(typeof(IComponent), exportDefinition.ContractType);
            Assert.IsNull(exportDefinition.RegistrationName);
            Assert.AreEqual("MefContrib.Hosting.Tests.FactoryExportDefinitionTests+IComponent", exportDefinition.ContractName);
        }
    }
}