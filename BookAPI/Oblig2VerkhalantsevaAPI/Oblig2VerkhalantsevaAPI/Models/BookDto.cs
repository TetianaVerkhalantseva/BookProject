namespace Oblig2VerkhalantsevaAPI.Models;

public class BookDto //Data Transfer Objects
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Year { get; set; }
}