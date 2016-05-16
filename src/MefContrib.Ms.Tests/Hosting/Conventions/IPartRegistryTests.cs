namespace MefContrib.Hosting.Conventions.Tests
{
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IPartRegistryTests
    {
        [TestMethod]
        public void IPartRegistry_should_implement_ihideobjectmemebers_interface()
        {
            typeof(IPartRegistry<IContractService>).ShouldImplementInterface<IHideObjectMembers>();
        }
    }
}