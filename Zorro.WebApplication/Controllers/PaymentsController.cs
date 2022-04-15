﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zorro.WebApplication.Data;
using Zorro.WebApplication.Dtos;
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

        public ActionResult CreateDeposit()
        {
            return View("CreateDeposit");
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
                ModelState.AddModelError(string.Empty, "The Recipient ID doesn't exist, please check your details again.");
                return View("CreateTransfer");
            }
            if(!(request.Amount > 0))
            {
                ModelState.AddModelError(string.Empty, "The amount entered is not a valid amount. Please try again.");
                return View("CreateTransfer");
            }

            var user = await _userManager.GetUserAsync(User);
            var sourceWallet = await _banker.GetWalletByDisplayName(user.NormalizedEmail);
            if (!await _banker.VerifyBalance(sourceWallet.Id, request.Amount))
            {
                ModelState.AddModelError(string.Empty, "You have insufficent funds to process this request. Please check the Amount Entered.");
                return View("CreateTransfer");
            }
            var destinationWallet = await _banker.GetWalletByDisplayName(request.RecipientWallet);
            if (destinationWallet is null)
            {
                ModelState.AddModelError(string.Empty, "The Recipient ID doesn't exist, please check your details again.");
                return View("CreateTransfer");
            }

            await _banker.TransferFunds(sourceWallet, destinationWallet, request.Amount, request.Description);

            transferResult.Status = TransferResultViewModelStatus.Approved;
            return View("TransferResult", transferResult);
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
            if (!(request.Amount > 0))
            {
                ModelState.AddModelError(string.Empty, "The amount entered is not a valid amount. Please try again.");
                return View("CreateDeposit");
            }

            var user = await _userManager.GetUserAsync(User);
            var CCNum = user.CreditCardNumber;
            var CCExpiry = user.CCExpiry;

            if(CCNum == null || CCExpiry == null)
            {
                ModelState.AddModelError(string.Empty, "You don't have a Credit Card Added to your Wallet. Please add one first.");
                return View("CreateDeposit");
            }
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
            if (!(request.Amount > 0))
            {
                ModelState.AddModelError(string.Empty, "The amount entered is not a valid amount. Please try again.");
                return View("CreateBPAY");
            }

            var user = await _userManager.GetUserAsync(User);
            var sourceWallet = await _banker.GetWalletByDisplayName(user.NormalizedEmail);

            if (!await _banker.VerifyBalance(sourceWallet.Id, request.Amount))
            {
                ModelState.AddModelError(string.Empty, "You have insufficent funds to process this request. Please check the Amount Entered.");
                return View("CreateBpay");
            }

            await _banker.BpayTransfer(sourceWallet, request.Amount, request.BillPayID, "bpay");

            bpayResult.Status = BpayResultViewModelStatus.Approved;
            return View("BpayResult", bpayResult);
        }
    }
}