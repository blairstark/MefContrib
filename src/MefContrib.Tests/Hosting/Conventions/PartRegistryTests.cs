namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PartRegistryTests
    {
        [TestMethod]
        public void ContractService_should_be_instance_of_defaultconventioncontractservice_on_new_instance()
        {
            var registry =
                new PartRegistry();

            registry.ContractService.ShouldBeOfType<DefaultConventionContractService>();
        }

        [TestMethod]
        public void TypeScanner_should_be_null_on_new_instance()
        {
            var registry =
                new PartRegistry();

            registry.TypeScanner.ShouldBeNull();
        }

        [TestMethod]
        public void Part_should_return_instance_of_partconventionbuilder_for_partconvention_type()
        {
            var registry =
                new PartRegistry();

            var result =
                registry.Part();

            result.ShouldBeOfType<PartConventionBuilder<PartConvention>>();
        }

        [TestMethod]
        public void Part_of_tconvention_should_return_instance_of_partconventionbuilder_for_tconvention_type()
        {
            var registry =
                new PartRegistry();

            var result =
                registry.PartWithConvention<PartConvention>();

            result.ShouldBeOfType<PartConventionBuilder<PartConvention>>();
        }

        [TestMethod]
        public void Scan_should_throw_argumentnullexception_when_called_with_null()
        {
            var registry =
                new PartRegistry();

            var exception =
                Catch.Exception(() => registry.Scan(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [TestMethod]
        public void Scan_should_set_typescanner_to_scanner_created_by_closure()
        {
            var registry =
                new PartRegistry();

            registry.Scan(x => {
                x.Types(new[] { typeof(object) });
                x.Types(new[] { typeof(string) });
            });

            var results =
                registry.TypeScanner.GetTypes(x => true);

            results.Count().ShouldEqual(2);
        }
    }
}