using Nettit.Data.Entity;

namespace Nettit.Models;

public class ViewModels
{
    public required List<Message> Messages { get; set; }
    public required Chatroom Chatroom { get; set; }
}

public class ChatroomOverviewViewModel
{
    public int ChatroomId { get; set; }
    public string Title { get; set; }
    public string? Code { get; set; }

    public List<UserViewModel> Users { get; set; } = new();
    public List<MessageViewModel> Messages { get; set; } = new();
}

public class UserViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}

public class MessageViewModel
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserViewModel? Sender { get; set; }
}
