using Microsoft.AspNetCore.Mvc;
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

        await _service.Save(newBook);
        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook); // 201: Created
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BookDtoAdd book)
    {
        if (id != book.Id)
            return BadRequest("Id from route does not match id from body.");
    
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingBook = await _service.GetBook(id);
        if (existingBook is null)
            return NotFound($"No book with id {id} found.");

        var updatedBook = new Book
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Year = book.Year,
            AuthorId = book.AuthorId,
            CategoryId = book.CategoryId,
            PublisherId = book.PublisherId,
            LanguageId = book.LanguageId
        };

        await _service.Save(updatedBook);
        return Ok(new { Message = $"Book with id {id} successfully updated!", Book = updatedBook });
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