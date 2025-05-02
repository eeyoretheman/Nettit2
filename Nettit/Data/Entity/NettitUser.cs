using Microsoft.AspNetCore.Identity;
using Nettit.Data.Entity;

public class NettitUser : IdentityUser
{
    public virtual ICollection<Chatroom> Chatrooms { get; set; } = new HashSet<Chatroom>();
}
