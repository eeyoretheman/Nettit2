using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Nettit.Data.Entity;
public class Chatroom : BaseEntity
{
    public virtual ICollection<NettitUser> Users { get; set; } = new HashSet<NettitUser>();

    [Required]
    public string Title { get; set; }
    public string? Code { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
}