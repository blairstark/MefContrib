using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ImportConventionTests
    {
        [TestMethod]
        public void RequiredMetadata_should_not_be_null_on_new_instance()
        {
            var instance =
                new ImportConvention();

            instance.RequiredMetadata.ShouldNotBeNull();
        }

        [TestMethod]
        public void Equals_should_return_false_when_requiredmetadata_is_not_same_on_the_instance_that_is_being_compared_againts()
        {
            var instance =
                new ImportConvention
                {
                    RequiredMetadata = new List<RequiredMetadataItem>
                    {
                        new RequiredMetadataItem("name", typeof(string)),
                        new RequiredMetadataItem("value", typeof(int))
                    }
                };

            var results =
                instance.Equals(new ImportConvention
                {
                    RequiredMetadata = new List<RequiredMetadataItem>
                    {
                        new RequiredMetadataItem("name", typeof (string)),
                    }
                });

            results.ShouldBeFalse();
        }

        [TestMethod]
        public void Equals_should_return_false_when_contract_is_not_same_on_the_instance_that_is_being_compared_against()
        {
            var instance =
                new ImportConvention { ContractName = x => "Foo" };

            var results =
                instance.Equals(new ImportConvention { ContractName = x => "Bar" });

            results.ShouldBeFalse();
        }

        [TestMethod]
        public void Equals_should_return_false_when_creation_policy_is_not_same_on_the_instance_that_is_being_compared_against()
        {
            var instance =
                new ImportConvention { CreationPolicy = CreationPolicy.Any };

            var results =
                instance.Equals(new ImportConvention { CreationPolicy = CreationPolicy.NonShared });

            results.ShouldBeFalse();
        }

        [TestMethod]
        public void Equals_should_return_false_when_contract_type_is_not_same_on_the_instance_that_is_being_compared_against()
        {
            var instance =
                new ImportConvention { ContractType = x => typeof(string) };

            var results =
                instance.Equals(new ImportConvention { ContractType = x => typeof(object) });

            results.ShouldBeFalse();
        }

        [TestMethod]
        public void Equals_should_return_false_when_recomposable_is_not_same_on_instance_that_is_being_compared_against()
        {
            var instance =
                new ImportConvention { Recomposable = true };

            var results =
                instance.Equals(new ImportConvention { Recomposable = false });

            results.ShouldBeFalse();
        }

        [TestMethod]
        public void Equals_should_return_false_if_called_with_null()
        {
            var instance =
                new ImportConvention();

            var results =
                instance.Equals((ImportConvention)null);

            results.ShouldBeFalse();
        }

        [TestMethod]
        public void Equals_should_return_true_if_called_with_itself()
        {
            var instance =
                new ImportConvention();

            var results =
                instance.Equals(instance);

            results.ShouldBeTrue();
        }

        [TestMethod]
        public void Equals_should_return_true_if_identical_with_instance_that_is_being_compared_against()
        {
            var instance =
                new ImportConvention();

            var results =
                instance.Equals(new ImportConvention());

            results.ShouldBeTrue();
        }
    }
}