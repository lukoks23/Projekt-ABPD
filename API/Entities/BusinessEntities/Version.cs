using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Versions"),PrimaryKey(nameof(Id))]
public class Version
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string Name { get; set; } = string.Empty;
    [Required]
    public int SoftwareId { get; set; }
    [Required]
    [Column(TypeName = "date")]
    public DateTime PublishDate { get; set; }

    [ForeignKey(nameof(SoftwareId))]
    public virtual Software Software { get; set; } = null!;
    
    public virtual ICollection<AvailableVersion> AvailableVersions { get; set; } = new List<AvailableVersion>();
}