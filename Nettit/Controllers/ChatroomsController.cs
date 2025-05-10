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

                return Redirect($"/n/{chatroom.Code}");
            }

            // Invalid state shouldn't happen, but it should be addressed
            return Redirect("/");
        }
    }
}
