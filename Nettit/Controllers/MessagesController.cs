using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nettit.Data;
using Nettit.Data.Entity;
using Nettit.Models;

namespace Nettit.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<NettitUser> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<NettitUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Messages.Include(m => m.Chatroom).Include(m => m.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Chatroom)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["ChatroomId"] = new SelectList(_context.Chatrooms, "Id", "Code");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");

            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,ChatroomId,Id")] Message message)
        {
            message.UserId = _userManager.GetUserId(User);
            message.CreatedAt = DateTime.Now;

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        // Log or breakpoint here to see the specific error messages
                        var key = state.Key;
                        var errorMessage = error.ErrorMessage;
                    }
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
            }

            // var messages = _context.Messages.Where(m => m.ChatroomId == message.ChatroomId).Include(m => m.User).ToList();
            var chatroom = _context.Chatrooms.Where(c => c.Id == message.ChatroomId).First();

            // var viewModel = new nChatroomViewModel { Chatroom = chatroom, Messages = messages};

            return Redirect($"/n/{chatroom.Code}");
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["ChatroomId"] = new SelectList(_context.Chatrooms, "Id", "Code", message.ChatroomId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", message.UserId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Content,UserId,ChatroomId,Id")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChatroomId"] = new SelectList(_context.Chatrooms, "Id", "Code", message.ChatroomId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", message.UserId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Chatroom)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.Messages.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            // Only allow deletion by the original user or an admin
            if (message.UserId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            // Redirect back to the chatroom after deletion
            var chatroom = await _context.Chatrooms.FirstOrDefaultAsync(c => c.Id == message.ChatroomId);
            var messages = await _context.Messages
                .Where(m => m.ChatroomId == message.ChatroomId)
                .Include(m => m.User)
                .ToListAsync();

            return Redirect($"/n/{chatroom.Code}");
        }


        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
