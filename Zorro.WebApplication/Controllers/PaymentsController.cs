using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zorro.WebApplication.Data;
using Zorro.Dal.Models;
using Zorro.WebApplication.Dtos;
using Zorro.WebApplication.ViewModels;
using Microsoft.EntityFrameworkCore;
using Zorro.Dal;

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

        public async Task <ActionResult> Bpay()
        {
            // find remembered biller and pass to view
            var user = await _userManager.GetUserAsync(User);
            var rememberedBillers = await _context.RememberedBillers
                .Where(x => x.ApplicationUser == user)
                .Include(z => z.BpayBiller)
                .ToListAsync();
            ViewBag.RememberedBillers = rememberedBillers;

            return View("CreateBpay");
        }

        public async ActionResult CreateDeposit()
        {
            return View("CreateDeposit");
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
            if(request.Description.Length > 50)
            {
                ModelState.AddModelError(string.Empty, "Error: the comment cannot be longer than 50 characters"); 
                return View("CreateTransfer");
            }
            //check for decimals greater than 3 places 
            String checkDec = request.Amount.ToString();
            if (checkDec.Contains("."))
            {
                string[] a = checkDec.Split(new char[] { '.' });
                int decimals = a[1].Length;
                if (decimals >= 3)
                {
                    ModelState.AddModelError(string.Empty, "The amount entered cannot have more than 2 decimal places"); 
                    return View("CreateTransfer");
                }
            }
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
        public async Task<ActionResult> CreateDeposit(DepositRequestDto request)
        {
            var deResult = new DepositViewModel()
            {
                Amount = request.Amount,
                Description = request.Description,
                Date = DateTime.UtcNow,
                Id = request.Id,
            };


            if (request.Description.Length > 50)
            {
                ModelState.AddModelError(string.Empty, "Error: the comment cannot be longer than 50 characters");
                return View("CreateDeposit");
            }


            if (!(request.Amount > 0))
            {
                ModelState.AddModelError(string.Empty, "The amount entered is not a valid amount. Please try again.");
                return View("CreateDeposit");
            }

            //check for decimals greater than 3 places 
            String checkDec = request.Amount.ToString();
                if (checkDec.Contains("."))
            {
                string[] a = checkDec.Split(new char[] { '.' });
                int decimals = a[1].Length;
                if (decimals >= 3)
                {
                    ModelState.AddModelError(string.Empty, "The amount entered cannot have more than 2 decimal places"); ;
                    return View("CreateDeposit");
                }
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

            //check for decimals greater than 3 places 
            String checkDec = request.Amount.ToString();
            if (checkDec.Contains("."))
            {
                string[] a = checkDec.Split(new char[] { '.' });

                int decimals = a[1].Length;

                if (decimals >= 3)
                {
                    ModelState.AddModelError(string.Empty, "The amount entered cannot have more than 2 decimal places"); ;
                    return View("CreateBPAY");
                }
            }

            if (request.Amount == 0)
            {
                ModelState.AddModelError(string.Empty, "The amount entered cannot by zero. Please try again");
                return View("CreateBPAY");
            }

            if(request.Amount > 10000)
            {
                ModelState.AddModelError(string.Empty, "The amount to pay must not be greater than $10 000. Please try again");
                return View("CreateBpay");
            }
            
            if (!(request.Amount >= 0))
            {
                ModelState.AddModelError(string.Empty, "The amount entered is not a valid amount. Please try again.");
                return View("CreateBPAY");
            }
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

            // remember biller
            if (request.RememberBiller)
            {
                var biller = await _context.Payees.FindAsync(request.BillPayID);
                var rememberedBiller = await _context.FindAsync<RememberedBiller>(biller.BillerCode, user.Id);
                if (rememberedBiller is null)
                {
                    rememberedBiller = new RememberedBiller()
                    {
                        ApplicationUser = user,
                        BpayBiller = biller,
                    };
                    await _context.AddAsync(rememberedBiller);
                    await _context.SaveChangesAsync();
                }
            }            

            return View("BpayResult", bpayResult);
        }

        public async Task<ActionResult> VerifyWalletId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusCode(404);
            var foundWallet = await _banker.GetWalletByDisplayName(id.ToUpper());
            if (foundWallet is null)
                return StatusCode(404);
            return StatusCode(204);
        }

        public async Task<BpayBillerVerificationResult> VerifyBillerCode(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new FileNotFoundException();
            var foundBiller = await _context.Payees.FindAsync(int.Parse(id));
            if (foundBiller is null)
                throw new FileNotFoundException();
            return new BpayBillerVerificationResult() { BillerName = foundBiller.BillerName };
        }
    }
}
