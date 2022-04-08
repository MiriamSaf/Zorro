using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zorro.WebApplication.Data;
using Zorro.WebApplication.Dtos;
using Zorro.WebApplication.Models;
using Zorro.WebApplication.ViewModels;

namespace Zorro.WebApplication.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ILogger<PaymentsController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBanker _banker;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentsController(ILogger<PaymentsController> logger, ApplicationDbContext applicationDbContext,
            UserManager<ApplicationUser> userManager, IBanker banker)
        {
            _logger = logger;
            _context = applicationDbContext;
            _userManager = userManager;
            _banker = banker;
        }

        public ActionResult Transfer()
        {
            return View("CreateTransfer");
        }

        public ActionResult Bpay()
        {
            return View("CreateBpay");
        }

        public ActionResult TransferResult()
        {
            var transferResult = new TransferResultViewModel()
            {
                Status = TransferResultViewModelStatus.InvalidRecipient,
                Amount = 50.00M,
                RecipientDisplayName = "luis.dabruzzo@gmail.com",
                Comment = "Ribs and Pizza"

            };
            return View("TransferResult", transferResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTransfer(TransferRequestDto request)
        {
            var transferResult = new TransferResultViewModel()
            {
                Amount = request.Amount,
                RecipientDisplayName = request.RecipientWallet,
                Comment = request.Description
            };

            var user = await _userManager.GetUserAsync(User);
            var sourceWallet = await _banker.GetWalletByDisplayName(user.NormalizedEmail);
            if (!await _banker.VerifyBalance(sourceWallet.Id, request.Amount))
            {
                transferResult.Status = TransferResultViewModelStatus.InsufficientFunds;
                return View("TransferResult", transferResult);
            }
            var destinationWallet = await _banker.GetWalletByDisplayName(request.RecipientWallet);
            if (destinationWallet is null)
            {
                transferResult.Status = TransferResultViewModelStatus.InvalidRecipient;
                return View("TransferResult", transferResult);
            }

            await _banker.TransferFunds(sourceWallet, destinationWallet, request.Amount, request.Description);

            transferResult.Status = TransferResultViewModelStatus.Approved;
            return View("TransferResult", transferResult);
        }

        // POST: PaymentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
