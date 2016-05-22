using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Generics.Tests
{
    [TestClass]
    public class GenericCatalogTests
    {
        public static ExportProvider ExportProvider;

        [ClassInitialize]
        public static void TestSetUp(TestContext context)
        {
            var typeCatalog = new TypeCatalog(
                typeof(CtorOrderProcessor),
                typeof(OrderProcessor),
                typeof(ConcreteCtorOrderProcessor),
                typeof(ConcreteOrderProcessor),
                typeof(MyCtorOrderProcessor),
                typeof(MyOrderProcessor),
                typeof(MyOrderProcessorSetterOnly),
                typeof(MultiOrderProcessor));
            var genericCatalog = new GenericCatalog(new TestGenericContractRegistry());
            var aggregateCatalog = new AggregateCatalog(typeCatalog, genericCatalog);
            var provider = new CatalogExportProvider(aggregateCatalog);
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

        [TestMethod]
        public void When_querying_for_concrete_order_processor_the_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<ConcreteOrderProcessor>();
            Assert.IsNotNull(orderProcessor);
            Assert.IsNotNull(orderProcessor.OrderRepository);
        }

        [TestMethod]
        public void When_querying_for_concrete_ctor_order_processor_the_ctor_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<ConcreteCtorOrderProcessor>();
            Assert.IsNotNull(orderProcessor);
            Assert.IsNotNull(orderProcessor.OrderRepository);
        }

        [TestMethod]
        public void When_querying_for_my_order_processor_the_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<MyOrderProcessor>();
            Assert.IsNotNull(orderProcessor);
            Assert.IsNotNull(orderProcessor.OrderRepository);
        }

        [TestMethod]
        public void When_querying_for_my_order_processor_with_setter_only_the_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<MyOrderProcessorSetterOnly>();
            Assert.IsNotNull(orderProcessor);
            Assert.IsNotNull(orderProcessor.orderRepository);
        }

        [TestMethod]
        public void When_querying_for_my_ctor_order_processor_the_ctor_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<MyCtorOrderProcessor>();
            Assert.IsNotNull(orderProcessor);
            Assert.IsNotNull(orderProcessor.OrderRepository);
        }

        [TestMethod]
        public void When_querying_for_multi_order_processor_the_multi_order_processor_is_created()
        {
            var orderProcessor = ExportProvider.GetExportedValue<MultiOrderProcessor>();
            Assert.IsNotNull(orderProcessor);
            
            Assert.IsNotNull(orderProcessor.OrderRepositoriesAsArray);
            Assert.AreEqual(2, orderProcessor.OrderRepositoriesAsArray.Length);

            Assert.IsNotNull(orderProcessor.OrderRepositoriesAsEnumerable);
            Assert.AreEqual(2, orderProcessor.OrderRepositoriesAsEnumerable.Count());
        }
    }
}