using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Generics.Tests
{
    [TestClass]
    public class GenericContractTypeMappingTests
    {
        [TestMethod]
        public void Generic_contract_type_mapping_is_sealed()
        {
            Assert.IsTrue(typeof(GenericContractTypeMapping).IsSealed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_ctor_with_null_generic_contract_type_definition_causes_argument_null_exception_to_be_thrown()
        {
            new GenericContractTypeMapping(null, typeof(IRepository<>));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calling_ctor_with_null_generic_implementation_type_definition_causes_argument_null_exception_to_be_thrown()
        {
            new GenericContractTypeMapping(typeof(IRepository<>), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_ctor_with_non_generic_contract_type_definition_causes_argument_exception_to_be_thrown()
        {
            new GenericContractTypeMapping(typeof(OrderProcessor), typeof(IRepository<>));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_ctor_with_non_generic_implementation_type_definition_causes_argument_exception_to_be_thrown()
        {
            new GenericContractTypeMapping(typeof(IRepository<>), typeof(OrderProcessor));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_ctor_with_closed_generic_contract_type_definition_causes_argument_exception_to_be_thrown()
        {
            new GenericContractTypeMapping(typeof(IRepository<Order>), typeof(IRepository<>));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calling_ctor_with_closed_generic_implementation_type_definition_causes_argument_exception_to_be_thrown()
        {
            new GenericContractTypeMapping(typeof(IRepository<>), typeof(IRepository<Order>));
        }
    }
}