namespace MefContrib.Hosting.Conventions.Tests
{
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ImportRegistryTests
    {
        [TestMethod]
        public void Import_should_return_instance_of_importconventionbuilder_for_importconvention_type()
        {
            var registry =
                new ImportRegistry();

            var result =
                registry.Import();

            result.ShouldBeOfType<ImportConventionBuilder<ImportConvention>>();
        }

        [TestMethod]
        public void Import_of_tconvention_should_return_instance_of_importconventionbuilder_for_tconvention_type()
        {
            var registry =
                new ImportRegistry();

            var result =
                registry.ImportWithConvention<ImportConvention>();

            result.ShouldBeOfType<ImportConventionBuilder<ImportConvention>>();
        }
    }
}