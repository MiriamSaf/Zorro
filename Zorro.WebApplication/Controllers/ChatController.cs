using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.WebApplication.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string id)
        {
            ApplicationUser otherUser = null;
            if (!string.IsNullOrEmpty(id))
                otherUser = await _userManager.FindByEmailAsync(id);

            var user = await _userManager.GetUserAsync(User);
            var messages = await GetCurrentConversationAsync(user, otherUser);

            ViewBag.UserName = User.Identity.Name;
            ViewBag.Recipient = id == null ? "" : id.ToLower();
            ViewBag.Friends = _userManager.Users.ToArray();

            return View("Chat", messages);
        }

        private async Task<IEnumerable<ChatMessage>> GetCurrentConversationAsync(ApplicationUser user1, ApplicationUser user2)
        {
            var messages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Where(m => (m.Sender == user1 & m.Recipient == user2) || (m.Recipient == user1 & m.Sender == user2))
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();

            return messages;
        }
    }
}
