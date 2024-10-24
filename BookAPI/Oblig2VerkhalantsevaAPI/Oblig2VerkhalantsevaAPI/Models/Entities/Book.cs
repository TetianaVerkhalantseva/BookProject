namespace Oblig2VerkhalantsevaAPI.Models;

public class Book

{
    public int Id { get; set; }
    public string Title { get; set; } = null!; // Title никогда не должен быть null, но пока временно инициализирован как null
    public string? Description { get; set; }
    public int Year { get; set; }
    public int AuthorId { get; set; } 
    public Author? Author { get; set; } 
    public int CategoryId { get; set; }
    public Category? Category { get; set; } 
    public int PublisherId { get; set; } 
    public Publisher? Publisher { get; set; }
    public int LanguageId { get; set; } 
    public Language? Language { get; set; }
}