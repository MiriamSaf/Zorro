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
    public class FriendingController : ControllerBase
    {
        public ActionResult Friending()
        {
            return RedirectToAction("Friending");
        }
    }
}
