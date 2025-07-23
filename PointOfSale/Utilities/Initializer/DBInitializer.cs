using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PointOfSale.Business.Contracts;
using PointOfSale.Data.DBContext;
using PointOfSale.Model;

namespace PointOfSale.Utilities.Initializer
{
    public class DBInitializer:IDBInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly POINTOFSALEContext _context;
        public DBInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, POINTOFSALEContext context)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;

        }

        public void Initialize()
        {

            try
            {

                if (_context.Database.GetPendingMigrations().Count() > 0)
                {

                    _context.Database.Migrate();
                }

            }
            catch
            {
                throw;
            }

            if (!_roleManager.RoleExistsAsync(SD.Cashier).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Cashier)).GetAwaiter().GetResult();
            }
            _userManager.CreateAsync(new ApplicationUser
            {
                 UserName = "xmontemorjerald@gmail.com",
                 Email = "xmontemorjerald@gmail.com",
                 Name = "Montemor, Jerald R.",
                //PhoneNumber = "09488749263",
                //Address = "Malabon City",

            }, "Admin123*").GetAwaiter().GetResult();


            ApplicationUser user = _context.AppUsers.FirstOrDefault(x => x.Email == "xmontemorjerald@gmail.com");
            _userManager.AddToRoleAsync(user, SD.Admin).GetAwaiter().GetResult();

            return;
        }

    }
}
