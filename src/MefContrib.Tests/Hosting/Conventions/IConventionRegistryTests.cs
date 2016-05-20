namespace MefContrib.Hosting.Conventions.Tests
{
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IConventionRegistryTests
    {
        [TestMethod]
        public void IConventionRegistry_should_implement_ihideobjectmemebers_interface()
        {
            typeof(IConventionRegistry<>).ShouldImplementInterface<IHideObjectMembers>();
        }
    }
}