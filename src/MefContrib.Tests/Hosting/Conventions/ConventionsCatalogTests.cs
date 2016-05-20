namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;
    using Moq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MefContrib.Tests;

    [TestClass]
    public class ConventionsCatalogTests
    {
        [TestMethod]
        public void Ctor_should_throw_argumentnullexception_is_called_with_null_params_array_of_registries()
        {
            var exception =
                Catch.Exception(() => new ConventionCatalog((IPartRegistry<IContractService>[])null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [TestMethod]
        public void Ctor_should_retrieve_registries_when_called_with_locator()
        {
            var locator = new Mock<IPartRegistryLocator>();

            var catalog =
                new ConventionCatalog(locator.Object);

            locator.Verify(x => x.GetRegistries(), Times.Once());
        }

        [TestMethod]
        public void Parts_should_return_compsable_part_definitions_that_was_created_based_on_the_part_registries()
        {
            var catalog =
                new ConventionCatalog(new NonEmptyRegistry());

            catalog.Parts.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void Parts_should_return_empty_result_when_registry_has_no_type_loader_defined()
        {
            var partDefinitions =
                GetPartDefinitionsFromEmptyRegistry();

            partDefinitions.Count().ShouldEqual(0);
        }

        [TestMethod]
        public void Parts_should_return_empty_result_when_registry_has_no_part_definitions_defined()
        {
            var partDefinitions =
                GetPartDefinitionsFromEmptyRegistry();

            partDefinitions.Count().ShouldEqual(0);
        }

        [TestMethod]
        public void Parts_should_return_one_part_definition_per_type_matched_by_convention_conditions()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            partDefinitions.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void Parts_should_return_definition_with_correct_type()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var partType =
                ReflectionModelServices.GetPartType(partDefinitions.First());

            partType.Value.Equals(typeof(FakePart));
        }

        [TestMethod]
        public void Parts_should_add_metadata_about_creation_policy_from_part_convention_to_part_definition()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            const string expectedMetadataKey =
                CompositionConstants.PartCreationPolicyMetadataName;

            partDefinitions.First().Metadata.ContainsKey(expectedMetadataKey).ShouldBeTrue();
        }

        [TestMethod]
        public void Parts_should_add_metadata_from_part_convention_to_part_definition()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var expectedMetadata =
                new Dictionary<string, object>
                {
                    { CompositionConstants.PartCreationPolicyMetadataName, CreationPolicy.Shared  },
                    { "Foo", "Bar" }
                };

            var inspectedPartDefinition =
                partDefinitions.First();

            expectedMetadata.All(x => inspectedPartDefinition.Metadata.ContainsKey(x.Key)).ShouldBeTrue();
        }

        [TestMethod]
        public void Parts_should_create_correct_number_of_export_definitions_from_part_convention_on_part_definition()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var inspectedPartDefinition =
                partDefinitions.First();

            inspectedPartDefinition.ExportDefinitions.Count().ShouldEqual(2);
        }

        [TestMethod]
        public void Parts_should_set_contract_name_on_export_definition_to_contract_name_from_export_convention_using_contract_service_that_is_defined_on_registry()
        {
            var registry =
                new NonEmptyRegistry();

            var conventions =
                registry.GetConventions();

            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Delegate);

            var inspectedExportDefinition =
                partDefinitions.First().ExportDefinitions.First();

            var expectedContractName =
                registry.ContractService.GetExportContractName(conventions.First().Exports.First(), member);

            inspectedExportDefinition.ContractName.ShouldEqual(expectedContractName);
        }

        [TestMethod]
        public void Parts_should_set_metadata_on_export_definition_from_metadata_on_export_convention()
        {
            var registry =
                new NonEmptyRegistry();

            var conventions =
                registry.GetConventions();

            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var expectedMetadata =
                conventions.First().Exports.First().Metadata;

            var inspectedExportDefinition =
                partDefinitions.First().ExportDefinitions.First();

            expectedMetadata.All(x =>
                inspectedExportDefinition.Metadata.Contains(
                new KeyValuePair<string, object>(x.Name, x.Value))).ShouldBeTrue();
        }

        [TestMethod]
        public void Parts_should_set_type_identity_metadata_on_export_definition_to_contract_type_from_export_convention_using_contract_service_that_is_defined_on_registry()
        {
            var registry =
                new NonEmptyRegistry();

            var conventions =
                registry.GetConventions();

            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Delegate);

            var inspectedExportDefinition =
                partDefinitions.First().ExportDefinitions.First();

            var expectedMetadata =
                new KeyValuePair<string, object>(CompositionConstants.ExportTypeIdentityMetadataName,
                     registry.ContractService.GetExportTypeIdentity(conventions.First().Exports.First(), member));

            inspectedExportDefinition.Metadata.Contains(expectedMetadata).ShouldBeTrue();
        }

        [TestMethod]
        public void Parts_should_create_correct_number_of_import_definitions_from_part_convention_on_part_definition()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var inspectedPartDefinition =
                partDefinitions.First();

            inspectedPartDefinition.ExportDefinitions.Count().ShouldEqual(2);
        }

        [TestMethod]
        public void Parts_should_set_cardinality_on_import_definition_to_zeroorone_when_import_convention_is_defined_for_non_collection_and_allow_default_values()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.Cast<ContractBasedImportDefinition>().Skip(2).First();

            inspectedImportDefinition.Cardinality.ShouldEqual(ImportCardinality.ZeroOrOne);
        }

        [TestMethod]
        public void Parts_should_set_cardinality_on_import_definition_to_exactlyone_when_import_convention_is_defined_for_non_collection_and_not_allow_default_values()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.Cast<ContractBasedImportDefinition>().Skip(3).First();

            inspectedImportDefinition.Cardinality.ShouldEqual(ImportCardinality.ExactlyOne);
        }

        [TestMethod]
        public void Parts_should_set_cardinality_on_import_definition_to_zeroormore_when_import_convention_is_for_collection()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.Cast<ContractBasedImportDefinition>().Last();

            inspectedImportDefinition.Cardinality.ShouldEqual(ImportCardinality.ZeroOrMore);
        }

        [TestMethod]
        public void Parts_should_set_contract_name_on_import_definition_to_contract_name_from_import_convention_using_contract_service_that_is_defined_on_registry()
        {
            var registry =
                new NonEmptyRegistry();

            var conventions =
                registry.GetConventions();

            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Delegate);

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.Skip(2).First();

            var expectedContractName =
                registry.ContractService.GetImportContractName(conventions.First().Imports.First(), member);

            inspectedImportDefinition.ContractName.ShouldEqual(expectedContractName);
        }

        [TestMethod]
        public void Parts_should_set_type_identity_on_import_deinition_to_contract_type_from_import_convention()
        {
            var registry =
                new NonEmptyRegistry();

            var conventions =
                registry.GetConventions();

            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var member =
                ReflectionServices.GetProperty<FakePart>(x => x.Delegate);

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.Cast<ContractBasedImportDefinition>().Skip(2).First();

            var expectedTypeIdentity =
                registry.ContractService.GetImportTypeIdentity(conventions.First().Imports.First(), member);

            inspectedImportDefinition.RequiredTypeIdentity.ShouldEqual(expectedTypeIdentity);
        }

        [TestMethod]
        public void Parts_should_set_required_metadata_on_import_definition_to_required_metadata_on_import_convention()
        {
            var registry =
                new NonEmptyRegistry();

            var conventions =
                registry.GetConventions();

            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var expectedRequiredMetadata =
                conventions.First().Imports.First().RequiredMetadata;

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            inspectedImportDefinition.RequiredMetadata.All(x =>
                expectedRequiredMetadata.Contains(new RequiredMetadataItem(x.Key, x.Value)));
        }

        [TestMethod]
        public void Parts_should_set_isrecomposable_on_import_definition_to_recomposable_on_import_definition()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.Cast<ContractBasedImportDefinition>().Skip(2).First();

            inspectedImportDefinition.IsRecomposable.ShouldBeTrue();
        }

        [TestMethod]
        public void Parts_should_return_same_number_import_definitions_as_parameters_when_called_with_constructor()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var inspectedPartDefinition =
                partDefinitions.First();

            inspectedPartDefinition.ImportDefinitions.Count().ShouldEqual(5);
        }

        [TestMethod]
        public void Parts_should_set_contract_name_on_import_deinfintion_to_contract_name_of_parameter_when_called_with_constructor()
        {
            var registry =
                new NonEmptyRegistry();

            var conventions =
                registry.GetConventions();

            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var member =
                typeof(FakePart).GetConstructor(new Type[] { typeof(int), typeof(string[]) });

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.First();

            var expectedContractName =
                registry.ContractService.GetImportContractName(conventions.First().Imports.First(), member.GetParameters()[0].ParameterType);

            inspectedImportDefinition.ContractName.ShouldEqual(expectedContractName);
        }

        [TestMethod]
        public void Parts_should_set_type_identity_on_import_deinfintion_to_contract_type_of_parameter_when_called_with_constructor()
        {
            var registry =
                new NonEmptyRegistry();

            var conventions =
                registry.GetConventions();

            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var member =
                typeof(FakePart).GetConstructor(new Type[] { typeof(int), typeof(string[]) });

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.First();

            var expectedTypeIdentity =
                registry.ContractService.GetImportTypeIdentity(conventions.First().Imports.First(), member.GetParameters()[0].ParameterType);

            inspectedImportDefinition.ContractName.ShouldEqual(expectedTypeIdentity);
        }

        [TestMethod]
        public void Parts_should_not_set_required_metadata_on_import_convention_when_called_with_constructor_info()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            inspectedImportDefinition.RequiredMetadata.ShouldBeSameAs(Enumerable.Empty<KeyValuePair<string, Type>>());
        }

        [TestMethod]
        public void Parts_should_set_cardinality_on_import_defintion_to_exaclyone_when_import_convention_not_is_for_collection_and_called_with_constructor_info()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.Cast<ContractBasedImportDefinition>().First();

            inspectedImportDefinition.Cardinality.ShouldEqual(ImportCardinality.ExactlyOne);
        }

        [TestMethod]
        public void Parts_should_set_cardinality_on_import_defintion_to_zeroormore_when_import_convention_not_is_for_collection_and_called_with_constructor_info()
        {
            var partDefinitions =
                GetPartDefinitionsFromNonEmptyRegistry();

            var inspectedImportDefinition =
                partDefinitions.First().ImportDefinitions.Cast<ContractBasedImportDefinition>().Skip(1).First();

            inspectedImportDefinition.Cardinality.ShouldEqual(ImportCardinality.ZeroOrMore);
        }

        private static IEnumerable<ComposablePartDefinition> GetPartDefinitionsFromEmptyRegistry()
        {
            var registry =
                new EmptyRegistry();

            var catalog =
                new ConventionCatalog(registry);

            return catalog.Parts;
        }

        private static IEnumerable<ComposablePartDefinition> GetPartDefinitionsFromNonEmptyRegistry()
        {
            var registry =
                new NonEmptyRegistry();

            var catalog =
                new ConventionCatalog(registry);

            return catalog.Parts;
        }
    }
}