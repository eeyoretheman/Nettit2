using Nettit.Data.Entity;

namespace Nettit.Models;

public class nChatroomViewModel
{
    public required List<Message> Messages { get; set; }
    public required Chatroom Chatroom { get; set; }
}
