﻿using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category
{
    [Collection(nameof(CategoryTestFixture))]
    public class CategoryTest
    {
        private readonly CategoryTestFixture _categoryTestFixture;

        public CategoryTest(CategoryTestFixture categoryTestFixture)
        {
            _categoryTestFixture = categoryTestFixture;
        }

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "Category - Aggregates")]
        public void Instantiate()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();

            var datetimeBefore = DateTime.UtcNow;

            DomainEntity.Category category = new(validCategory.Name, validCategory.Description);

            var datetimeAfter = DateTime.UtcNow.AddSeconds(1);

            category.Should().NotBeNull();
            category.Name.Should().Be(validCategory.Name);
            category.Description.Should().Be(validCategory.Description);
            category.Id.Should().NotBeEmpty();
            category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
            (category.CreatedAt >= datetimeBefore).Should().BeTrue();
            (category.CreatedAt <= datetimeAfter).Should().BeTrue();
            category.IsActive.Should().BeTrue();
        }

        [Theory(DisplayName = nameof(InstantiateWithIsActive))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void InstantiateWithIsActive(bool isActive)
        {
            var validCategory = _categoryTestFixture.GetValidCategory();

            var datetimeBefore = DateTime.UtcNow;

            DomainEntity.Category category = new(validCategory.Name, validCategory.Description, isActive);

            var datetimeAfter = DateTime.UtcNow.AddSeconds(1);

            category.Should().NotBeNull();
            category.Name.Should().Be(validCategory.Name);
            category.Description.Should().Be(validCategory.Description);
            category.Id.Should().NotBeEmpty();
            category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
            (category.CreatedAt >= datetimeBefore).Should().BeTrue();
            (category.CreatedAt <= datetimeAfter).Should().BeTrue();
            category.IsActive.Should().Be(isActive);
        }

        [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsNullOrEmpty))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void InstantiateErrorWhenNameIsNullOrEmpty(string? name)
        {
            var validCategory = _categoryTestFixture.GetValidCategory();

            Action action = () => new DomainEntity.Category(name, validCategory.Description);

            action.Should()
                  .Throw<EntityValidationException>()
                  .WithMessage("Name should not be null or empty");
        }

        [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
        [Trait("Domain", "Category - Aggregates")]
        public void InstantiateErrorWhenDescriptionIsNull()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();

            Action action = () => new DomainEntity.Category(validCategory.Name, null);

            action.Should()
                  .Throw<EntityValidationException>()
                  .WithMessage("Description should not be null");
        }

        [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characteres))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("a")]
        [InlineData("ab")]
        public void InstantiateErrorWhenNameIsLessThan3Characteres(string invalidName)
        {
            var validCategory = _categoryTestFixture.GetValidCategory();

            Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);

            action.Should()
                  .Throw<EntityValidationException>()
                  .WithMessage("Name should be at least 3 characters long");
        }

        [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characteres))]
        [Trait("Domain", "Category - Aggregates")]
        public void InstantiateErrorWhenNameIsGreaterThan255Characteres()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();

            var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

            Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);

            action.Should()
                  .Throw<EntityValidationException>()
                  .WithMessage("Name should be less or equal 255 characters long");
        }

        [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characteres))]
        [Trait("Domain", "Category - Aggregates")]
        public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characteres()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();

            var invalidDescription = String.Join(null, Enumerable.Range(1, 10_001).Select(_ => "a").ToArray());

            Action action = () => new DomainEntity.Category(validCategory.Name, invalidDescription);

            action.Should()
                  .Throw<EntityValidationException>()
                  .WithMessage("Description should be less or equal 10000 characters long");
        }

        [Fact(DisplayName = nameof(Activate))]
        [Trait("Domain", "Category - Aggregates")]
        public void Activate()
        {
            var validCategory = _categoryTestFixture.GetValidCategory();

            DomainEntity.Category category = new(validCategory.Name, validCategory.Description, false);

            category.Activate();

            category.IsActive.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(Deactivate))]
        [Trait("Domain", "Category - Aggregates")]
        public void Deactivate()
        {
            var category = _categoryTestFixture.GetValidCategory();

            category.Deactivate();
            
            category.IsActive.Should().BeFalse();
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Category - Aggregates")]
        public void Update()
        {
            var category = _categoryTestFixture.GetValidCategory();

            var categoryWithNewValues = _categoryTestFixture.GetValidCategory();

            category.Update(categoryWithNewValues.Name, categoryWithNewValues.Description);

            category.Name.Should().Be(categoryWithNewValues.Name);
            category.Description.Should().Be(categoryWithNewValues.Description);
        }

        [Fact(DisplayName = nameof(UpdateOnlyName))]
        [Trait("Domain", "Category - Aggregates")]
        public void UpdateOnlyName()
        {
            var category = _categoryTestFixture.GetValidCategory();

            var newName = _categoryTestFixture.GetValidCategoryName();
            
            var currentDescription = category.Description;

            category.Update(newName);

            category.Name.Should().Be(newName);
            category.Description.Should().Be(currentDescription);
        }

        [Theory(DisplayName = nameof(UpdateErrorWhenNameIsNullOrEmpty))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void UpdateErrorWhenNameIsNullOrEmpty(string? name)
        {
            var category = _categoryTestFixture.GetValidCategory();

            Action action = () => category.Update(name);

            action.Should()
                  .Throw<EntityValidationException>()
                  .WithMessage("Name should not be null or empty");
        }

        [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characteres))]
        [Trait("Domain", "Category - Aggregates")]
        [MemberData(nameof(GetNamesWithLessThan3Characteres), parameters: 10)]
        public void UpdateErrorWhenNameIsLessThan3Characteres(string invalidName)
        {
            var category = _categoryTestFixture.GetValidCategory();

            Action action = () => category.Update(invalidName);

            action.Should()
                  .Throw<EntityValidationException>()
                  .WithMessage("Name should be at least 3 characters long");
        }

        // Método para gerar os valores necessários para o teste
        // Chamado no MemberData do teste UpdateErrorWhenNameIsLessThan3Characteres
        public static IEnumerable<object[]> GetNamesWithLessThan3Characteres(int numberOfTests = 6)
        {
            var fixture = new CategoryTestFixture();

            for(int i = 0; i < numberOfTests; i++)
            {
                var isOdd = i % 2 == 1;
                yield return new object[] { fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)]};
            }
        }

        [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Characteres))]
        [Trait("Domain", "Category - Aggregates")]
        public void UpdateErrorWhenNameIsGreaterThan255Characteres()
        {
            var category = _categoryTestFixture.GetValidCategory();

            var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256);

            Action action = () => category.Update(invalidName);

            action.Should()
                  .Throw<EntityValidationException>()
                  .WithMessage("Name should be less or equal 255 characters long");
        }

        [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000Characteres))]
        [Trait("Domain", "Category - Aggregates")]
        public void UpdateErrorWhenDescriptionIsGreaterThan10_000Characteres()
        {
            var category = _categoryTestFixture.GetValidCategory();

            var invalidDescription = "";

            while(invalidDescription.Length <= 10_000)
                invalidDescription += _categoryTestFixture.Faker.Commerce.ProductDescription();

            Action action = () => category.Update(category.Name, invalidDescription);

            action.Should()
                  .Throw<EntityValidationException>()
                  .WithMessage("Description should be less or equal 10000 characters long");
        }
    }
}
