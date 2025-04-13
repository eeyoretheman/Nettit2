using System.ComponentModel.DataAnnotations;

namespace Nettit.Data.Entity;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}