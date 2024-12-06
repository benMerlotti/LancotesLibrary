using System.ComponentModel.DataAnnotations;

namespace LancotesLibrary.Models;
public class Material
{
    public int Id { get; set; }
    [Required]
    public string MaterialName { get; set; }
    public MaterialType MaterialType { get; set; }
    [Required]
    public int MaterialTypeId { get; set; }
    public Genre Genre { get; set; }
    [Required]
    public int GenreId { get; set; }
    public DateTime? OutOfCirculationSince { get; set; }
}