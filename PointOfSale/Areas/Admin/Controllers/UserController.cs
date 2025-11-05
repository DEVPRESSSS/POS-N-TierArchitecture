using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PointOfSale.Model;
using PointOfSale.Models;
using PointOfSale.Utilities;
using PointOfSale.Utilities.Response;

namespace PointOfSale.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin)]
    public class UserController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public IActionResult Users()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles
                .Where(x=> x.Name != "Admin")
                .Select(r => new { idRol = r.Id, description = r.Name })
                .ToListAsync();

            return Json(roles);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userManager.Users.
                    Where(x=>x.Email!= "xmontemorjerald@gmail.com").
                    ToListAsync();
                var vmUsers = new List<object>();

                foreach (var user in users)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var roleName = userRoles.FirstOrDefault() ?? "No Role";
                    var roleId = "";

                    if (userRoles.Any())
                    {
                        var role = await _roleManager.FindByNameAsync(userRoles.First());
                        roleId = role?.Id ?? "";
                    }

                    // Get photo as base64 if exists
                    string photoBase64 = "";
                    if (!string.IsNullOrEmpty(user.ProfilePath))
                    {
                        try
                        {
                            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePath.TrimStart('/'));
                            if (System.IO.File.Exists(fullPath))
                            {
                                byte[] imageBytes = System.IO.File.ReadAllBytes(fullPath);
                                photoBase64 = Convert.ToBase64String(imageBytes);
                            }
                        }
                        catch
                        {
                            // If photo can't be read, use empty string
                            photoBase64 = "";
                        }
                    }

                    vmUsers.Add(new
                    {
                        idUsers = user.Id ?? "",
                        name = user.Name ?? "",
                        email = user.Email ?? "",
                        phone = user.PhoneNumber ?? "",
                        nameRol = roleName,
                        idRol = roleId,
                        isActive = user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow ? 1 : 0,
                        photo = user.ProfilePath ?? "",
                        photoBase64 = photoBase64
                    });
                }

                // Return data in the format DataTables expects
                return Json(new { data = vmUsers });
            }
            catch (Exception ex)
            {
                // Return empty data array in case of error, in the format DataTables expects
                return Json(new { data = new List<object>(), error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new List<string>());
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Json(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] IFormFile photo, [FromForm] string model)
        {
            try
            {
                var userModel = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(model);

                string profilePath = null;

                if (photo != null && photo.Length > 0)
                {
                    string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                    string filePath = Path.Combine(uploadsPath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    profilePath = $"/uploads/{fileName}";
                }

                // Create ApplicationUser
                var applicationUser = new ApplicationUser
                {
                    UserName = userModel["email"]?.ToString(),
                    Email = userModel["email"]?.ToString(),
                    Name = userModel["name"]?.ToString(),
                    PhoneNumber = userModel["phone"]?.ToString(),
                    ProfilePath = profilePath,
                    EmailConfirmed = true
                };

                // Create user with password
                var password = userModel["password"]?.ToString() ?? "DefaultPassword123!";
                var result = await _userManager.CreateAsync(applicationUser, password);

                if (result.Succeeded)
                {
                    // Assign role if specified
                    var roleId = userModel["idRol"]?.ToString();
                    if (!string.IsNullOrEmpty(roleId))
                    {
                        var role = await _roleManager.FindByIdAsync(roleId);
                        if (role != null)
                        {
                            await _userManager.AddToRoleAsync(applicationUser, role.Name);
                        }
                    }

                    // Set user active/inactive status
                    var isActive = userModel["isActive"]?.ToString();
                    if (isActive == "0")
                    {
                        await _userManager.SetLockoutEndDateAsync(applicationUser, DateTimeOffset.MaxValue);
                    }

                    // Get role name for response
                    var userRoles = await _userManager.GetRolesAsync(applicationUser);
                    var roleName = userRoles.FirstOrDefault() ?? "No Role";

                    var responseUser = new
                    {
                        idUsers = applicationUser.Id,
                        name = applicationUser.Name,
                        email = applicationUser.Email,
                        phone = applicationUser.PhoneNumber,
                        nameRol = roleName,
                        idRol = roleId,
                        isActive = isActive == "0" ? 0 : 1,
                        photo = applicationUser.ProfilePath
                    };

                    return Json(new { state = true, message = "User created successfully", @object = responseUser });
                }

                // Return validation errors from Identity
                var errors = result.Errors?.Any() == true
                    ? string.Join(", ", result.Errors.Select(e => e.Description ?? e.Code ?? "Unknown error"))
                    : "User creation failed";
                return Json(new { state = false, message = errors });
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = "Name, contact or email are already taken"});
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromForm] IFormFile photo, [FromForm] string model)
        {
            try
            {
                // Parse the JSON into a JObject to avoid dynamic issues
                var userModel = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(model);

                var userId = userModel["idUsers"]?.ToString();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { state = false, message = "User not found" });
                }

                if (photo != null && photo.Length > 0)
                {
                    if (!string.IsNullOrEmpty(user.ProfilePath))
                    {
                        string oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPhotoPath))
                        {
                            System.IO.File.Delete(oldPhotoPath);
                        }
                    }

                    string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                    string filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    user.ProfilePath = $"/uploads/{fileName}";
                }

                // Update user properties
                user.Name = userModel["name"]?.ToString();
                user.Email = userModel["email"]?.ToString();
                user.UserName = userModel["email"]?.ToString();
                user.PhoneNumber = userModel["phone"]?.ToString();

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors?.Any() == true
                        ? string.Join(", ", updateResult.Errors.Select(e => e.Description ?? e.Code ?? "Unknown error"))
                        : "User update failed";
                    return Json(new { state = false, message = errors });
                }

                // Update password if provided
                var password = userModel["password"]?.ToString();
                if (!string.IsNullOrEmpty(password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, password);
                    if (!passwordResult.Succeeded)
                    {
                        var errors = passwordResult.Errors?.Any() == true
                            ? string.Join(", ", passwordResult.Errors.Select(e => e.Description ?? e.Code ?? "Unknown error"))
                            : "Password change failed";
                        return Json(new { state = false, message = "User updated but password change failed: " + errors });
                    }
                }

                // Update role
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                var roleId = userModel["idRol"]?.ToString();
                if (!string.IsNullOrEmpty(roleId))
                {
                    var role = await _roleManager.FindByIdAsync(roleId);
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }

                // Update active/inactive status
                var isActive = userModel["isActive"]?.ToString();
                if (isActive == "0")
                {
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                }
                else
                {
                    await _userManager.SetLockoutEndDateAsync(user, null);
                }

                // Get updated role name for response
                var updatedRoles = await _userManager.GetRolesAsync(user);
                var roleName = updatedRoles.FirstOrDefault() ?? "No Role";

                var responseUser = new
                {
                    idUsers = user.Id,
                    name = user.Name,
                    email = user.Email,
                    phone = user.PhoneNumber,
                    nameRol = roleName,
                    idRol = roleId,
                    isActive = isActive == "0" ? 0 : 1,
                    photo = user.ProfilePath
                };

                return Json(new { state = true, message = "User updated successfully", @object = responseUser });
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = "Name, contact or email are already taken" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string IdUser)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(IdUser);
                if (user == null)
                {
                    return Json(new { state = false, message = "User not found" });
                }

                if (!string.IsNullOrEmpty(user.ProfilePath))
                {
                    string photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePath.TrimStart('/'));
                    Console.WriteLine($"{photoPath}");
                    if (System.IO.File.Exists(photoPath))
                    {
                        System.IO.File.Delete(photoPath);
                    }
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Json(new { state = true, message = "User deleted successfully" });
                }

                var errors = result.Errors?.Any() == true
                    ? string.Join(", ", result.Errors.Select(e => e.Description ?? e.Code ?? "Unknown error"))
                    : "User deletion failed";
                return Json(new { state = false, message = errors });
            }
            catch (Exception ex)
            {
                return Json(new { state = false, message = "Cannot delete user because there are existing sales linked to this account."});
            }
        }
    }
}