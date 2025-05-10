using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nettit.Data;
using Nettit.Data.Entity;
using Nettit.Models;

namespace Nettit.Controllers
{
    public class ChatroomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatroomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Id")] Chatroom chatroom)
        {
            chatroom.Code = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            if (ModelState.IsValid)
            {
                _context.Add(chatroom);
                await _context.SaveChangesAsync();

                // Turn these into views for better code clarity
                // Should always be empty
                // var messages = _context.Messages.Where(m => m.ChatroomId == chatroom.Id).Include(m => m.User).ToList();
                // var viewModel = new nChatroomViewModel { Chatroom = chatroom, Messages = messages };

                return Redirect($"/n/{chatroom.Code}");
            }

            // Invalid state shouldn't happen, but it should probably be addressed anyway
            return View("/Views/Home/Index.cshtml");
        }

        [Authorize(Roles = "Admin")]
        // POST: Chatrooms/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var chatroom = await _context.Chatrooms.FindAsync(id);
            if (chatroom != null)
            {
                _context.Chatrooms.Remove(chatroom);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatroomExists(int id)
        {
            return _context.Chatrooms.Any(e => e.Id == id);
        }
    }
}
