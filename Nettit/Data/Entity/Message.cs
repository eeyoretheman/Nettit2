using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Nettit.Data.Entity;
public class Message : BaseEntity
{
    public string Content { get; set; }

    public string UserId { get; set; }  // Identity uses string for user IDs

    // Navigation property to link to the Identity user
    public virtual IdentityUser User { get; set; }

    // Foreign key for the Chatroom relationship
    public int ChatroomId { get; set; }
    public virtual Chatroom Chatroom { get; set; }
}