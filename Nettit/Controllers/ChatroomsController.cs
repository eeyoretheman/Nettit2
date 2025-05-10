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

        [Route("/n/{code}/changed")]
        public async Task<IActionResult> ChatroomChanged(string code)
        {
            var messages = new List<int>();

            var chatrooms = _context.Chatrooms.Where(c => c.Code == code).Include(m => m.Messages);

            if (chatrooms.Count() == 0)
            {
                return Ok(new { });
            }

            var chatroom = chatrooms.First();

            foreach (var message in chatroom.Messages)
            {
                messages.Add(message.Id);
            }

            return Ok(messages);
        }


        [Authorize(Roles = "Admin")]
        [Route("/n/all")]
        public async Task<IActionResult> All()
        {
            var chatrooms = await _context.Chatrooms
                .Include(c => c.Users)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.User)
                .ToListAsync();

            var viewModel = chatrooms.Select(cr => new ChatroomOverviewViewModel
            {
                ChatroomId = cr.Id,
                Title = cr.Title,
                Code = cr.Code,
                Users = cr.Users.Select(u => new UserViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                }).ToList(),
                Messages = cr.Messages.Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    Sender = m.User != null ? new UserViewModel
                    {
                        Id = m.User.Id,
                        UserName = m.User.UserName,
                        Email = m.User.Email
                    } : null
                }).ToList()
            }).ToList();

            return View(viewModel);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("/n/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var chatroom = await _context.Chatrooms
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chatroom == null)
                return NotFound();

            _context.Messages.RemoveRange(chatroom.Messages);
            _context.Chatrooms.Remove(chatroom);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
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

                return Redirect($"/n/{chatroom.Code}");
            }

            // Invalid state shouldn't happen, but it should be addressed
            return Redirect("/");
        }
    }
}
