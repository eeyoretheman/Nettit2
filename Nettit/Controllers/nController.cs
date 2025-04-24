using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nettit.Data;
using Nettit.Models;

namespace Nettit.Controllers
{
    public class nController : Controller
    {
        private readonly ApplicationDbContext _context;

        public nController(ApplicationDbContext context)
        {
            _context = context;

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

            var messages = _context.Messages.Where(m => m.ChatroomId == chatroom.Id).Include(m => m.User).ToList();

            var viewModel = new nChatroomViewModel { Chatroom = chatroom, Messages = messages };

            return View(viewModel);
        }
    }
}
