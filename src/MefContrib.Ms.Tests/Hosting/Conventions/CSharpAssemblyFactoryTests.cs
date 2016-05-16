using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Conventions.Tests
{

    [TestClass]
    public class CSharpAssemblyFactoryTests
    {
        [TestMethod]
        public void Compile_should_return_instance_of_assembly()
        {
            var results =
                CSharpAssemblyFactory.Compile(
                    @"
                        public class Foo
                        {
                        }
                    ");

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void Compile_should_return_assembly_with_one_public_type()
        {
            var results =
                CSharpAssemblyFactory.Compile(
                    @"
                        public class Foo
                        {
                        }
                    ");

            Assert.AreEqual(1, results.GetExportedTypes().Length);
        }
    }
}