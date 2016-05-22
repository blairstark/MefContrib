using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    [TestClass]
    public class TypeExtensionsTests
    {
        [TestMethod]
        public void GetRequiredMetadata_should_return_metadata_items_for_public_non_readable_properties_on_interface()
        {
            var requiredMetadata =
                typeof(IFakeMetadataView).GetRequiredMetadata();

            requiredMetadata.Count().ShouldEqual(1);
        }
    }
}