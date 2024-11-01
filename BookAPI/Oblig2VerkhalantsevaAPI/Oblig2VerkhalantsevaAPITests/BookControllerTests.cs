using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using Oblig2VerkhalantsevaAPI.Controllers;
using Oblig2VerkhalantsevaAPI.Models;
using Oblig2VerkhalantsevaAPI.Services;

namespace Oblig2VerkhalantsevaAPITests;

public class BookControllerTests
{
    private readonly Mock<IBookService> _mock;
    private readonly BookController _controller;

    public BookControllerTests()
    {
        _mock = new Mock<IBookService>();
        _controller = new BookController(_mock.Object);
    }

    private static List<BookDto> GetTestBookDtos()
    {
        return new List<BookDto>
        {
            new BookDto
            {
                Id = 1,
                Title = "The Great Gatsby",
                Description = "A classic novel by F. Scott Fitzgerald",
                Year = 1925,
                Author = new AuthorDto { Id = 1, FirstName = "F. Scott", LastName = "Fitzgerald" },
                Category = new CategoryDto { Id = 1, Name = "Classic Literature" },
                Publisher = new PublisherDto { Id = 1, Name = "Scribner" },
                Language = new LanguageDto { Id = 1, Name = "English" }
            },
            new BookDto
            {
                Id = 2,
                Title = "To Kill a Mockingbird",
                Description = "A novel by Harper Lee about racial injustice",
                Year = 1960,
                Author = new AuthorDto { Id = 2, FirstName = "Harper", LastName = "Lee" },
                Category = new CategoryDto { Id = 2, Name = "Historical Fiction" },
                Publisher = new PublisherDto { Id = 2, Name = "J.B. Lippincott & Co." },
                Language = new LanguageDto { Id = 1, Name = "English" }
            },
            new BookDto
            {
                Id = 3,
                Title = "1984",
                Description = "A dystopian novel by George Orwell",
                Year = 1949,
                Author = new AuthorDto { Id = 3, FirstName = "George", LastName = "Orwell" },
                Category = new CategoryDto { Id = 3, Name = "Dystopian" },
                Publisher = new PublisherDto { Id = 3, Name = "Secker & Warburg" },
                Language = new LanguageDto { Id = 1, Name = "English" }
            },
            new BookDto
            {
                Id = 4,
                Title = "Hunger",
                Description = "A psychological novel by Knut Hamsun",
                Year = 1890,
                Author = new AuthorDto { Id = 4, FirstName = "Knut", LastName = "Hamsun" },
                Category = new CategoryDto { Id = 4, Name = "Psychological Fiction" },
                Publisher = new PublisherDto { Id = 4, Name = "Gyldendal Norsk Forlag" },
                Language = new LanguageDto { Id = 2, Name = "Norwegian" }
            }
        };
    }
    
    //GET books
    [Fact]
    public async Task GetAllBooks_ReturnsCorrectTypeAndCount()
    {
        // Arrange
        var testBooks = GetTestBookDtos();
        _mock.Setup(service => service.GetAllBooks()).ReturnsAsync(testBooks);

        // Act
        var result = await _controller.GetBooks() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var books = Assert.IsType<List<BookDto>>(result.Value);
        Assert.Equal(4, books.Count);
    }
    
    //GET book
    [Fact]
    public async Task Get_ReturnsBook_WhenBookExists()
    {
        // Arrange
        var testBook = GetTestBookDtos().First();
        _mock.Setup(service => service.GetBook(1)).ReturnsAsync(testBook);

        // Act
        var result = await _controller.Get(1) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BookDto>(result.Value);
        Assert.Equal(testBook.Title, ((BookDto)result.Value).Title);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        _mock.Setup(service => service.GetBook(5)).ReturnsAsync((BookDto)null);

        // Act
        var result = await _controller.Get(5);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task Get_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("TestError", "Invalid Model State");

        // Act
        var result = await _controller.Get(1) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }
    
    //CREATE
    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenBookIsValid()
    {
        // Arrange
        var newBookDto = new BookDtoAdd
        {
            Title = "New Book",
            Description = "New Description",
            Year = 2023,
            AuthorId = 1,
            CategoryId = 1,
            PublisherId = 1,
            LanguageId = 1
        };

        _mock.Setup(service => service.GetBookByTitle(newBookDto.Title)).ReturnsAsync((BookDto)null);
        _mock.Setup(service => service.Save(It.IsAny<Book>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(newBookDto) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(_controller.Get), result.ActionName);
        Assert.Equal(201, result.StatusCode);
    }
    
    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Title", "Title is required");
        var bookDtoAdd = new BookDtoAdd(); // Невалидно, так как отсутствует поле Title

        // Act
        var result = await _controller.Create(bookDtoAdd) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        if (result != null)
        {
            Assert.Equal(400, result.StatusCode);
        }
    }
    
    [Fact]
    public async Task Create_ReturnsConflict_WhenBookWithTitleAlreadyExists()
    {
        // Arrange
        var bookDto = new BookDtoAdd
        {
            Title = "Existing Book",
            Description = "Description",
            Year = 2022,
            AuthorId = 1,
            CategoryId = 1,
            PublisherId = 1,
            LanguageId = 1
        };
        _mock.Setup(service => service.GetBookByTitle(bookDto.Title)).ReturnsAsync(new BookDto { Title = "Existing Book" });

        // Act
        var result = await _controller.Create(bookDto) as ConflictObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(409, result.StatusCode);
    }
    
    
    //UPDATE
    [Fact]
    public async Task Update_ReturnsOk_WhenBookIsUpdated()
    {
        // Arrange
        var existingBook = GetTestBookDtos().First();
        var updatedBookDto = new BookDtoAdd
        {
            Id = existingBook.Id,
            Title = "Updated Book",
            Description = "Updated Description",
            Year = 2024,
            AuthorId = 1,
            CategoryId = 1,
            PublisherId = 1,
            LanguageId = 1
        };

        _mock.Setup(service => service.GetBook(updatedBookDto.Id)).ReturnsAsync(existingBook);
        _mock.Setup(service => service.Save(It.IsAny<Book>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(updatedBookDto.Id, updatedBookDto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        if (result != null)
        {
            // Convert the result to a JObject and check if the Message property matches the expected value
            var resultJson = JObject.FromObject(result.Value);
            Assert.Equal($"Book with id {updatedBookDto.Id} successfully updated!", resultJson["Message"]?.ToString());
        }
    }
    
    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var bookDtoAdd = new BookDtoAdd { Id = 2, Title = "Updated Book", Description = "Updated Description", Year = 2024 };

        // Act
        var result = await _controller.Update(1, bookDtoAdd) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        if (result != null)
        {
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Id from route does not match id from body.", result.Value);
        }
    }
    
    [Fact]
    public async Task Update_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var bookDtoAdd = new BookDtoAdd { Id = 1, Title = "Nonexistent Book", Description = "Nonexistent Description", Year = 2024 };
        _mock.Setup(service => service.GetBook(1)).ReturnsAsync((BookDto)null);

        // Act
        var result = await _controller.Update(1, bookDtoAdd) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        if (result != null)
        {
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("No book with id 1 found.", result.Value);
        }
    }
    
    [Fact]
    public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var bookDto = new BookDtoAdd
        {
            Id = 1,
            Title = "Updated Book",
            Description = "Updated Description",
            Year = 2023,
            AuthorId = 1,
            CategoryId = 1,
            PublisherId = 1,
            LanguageId = 1
        };
        _controller.ModelState.AddModelError("TestError", "Invalid Model State");

        // Act
        var result = await _controller.Update(bookDto.Id, bookDto) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    
    
    //DELETE
    [Fact]
    public async Task Delete_ReturnsNoContent_WhenBookExists()
    {
        // Arrange
        var testBook = GetTestBookDtos().First();
        _mock.Setup(service => service.GetBook(testBook.Id)).ReturnsAsync(testBook);
        _mock.Setup(service => service.Delete(testBook.Id)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(testBook.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        _mock.Setup(service => service.GetBook(5)).ReturnsAsync((BookDto)null);

        // Act
        var result = await _controller.Delete(5);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
 
    
    //Arrange
        
    //Act
        
    //Assert
    
}