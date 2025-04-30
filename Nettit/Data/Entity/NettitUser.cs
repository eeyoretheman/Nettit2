using Microsoft.AspNetCore.Identity;

namespace Nettit.Data.Entity
{

    public class NettitUser : IdentityUser
    {
        // Add your custom properties
        public virtual ICollection<Chatroom> Chatrooms { get; set; }

        // Constructor to initialize collections
        public NettitUser()
        {
            Chatrooms = new HashSet<Chatroom>();
        }
    }

}
