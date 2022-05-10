using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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
            if (string.IsNullOrWhiteSpace(sender) || string.IsNullOrWhiteSpace(recipient) || string.IsNullOrWhiteSpace(message))
                return;

            // send message to recipient
            await Clients.User(recipient).SendAsync("ReceiveMessage", sender, message);
            // send message back to sender so they see it in their chat window as well
            await Clients.Caller.SendAsync("ReceiveMessage", sender, message);

            await SaveChatMessage(sender, recipient, message, DateTime.Now);
        }

        private async Task SaveChatMessage(string sender, string recipient, string message, DateTime timestamp)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            var senderUser = await _userManager.FindByEmailAsync(sender);
            if (senderUser is null)
                return;
            var recipientuser = await _userManager.FindByEmailAsync(recipient);
            if (recipientuser is null)
                return;

            var chatMessage = new ChatMessage()
            {
                Sender = senderUser,
                Recipient = recipientuser,
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
