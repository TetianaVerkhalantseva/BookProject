using System.ComponentModel.DataAnnotations;

namespace Oblig2VerkhalantsevaAPI.Models;

public class AuthorDto
{
    public int Id { get; set; } 
    [MaxLength(20)] 
    public string FirstName { get; set; } = "FirstName";
    [MaxLength(20)] 
    public string LastName { get; set; } = "LastName";
}