using System.ComponentModel.Composition.Hosting;
using MefContrib.Hosting.Interception.Configuration;
using MefContrib.Hosting.Interception;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Generics.Tests
{
    [TestClass]
    public class GenericExportHandlerIntegrationTests
    {
        public static ExportProvider ExportProvider;

        [ClassInitialize]
        public static void TestSetUp(TestContext context)
        {
            var typeCatalog = new TypeCatalog(typeof(CtorOrderProcessor), typeof(OrderProcessor), typeof(TestGenericContractRegistry));
            var cfg = new InterceptionConfiguration().AddHandler(new GenericExportHandler());
            var catalog = new InterceptingCatalog(typeCatalog, cfg);

            var provider = new CatalogExportProvider(catalog);
            provider.SourceProvider = provider;

            ExportProvider = provider;
        }

        [TestMethod]
        public void When_querying_for_order_processor_the_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<OrderProcessor>();
            Assert.IsNotNull(orderProcessor);
            Assert.IsNotNull(orderProcessor.OrderRepository);
        }

        [TestMethod]
        public void When_querying_for_ctor_order_processor_the_ctor_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<CtorOrderProcessor>();
            Assert.IsNotNull(orderProcessor);
            Assert.IsNotNull(orderProcessor.OrderRepository);
        }
    }
}