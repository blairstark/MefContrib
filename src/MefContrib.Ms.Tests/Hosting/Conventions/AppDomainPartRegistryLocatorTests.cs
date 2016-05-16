namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using MefContrib.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AppDomainPartRegistryLocatorTests
    {
        [TestMethod]
        public void Should_throw_argumentnullexception_when_instantiated_with_null()
        {
            // Arrange, Act
            var exception =
                Catch.Exception(() => new AppDomainPartRegistryLocator(null));

            // Assert
            exception.ShouldBeOfType<ArgumentNullException>();
        }
    }
}