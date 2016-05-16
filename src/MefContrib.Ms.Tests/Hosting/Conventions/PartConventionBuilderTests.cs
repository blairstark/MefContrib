﻿namespace MefContrib.Hosting.Conventions.Tests
{
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;


    [TestClass]
    public class PartConventionBuilderTests
    {
        private PartConventionBuilder<PartConvention> conventionBuilder;

        public PartConventionBuilderTests()
        {
        }

        [TestInitialize]
        public void Setup()
        {
            this.conventionBuilder = new PartConventionBuilder<PartConvention>();
        }

        [TestMethod]
        public void Exports_should_set_exports_on_convention()
        {
            var reference =
                this.conventionBuilder
                    .Exports(x =>
                    {
                        x.ExportWithConvention<ExportConvention>();
                        x.ExportWithConvention<ExportConvention>();
                    });

            var convention =
                this.conventionBuilder.GetConvention();

            convention.Exports.Count().ShouldEqual(2);
        }

        [TestMethod]
        public void Exports_should_throw_argument_null_exception_when_called_with_null()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.Exports(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [TestMethod]
        public void Exports_should_return_reference_to_itself()
        {
            var reference =
                this.conventionBuilder
                    .Exports(x => x.ExportWithConvention<ExportConvention>());

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [TestMethod]
        public void Imports_should_set_imports_on_convention()
        {
            var reference =
                this.conventionBuilder
                    .Imports(x =>
                    {
                        x.ImportWithConvention<ImportConvention>();
                        x.ImportWithConvention<ImportConvention>();
                    });

            var convention =
                this.conventionBuilder.GetConvention();

            convention.Imports.Count().ShouldEqual(2);
        }

        [TestMethod]
        public void Imports_should_throw_argument_null_exception_when_called_with_null()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.Imports(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [TestMethod]
        public void Imports_should_return_reference_to_itself()
        {
            var reference =
                this.conventionBuilder
                    .Imports(x => x.ImportWithConvention<ImportConvention>());

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [TestMethod]
        public void MakeShared_should_set_creation_policy_on_convention_to_shared()
        {
            this.conventionBuilder
                .MakeShared();
            
            var convention =
                this.conventionBuilder.GetConvention();

            convention.CreationPolicy.ShouldEqual(CreationPolicy.Shared);
        }

        [TestMethod]
        public void MakeShared_should_return_reference_to_itself()
        {
            var reference =
                this.conventionBuilder
                    .MakeShared();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [TestMethod]
        public void MakeNonShared_should_set_creation_policy_on_convention_to_non_shared()
        {
            this.conventionBuilder
                .MakeNonShared();
            
            var convention =
                this.conventionBuilder.GetConvention();

            convention.CreationPolicy.ShouldEqual(CreationPolicy.NonShared);
        }

        [TestMethod]
        public void MakeNonShared_should_return_reference_to_itself()
        {
            var reference =
                this.conventionBuilder
                    .MakeNonShared();

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [TestMethod]
        public void ForTypesMatching_should_set_condition_on_convention()
        {
            this.conventionBuilder
                .ForTypesMatching(x => x == typeof(IPartConvention));
            
            var convention =
                this.conventionBuilder.GetConvention();

            var availableTypes =
                new List<Type> { typeof(IPartConvention), typeof(IImportConvention) };

            var matchedTypes =
                availableTypes.Where(x => convention.Condition(x));

            matchedTypes.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void ForTypesMatching_should_throw_argument_null_exception_if_called_with_null()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.ForTypesMatching(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [TestMethod]
        public void ForTypesMatching_should_return_reference_to_itself()
        {
            var reference =
                this.conventionBuilder
                    .ForTypesMatching(x => x.IsPublic);

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [TestMethod]
        public void GetBuiltInstance_should_not_return_null_on_new_builder()
        {
            var convention =
                this.conventionBuilder
                    .GetConvention();

            convention.ShouldNotBeNull();
        }

        [TestMethod]
        public void AddMetadata_should_return_reference_to_itself_when_called_with_name_and_value()
        {
            var reference =
                this.conventionBuilder
                    .AddMetadata("Foo", new object());

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [TestMethod]
        public void AddMetadata_should_throw_argument_null_exception_when_called_with_null_name_and_value()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata(null, new object()));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [TestMethod]
        public void AddMetadata_should_throw_argument_out_of_range_exception_when_called_with_empty_name_and_value()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata(string.Empty, new object()));

            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void AddMetadata_should_throw_argument_null_exception_when_called_with_name_and_null_value()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata("Foo", null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [TestMethod]
        public void AddMetadata_should_add_metadata_to_convention()
        {
            this.conventionBuilder
                .AddMetadata("Foo", "Bar");
            
            var convention =
                this.conventionBuilder.GetConvention();

            var expectedMetadata =
                new MetadataItem("Foo", "Bar");

            convention.Metadata.First().Equals(expectedMetadata).ShouldBeTrue();
        }

        [TestMethod]
        public void AddMetadata_should_return_reference_to_itself_when_called_with_anonymous_type()
        {
            var reference =
                this.conventionBuilder
                    .AddMetadata(new { Foo = "Bar" });

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [TestMethod]
        public void AddMetadata_should_throw_argument_null_exception_when_called_with_null_anonymous_type()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata((object)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [TestMethod]
        public void AddMetadata_should_set_metadata_on_convention_from_anonymous_type()
        {
            this.conventionBuilder
                .AddMetadata(new {Foo = "Bar"});

            var convention =
                this.conventionBuilder.GetConvention();

            var expectedMetadata =
                new MetadataItem("Foo", "Bar");

            convention.Metadata.First().Equals(expectedMetadata).ShouldBeTrue();
        }

        [TestMethod]
        public void AddMetadata_should_return_reference_to_itself_when_called_with_a_function()
        {
            var reference =
                this.conventionBuilder
                    .AddMetadata(() => new[] { new KeyValuePair<string, object>("Foo", "Bar") });

            reference.ShouldBeSameAs(this.conventionBuilder);
        }

        [TestMethod]
        public void AddMetadata_should_throw_argument_null_exception_when_called_with_null()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata((Func<KeyValuePair<string, object>[]>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [TestMethod]
        public void AddMetadata_should_throw_argument_null_exception_when_called_with_function_that_returns_null()
        {
            var exception =
                Catch.Exception(() => this.conventionBuilder.AddMetadata(() => null));

            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [TestMethod]
        public void AddMetadata_should_set_metadata_on_convention_when_called_with_a_function()
        {
            this.conventionBuilder
                .AddMetadata(() => new[] {new KeyValuePair<string, object>("Foo", "Bar")});
            
            var convention =
                this.conventionBuilder.GetConvention();

            var expectedMetadata =
                new MetadataItem("Foo", "Bar");

            convention.Metadata.First().Equals(expectedMetadata).ShouldBeTrue();
        }
    }
}