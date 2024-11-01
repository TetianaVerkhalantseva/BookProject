using System.ComponentModel.DataAnnotations;

namespace Oblig2VerkhalantsevaAPI.Models;

public class Book

{
    public int Id { get; set; }
    [MaxLength(25)]
    public string Title { get; set; } = "New Title";
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