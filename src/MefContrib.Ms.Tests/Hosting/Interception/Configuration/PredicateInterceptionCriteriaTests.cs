using System;
using MefContrib.Hosting.Interception.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MefContrib.Hosting.Interception.Tests.Configuration
{
    [TestClass]
    public class PredicateInterceptionCriteriaTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void When_calling_ctor_with_null_interceptor_argument_null_exception_is_thrown()
        {
            new PredicateInterceptionCriteria(null, def => true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void When_calling_ctor_with_null_predicate_argument_null_exception_is_thrown()
        {
            new PredicateInterceptionCriteria(new FakeInterceptor(), null);
        }
    }
}