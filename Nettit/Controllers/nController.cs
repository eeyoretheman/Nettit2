using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nettit.Data;
using Nettit.Data.Entity;
using Nettit.Models;

namespace Nettit.Controllers
{
    public class nController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NettitUser> _userManager;

        public nController(ApplicationDbContext context, UserManager<NettitUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("/n/{n}")]
        public async Task<IActionResult> Index(string n)
        {
            if (string.IsNullOrEmpty(n))
            {
                return NotFound();
            }

            var chatroom = await _context.Chatrooms
                .FirstOrDefaultAsync(m => m.Code == n);

            if (chatroom == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            await _context.Entry(user).Collection(u => u.Chatrooms).LoadAsync();

            if (!user.Chatrooms.Any(c => c.Id == chatroom.Id))
            {
                user.Chatrooms.Add(chatroom);
                await _context.SaveChangesAsync();
            }

            var messages = _context.Messages.Where(m => m.ChatroomId == chatroom.Id).Include(m => m.User).ToList();

            var viewModel = new nChatroomViewModel { Chatroom = chatroom, Messages = messages };

            return View(viewModel);
        }

        [HttpGet]
        [Route("/n/me")]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge(); // Redirect to login

            await _context.Entry(user)
                .Collection(u => u.Chatrooms)
                .LoadAsync();

            return View(user.Chatrooms);
        }
    }
}
