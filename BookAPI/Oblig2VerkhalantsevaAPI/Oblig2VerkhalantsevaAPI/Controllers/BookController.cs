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
    public async Task<List<Book>> GetAll() => await _service.GetAll();

    [HttpGet("{id:int}")]
    public IActionResult Get([FromRoute] int id)
    {
        var book = _service.Get(id);

        if (book == null)
            return NotFound($"Not book with this id {id}.");
        
        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookDto book)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var newBook = new Book
        {
            Title = book.Title,
            Description = book.Description,
            Year = book.Year,
        };

        await _service.Save(newBook);
        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);

    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Book book)
    {
        if (id != book.Id)
            return BadRequest("Id from Route does not match book id.");

        var existingBook = _service.Get(id);

        if (existingBook is null)
            return NotFound($"No book with the id {id}.");

        var newBook = new Book
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Year = book.Year,
        };

        await _service.Save(newBook);
        return Ok("Book successfully updated.");
    }
    
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var book = _service.Get(id);
        if (book == null)
            return NotFound($"No book with the id {id}.");
        
        _service.Delete(id);
        return Ok("Book successfully deleted.");
    }
    
}