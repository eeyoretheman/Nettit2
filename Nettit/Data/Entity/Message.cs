using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Nettit.Data.Entity;
public class Message : BaseEntity
{
    [Required]
    public string Content { get; set; }

    public string? UserId { get; set; }  // Identity uses string for user IDs

    // Navigation property to link to the Identity user
    [ForeignKey("UserId")]
    public virtual NettitUser? User { get; set; }

    // Foreign key for the Chatroom relationship
    [Required]
    public int ChatroomId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ChatroomId")]
    public virtual Chatroom? Chatroom { get; set; }
}