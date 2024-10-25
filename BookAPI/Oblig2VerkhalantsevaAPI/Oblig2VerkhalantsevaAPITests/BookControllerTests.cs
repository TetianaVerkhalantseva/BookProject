using Microsoft.AspNetCore.Mvc;
using Moq;
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

    private static List<Book> GetTestBooks()
    {
        return new List<Book>
        {
            new Book { Id = 1, Title = "The Great Gatsby", Description = "A classic novel by F. Scott Fitzgerald", Year = 1925, AuthorId = 0, CategoryId = 0, PublisherId = 0, LanguageId = 0 },
            new Book { Id = 2, Title = "To Kill a Mockingbird", Description = "A novel by Harper Lee about racial injustice", Year = 1960, AuthorId = 0, CategoryId = 0, PublisherId = 0, LanguageId = 0 },
            new Book { Id = 3, Title = "1984", Description = "A dystopian novel by George Orwell", Year = 1949, AuthorId = 0, CategoryId = 0, PublisherId = 0, LanguageId = 0 }
        };
    }
    
    //GET
    [Fact]
    public async Task GetAll_ReturnsCorrectType()
    {
        //Arrange
        _mock.Setup(service => service.GetAll()).ReturnsAsync(GetTestBooks);

        //Act
        var result = await _controller.GetAll();

        //Assert
        Assert.IsType<List<Book>>(result);
        if (result != null) Assert.Equal(3, result.Count); // Not necessary for 100% coverage.

    }
    
    
    [Fact]
    public void Get_ReturnsProduct_WhenProductExists()
    {
        //Arrange
        var book = new Book { Id = 1, Title = "The Great Gatsby", Description = "A classic novel by F. Scott Fitzgerald", Year = 1925, AuthorId = 0, CategoryId = 0, PublisherId = 0, LanguageId = 0 };
        _mock.Setup(service => service.Get(1)).Returns(book);
        
        //Act
        var result = _controller.Get(1) as OkObjectResult;
        
        //Assert
        Assert.NotNull(result);
        if (result != null) Assert.Equal(book, result.Value);
    }
    
    [Fact]
    public void Get_ReturnsProduct_WhenProductDoesNotExist()
    {
        //Arrange
        _mock.Setup(service => service.Get(4)).Returns((Book)null);
        
        //Act
        var result = _controller.Get(4);
        
        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    //CREATE
    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenBookIsValid()
    {
        // Arrange
        var bookDto = new BookDto { Title = "New Book", Description = "New Description", Year = 2023 };
        var book = new Book { Id = 4, Title = "New Book", Description = "New Description", Year = 2023 };
    
        _mock.Setup(service => service.Save(It.IsAny<Book>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(bookDto) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        if (result != null)
        {
            Assert.Equal(nameof(_controller.Get), result.ActionName);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal(bookDto.Title, ((Book)result.Value).Title);
        }
    }
    
    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Title", "Title is required");
        var bookDto = new BookDto(); // Невалидно, так как отсутствует поле Title

        // Act
        var result = await _controller.Create(bookDto) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        if (result != null)
        {
            Assert.Equal(400, result.StatusCode);
        }
    }
    
    //UPDATE
    [Fact]
    public async Task Update_ReturnsOk_WhenBookIsUpdated()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Updated Book", Description = "Updated Description", Year = 2024 };
        _mock.Setup(service => service.Get(1)).Returns(book);
        _mock.Setup(service => service.Save(It.IsAny<Book>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(1, book) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        if (result != null) Assert.Equal("Book successfully updated.", result.Value);
    }
    
    
    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var book = new Book { Id = 2, Title = "Updated Book", Description = "Updated Description", Year = 2024 };

        // Act
        var result = await _controller.Update(1, book) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        if (result != null) Assert.Equal(400, result.StatusCode);
        Assert.Equal("Id from Route does not match book id.", result.Value);
    }
    
    [Fact]
    public async Task Update_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Nonexistent Book", Description = "Nonexistent Description", Year = 2024 };
        _mock.Setup(service => service.Get(1)).Returns((Book)null);

        // Act
        var result = await _controller.Update(1, book) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        if (result != null) Assert.Equal(404, result.StatusCode);
        Assert.Equal("No book with the id 1.", result.Value);
    }
    
    //DELETE
    [Fact]
    public void Delete_ReturnsOk_WhenProductExists()
    {
        //Arrange
        var book = new Book { Id = 1, Title = "The Great Gatsby", Description = "A classic novel by F. Scott Fitzgerald", Year = 1925, AuthorId = 0, CategoryId = 0, PublisherId = 0, LanguageId = 0 };
        _mock.Setup(service => service.Get(1)).Returns(book);
        _mock.Setup(service => service.Delete(1));
        
        //Act
        var result = _controller.Delete(1) as OkObjectResult; 
        
        //Assert
        Assert.NotNull(result);
        if (result != null) Assert.Equal("Book successfully deleted.", result.Value);
    } 
    
    
    [Fact]
    public void Delete_ReturnsNotFound_WhenProductDoesNotExist()
    {
        //Arrange
        _mock.Setup(service => service.Get(1)).Returns((Book)null);
        
        //Act
        var result = _controller.Delete(1); 
        
        //Assert
        Assert.IsType<NotFoundObjectResult>(result);
    } 
    
    //Arrange
        
    //Act
        
    //Assert
    
}