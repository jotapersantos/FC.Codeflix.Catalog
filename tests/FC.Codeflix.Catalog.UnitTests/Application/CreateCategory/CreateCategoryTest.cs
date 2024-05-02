using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;
using FluentAssertions;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory
{
    [Collection(nameof(CreateCategoryTestFixture))]
    public class CreateCategoryTest
    {
        private readonly CreateCategoryTestFixture _fixture;

        public CreateCategoryTest(CreateCategoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("Application", "CreateCategory - Use Cases")]
        public async void CreateCategory()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            var useCase = new UseCase.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            var input = _fixture.GetInput();

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(repository => repository
                .Insert(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
            
            unitOfWorkMock.Verify(unitOfWork => unitOfWork
                .Commit(It.IsAny<CancellationToken>()),
                Times.Once
            );

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(input.IsActive);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        }

        [Theory(DisplayName = nameof(ThrowWhenCantInstantiateAggregate))]
        [Trait("Application", "CreateCategory - Use Cases")]
        [MemberData(nameof(GetInvalidInputs))]
        public async void ThrowWhenCantInstantiateAggregate(CreateCategoryInput input, string exceptionMessage)
        {
            var useCase = new UseCase.CreateCategory(_fixture.GetRepositoryMock().Object,
                                                     _fixture.GetUnitOfWorkMock().Object);

            Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<EntityValidationException>()
                               .WithMessage(exceptionMessage);
        }

        public static IEnumerable<object[]> GetInvalidInputs()
        {
            var fixture = new CreateCategoryTestFixture();

            var invalidInputsList = new List<object[]>();

            var invalidInputShortName = fixture.GetInput();
            invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);

            invalidInputsList.Add(new object[] {
                invalidInputShortName,
                $"Name should be at least 3 characters long"
            });

            var invalidInputTooLongName = fixture.GetInput();

            string tooLongNameForCategory = "";
            
            while (tooLongNameForCategory.Length <= 255)
                tooLongNameForCategory += fixture.Faker.Commerce.ProductDescription();

            invalidInputTooLongName.Name = tooLongNameForCategory;

            invalidInputsList.Add(new object[] {
                invalidInputTooLongName,
                $"Name should be less or equal 255 characters long"
            });



            return invalidInputsList;
        }
    }
}
