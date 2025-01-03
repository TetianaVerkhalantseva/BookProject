using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Oblig2VerkhalantsevaAPI.Models;
using Oblig2VerkhalantsevaAPI.Services;

namespace Oblig2VerkhalantsevaAPI.Controllers;

[Route("/api/[controller]")]

public class BookController : ControllerBase
{
    private readonly IBookService _service;
    
    public BookController(IBookService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        var result = await _service.GetAllBooks();
        return Ok(result); // 200: Ok
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // 400: Bad Request: Invalid request format
        }

        var c = await _service.GetBook(id);
        if (c == null)
        {
            return NotFound($"No book with Id {id} was found."); // 404: Not Found
        }

        return Ok(c);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookDtoAdd book)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
    
        var existingBook = await _service.GetBookByTitle(book.Title);
        if (existingBook != null)
        {
            return Conflict("A book with the same title already exists."); // 409: Conflict
        }
    
        var newBook = new Book
        {
            Title = book.Title,
            Description = book.Description,
            Year = book.Year,
            AuthorId = book.AuthorId,
            CategoryId = book.CategoryId,
            PublisherId = book.PublisherId,
            LanguageId = book.LanguageId
        };

        try
        {
            await _service.Save(newBook);
            return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook); // 201: Created
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
        {
            return BadRequest("Creation failed due to invalid foreign key. Please ensure that Author, Category, Publisher, and Language IDs exist in the database.");
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An unexpected error occurred. Please try again later."); // 500: Internal Server Error
        }
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BookDtoAdd bookDto)
    {
        if (id != bookDto.Id)
            return BadRequest("Id from route does not match id from body.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingBook = await _service.GetBook(id);
        if (existingBook is null)
            return NotFound($"No book with id {id} found.");

        var updatedBook = new Book
        {
            Id = bookDto.Id,
            Title = bookDto.Title,
            Description = bookDto.Description,
            Year = bookDto.Year,
            AuthorId = bookDto.AuthorId,
            CategoryId = bookDto.CategoryId,
            PublisherId = bookDto.PublisherId,
            LanguageId = bookDto.LanguageId
        };

        try
        {
            await _service.Save(updatedBook);
            return Ok(new { Message = $"Book with id {id} successfully updated!" });
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
        {
            return BadRequest("Update failed due to invalid foreign key. Please ensure that Author, Category, Publisher, and Language IDs exist in the database.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var book = await _service.GetBook(id); 
        if (book is null)
            return NotFound($"No book with id {id} found.");
    
        await _service.Delete(id); 
    
        return NoContent(); // 204: No Content
        //return Ok($"Book with id {id} successfully deleted!");
    }
    
}

// Summary:

// 200: Ok
// 201: Created
// 204: No Content

// 400: Bad Request
// 404: Not Found
// 409: Conflict
// 500: Internal Server Error