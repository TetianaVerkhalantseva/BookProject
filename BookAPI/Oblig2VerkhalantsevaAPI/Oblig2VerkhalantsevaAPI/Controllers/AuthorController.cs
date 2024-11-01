using Microsoft.AspNetCore.Mvc;
using Oblig2VerkhalantsevaAPI.Models;
using Oblig2VerkhalantsevaAPI.Models.Entities;
using Oblig2VerkhalantsevaAPI.Services;

namespace Oblig2VerkhalantsevaAPI.Controllers;

[Route("/api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _service;

    public AuthorController(IAuthorService service)
    {
        _service = service;
    }

    // GET: api/author
    [HttpGet]
    public async Task<IActionResult> GetAuthors()
    {
        var result = await _service.GetAllAuthors();
        return Ok(result); // 200: Ok
    }

    // GET: api/author/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState); // 400: Bad Request

        var author = await _service.GetAuthorDto(id);
        if (author == null)
            return NotFound($"No author with Id {id} found."); // 404: Not Found

        return Ok(author);
    }

    // POST: api/author
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AuthorDto authorDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState); // 400: Bad Request
        
        // Check if an author with the same first and last name already exists
        var existingAuthor = await _service.GetAuthorByName(authorDto.FirstName, authorDto.LastName);
        if (existingAuthor != null)
        {
            return Conflict("An author with the same name already exists."); // 409: Conflict
        }

        var newAuthor = new Author
        {
            FirstName = authorDto.FirstName,
            LastName = authorDto.LastName
        };

        await _service.Save(newAuthor);
        return CreatedAtAction(nameof(Get), new { id = newAuthor.Id }, newAuthor); // 201: Created
    }

    // PUT: api/author/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] AuthorDto authorDto)
    {
        if (id != authorDto.Id)
            return BadRequest("Id from route does not match id from body."); // 400: Bad Request
    
        if (!ModelState.IsValid)
            return BadRequest(ModelState); // 400: Bad Request

        // Check if LastName and FirstName have valid values
        if (string.IsNullOrWhiteSpace(authorDto.FirstName))
            return BadRequest(new { Message = "First name is required and cannot be empty." }); // 400: Bad Request
        if (string.IsNullOrWhiteSpace(authorDto.LastName))
            return BadRequest(new { Message = "Last name is required and cannot be empty." }); // 400: Bad Request

        var existingAuthor = await _service.GetAuthorDto(id);
        if (existingAuthor == null)
            return NotFound($"No author with id {id} found."); // 404: Not Found

        var updatedAuthor = new Author
        {
            Id = authorDto.Id,
            FirstName = authorDto.FirstName,
            LastName = authorDto.LastName
        };

        try
        {
            await _service.Save(updatedAuthor);
            return Ok(new { Message = $"Author with id {id} successfully updated!", Author = updatedAuthor });
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            return StatusCode(500, "An unexpected error occurred. Please try again later."); // 500: Internal Server Error
        }
    }
    
    
    // DELETE: api/author/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var author = await _service.GetAuthorDto(id);
        if (author == null)
            return NotFound($"No author with id {id} found."); // 404: Not Found

        await _service.Delete(id);
        return NoContent(); // 204: No Content
    }
}
