using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nettit.Controllers;
using Nettit.Data;
using Nettit.Data.Entity;
using Xunit;
using System.Threading.Tasks;

namespace Tests;

public class ChatroomsControllerTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("ChatroomsTestDb_" + System.Guid.NewGuid())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Create_ValidModel_RedirectsToChatroom()
    {
        var db = GetDbContext();
        var controller = new ChatroomsController(db);

        var chatroom = new Chatroom { Title = "Test Room" };

        var result = await controller.Create(chatroom);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.StartsWith("/n/", redirect.Url);
    }

    [Fact]
    public async Task Create_InvalidModel_RedirectsToHome()
    {
        var db = GetDbContext();
        var controller = new ChatroomsController(db);

        controller.ModelState.AddModelError("Title", "Required");

        var chatroom = new Chatroom(); // Missing Title

        var result = await controller.Create(chatroom);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/", redirect.Url);
    }
}
