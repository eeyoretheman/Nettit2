using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Nettit.Data.Entity;

public class Chatroom : BaseEntity
{
    // Navigation property for messages
    public virtual ICollection<Message> Messages { get; set; }

    [Required]
    public string Title { get; set; }
    
    public string? Code { get; set; }

    public Chatroom()
    {
        Messages = new List<Message>();
    }
}