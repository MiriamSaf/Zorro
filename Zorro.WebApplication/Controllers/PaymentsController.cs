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
    //payments controller - deals with sending payment views and getting the data for the views
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly ILogger<PaymentsController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBanker _banker;
        private readonly UserManager<ApplicationUser> _userManager;

        //gets DI needed for payments controller
        public PaymentsController(ILogger<PaymentsController> logger, ApplicationDbContext applicationDbContext,
            UserManager<ApplicationUser> userManager, IBanker banker)
        {
            _logger = logger;
            _context = applicationDbContext;
            _userManager = userManager;
            _banker = banker;
        }

        //return transfer creation view
        public ActionResult Transfer()
        {
            return View("CreateTransfer");
        }

        //bpay view and gets the details needed for the view
        public async Task <ActionResult> Bpay()
        {
            // find remembered biller and pass to view
            var user = await _userManager.GetUserAsync(User);
            var rememberedBillers = await _context.RememberedBillers
                .Where(x => x.ApplicationUser == user)
                .Include(z => z.BpayBiller)
                .ToListAsync();
            ViewBag.RememberedBillers = rememberedBillers;
            //return the view 
            return View("CreateBpay");
        }

        //return create deposit view
        public ActionResult CreateDeposit()
        {
            return View("CreateDeposit");
        }

        //post the result for creating a transfer to another user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTransfer(TransferRequestDto request)
        {
            Guid guid = Guid.NewGuid();
            var TransactionID = guid.ToString();

            //creates a transfer result from the passed in request
            var transferResult = new TransferResultViewModel()
            {
                Amount = request.Amount,
                RecipientDisplayName = request.RecipientWallet,
                Comment = request.Description,
                TransactionID = TransactionID
            };

            //set desciption to blank if no desciption entered 
            if (request.Description == null)
            {
                request.Description = "";
            }

            //checks the description length and adds error if too long
            if (request.Description.Length > 50)
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
                //add error if more than 3 decimals
                if (decimals >= 3)
                {
                    ModelState.AddModelError(string.Empty, "The amount entered cannot have more than 2 decimal places"); 
                    return View("CreateTransfer");
                }
            }
            //if the recipient doesn't exists then let user know with error
            if (request.RecipientWallet is null)
            {
                ModelState.AddModelError(string.Empty, "The Recipient ID doesn't exist, please check your details again.");
                return View("CreateTransfer");
            }
            //if amount below 0 then add error
            if(!(request.Amount > 0))
            {
                ModelState.AddModelError(string.Empty, "The amount entered is below zero and not a valid amount. Please try again.");
                return View("CreateTransfer");
            }
            //if amount is 0 then add error
            if ((request.Amount == 0))
            {
                ModelState.AddModelError(string.Empty, "The amount entered cannot be zero. Please try again.");
                return View("CreateTransfer");
            }
            //check user and source wallet
            var user = await _userManager.GetUserAsync(User);
            var sourceWallet = await _banker.GetWalletByDisplayName(user.NormalizedEmail);
            //if there is not enough funds in the users wallet then they should get an error message
            if (!await _banker.VerifyBalance(sourceWallet.Id, request.Amount))
            {
                ModelState.AddModelError(string.Empty, "You have insufficent funds to process this request. Please check the Amount Entered.");
                return View("CreateTransfer");
            }
            //check the destination wallet and that it exists and add error if doesn't exist
            var destinationWallet = await _banker.GetWalletByDisplayName(request.RecipientWallet);
            if (destinationWallet is null)
            {
                ModelState.AddModelError(string.Empty, "The Recipient ID doesn't exist, please check your details again.");
                return View("CreateTransfer");
            }
            //all is okay so transfer the funds by calling bankers transfer method
            await _banker.TransferFunds(sourceWallet, destinationWallet, request.Amount, request.Description);
            //set the status and return the transfer page 
            transferResult.Status = TransferResultViewModelStatus.Approved;
            return View("TransferResult", transferResult);
        }

        //deposit task post and does checks for correct request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDeposit(DepositRequestDto request)
        {
            Guid guid = Guid.NewGuid();
            var TransactionID = guid.ToString();
            //set the deposit result fields from passed in request
            var deResult = new DepositViewModel()
            {
                Amount = request.Amount,
                Description = request.Description,
                Date = DateTime.UtcNow,
                TransactionID = TransactionID
            };

            //if there is a description
            if (request.Description != null)
            {
                //if greater than 50 chars in description add error
                if (request.Description.Length > 50)
                {
                    ModelState.AddModelError(string.Empty, "Error: the comment cannot be longer than 50 characters");
                    return View("CreateDeposit");
                }
            }

            //if request less than 0, add error
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
                //add error if 3 or more decimals
                if (decimals >= 3)
                {
                    ModelState.AddModelError(string.Empty, "The amount entered cannot have more than 2 decimal places"); ;
                    return View("CreateDeposit");
                }
            }

            //see if user has a credit card in their wallet
            var user = await _userManager.GetUserAsync(User);
            var CCNum = user.CreditCardNumber;
            var CCExpiry = user.CCExpiry;
            //if no cc do not allow them to make deposit and show error 
            if(CCNum == null || CCExpiry == null)
            {
                ModelState.AddModelError(string.Empty, "You don't have a Credit Card Added to your Wallet. Please add one first.");
                return View("CreateDeposit");
            }
            var sourceWallet = await _banker.GetWalletByDisplayName(user.NormalizedEmail);
           
            await _banker.DepositFunds(sourceWallet, request.Amount, request.Description);
            deResult.Status = DepResultViewModelStatus.Approved;
           //show the success receipt page
            return View("DepositResult", deResult);
        }

        //bpay task post 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBPay(BpayRequestDto request)
        {
            Guid guid = Guid.NewGuid();
            var TransactionID = guid.ToString();

            var payeeName = "blank";
            //get billernames in payees table
            foreach (var billername in _context.Payees)
            {
                if (billername.BillerCode == request.BillPayID)
                {
                    payeeName = billername.BillerName;
                }
            }
            //create a bpay result with request passed in 
            var bpayResult = new BpayRequestViewModel()
            {
                TransactionID = TransactionID,
                Amount = request.Amount,
                Description = "BPAY Paid to: " + payeeName,
                Date = DateTime.UtcNow,
                BillPayID = request.BillPayID,
            };

            //check for decimals greater than 3 places 
            String checkDec = request.Amount.ToString();
            if (checkDec.Contains("."))
            {
                string[] a = checkDec.Split(new char[] { '.' });

                int decimals = a[1].Length;

                //if >=3 decimals add error
                if (decimals >= 3)
                {
                    ModelState.AddModelError(string.Empty, "The amount entered cannot have more than 2 decimal places"); ;
                    return View("CreateBPAY");
                }
            }
            //0 amoutn add error
            if (request.Amount == 0)
            {
                ModelState.AddModelError(string.Empty, "The amount entered cannot by zero. Please try again");
                return View("CreateBPAY");
            }
            //greater than 10000 add error
            if(request.Amount > 10000)
            {
                ModelState.AddModelError(string.Empty, "The amount to pay must not be greater than $10 000. Please try again");
                return View("CreateBpay");
            }
            //less than 0 add error
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


            await _banker.BpayTransfer(sourceWallet, request.Amount, request.BillPayID, "BPAY Paid to: " + payeeName);

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
