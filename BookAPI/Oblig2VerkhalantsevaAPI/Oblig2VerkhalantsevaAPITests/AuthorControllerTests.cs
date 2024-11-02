using Microsoft.AspNetCore.Mvc;
using Moq;
using Oblig2VerkhalantsevaAPI.Controllers;
using Oblig2VerkhalantsevaAPI.Models;
using Oblig2VerkhalantsevaAPI.Services;

namespace Oblig2VerkhalantsevaAPITests
{
    public class AuthorControllerTests
    {
        private readonly Mock<IAuthorService> _mock;
        private readonly AuthorController _controller;

        public AuthorControllerTests()
        {
            _mock = new Mock<IAuthorService>();
            _controller = new AuthorController(_mock.Object);
        }

        private static List<AuthorDto> GetTestAuthorDtos()
        {
            return new List<AuthorDto>
            {
                new AuthorDto { Id = 1, FirstName = "F. Scott", LastName = "Fitzgerald" },
                new AuthorDto { Id = 2, FirstName = "Harper", LastName = "Lee" },
                new AuthorDto { Id = 3, FirstName = "George", LastName = "Orwell" },
                new AuthorDto { Id = 4, FirstName = "Knut", LastName = "Hamsun" }
            };
        }

        // GET: api/author
        [Fact]
        public async Task GetAuthors_ReturnsOkResultWithAuthors()
        {
            var authors = GetTestAuthorDtos();
            _mock.Setup(service => service.GetAllAuthors()).ReturnsAsync(authors);

            var result = await _controller.GetAuthors() as OkObjectResult;

            Assert.NotNull(result);
            Assert.IsType<List<AuthorDto>>(result!.Value);
            Assert.Equal(authors.Count, ((List<AuthorDto>)result.Value).Count);
        }

        // GET: api/author/{id}
        [Fact]
        public async Task Get_ReturnsAuthor_WhenAuthorExists()
        {
            var author = GetTestAuthorDtos()[0];
            _mock.Setup(service => service.GetAuthorDto(author.Id)).ReturnsAsync(author);

            var result = await _controller.Get(author.Id) as OkObjectResult;

            Assert.NotNull(result);
            Assert.IsType<AuthorDto>(result!.Value);
            Assert.Equal(author.FirstName, ((AuthorDto)result.Value).FirstName);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            _mock.Setup(service => service.GetAuthorDto(5)).ReturnsAsync((AuthorDto?)null);

            var result = await _controller.Get(5);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Get_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("TestError", "Invalid Model State");

            var result = await _controller.Get(1) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
        }

        // POST: api/author
        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenAuthorIsValid()
        {
            var newAuthorDto = new AuthorDto { FirstName = "New", LastName = "Author" };
            _mock.Setup(service => service.GetAuthorByName(newAuthorDto.FirstName, newAuthorDto.LastName))
                .ReturnsAsync((AuthorDto?)null);

            var result = await _controller.Create(newAuthorDto) as CreatedAtActionResult;

            Assert.NotNull(result);
            Assert.Equal(nameof(_controller.Get), result!.ActionName);
            Assert.Equal(201, result.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenAuthorAlreadyExists()
        {
            var existingAuthor = new AuthorDto { FirstName = "Existing", LastName = "Author" };
            _mock.Setup(service => service.GetAuthorByName(existingAuthor.FirstName, existingAuthor.LastName))
                .ReturnsAsync(new AuthorDto());

            var result = await _controller.Create(existingAuthor) as ConflictObjectResult;

            Assert.NotNull(result);
            Assert.Equal(409, result!.StatusCode);
        }
        
        // PUT: api/author/{id}
        [Fact]
        public async Task Update_ReturnsOk_WhenAuthorIsUpdated()
        {
            var updatedAuthorDto = new AuthorDto { Id = 1, FirstName = "Updated", LastName = "Author" };
            _mock.Setup(service => service.GetAuthorDto(updatedAuthorDto.Id)).ReturnsAsync(updatedAuthorDto);

            var result = await _controller.Update(updatedAuthorDto.Id, updatedAuthorDto) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            var updatedAuthorDto = new AuthorDto { Id = 5, FirstName = "Nonexistent", LastName = "Author" };
            _mock.Setup(service => service.GetAuthorDto(updatedAuthorDto.Id)).ReturnsAsync((AuthorDto?)null);

            var result = await _controller.Update(updatedAuthorDto.Id, updatedAuthorDto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var authorDto = new AuthorDto { Id = 2, FirstName = "Mismatch", LastName = "Id" };

            var result = await _controller.Update(1, authorDto) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result!.StatusCode);
            Assert.Equal("Id from route does not match id from body.", result.Value);
        }

        // DELETE: api/author/{id}
        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAuthorExists()
        {
            var testAuthor = GetTestAuthorDtos().First();
            _mock.Setup(service => service.GetAuthorDto(testAuthor.Id)).ReturnsAsync(testAuthor);
            _mock.Setup(service => service.Delete(testAuthor.Id)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(testAuthor.Id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            _mock.Setup(service => service.GetAuthorDto(5)).ReturnsAsync((AuthorDto?)null);

            var result = await _controller.Delete(5);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}