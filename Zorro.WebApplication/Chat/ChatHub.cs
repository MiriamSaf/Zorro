using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.WebApplication.Chat
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SendMessage(string sender, string recipient, string message)
        {
            await Clients.User(recipient).SendAsync("ReceiveMessage", sender, message);
            await Clients.Caller.SendAsync("ReceiveMessage", sender, message);
            await SaveChatMessage(sender, recipient, message, DateTime.Now);
        }

        private async Task SaveChatMessage(string sender, string recipient, string message, DateTime timestamp)
        {
            var chatMessage = new ChatMessage()
            {
                Sender = await _userManager.FindByEmailAsync(sender),
                Recipient = await _userManager.FindByEmailAsync(recipient),
                Timestamp = timestamp,
                Message = message
            };
            await _context.ChatMessages.AddAsync(chatMessage);
            await _context.SaveChangesAsync();
        }
    }

    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity?.Name;
        }
    }
}
