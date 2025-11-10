using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("wi_weighted")]
public class WiWeighted
{
    [Key, Column(Order = 0)]
    public int Id { get; set; }

    [Key, Column(Order = 1)]
    [Required, StringLength(255)]
    public string What { get; set; } = string.Empty;

    [Key, Column(Order = 2)]
    [Required, StringLength(255)]
    public string Word { get; set; } = string.Empty;

    [Column(TypeName = "decimal")]
    public decimal? Tfidf { get; set; }
}