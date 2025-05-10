using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nettit.Controllers;
using Nettit.Data;
using Nettit.Data.Entity;
using System.Security.Claims;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Tests;

public class MainControllerTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        return new ApplicationDbContext(options);
    }

    private Mock<UserManager<NettitUser>> GetMockUserManager()
    {
        var store = new Mock<IUserStore<NettitUser>>();
        return new Mock<UserManager<NettitUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );
    }

    private ClaimsPrincipal GetFakeUser(string userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));
    }

    [Fact]
    public async Task Index_ReturnsNotFound_WhenCodeIsNull()
    {
        var db = GetInMemoryDbContext();
        var userManager = GetMockUserManager();

        var controller = new MainController(db, userManager.Object);
        var result = await controller.Index(null);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Index_ReturnsNotFound_WhenChatroomNotFound()
    {
        var db = GetInMemoryDbContext();
        var userManager = GetMockUserManager();

        var user = new NettitUser
        {
            Id = "user1",
            Chatrooms = new List<Chatroom> { }
        };

        db.Users.Add(user);
        db.SaveChanges();

        userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var controller = new MainController(db, userManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetFakeUser(user.Id)
                }
            }
        };

        var result = await controller.Index("nonexistent");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Index_AddsUserToChatroom_IfNotAlreadyJoined()
    {
        var db = GetInMemoryDbContext();
        var userManager = GetMockUserManager();

        var user = new NettitUser { Id = "user1", Chatrooms = new List<Chatroom>() };
        var chatroom = new Chatroom { Id = 1, Title = "chat1", Code = "1234" };

        db.Chatrooms.Add(chatroom);
        db.Users.Add(user);
        db.SaveChanges();

        userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var controller = new MainController(db, userManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetFakeUser(user.Id)
                }
            }
        };

        // Act
        var result = await controller.Index("1234");

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Contains(user.Chatrooms, c => c.Id == chatroom.Id);
    }

    [Fact]
    public async Task Me_ReturnsChallenge_WhenUserIsNull()
    {
        var db = GetInMemoryDbContext();
        var userManager = GetMockUserManager();

        userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((NettitUser)null);

        var controller = new MainController(db, userManager.Object);

        var result = await controller.Me();

        Assert.IsType<ChallengeResult>(result);
    }

    [Fact]
    public async Task Me_ReturnsView_WithUserChatrooms()
    {
        var db = GetInMemoryDbContext();
        var userManager = GetMockUserManager();

        var chatroom = new Chatroom { Id = 1, Title = "chat1", Code = "1234" };
        var user = new NettitUser
        {
            Id = "user1",
            Chatrooms = new List<Chatroom> { chatroom }
        };

        db.Chatrooms.Add(chatroom);
        db.Users.Add(user);
        db.SaveChanges();

        userManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var controller = new MainController(db, userManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetFakeUser(user.Id)
                }
            }
        };

        var result = await controller.Me();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Chatroom>>(viewResult.Model);
        Assert.Contains(model, c => c.Id == chatroom.Id);
    }
}