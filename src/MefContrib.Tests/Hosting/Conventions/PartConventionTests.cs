using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PartConventionTests
    {
        [TestMethod]
        public void Export_should_not_be_null_on_new_instance()
        {
            var instance =
                new PartConvention();

            instance.Exports.ShouldNotBeNull();
        }

        [TestMethod]
        public void Import_should_not_be_null_on_new_instance()
        {
            var instance =
                new PartConvention();

            instance.Imports.ShouldNotBeNull();
        }

        [TestMethod]
        public void Metadata_should_not_be_null_on_new_instance()
        {
            var instance =
                new PartConvention();

            instance.Metadata.ShouldNotBeNull();
        }
    }
}