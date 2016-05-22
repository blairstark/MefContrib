namespace MefContrib.Hosting.Conventions.Tests
{
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExportRegistryTests
    {
        [TestMethod]
        public void Export_should_return_instance_of_exportconventionbuilder_for_exportconvention_type()
        {
            var registry =
                new ExportRegistry();

            var result =
                registry.Export();

            result.ShouldBeOfType<ExportConventionBuilder<ExportConvention>>();
        }

        [TestMethod]
        public void Export_of_tconvention_should_return_instance_of_exportconventionbuilder_for_tconvention_type()
        {
            var registry =
                new ExportRegistry();

            var result =
                registry.ExportWithConvention<ExportConvention>();

            result.ShouldBeOfType<ExportConventionBuilder<ExportConvention>>();
        }
    }
}