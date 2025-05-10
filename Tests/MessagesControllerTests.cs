using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nettit.Controllers;
using Nettit.Data;
using Nettit.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Tests;

public class MessagesControllerTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("MessagesTestDb_" + Guid.NewGuid())
            .Options;

        return new ApplicationDbContext(options);
    }

    private Mock<UserManager<NettitUser>> GetUserManagerMock()
    {
        var store = new Mock<IUserStore<NettitUser>>();
        return new Mock<UserManager<NettitUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    private ClaimsPrincipal GetUser(string userId, bool isAdmin = false)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
        };

        if (isAdmin)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
    }

    [Fact]
    public async Task Create_ValidMessage_RedirectsToChatroom()
    {
        var db = GetDbContext();
        var userManager = GetUserManagerMock();

        var chatroom = new Chatroom { Id = 1, Title = "abc", Code = "1234" };
        db.Chatrooms.Add(chatroom);
        db.SaveChanges();

        userManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

        var controller = new MessagesController(db, userManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = GetUser("user1") }
            }
        };

        var message = new Message { Content = "Hi", ChatroomId = chatroom.Id };
        var result = await controller.Create(message);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/n/1234", redirect.Url);
    }

    [Fact]
    public async Task Delete_ByOriginalUser_DeletesMessage()
    {
        var db = GetDbContext();
        var userManager = GetUserManagerMock();

        var user = new NettitUser { Id = "user1" };
        var chatroom = new Chatroom { Id = 1, Title = "abc", Code = "1234" };
        var message = new Message { Id = 1, ChatroomId = 1, UserId = "user1", Content = "Hi" };

        db.Chatrooms.Add(chatroom);
        db.Messages.Add(message);
        db.SaveChanges();

        userManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

        var controller = new MessagesController(db, userManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = GetUser("user1") }
            }
        };

        var result = await controller.Delete(1);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/n/1234", redirect.Url);
        Assert.Empty(db.Messages.ToList());
    }

    [Fact]
    public async Task Delete_ByUnauthorizedUser_ReturnsForbid()
    {
        var db = GetDbContext();
        var userManager = GetUserManagerMock();

        db.Chatrooms.Add(new Chatroom { Id = 1, Title = "abc", Code = "1234" });
        db.Messages.Add(new Message { Id = 1, ChatroomId = 1, UserId = "owner", Content = "Hi" });
        db.SaveChanges();

        userManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("otherUser");

        var controller = new MessagesController(db, userManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = GetUser("otherUser") }
            }
        };

        var result = await controller.Delete(1);

        Assert.IsType<ForbidResult>(result);
    }
}
