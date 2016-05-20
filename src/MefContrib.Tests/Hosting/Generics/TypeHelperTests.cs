using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Generics.Tests
{
    [TestClass]
    public class TypeHelperTests
    {
        [TestMethod]
        public void When_building_a_closed_generic_repository_Order_repository_is_returned()
        {
            var importDefinitionType = typeof(IRepository<Order>);
            var implementations = new List<Type>
            {
                typeof (Repository<>)
            };
            var orderRepositoryTypes = TypeHelper.BuildGenericTypes(importDefinitionType, implementations);

            Assert.AreEqual(typeof(Repository<Order>), orderRepositoryTypes.Single());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void When_building_a_closed_generic_repository_and_no_implementations_are_present_ArgumentException_is_thrown()
        {
            var importDefinitionType = typeof(IRepository<Order>);
            var implementations = new List<Type>();
            TypeHelper.BuildGenericTypes(importDefinitionType, implementations);
        }

        [TestMethod]
        public void IsCollection_method_test()
        {
            Assert.IsFalse(TypeHelper.IsCollection(typeof(int)));
            Assert.IsFalse(TypeHelper.IsCollection(typeof(string)));
            Assert.IsTrue(TypeHelper.IsCollection(typeof(IEnumerable)));
            Assert.IsTrue(TypeHelper.IsCollection(typeof(IEnumerable<string>)));
        }

        [TestMethod]
        public void IsGenericCollection_method_test()
        {
            Assert.IsFalse(TypeHelper.IsGenericCollection(typeof(int)));
            Assert.IsFalse(TypeHelper.IsGenericCollection(typeof(string)));
            Assert.IsFalse(TypeHelper.IsGenericCollection(typeof(IEnumerable)));
            Assert.IsTrue(TypeHelper.IsGenericCollection(typeof(IEnumerable<string>)));
            Assert.IsTrue(TypeHelper.IsGenericCollection(typeof(MyClass)));
        }

        [TestMethod]
        public void TryGetAncestor_method_test()
        {
            var ancestor = TypeHelper.TryGetAncestor(typeof(IList<string>), typeof(IEnumerable<string>));
            Assert.IsNotNull(ancestor);
            Assert.AreEqual(typeof(string), ancestor.GetGenericArguments()[0]);
        }

        [TestMethod]
        public void TryGetAncestor_method_test_with_open_generics()
        {
            var ancestor = TypeHelper.TryGetAncestor(typeof(IList<string>), typeof(IEnumerable<>));
            Assert.IsNotNull(ancestor);
            Assert.AreEqual(typeof(string), ancestor.GetGenericArguments()[0]);
        }

        [TestMethod]
        public void GetGenericCollectionParameter_method_test()
        {
            var ancestor = TypeHelper.GetGenericCollectionParameter(typeof(MyClass));
            Assert.IsNotNull(ancestor);
            Assert.AreEqual(typeof(string), ancestor);

            ancestor = TypeHelper.GetGenericCollectionParameter(typeof(MyClass2));
            Assert.IsNotNull(ancestor);
            Assert.AreEqual(typeof(string), ancestor);
        }

        private class MyClass : List<string> { }

        private class MyClass2 : MyClass { }
    }
}
