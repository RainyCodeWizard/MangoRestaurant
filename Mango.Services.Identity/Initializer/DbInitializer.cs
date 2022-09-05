using IdentityModel;
using Mango.Services.Identity.DbContext;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;

namespace Mango.Services.Identity.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(Constants.Admin).Result == null) // If Role does not exist, then probably running this for the first time
            {
                _roleManager.CreateAsync(new IdentityRole(Constants.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Constants.Customer)).GetAwaiter().GetResult();
            }
            else { return; }

            ApplicationUser adminUser = new()
            {
                UserName = "admin1",
                Email = "admin1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "111111111111",
                FirstName = "Ben",
                LastName = "Admin"
            };

            _userManager.CreateAsync(adminUser, "Abc123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, Constants.Admin).GetAwaiter().GetResult();

            var temp1 = _userManager.AddClaimsAsync(adminUser, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, adminUser.FirstName + ' ' + adminUser.LastName),
                new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
                new Claim(JwtClaimTypes.Role, Constants.Admin),
            }).Result;

            ApplicationUser customerUser = new()
            {
                UserName = "customer1",
                Email = "customer1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "111111111111",
                FirstName = "Ben",
                LastName = "Cust"
            };

            _userManager.CreateAsync(customerUser, "Abc123!").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customerUser, Constants.Customer).GetAwaiter().GetResult();

            var temp2 = _userManager.AddClaimsAsync(customerUser, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, customerUser.FirstName + ' ' + customerUser.LastName),
                new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
                new Claim(JwtClaimTypes.Role, Constants.Customer),
            }).Result;
        }
    }
}
