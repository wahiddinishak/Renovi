using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using renovi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace renovi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext db;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            db = context;
        }

        #region "Init"
        [Authorize(Roles = "1,2,3")]
        public IActionResult Index()
        {
            var data = db.proyeks.Select(a => a).ToList();

            ViewData["total"] = data.Where(a => a.isDelete == false).Count();
            ViewData["active"] = data.Where(a => a.isDelete == false && a.isActive == true).Count();
            ViewData["nonActive"] = data.Where(a => a.isDelete == false && a.isActive == false).Count();

            return View();
        }

        #endregion

        #region "GetProgress"
        public IActionResult getProgress()
        {

            var proyekActive = db.proyeks.Where(a => a.isActive == true).ToList();

            List<ProjectStatic> modelProjectsStatic = new List<ProjectStatic>();
            foreach (var proyek in proyekActive)
            {
                string period = string.Concat(DateTime.Now.ToString("MM"), "-", DateTime.Now.ToString("yyyy"));

                // Temp Data
                var _actH = db.actHs.Where(a => a.idProyek == proyek.idProyek).OrderBy(a => a.Id).ToList();
                var _actD = db.actDs.Where(a => a.idProyek == proyek.idProyek).OrderBy(a => a.Id).ToList();
                var _laporan = db.laporans.Where(a => a.idProyek == proyek.idProyek).OrderBy(a => a.Id).ToList();
                var _laporanUsage = db.laporanUsages.Where(a => a.idProyek == proyek.idProyek).OrderBy(a => a.Id).ToList();
                var _laporanOverheadProfit = db.laporanOverheadProfits.Where(a => a.idProyek == proyek.idProyek).OrderBy(a => a.Id).ToList();
                var _material = db.materials.Where(a => a.idProyek == proyek.idProyek).OrderBy(a => a.Id).ToList();
                var _analisa = (from a in db.analisas
                                join b in db.actDs on a.idActD equals b.Id
                                join c in db.materials on a.idMaterial equals c.Id
                                where b.idProyek == proyek.idProyek
                                select new analisa
                                {
                                    Id = a.Id,
                                    idActD = a.idActD,
                                    koefisien = a.koefisien,
                                    idMaterial = a.idMaterial
                                }).ToList();

                List<rekapDView> detail = new List<rekapDView>();
                List<rekapWipActDView> detailProgressPekerjaan = new List<rekapWipActDView>();

                detail = getRekapActHBaseAtcD(_actH, _actD, _analisa, _material, _laporan,_laporanUsage,_laporanOverheadProfit,period);
                detailProgressPekerjaan = getRekapWip(_actH, _actD, _analisa, _material, _laporan, period);

                modelProjectsStatic.Add(new ProjectStatic
                {
                    judul = proyek.judul,
                    progressBiaya = detail.Sum(a => a.progressBobotOverall).ToString("n2"),
                    progressPekerjaan = detailProgressPekerjaan.Sum(a => a.wip).ToString("n2")
                });
            }

            return Json(new
            {
                data = modelProjectsStatic
            });
        }

        public List<rekapDView> getRekapActHBaseAtcD(List<ActH> actH, List<ActD> actD,
            List<analisa> analisa, List<Material> material, List<Laporan> laporan,
            List<LaporanUsage> laporanUsages, List<LaporanOverheadProfit> laporanOverheadProfits,
            string period)
        {
            int x = 1;
            List<rekapDView> model = new List<rekapDView>();
            foreach (var data in actH)
            {
                model.Add(new rekapDView
                {
                    no = x,
                    progressBobotOverall = calcBobot(calcProgressNilaiOverallActH(laporan, laporanUsages, laporanOverheadProfits, data.Id,period),
                                                     calcHargaRekapPerIdProyek(actH, actD, analisa, material))

                });

                x++;
            }
            return model;
        }

        public double calcBobot(double harga, double totalHarga)
        {
            return harga / totalHarga * 100;
        }

        public double calcProgressNilaiOverallActH(List<Laporan> laporan, List<LaporanUsage> laporanUsages,
            List<LaporanOverheadProfit> laporanOverheadProfits, int idActH, string period)
        {

            string[] periodx = period.Split('-');
            var EoD = DateTime.DaysInMonth(Convert.ToInt32(periodx[1]), Convert.ToInt32(periodx[0]));
            DateTime periodxx = DateTime.Parse(string.Concat(periodx[1].ToString(), "-", periodx[0].ToString(), "-", EoD.ToString()));

            // laporan header id with ActH
            var laporans = from a in laporan
                           where a.idActH == idActH
                           && a.reportDate.Date <= periodxx.Date
                           select new
                           {
                               a.Id
                           };
            int[] headers = laporans.Select(a => a.Id).ToArray();

            var usage = laporanUsages.Where(a => headers.Contains(a.header)).ToList();
            var overhead = laporanOverheadProfits.Where(a => headers.Contains(a.header)).ToList();

            return usage.Sum(a => a.amount) + overhead.Sum(a => a.amount);
        }

        public double calcHargaRekapPerIdProyek(List<ActH> actH, List<ActD> actD, List<analisa> analisa, List<Material> material)
        {
            double result = 0;
            foreach (var header in actH)
            {
                foreach (var detail in actD.Where(a => a.header == header.Id).ToList())
                {
                    var xdata = (from a in analisa
                                 join b in material on a.idMaterial equals b.Id
                                 where a.idActD == detail.Id
                                 select new
                                 {
                                     harga = b.harga,
                                     koefisien = a.koefisien
                                 }).ToList();

                    double harga = xdata.Sum(a => a.harga * a.koefisien);
                    double hargaProfit = xdata.Sum(a => a.harga * a.koefisien) * detail.profit / 100;

                    result += (harga + hargaProfit) * detail.volume;
                }
            }
            return result;
        }

        public List<rekapWipActDView> getRekapWip(List<ActH> actH, List<ActD> actD,
            List<analisa> analisa, List<Material> material, List<Laporan> laporan, string period)
        {
            string[] periodx = period.Split('-');
            var EoD = DateTime.DaysInMonth(Convert.ToInt32(periodx[1]), Convert.ToInt32(periodx[0]));
            DateTime periodxx = DateTime.Parse(string.Concat(periodx[1].ToString(), "-", periodx[0].ToString(), "-", EoD.ToString()));

            List<rekapWipActDView> model = new List<rekapWipActDView>();

            foreach (var detail in actD)
            {
                model.Add(new rekapWipActDView
                {
                    wip = WipData(laporan, detail.Id, calcHargaRekapPerActD(actD, analisa, material, detail.Id), 
                                calcHargaRekapPerIdProyek(actH, actD, analisa, material), period)
                });
            }
            return model;
        }


        public double WipData(List<Laporan> laporans, int idActD, double jumlahHarga, double total, string period)
        {
            string[] periodx = period.Split('-');
            var EoD = DateTime.DaysInMonth(Convert.ToInt32(periodx[1]), Convert.ToInt32(periodx[0]));
            DateTime periodxx = DateTime.Parse(string.Concat(periodx[1].ToString(), "-", periodx[0].ToString(), "-", EoD.ToString()));

            double calc = 0;
            var wip = laporans.Where(a => a.idActD == idActD && a.reportDate <= periodxx.Date).OrderByDescending(a => a.reportDate).ToList();
            if (wip.Count() > 0)
            {
                double pengali = wip.Select(a => a.wip).FirstOrDefault();
                calc = jumlahHarga / total * pengali;
            }
            return calc;
        }

       
        public double calcHargaRekapPerActD(List<ActD> actDs, List<analisa> analisas, List<Material> materials, int idActD)
        {
            double result = 0;
            var ActD = actDs.Where(a => a.Id == idActD).ToList();
            foreach (var data in ActD)
            {
                var xdata = (from a in analisas
                             join b in materials on a.idMaterial equals b.Id
                             where a.idActD == data.Id
                             select new
                             {
                                 harga = b.harga,
                                 koefisien = a.koefisien
                             }).ToList();

                double harga = xdata.Sum(a => a.harga * a.koefisien);
                double hargaProfit = xdata.Sum(a => a.harga * a.koefisien) * data.profit / 100;

                result += (harga + hargaProfit) * data.volume;
            }
            return result;
        }
        #endregion

        #region "Auth"
        [AllowAnonymous]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Login(user data, string returnUrl)
        {
            var users = db.Users.Where(a => a.username == data.username && a.password == data.password).FirstOrDefault();
            if (users != null)
            {
                var userClaim = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, users.fullname),
                    new Claim(ClaimTypes.Role, users.role.ToString()),
                    new Claim(ClaimTypes.Sid, users.id.ToString())
                };

                var userIdentity = new ClaimsIdentity(userClaim, "User Identity");
                var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });
                HttpContext.SignInAsync(userPrincipal);
                return Redirect(returnUrl);
            }
            else
            {
                ModelState.AddModelError("Error", "Username or Password incorrect.");
                return View(data);
            }
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

    }
}
