using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("AvailableVersions"),PrimaryKey(nameof(Id))]
public class AvailableVersion
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int VersionId { get; set; }
    [Required]
    public int ContractId { get; set; }

    [ForeignKey(nameof(ContractId))]
    public virtual Contract Contract { get; set; } = null!;

    [ForeignKey(nameof(VersionId))]
    public virtual Version Version { get; set; } = null!;
}