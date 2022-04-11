using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
            if (request.RecipientWallet is null)
            {
                transferResult.Status = TransferResultViewModelStatus.InvalidRecipient;
                return View("TransferResult", transferResult);
            }
            if(!(request.Amount > 0))
            {
                transferResult.Status = TransferResultViewModelStatus.InvalidAmount;
                return View("TransferResult", transferResult);
            }

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


        public ActionResult CreateDeposit()
        {
            return View("CreateDeposit");
        }


        //deposit task post 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> View(DepositRequestDto request)
        {
            var deResult = new DepositViewModel()
            {
                Amount = request.Amount,
                Description = request.Description,
                Date = DateTime.UtcNow,
                Id = request.Id,
            };
            if (!(request.Amount >= 0))
            {
                ModelState.AddModelError("error", "number must be above 0");
                deResult.Status = DepResultViewModelStatus.InvalidAmount;
                return View("DepositResult", deResult);
            }

            if (request.Amount == 0)
            {
                ModelState.AddModelError("error", "number must be a posotive number above 0");
                deResult.Status = DepResultViewModelStatus.InvalidAmount;
                return View("DepositResult", deResult);
            }

            var user = await _userManager.GetUserAsync(User);
            var sourceWallet = await _banker.GetWalletByDisplayName(user.NormalizedEmail);
           
            await _banker.DepositFunds(sourceWallet, request.Amount, request.Description);
            deResult.Status = DepResultViewModelStatus.Approved;

            return View("DepositResult", deResult);
        }

        //bpay task post 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBPay(BpayRequestDto request)
        {
            var bpayResult = new BpayRequestViewModel()
            {
                Amount = request.Amount,
                Description = "bpay to "+request.BillPayID,
                Date = DateTime.UtcNow,
                BillPayID = request.BillPayID,
            };
            if (!(request.Amount >= 0))
            {
                bpayResult.Status = BpayResultViewModelStatus.InvalidAmount;
                return View("BpayResult", bpayResult);
            }


                if (request.Amount == 0)
            {
                bpayResult.Status = BpayResultViewModelStatus.InvalidAmount;
                return View("BpayResult", bpayResult);
            }


       

            var user = await _userManager.GetUserAsync(User);
            var sourceWallet = await _banker.GetWalletByDisplayName(user.NormalizedEmail);

            if (!await _banker.VerifyBalance(sourceWallet.Id, request.Amount))
            {
                bpayResult.Status = BpayResultViewModelStatus.InsufficientFunds;
                return View("BpayResult", bpayResult);
            }


            await _banker.BpayTransfer(sourceWallet, request.Amount, request.BillPayID, "bpay");

            bpayResult.Status = BpayResultViewModelStatus.Approved;
            return View("BpayResult", bpayResult);
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
