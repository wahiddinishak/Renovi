using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using renovi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace renovi.Controllers
{
    public class ProfileController : Controller
    {

        private readonly AppDbContext db;

        public ProfileController(AppDbContext context)
        {
            db = context;
        }

        [Authorize(Roles = "1,2,3")]
        public IActionResult Index()
        {
            int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
            userView model = new userView();
            var data = db.Users.FirstOrDefault(a => a.id == user);
            model.id = data.id;
            model.username = data.username;
            model.fullname = data.fullname;
            model.password = data.password;
            model.email = data.email;
            model.role = db.Roles.FirstOrDefault(a => a.Id == data.role).roleName;

            return View(model);
        }

        public async Task<IActionResult> submitProfile(userView param)
        {
            try
            {
                var data = db.Users.FirstOrDefault(a => a.id == param.id);
                data.fullname = param.fullname;
                data.password = string.IsNullOrEmpty(param.password) ? data.password : param.password;
                data.email = param.email;
                db.Users.Update(data);
                await db.SaveChangesAsync();

                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }
    }
}
