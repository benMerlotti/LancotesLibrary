using System.ComponentModel.DataAnnotations;

namespace LancotesLibrary.Models;
public class Genre
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
}