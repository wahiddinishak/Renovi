using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using renovi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace renovi.Controllers
{
    public class AbsenController : Controller
    {
        private readonly AppDbContext db;
        private readonly IHttpContextAccessor httpConn;
        private readonly IWebHostEnvironment hostingEnv;
        public AbsenController(AppDbContext context, IHttpContextAccessor httpCon, IWebHostEnvironment env)
        {
            db = context;
            httpConn = httpCon;
            hostingEnv = env;
        }

        [Authorize(Roles = "1,2,3")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "1,2,3")]
        public IActionResult AbsenList(int id)
        {
            var proyek = db.proyeks.FirstOrDefault(a => a.Id == id);
            ViewData["namaProyek"] = proyek.judul;
            ViewData["idProyek"] = proyek.idProyek;
            ViewData["id"] = proyek.Id;

            return View();
        }


        public IActionResult frmPayroll(string idProyek)
        {
            PayrollParam model = new PayrollParam();
            model.start = DateTime.Now;
            model.end = DateTime.Now;
            model.idProyek = idProyek;
            model.judul = db.proyeks.FirstOrDefault(a => a.idProyek == idProyek).judul;

            return PartialView("frmPayroll", model);
        }

        public IActionResult generatePayroll(PayrollParam data)
        {
            ViewData["Judul"] = data.judul;
            ViewData["Periode"] = string.Concat(data.start.Date.ToString("dd MMM yyyy"), " s/d ", data.end.Date.ToString("dd MMM yyyy"));


            // Id Header Population
            var absen = from a in db.absens
                        where a.idProyek == data.idProyek
                        && (a.periode.Date >= data.start.Date && a.periode.Date <= data.end.Date)
                        select new
                        {
                            a.Id
                        };

            int[] headers = absen.Select(a => a.Id).ToArray();

            // absenDetail contains
            var absenDetail = db.absenDetails.Where(a => headers.Contains(a.header));

            // calc Madays
            var calcMandays = from a in absenDetail
                              group a by a.idPersonil into g
                              select new
                              {
                                  idPersonel = g.Key,
                                  Mandays = g.Sum(a => a.mandays)
                              };

            // join with personil
            var personil = from a in db.personils
                           join b in calcMandays on a.Id equals b.idPersonel
                           join c in db.materials on a.role equals c.Id
                           where a.idProyek == data.idProyek && c.jenis == "J"
                           select new PayrollView
                           {
                               nama = a.nama,
                               telepon = a.telepon,
                               role = c.item,
                               rekening = a.akunBank,
                               rate = c.harga,
                               mandays = b.Mandays,
                               amount = b.Mandays * c.harga
                           };

            listPayroll model = new listPayroll();
            model.data = personil.ToList();
            return PartialView("PayrollInfo", model);
        }

        public IActionResult frmAbsen(string idProyek, int idAbsen)
        {
            var data = db.absens.FirstOrDefault(a => a.idProyek == idProyek && a.Id == idAbsen);
            AbsenView model = new AbsenView();
            model.id = data != null ? data.Id : 0;
            model.periode = data != null ? data.periode : DateTime.Now;
            model.idProyek = idProyek;

            var detail = from a in db.personils
                         join z in db.materials on a.role equals z.Id
                         join b in db.absenDetails on new { x1 = a.Id, x2 = model.id } equals new { x1 = b.idPersonil, x2 = b.header } into gp
                         from x in gp.DefaultIfEmpty()
                         where a.idProyek == idProyek
                         select new AbsenDetailView
                            {
                                idPersonil = a.Id,
                                nama = a.nama,
                                role = z.item,
                                mandays = x.mandays
                            };

            model.detail = detail.ToList();
            return PartialView("frmAbsen", model);
        }

        public IActionResult checkAbsen(string idProyek, DateTime period)
        {
            string strDate = period.ToString("yyyy-MM-dd");
            DateTime paramDate = Convert.ToDateTime(strDate);
            var data = db.absens.FirstOrDefault(a => a.idProyek == idProyek && a.periode.Date == paramDate);
            if (data != null)
            {
                return Content("NotOk");
            }
            else
            {
                return Content("Ok");
            }
        }

        public async Task<IActionResult> submitAbsen(AbsenView data)
        {
            try
            {
                int idAbsen = 0;
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                var dataAbsen = JsonConvert.DeserializeObject<List<AbsenParam>>(data.strDetails);
               
                if (data.id != 0)
                {
                    var model = db.absens.FirstOrDefault(a => a.Id ==  data.id);
                    model.periode = data.periode;
                    model.idProyek = data.idProyek;
                    model.UpdateBy = user;
                    model.UpdateDate = DateTime.Now;
                    db.absens.Update(model);
                    await db.SaveChangesAsync();

                    idAbsen = model.Id;
                }
                else
                {
                    Absen model = new Absen();
                    model.periode = data.periode;
                    model.idProyek = data.idProyek;
                    model.CreateBy = user;
                    model.CreateDate = DateTime.Now;
                    db.absens.Add(model);
                    await db.SaveChangesAsync();

                    idAbsen = model.Id;
                }

                // drop detail
                db.absenDetails.RemoveRange(db.absenDetails.Where(a => a.header == idAbsen));
                await db.SaveChangesAsync();

                // insert
                foreach (var da in dataAbsen)
                {
                    AbsenDetail model = new AbsenDetail();
                    model.header = idAbsen;
                    model.idPersonil = (int)Convert.ToInt64(da.idPersonil);
                    model.mandays = (double)Convert.ToDouble(da.Mandays);
                    model.CreateBy = user;
                    model.CreateDate = DateTime.Now;
                    db.absenDetails.Add(model);
                }

                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        public async Task<IActionResult> deleteAbsen(int id)
        {
            try
            {
                db.absens.Remove(db.absens.Where(a => a.Id == id).FirstOrDefault());
                db.absenDetails.RemoveRange(db.absenDetails.Where(a => a.header == id));

                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        public IActionResult getProyek()
        {
            int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
            string roleId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            ViewData["RoleId"] = roleId;
            listProyek model = new listProyek();
            // cek jika mandor, hanya proyek atas mandor itu saja
            // cek jika pemilik proyek, hanya proyek yang dia miliki saja
            // jika owner bisa liat semua proyek
            if (roleId == "1" || roleId == "3")
            {
                var data = from a in db.proyeks
                           join b in db.Users on a.mandor equals b.id
                           where b.role == 2 && a.isDelete == false
                           orderby a.Id descending
                           select new ProyekView
                           {
                               Id = a.Id,
                               idProyek = a.idProyek,
                               judul = a.judul,
                               Alamat = a.alamat,
                               mandor = b.fullname,
                               tglMulai = a.tglMulai,
                               tglSelesai = a.tglSelesai,
                               kontrak = a.kontrak,
                               desain = a.desain,
                               isActive = a.isActive,
                               namaKlien = a.namaKlien
                           };

                model.data = data.ToList();
            }
            else
            {
                var data = from a in db.proyeks
                           join b in db.Users on a.mandor equals b.id
                           where b.role == 2 && a.mandor == user && a.isActive == true && a.isDelete == false
                           orderby a.Id descending
                           select new ProyekView
                           {
                               Id = a.Id,
                               idProyek = a.idProyek,
                               judul = a.judul,
                               Alamat = a.alamat,
                               mandor = b.fullname,
                               tglMulai = a.tglMulai,
                               tglSelesai = a.tglSelesai,
                               kontrak = a.kontrak,
                               desain = a.desain,
                               isActive = a.isActive,
                               namaKlien = a.namaKlien
                           };

                model.data = data.ToList();
            }
            return PartialView("proyekInfo", model);
        }

        [HttpPost]
        public IActionResult getAbsens(string idProyek)
        {
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var data = db.absens.Where(a => a.idProyek == idProyek).Select(a => new { a.Id, a.periode}).OrderByDescending(a => a.periode);

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(b => b.periode.ToString().Contains(searchValue)).OrderByDescending(a => a.Id);
            }

            return Json(new
            {
                draw = Convert.ToInt32(HttpContext.Request.Form["draw"].FirstOrDefault()),
                recordsFiltered = data.Count(),
                recordsTotal = data.Count(),
                data = data.Skip(skip).Take(pageSize).ToArray()
            });
        }


    }
}
