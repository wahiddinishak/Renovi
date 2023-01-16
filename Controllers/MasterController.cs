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
    public class MasterController : Controller
    {

        private readonly AppDbContext db;
        public MasterController(AppDbContext context)
        {
            db = context;
        }

        #region "Items"
        [Authorize(Roles = "1")]
        public IActionResult Item()
        {
            return View();
        }

        [HttpPost]
        public IActionResult getItems()
        {
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var data = db.Items.Select(a => a).OrderByDescending(a => a.Id);

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(b => b.nama.Contains(searchValue) || b.uom.Contains(searchValue)).OrderBy(a => a.Id);
            }

            return Json(new
            {
                draw = Convert.ToInt32(HttpContext.Request.Form["draw"].FirstOrDefault()),
                recordsFiltered = data.Count(),
                recordsTotal = data.Count(),
                data = data.Skip(skip).Take(pageSize).ToArray()
            });
        }

        public IActionResult modalItems(int id)
        {
            item model = new item();
            if (id != 0)
            {
                var data = db.Items.FirstOrDefault(a => a.Id == id);
                model.Id = data.Id;
                model.nama = data.nama;
                model.uom = data.uom;
                model.harga = data.harga;
            }
            return PartialView("modalItems", model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> submitItems(item data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);

                    if (data.Id != 0)
                    {
                        var model = db.Items.FirstOrDefault(a => a.Id == data.Id);
                        model.nama = data.nama;
                        model.uom = data.uom;
                        model.harga = data.harga;
                        model.UpdateBy = user;
                        model.UpdateDate = DateTime.Now;

                        db.Items.Update(model);
                    }
                    else
                    {
                        item model = new item();
                        model.nama = data.nama;
                        model.uom = data.uom;
                        model.harga = data.harga;
                        model.CreateBy = user;
                        model.CreateDate = DateTime.Now;

                        db.Items.Add(model);
                    }

                    await db.SaveChangesAsync();
                    return Content("Ok");
                }
                catch (Exception ex)
                {
                    return Content(ex.ToString());
                }
            }
            return View(data);
        }
        public async Task<IActionResult> hapusItem(int id)
        {
            try
            {
                var data = await db.Items.FindAsync(id);
                db.Items.Remove(data);
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        #region "Tukang"
        [Authorize(Roles = "1")]
        public IActionResult Tukang()
        {
            return View();
        }

        [HttpPost]
        public IActionResult getTukangs()
        {
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var data = db.Tukangs.Select(a => a).OrderByDescending(a => a.Id);

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(b => b.nama.Contains(searchValue) || b.telepon.Contains(searchValue)).OrderBy(a => a.Id);
            }

            return Json(new
            {
                draw = Convert.ToInt32(HttpContext.Request.Form["draw"].FirstOrDefault()),
                recordsFiltered = data.Count(),
                recordsTotal = data.Count(),
                data = data.Skip(skip).Take(pageSize).ToArray()
            });
        }

        public IActionResult modalTukang(int id)
        {
            tukang model = new tukang();
            if (id != 0)
            {
                var data = db.Tukangs.FirstOrDefault(a => a.Id == id);
                model.Id = data.Id;
                model.nama = data.nama;
                model.telepon = data.telepon;
                model.namaBank = data.namaBank;
                model.rekening = data.rekening;
            }
            return PartialView("modalTukang", model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> submitTukang(tukang data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);

                    if (data.Id != 0)
                    {
                        var model = db.Tukangs.FirstOrDefault(a => a.Id == data.Id);
                        model.Id = data.Id;
                        model.nama = data.nama;
                        model.telepon = data.telepon;
                        model.namaBank = data.namaBank;
                        model.rekening = data.rekening;
                        model.UpdateBy = user;
                        model.UpdateDate = DateTime.Now;

                        db.Tukangs.Update(model);
                    }
                    else
                    {
                        tukang model = new tukang();
                        model.Id = data.Id;
                        model.nama = data.nama;
                        model.telepon = data.telepon;
                        model.namaBank = data.namaBank;
                        model.rekening = data.rekening;
                        model.CreateBy = user;
                        model.CreateDate = DateTime.Now;

                        db.Tukangs.Add(model);
                    }

                    await db.SaveChangesAsync();
                    return Content("Ok");
                }
                catch (Exception ex)
                {
                    return Content(ex.ToString());
                }
            }
            return View(data);
        }
        public async Task<IActionResult> hapusTukang(int id)
        {
            try
            {
                var data = await db.Tukangs.FindAsync(id);
                db.Tukangs.Remove(data);
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        #region "Users"
        [Authorize(Roles = "1,3")]
        public IActionResult Users()
        {
            return View();
        }

        [HttpPost]
        public IActionResult getUsers()
        {
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var data = (from a in db.Users
                        join c in db.Roles on a.role equals c.Id
                        orderby a.id descending
                        select new
                        {
                            a.id,
                            a.fullname,
                            c.roleName
                        });

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(b => b.fullname.Contains(searchValue)
                || Convert.ToString(b.roleName).Contains(searchValue)).OrderBy(a => a.id);
            }

            return Json(new
            {
                draw = Convert.ToInt32(HttpContext.Request.Form["draw"].FirstOrDefault()),
                recordsFiltered = data.Count(),
                recordsTotal = data.Count(),
                data = data.Skip(skip).Take(pageSize).ToArray()
            });
        }

        public IActionResult modalUsers(int id)
        {
            user model = new user();
            if (id != 0)
            {
                var data = db.Users.FirstOrDefault(a => a.id == id);
                model.id = data.id;
                model.username = data.username;
                model.fullname = data.fullname;
                model.password = data.password;
                model.role = data.role;
            }
            return PartialView("modalUsers", model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> submitUsers(user data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                    if (data.id != 0)
                    {
                        var model = db.Users.FirstOrDefault(a => a.id == data.id);
                        model.username = data.username;
                        model.fullname = data.fullname;
                        model.password = data.password;
                        model.role = data.role;
                        model.isActive = true;
                        db.Users.Update(model);
                    }
                    else
                    {
                        //checked if exists
                        var check = db.Users.Where(a => a.username == data.username);
                        if (check.Count() > 0)
                        {
                            return Content("User name already exists, please use another.");
                        }

                        user model = new user();
                        model.username = data.username;
                        model.fullname = data.fullname;
                        model.password = data.password;
                        model.role = data.role;
                        model.isActive = true;
                        db.Users.Add(model);
                    }
                    await db.SaveChangesAsync();
                    return Content("Ok");

                }
                catch (Exception ex)
                {
                    return Content(ex.ToString());
                }
            }
            return View(data);
        }
        public async Task<IActionResult> hapusUser(int id)
        {
            try
            {
                var data = await db.Users.FindAsync(id);
                db.Users.Remove(data);
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }


}
