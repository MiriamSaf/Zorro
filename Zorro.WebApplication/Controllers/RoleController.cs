using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zorro.WebApplication.ViewModels;

namespace Zorro.WebApplication.Controllers
{
    //roles controller - deals with the user roles in the system 
    //only admin can view these pages
    [Authorize(Roles = "Administrator")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleController> _logger;

        //DI for role controller
        public RoleController(RoleManager<IdentityRole> roleManager, ILogger<RoleController> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        //returns index view for roles
        public ActionResult Index()
        {
            var roles = new List<RoleViewModel>();
            //shows the role name and shows it to the view
            foreach (var role in _roleManager.Roles)
            {
                roles.Add(new RoleViewModel() { Name = role.Name });
            }
            return View(roles);
        }

        // GET: RoleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoleController/Create - used for creating role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            var role = await _roleManager.FindByNameAsync(roleViewModel.Name);
            if (role is not null)
            {
                _logger.LogWarning("Role {roleName} already exists", roleViewModel.Name);
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleViewModel.Name));
            if (!result.Succeeded)   
            {
                _logger.LogWarning("Unable to create {roleName} role", roleViewModel.Name);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
