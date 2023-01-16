using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using Newtonsoft.Json;
using renovi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace renovi.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppDbContext db;
        private readonly IHttpContextAccessor httpConn;
        private readonly IWebHostEnvironment hostingEnv;
        private readonly IEmailService mail;

        public ReportController(AppDbContext context, IHttpContextAccessor httpCon, IWebHostEnvironment env, IEmailService mailService)
        {
            db = context;
            httpConn = httpCon;
            hostingEnv = env;
            mail = mailService;
        }

        #region "init"

        [Authorize(Roles = "1,2,3")]
        public IActionResult Index()
        {
            return View();
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

        [Authorize(Roles = "1,2,3")]
        public IActionResult ReportList(int id)
        {
            var proyek = db.proyeks.FirstOrDefault(a => a.Id == id);
            ViewData["namaProyek"] = proyek.judul;
            ViewData["idProyek"] = proyek.idProyek;
            ViewData["id"] = proyek.Id;
            return View();
        }

        public async Task<IActionResult> deleteReport(int id)
        {
            try
            {
                db.laporans.Remove(db.laporans.Where(a => a.Id == id).FirstOrDefault());
                db.laporanUsages.RemoveRange(db.laporanUsages.Where(a => a.header == id));
                db.laporanOverheadProfits.RemoveRange(db.laporanOverheadProfits.Where(a => a.header == id));

                var attchment = db.laporanAttachments.Where(a => a.header == id).ToList();
                foreach(var file in attchment)
                {
                    var filePath = Path.Combine(hostingEnv.WebRootPath, string.Concat("file/", file.filename));
                    System.IO.File.Delete(filePath);
                    db.laporanAttachments.Remove(file);
                }

                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }
        #endregion

        #region "Report Base on Proyek"
        public IActionResult getReport(string idProyek)
        {
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var data = from a in db.laporans
                       join b in db.actDs on a.idActD equals b.Id
                       join c in db.actHs on b.header equals c.Id
                       where a.idProyek == idProyek
                       orderby a.reportDate descending
                       select new
                       {
                           a.Id,
                           a.reportDate,
                           pekerjaan = c.kegiatan,
                           subPekerjaan = b.kegiatan
                       };

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(b => b.pekerjaan.Contains(searchValue) || b.subPekerjaan.Contains(searchValue)).OrderBy(a => a.Id);
            }

            return Json(new
            {
                draw = Convert.ToInt32(HttpContext.Request.Form["draw"].FirstOrDefault()),
                recordsFiltered = data.Count(),
                recordsTotal = data.Count(),
                data = data.Skip(skip).Take(pageSize).ToArray()
            });
        }

        public IActionResult frmReport(string idProyek, int idReport)
        {
            var data = from a in db.laporans
                       where a.idProyek == idProyek && a.Id == idReport
                       select new
                       {
                           a.Id,
                           a.reportDate,
                           a.idActH,
                           a.idActD,
                           a.note
                       };

            LaporanView model = new LaporanView();
            model.id = data.Count() > 0 ? data.Select(a => a.Id).FirstOrDefault() : 0;
            model.reportDate = data.Count() > 0 ? data.Select(a => a.reportDate).FirstOrDefault() : DateTime.Now;
            model.idActH = data.Count() > 0 ? data.Select(a => a.idActH).FirstOrDefault() : 0;
            model.idActD = data.Count() > 0 ? data.Select(a => a.idActD).FirstOrDefault() : 0;
            model.note = data.Count() > 0 ? data.Select(a => a.note).FirstOrDefault() : "";
            model.idProyek = idProyek;

            return PartialView("frmReport", model);
        }

        public IActionResult getActH(string idProyek)
        {
            var data = from a in db.actHs
                       where a.idProyek == idProyek
                       select new
                       {
                           id = a.Id,
                           text = a.kegiatan                           
                       };
            return Json(data.ToList());
        }

        public IActionResult getActD(string idProyek, int idActH)
        {
            var data = from a in db.actDs
                       where a.idProyek == idProyek && a.header == idActH
                       select new
                       {
                           id = a.Id,
                           text = a.kegiatan                           
                       };
            return Json(data.ToList());
        }

        public IActionResult checkReport(DateTime reportDate, int idActD)
        {
            string strDate = reportDate.ToString("yyyy-MM-dd");
            DateTime paramDate = Convert.ToDateTime(strDate);

            var data = db.laporans.Where(a => a.reportDate.Date == paramDate.Date
                        && a.idActD == idActD).FirstOrDefault();

            if (data != null)
            {
                return Content("NotOk");
            }
            else
            {
                return Content("Ok");
            }
        }
        #endregion

        #region "Penggunaan"
        public IActionResult Penggunaan(int idActD, int idReport)
        {
            listLaporanUsage list = new listLaporanUsage();
            List<LaporanUsageView> model = new List<LaporanUsageView>();

            var data = from a in db.analisas
                       join b in db.actDs on a.idActD equals b.Id
                       join c in db.materials on a.idMaterial equals c.Id
                       join d in db.laporanUsages on new { x1 = c.Id, x2 = idReport } equals new { x1 = d.idMaterial, x2 = d.header } into gp
                       from x in gp.DefaultIfEmpty()
                       where a.idActD == idActD
                       select new LaporanUsageView
                       {
                           idMaterial = a.idMaterial,
                           materialInfo = string.Concat(c.item, " @ ", c.harga.ToString("n2"), "/", c.uom),
                           materialBudget = string.Concat("<ul><li><b>Kuantitas: </b>", (a.koefisien * b.volume).ToString("n2"), " ", c.uom,
                                                "</li><li><b>Biaya: </b>", ((a.koefisien * c.harga) * b.volume).ToString("n2"),
                                                "</li></ul>"),
                           idActD = b.Id,
                           uom = c.uom,
                           qty = x.qty,
                           amount = x.amount
                       };

            foreach (var d in data.ToList())
            {
                model.Add(new LaporanUsageView
                {
                    idMaterial = d.idMaterial,
                    materialInfo = d.materialInfo,
                    materialBudget = d.materialBudget,
                    materialUsage = sumUsage(d.idMaterial, idActD, d.uom),
                    qty = d.qty,
                    amount = d.amount
                });
            }

            list.data = model;
            return PartialView("Penggunaan", list);            
        }

        public string sumUsage(int idMaterial, int idActD, string uom)
        {
            var data = from a in db.laporanUsages
                       join b in db.laporans on a.header equals b.Id
                       where b.idActD == idActD && a.idMaterial == idMaterial
                       select new
                       {
                           a.qty,
                           a.amount
                       };

            return string.Concat("<ul><li><b>Kuantitas: </b>", data.Sum(a => a.qty).ToString("n2"), " ",uom,
                         "</li><li><b>Biaya: </b>", data.Sum(a => a.amount).ToString("n2"), "</li></ul>");
        }
        #endregion

        #region "Overhead"
        public IActionResult Overhead(int idActD, int idReport)
        {
            // Existing data
            var data = db.laporanOverheadProfits.Where(a => a.header == idReport).Select(a => new { a.info, a.amount }).FirstOrDefault();
            // Overhead Usage: Calc base idActD.
            var overheadUsage = from a in db.laporanOverheadProfits
                                join b in db.laporans on a.header equals b.Id
                                where b.idActD == idActD
                                select new
                                {
                                    a.amount
                                };

            LaporanOverheadProfitView model = new LaporanOverheadProfitView();
            model.info = data != null ? data.info : string.Empty;
            model.budget = calcBudgetOverhead(idActD);
            model.usage = overheadUsage.Sum(a => a.amount);
            model.amount = data != null ? data.amount : 0;

            return PartialView("Overhead", model);
        }

        public double calcBudgetOverhead(int idActD)
        {
            var ActD = db.actDs.Where(a => a.Id == idActD).FirstOrDefault();
            var xdata = from a in db.analisas
                        join b in db.materials on a.idMaterial equals b.Id
                        where a.idActD == idActD
                        select new
                        {
                            harga = b.harga,
                            koefisien = a.koefisien
                        };

            return (xdata.Sum(a => a.harga * a.koefisien) * ActD.profit / 100) * ActD.volume;
        }

        #endregion

        #region "WIP"
        public IActionResult WIP(int idActD, int idReport)
        {
            // WIP base on data (update)
            double wip = idReport != 0 ? db.laporans.FirstOrDefault(a => a.idActD == idActD && a.Id == idReport).wip : 0;
            ViewData["Wip"] = wip;

            // Progress terakhir
            var last = db.laporans.Where(a => a.idActD == idActD).OrderByDescending(a => a.reportDate).FirstOrDefault();
            ViewData["LastWip"] = last != null ? last.wip : 0;

            return PartialView("WIP");
        }
        #endregion

        #region "Nota"
        public IActionResult Nota(int idReport)
        {
            List<LaporanAttachmentView> model = new List<LaporanAttachmentView>();
            var data = db.laporanAttachments.Where(a => a.jenis == "N" && a.header == idReport);
            foreach (var d in data)
            {
                model.Add(new LaporanAttachmentView
                {
                    id = d.Id,
                    header = d.header,
                    idProyek = d.idProyek,
                    filename = d.filename,
                    jenis = d.jenis
                });
            };
            listLaporanAttachment list = new listLaporanAttachment();
            list.data = model;
            return PartialView("Nota", list);
        }
        #endregion

        #region "Progress"
        public IActionResult Progress(int idReport)
        {
            List<LaporanAttachmentView> model = new List<LaporanAttachmentView>();
            var data = db.laporanAttachments.Where(a => a.jenis == "P" && a.header == idReport);
            foreach (var d in data)
            {
                model.Add(new LaporanAttachmentView
                {
                    id = d.Id,
                    header = d.header,
                    idProyek = d.idProyek,
                    filename = d.filename,
                    jenis = d.jenis
                });
            };
            listLaporanAttachment list = new listLaporanAttachment();
            list.data = model;
            return PartialView("Progress", list);
        }
        #endregion

        #region "Submit"

        public IActionResult checkPeriod(LaporanView data)
        {
            try
            {
                DateTime dueDateReport = db.proyeks.FirstOrDefault(a => a.idProyek == data.idProyek).tglSelesai;
                if (data.reportDate.Date > dueDateReport.Date)
                {
                    return Content("NotOk");
                }
                else
                {
                    return Content("Ok");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        public async Task<IActionResult> submitLaporan(LaporanView data)
        {
            try
            {
                int idReport = data.id;
                string Subject = string.Empty;
                var proyekInfo = db.proyeks.FirstOrDefault(a => a.idProyek == data.idProyek);
                
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                if (idReport != 0)
                {
                    var laporan = db.laporans.FirstOrDefault(a => a.Id == idReport);
                    laporan.idProyek = data.idProyek;
                    laporan.idActD = data.idActD;
                    laporan.idActH = data.idActH;
                    laporan.reportDate = data.reportDate;
                    laporan.note = data.note;
                    laporan.wip = data.wip;
                    laporan.UpdateBy = user;
                    laporan.UpdateDate = DateTime.Now;
                    db.laporans.Update(laporan);
                    await db.SaveChangesAsync();

                    // idReport
                    idReport = laporan.Id;
                    Subject = string.Concat("[PERUBAHAN] Laporan ", proyekInfo.judul, " Periode ",laporan.reportDate.ToString("dd.MM.yyyy"));
                }
                else
                {
                    Laporan laporan = new Laporan();
                    laporan.idProyek = data.idProyek;
                    laporan.idActD = data.idActD;
                    laporan.idActH = data.idActH;
                    laporan.reportDate = data.reportDate;
                    laporan.note = data.note;
                    laporan.wip = data.wip;
                    laporan.CreateBy = user;
                    laporan.CreateDate = DateTime.Now;
                    db.laporans.Add(laporan);
                    await db.SaveChangesAsync();

                    // idReport
                    idReport = laporan.Id;
                    Subject = string.Concat("Laporan ", proyekInfo.judul, " Periode ", laporan.reportDate.ToString("dd.MM.yyyy"));
                }

                // usage
                var usage = JsonConvert.DeserializeObject<List<LaporanUsageParam>>(data.usage);

                // usage drop
                db.laporanUsages.RemoveRange(db.laporanUsages.Where(a => a.header == idReport));
                await db.SaveChangesAsync();

                // usage insert 
                foreach (var u in usage)
                {
                    if (u.idMaterial != "x")
                    {
                        LaporanUsage model = new LaporanUsage();
                        model.idProyek = data.idProyek;
                        model.header = idReport;
                        model.idMaterial = (int)Convert.ToInt64(u.idMaterial);
                        model.qty = (double)Convert.ToDouble(u.Kuantitas);
                        model.amount = (double)Convert.ToDouble(u.Biaya);
                        model.CreateBy = user;
                        model.CreateDate = DateTime.Now;

                        db.laporanUsages.Add(model);
                    }
                }
                await db.SaveChangesAsync();

                // overhead
                var overhead = JsonConvert.DeserializeObject<List<LaporanOverheadProfitParam>>(data.overhead);

                // overhead drop
                db.laporanOverheadProfits.RemoveRange(db.laporanOverheadProfits.Where(a => a.header == idReport));
                await db.SaveChangesAsync();

                // overhead insert 
                foreach (var u in overhead)
                {
                    LaporanOverheadProfit model = new LaporanOverheadProfit();
                    model.idProyek = data.idProyek;
                    model.header = idReport;
                    model.amount = (int)Convert.ToInt64(u.Biaya);
                    model.info = u.Keterangan;
                    model.CreateBy = user;
                    model.CreateDate = DateTime.Now;

                    db.laporanOverheadProfits.Add(model);
                }
                await db.SaveChangesAsync();

                // Attachment
                // Nota
                if (data.fileNota != null)
                {
                    foreach (var file in data.fileNota)
                    {
                        // Rename file set prefix to idProyek+idReport+Guid
                        string extension = Path.GetExtension(file.FileName);
                        var fileName = string.Concat(data.idProyek, idReport, "_", Guid.NewGuid().ToString().ToUpper().Substring(1, 7), extension);
                        var filePath = Path.Combine(hostingEnv.WebRootPath, string.Concat("file/", fileName));

                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileSteam);
                        }

                        LaporanAttachment modelAttach = new LaporanAttachment();
                        modelAttach.header = idReport;
                        modelAttach.idProyek = data.idProyek;
                        modelAttach.jenis = "N";
                        modelAttach.filename = fileName;
                        modelAttach.CreateBy = user;
                        modelAttach.CreateDate = DateTime.Now;
                        db.laporanAttachments.Add(modelAttach);
                    }
                    await db.SaveChangesAsync();
                }
                
                // Progress
                if (data.fileProgress != null)
                {
                    foreach (var file in data.fileProgress)
                    {
                        // Rename file set prefix to idProyek+idReport+Guid
                        string extension = Path.GetExtension(file.FileName);
                        var fileName = string.Concat(data.idProyek, idReport, "_", Guid.NewGuid().ToString().ToUpper().Substring(1, 7), extension);
                        var filePath = Path.Combine(hostingEnv.WebRootPath, string.Concat("file/", fileName));

                        using (var fileSteam = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileSteam);
                        }

                        LaporanAttachment modelAttach = new LaporanAttachment();
                        modelAttach.header = idReport;
                        modelAttach.idProyek = data.idProyek;
                        modelAttach.jenis = "P";
                        modelAttach.filename = fileName;
                        modelAttach.CreateBy = user;
                        modelAttach.CreateDate = DateTime.Now;
                        db.laporanAttachments.Add(modelAttach);
                    }
                    await db.SaveChangesAsync();
                }
                
                // Send Email
                await sendMail(idReport, Subject);

                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }


        public async Task<IActionResult> sendMail(int idReport, string Subject)
        {
            try
            {
                var data = db.laporans.FirstOrDefault(a => a.Id == idReport);
                // modeling template base data
                StringBuilder sb = new StringBuilder();
                string template = string.Empty;
                using (StreamReader sr = new StreamReader(hostingEnv.WebRootPath + "/file/ReportTemplate.html"))
                {
                    template = sr.ReadToEnd();
                }

                foreach (var content in new string[] { "{reportDate}", "{actH}", "{actD}", "{createdBy}", "{usage}", "{overhead}", "{nota}", "{progress}","{note}","{wip}" })
                {
                    string temp = template;
                    if (content == "{wip}")
                    {
                        template = string.Empty;
                        template = temp.Replace(content, string.Concat(data.wip.ToString("n2"),"%"));
                    }
                    if (content == "{note}")
                    {
                        template = string.Empty;
                        template = temp.Replace(content, data.note.ToString());
                    }
                    if (content == "{reportDate}")
                    {
                        template = string.Empty;
                        template = temp.Replace(content, data.reportDate.ToString("dd.MM.yyyy"));
                    }
                    else if (content == "{actH}")
                    {
                        template = string.Empty;
                        template = temp.Replace(content, db.actHs.FirstOrDefault(a => a.Id == data.idActH).kegiatan);
                    }
                    else if (content == "{actD}")
                    {
                        template = string.Empty;
                        template = temp.Replace(content, db.actDs.FirstOrDefault(a => a.Id == data.idActD).kegiatan);
                    }
                    else if (content == "{createdBy}")
                    {
                        string roleId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                        string fullname = db.Users.FirstOrDefault(a => a.id == data.CreateBy).fullname;
                        string role = db.Roles.FirstOrDefault(a => a.Id == Convert.ToInt32(roleId)).roleName;
                        string label = string.Empty;
                        
                        if (roleId == "1")
                        {
                            label = string.Concat("<font style='color: white; background-color: Red;'>",fullname, " (", role, ")</font>");
                        }
                        else if (roleId == "2")
                        {
                            label = string.Concat("<font style='color: white; background-color: Blue;'>", fullname, " (", role, ")</font>");
                        }
                        else if (roleId == "3")
                        {
                            label = string.Concat("<font style='color: white; background-color: Green;'>", fullname, " (", role, ")</font>");
                        }
                        

                        template = string.Empty;
                        template = temp.Replace(content, label);
                    }
                    else if (content == "{usage}")
                    {
                        template = string.Empty;
                        sb.Clear();
                        List<LaporanUsageView> model = new List<LaporanUsageView>();

                        var dataUsage = from a in db.analisas
                                       join b in db.actDs on a.idActD equals b.Id
                                       join c in db.materials on a.idMaterial equals c.Id
                                       join d in db.laporanUsages on new { x1 = c.Id, x2 = data.Id } equals new { x1 = d.idMaterial, x2 = d.header } into gp
                                       from x in gp.DefaultIfEmpty()
                                       where a.idActD == data.idActD
                                       select new LaporanUsageView
                                       {
                                           idMaterial = a.idMaterial,
                                           materialInfo = string.Concat(c.item, " @ ", c.harga.ToString("n2"), "/", c.uom),
                                           materialBudget = string.Concat("<td style='padding: 5px;'>Kuantitas: ",
                                                                (a.koefisien * b.volume).ToString("n2"), " ", c.uom,
                                                                "<br />Biaya: ", 
                                                                ((a.koefisien * c.harga) * b.volume).ToString("n2"), "</td>"),
                                           uom = c.uom,
                                           qty = x.qty,
                                           amount = x.amount
                                       };

                        foreach (var d in dataUsage.ToList())
                        {
                            model.Add(new LaporanUsageView
                            {
                                idMaterial = d.idMaterial,
                                materialInfo = d.materialInfo,
                                materialBudget = d.materialBudget,
                                materialUsage = sumUsageInEmail(d.idMaterial, data.idActD, d.uom),
                                qty = d.qty,
                                amount = d.amount
                            });
                        }

                        foreach (var usage in model)
                        {
                            sb.Append("<tr style='background-color: darkcyan; color: white;'>" +
                                      "<td colspan='2' style='padding: 5px;'>" + usage.materialInfo + "</td>" +
                                      "<td style='text-align: right; padding: 5px;'>" + usage.qty.ToString("n2") + "</td>" +
                                      "<td style='text-align: right; padding: 5px;'>" + usage.amount.ToString("n2") + "</td></tr>" +
                                      "<tr style='background-color: whitesmoke; color: black;'>" +
                                      usage.materialBudget + usage.materialUsage +
                                      "<td colspan='2'></td></tr>");
                        }
                        template = temp.Replace(content, sb.ToString());
                    }
                    else if (content == "{overhead}")
                    {
                        template = string.Empty;
                        sb.Clear();
                        // Existing data
                        var dataOverhead = db.laporanOverheadProfits.Where(a => a.header == idReport).Select(a => new { a.info, a.amount }).FirstOrDefault();
                        // Overhead Usage: Calc base idActD.
                        var overheadUsage = from a in db.laporanOverheadProfits
                                            join b in db.laporans on a.header equals b.Id
                                            where b.idActD == data.idActD
                                            select new
                                            {
                                                a.amount
                                            };

                        LaporanOverheadProfitView model = new LaporanOverheadProfitView();
                        model.info = dataOverhead.info;
                        model.budget = calcBudgetOverhead(data.idActD);
                        model.usage = overheadUsage.Sum(a => a.amount);
                        model.amount = dataOverhead.amount;

                        sb.Append("<tr style='background-color: whitesmoke; color: black;'>" +
                                    "<td style='padding: 5px;'>" + model.budget.ToString("n2") + "</td>" +
                                    "<td style='padding: 5px;'>" + model.usage.ToString("n2") + "</td>" +
                                    "<td style='padding: 5px;'>" + model.info + "</td>" +
                                    "<td style='text-align: right; padding: 5px;'>" + model.amount.ToString("n2") + 
                                    "</td></tr>");

                        template = temp.Replace(content, sb.ToString());
                    }
                    else if (content == "{nota}")
                    {
                        template = string.Empty;
                        sb.Clear();
                        List<LaporanAttachmentView> model = new List<LaporanAttachmentView>();
                        var dataNota = db.laporanAttachments.Where(a => a.jenis == "N" && a.header == idReport);
                        foreach (var d in dataNota)
                        {
                            model.Add(new LaporanAttachmentView
                            {
                                id = d.Id,
                                header = d.header,
                                idProyek = d.idProyek,
                                filename = d.filename,
                                jenis = d.jenis
                            });
                        };
                        foreach (var d in model)
                        {
                            // sb.Append("<li><a href='"+ hostingEnv.WebRootPath + "\\file\\" + d.filename + "' target='_blank'>" + d.filename + "</a></li>");
                            sb.Append("<li><a href='http://139.180.140.13/file/" + d.filename + "' target='_blank'>" + d.filename + "</a></li>");
                        }
                        template = temp.Replace(content, sb.ToString());
                    }
                    else if (content == "{progress}")
                    {
                        template = string.Empty;
                        sb.Clear();
                        List<LaporanAttachmentView> model = new List<LaporanAttachmentView>();
                        var dataProgress = db.laporanAttachments.Where(a => a.jenis == "P" && a.header == idReport);
                        foreach (var d in dataProgress)
                        {
                            model.Add(new LaporanAttachmentView
                            {
                                id = d.Id,
                                header = d.header,
                                idProyek = d.idProyek,
                                filename = d.filename,
                                jenis = d.jenis
                            });
                        };
                        foreach (var d in model)
                        {
                            //sb.Append("<li><a href='" + hostingEnv.WebRootPath + "\\file\\" + d.filename + "' target='_blank'>" + d.filename + "</a></li>");
                            sb.Append("<li><a href='http://139.180.140.13/file/" + d.filename + "' target='_blank'>" + d.filename + "</a></li>");
                        }
                        template = temp.Replace(content, sb.ToString());
                    }
                }


                var mailTo = db.Users.Where(a => a.role == 1);
                foreach (var owner in mailTo)
                {
                    await mail.SendAsync(owner.email,Subject,template,true);
                }

                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        public string sumUsageInEmail(int idMaterial, int idActD, string uom)
        {
            var data = from a in db.laporanUsages
                       join b in db.laporans on a.header equals b.Id
                       where b.idActD == idActD && a.idMaterial == idMaterial
                       select new
                       {
                           a.qty,
                           a.amount
                       };

            return string.Concat("<td style='padding: 5px;'>Kuantitas: ", data.Sum(a => a.qty).ToString("n2"), " ", uom,
                         "<br />Biaya: ", data.Sum(a => a.amount).ToString("n2"), "</td>");
        }

        public async Task<IActionResult> deleteAttach(int id)
        {
            try
            {
                var data = db.laporanAttachments.FirstOrDefault(a => a.Id == id);
                var filePath = Path.Combine(hostingEnv.WebRootPath, string.Concat("file/", data.filename));
                System.IO.File.Delete(filePath);
                db.laporanAttachments.Remove(data);
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }
        #endregion
    }
}
