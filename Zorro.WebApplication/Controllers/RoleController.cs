using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zorro.WebApplication.ViewModels;

namespace Zorro.WebApplication.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleController> _logger;

        public RoleController(RoleManager<IdentityRole> roleManager, ILogger<RoleController> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public ActionResult Index()
        {
            var roles = new List<RoleViewModel>();
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

        // POST: RoleController/Create
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
