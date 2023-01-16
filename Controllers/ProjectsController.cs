using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using renovi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace renovi.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly AppDbContext db;
        private readonly IHttpContextAccessor httpConn;
        private readonly IWebHostEnvironment hostingEnv;

        public ProjectsController(AppDbContext context, IHttpContextAccessor httpCon, IWebHostEnvironment env)
        {
            db = context;
            httpConn = httpCon;
            hostingEnv = env;
        }

        #region "init"
        [Authorize(Roles = "1,2,3")]
        public IActionResult Index()
        {
            ViewData["showAddButton"] = db.showAddButtons.FirstOrDefault(a => a.Id == 1).show;
            ViewData["RoleId"] = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
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

        public IActionResult modalProyek()
        {
            ProyekInfoParam model = new ProyekInfoParam();
            model.tglMulai = DateTime.Now;
            model.tglSelesai = DateTime.Now;
            return PartialView("modalProyek", model);
        }

        public IActionResult getMandor(string index)
        {
            var data = from a in db.Users
                       join b in db.proyeks
                       on new { x1 = a.id, x2 = index }
                       equals new { x1 = b.mandor, x2 = b.idProyek }
                       into gp
                       from x in gp.DefaultIfEmpty()
                       where a.role == 2
                       select new
                       {
                           id = a.id,
                           text = a.fullname,
                           selected = x.idProyek != null
                       };
            return Json(data);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> submitProyekInfo(ProyekInfoParam data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                    if (data.Id != 0)
                    {
                        var model = db.proyeks.FirstOrDefault(a => a.Id == data.Id);
                        model.judul = data.judul;
                        model.alamat = data.Alamat;
                        model.tglMulai = data.tglMulai;
                        model.tglSelesai = data.tglSelesai;
                        model.mandor = data.mandor;
                        model.namaKlien = data.namaKlien;
                        model.UpdateBy = user;
                        model.UpdateDate = DateTime.Now;

                        if (data.kontrak != null)
                        {
                            var fileName = string.Concat(model.idProyek, "_Kontrak", ".pdf");
                            var filePath = Path.Combine(hostingEnv.WebRootPath, "file/", fileName);
                            using (var fileSteam = new FileStream(filePath, FileMode.Create))
                            {
                                await data.kontrak.CopyToAsync(fileSteam);
                            }
                            model.kontrak = fileName;
                        }

                        if (data.desain != null)
                        {
                            var fileName = string.Concat(model.idProyek, "_Desain", ".pdf");
                            var filePath = Path.Combine(hostingEnv.WebRootPath, "file/", fileName);
                            using (var fileSteam = new FileStream(filePath, FileMode.Create))
                            {
                                await data.desain.CopyToAsync(fileSteam);
                            }
                            model.desain = fileName;
                        }

                        db.proyeks.Update(model);
                        await db.SaveChangesAsync();
                        return Content("Ok");
                    }
                    else
                    {
                        string idProyek = string.Concat(DateTime.Now.ToString("yyyyMM"), Guid.NewGuid().ToString().ToUpper().Substring(1, 5));

                        Proyek model = new Proyek();
                        model.idProyek = idProyek;
                        model.Id = data.Id;
                        model.judul = data.judul;
                        model.alamat = data.Alamat;
                        model.tglMulai = data.tglMulai;
                        model.tglSelesai = data.tglSelesai;
                        model.mandor = data.mandor;
                        model.namaKlien = data.namaKlien;
                        model.isActive = false;
                        model.isDelete = false;
                        model.CreateBy = user;
                        model.CreateDate = DateTime.Now;


                        if (data.kontrak != null)
                        {
                            var fileName = string.Concat(idProyek, "_Kontrak", ".pdf");
                            var filePath = Path.Combine(hostingEnv.WebRootPath, "file/", fileName);
                            using (var fileSteam = new FileStream(filePath, FileMode.Create))
                            {
                                await data.kontrak.CopyToAsync(fileSteam);
                            }
                            model.kontrak = fileName;
                        }

                        if (data.desain != null)
                        {
                            var fileName = string.Concat(idProyek, "_Desain", ".pdf");
                            var filePath = Path.Combine(hostingEnv.WebRootPath, "file/", fileName);
                            using (var fileSteam = new FileStream(filePath, FileMode.Create))
                            {
                                await data.desain.CopyToAsync(fileSteam);
                            }
                            model.desain = fileName;
                        }

                        db.proyeks.Add(model);
                        await db.SaveChangesAsync();
                        return Content("Ok");
                    }
                }
                catch (Exception ex)
                {
                    return Content(ex.ToString());
                }
            }
            return View(data);
        }

        public async Task<IActionResult> setActive(int id, bool val)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                var data = db.proyeks.FirstOrDefault(a => a.Id == id);
                data.isActive = val;
                data.UpdateBy = user;
                data.UpdateDate = DateTime.Now;
                db.proyeks.Update(data);
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public async Task<IActionResult> deleteProject(int id)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                var data = db.proyeks.FirstOrDefault(a => a.Id == id);
                data.isDelete = true;
                data.UpdateBy = user;
                data.UpdateDate = DateTime.Now;
                db.proyeks.Update(data);
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
        #endregion

        #region "Detail"
        [Authorize(Roles = "1,3")]
        public IActionResult Details(int id)
        {
            var data = db.proyeks.FirstOrDefault(a => a.Id == id);
            ViewData["idProyek"] = data.idProyek;
            ViewData["namaProyek"] = string.Concat(data.judul, " (", data.idProyek, ")");
            return View();
        }

        public IActionResult DetailGeneralInfo(string idProyek)
        {
            ProyekInfoParam model = new ProyekInfoParam();
            var data = db.proyeks.FirstOrDefault(a => a.idProyek == idProyek);
            model.Id = data.Id;
            model.idProyek = data.idProyek;
            model.judul = data.judul;
            model.Alamat = data.alamat;
            model.tglMulai = data.tglMulai;
            model.tglSelesai = data.tglSelesai;
            model.mandor = data.mandor;
            model.namaKlien = data.namaKlien;
            return PartialView("DetailGeneralInfo", model);
        }
        #endregion

        #region "Pekerjaan"
        [Authorize(Roles = "1,3")]
        public IActionResult Pekerjaan(string idProyek)
        {
            // direct if idProyek empty
            if (string.IsNullOrEmpty(idProyek))
            {
                return RedirectToAction("index", "Project");
            }

            List<ActView> model = new List<ActView>();
            foreach (var data in db.actHs.Where(a => a.idProyek == idProyek).OrderBy(a => a.Seq).ToList())
            {
                model.Add(new ActView
                {
                    id = data.Id,
                    idProyek = data.idProyek,
                    Seq = data.Seq,
                    kegiatan = data.kegiatan,
                    details = getPekerjaanDetail(data.Id)
                });
            }

            listAction list = new listAction();
            list.data = model;

            ViewData["idProyek"] = idProyek;
            return PartialView("Pekerjaan", list);
        }

        public List<ActDView> getPekerjaanDetail(int id)
        {
            int i = 1;
            List<ActDView> model = new List<ActDView>();
            foreach (var data in db.actDs.Where(a => a.header == id).OrderBy(a => a.Id).ToList())
            {
                model.Add(new ActDView
                {
                    Id = i,
                    kegiatan = data.kegiatan,
                    uom = data.uom,
                    volume = data.volume,
                    harga = 0,
                    profit = 0
                });
                i++;
            }
            return model;
        }

        public IActionResult export(string idProyek)
        {
            try
            {
                DataTable dt = new DataTable("temp");
                dt.Columns.Add("Ref", typeof(int));
                dt.Columns.Add("IsHeader", typeof(int));
                dt.Columns.Add("No", typeof(int));
                dt.Columns.Add("Pekerjaan", typeof(string));
                dt.Columns.Add("Satuan", typeof(string));
                dt.Columns.Add("Volume", typeof(double));

                if (!string.IsNullOrEmpty(idProyek))
                {
                    var header = db.actHs.Where(a => a.idProyek == idProyek).ToList();
                    var detail = db.actDs.Where(a => a.idProyek == idProyek).ToList();

                    foreach (var h in header)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Ref"] = h.Id;
                        dr["IsHeader"] = 1;
                        dr["No"] = h.Seq;
                        dr["Pekerjaan"] = h.kegiatan;
                        dr["Satuan"] = string.Empty;
                        dt.Rows.Add(dr);

                        int i = 1;
                        foreach (var d in detail.Where(a => a.header == h.Id).ToList())
                        {
                            DataRow drx = dt.NewRow();
                            drx["Ref"] = d.Id;
                            drx["IsHeader"] = 0;
                            drx["No"] = h.Seq;
                            drx["Pekerjaan"] = d.kegiatan;
                            drx["Satuan"] = d.uom;
                            drx["Volume"] = d.volume;
                            dt.Rows.Add(drx);
                            i++;
                        }
                    }
                }

                var stream = new MemoryStream();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var pkg = new OfficeOpenXml.ExcelPackage(stream))
                {
                    ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Sheet1");
                    ws.Cells.LoadFromDataTable(dt, true);
                    ws.Cells["A1"].Style.Font.Bold = true;
                    ws.Cells["B1"].Style.Font.Bold = true;
                    ws.Cells["C1"].Style.Font.Bold = true;
                    ws.Cells["D1"].Style.Font.Bold = true;
                    ws.Cells["E1"].Style.Font.Bold = true;
                    ws.Cells["F1"].Style.Font.Bold = true;

                    ws.View.FreezePanes(2, 1);
                    ws.Cells.AutoFitColumns();
                    pkg.Save();
                }
                stream.Position = 0;
                string filename = string.Concat("Pekerjaan_", idProyek ?? "Baru", ".xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        public IActionResult importModal(string idProyek)
        {
            upload model = new upload();
            model.idProyek = idProyek;
            return PartialView("importModal", model);
        }

        public async Task<IActionResult> import(upload file)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);

                // grab xlsx
                List<ImportPekerjaanHeader> modelHeader = new List<ImportPekerjaanHeader>();
                List<ImportPekerjaanDetail> modelDetail = new List<ImportPekerjaanDetail>();

                using (var stream = new MemoryStream())
                {
                    await file.data.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var pkg = new OfficeOpenXml.ExcelPackage(stream))
                    {
                        ExcelWorksheet ws = pkg.Workbook.Worksheets[0];
                        var rowCount = ws.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var Ref = ws.Cells[row, 1].Text;
                            var IsHeader = ws.Cells[row, 2].Text;
                            var No = ws.Cells[row, 3].Text;
                            var Pekerjaan = ws.Cells[row, 4].Text.ToString().Trim();
                            var Satuan = ws.Cells[row, 5].Text.ToString().Trim();
                            var Volume = ws.Cells[row, 6].Text;

                            if (IsHeader == "1")
                            {
                                modelHeader.Add(new ImportPekerjaanHeader
                                {
                                    id = Ref,
                                    idProyek = file.idProyek,
                                    Seq = (int)Convert.ToInt64(No),
                                    kegiatan = Pekerjaan
                                });
                            }
                            else
                            {
                                modelDetail.Add(new ImportPekerjaanDetail
                                {
                                    Id = Ref,
                                    header = No,
                                    kegiatan = Pekerjaan,
                                    satuan = Satuan,
                                    volume = (Double)Convert.ToDouble(Volume)
                                });
                            }
                        }
                    }
                    stream.Close();
                }

                // model to list
                List<ActView> Header = new List<ActView>();
                foreach (var header in modelHeader)
                {
                    Header.Add(new ActView
                    {
                        id = header.id == "" ? 0 : (int)Convert.ToInt64(header.id),
                        idProyek = header.idProyek,
                        Seq = header.Seq,
                        kegiatan = header.kegiatan,
                        details = getPekerjaanDetailImport(header.Seq, modelDetail)
                    });
                }

                // insert update data
                foreach (var data in Header)
                {
                    // kalau ref ada, update!
                    if (data.id != 0)
                    {
                        var dheader = db.actHs.FirstOrDefault(a => a.Id == data.id);
                        dheader.Seq = data.Seq;
                        dheader.idProyek = data.idProyek;
                        dheader.kegiatan = data.kegiatan;
                        dheader.UpdateBy = user;
                        dheader.UpdateDate = DateTime.Now;
                        db.actHs.Update(dheader);
                        await db.SaveChangesAsync();

                        await ActDetailInsertUpdate(data.details, data.idProyek, dheader.Id);
                    }
                    else
                    {
                        ActH dheader = new ActH();
                        dheader.Seq = data.Seq;
                        dheader.idProyek = data.idProyek;
                        dheader.kegiatan = data.kegiatan;
                        dheader.CreateBy = user;
                        dheader.CreateDate = DateTime.Now;
                        db.actHs.Add(dheader);
                        await db.SaveChangesAsync();

                        await ActDetailInsertUpdate(data.details, data.idProyek, dheader.Id);
                    }
                }
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
                throw;
            }
        }

        public async Task<IActionResult> ActDetailInsertUpdate(List<ActDView> data, string idProyek, int header)
        {
            int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);

            foreach (var x in data)
            {
                if (x.Id != 0)
                {
                    var detail = db.actDs.FirstOrDefault(a => a.Id == x.Id);
                    detail.idProyek = idProyek;
                    detail.header = header;
                    detail.kegiatan = x.kegiatan;
                    detail.volume = x.volume;
                    detail.uom = x.uom;
                    detail.UpdateBy = user;
                    detail.UpdateDate = DateTime.Now;

                    db.actDs.Update(detail);
                }
                else
                {
                    ActD detail = new ActD();
                    detail.idProyek = idProyek;
                    detail.header = header;
                    detail.kegiatan = x.kegiatan;
                    detail.volume = x.volume;
                    detail.uom = x.uom;
                    detail.CreateBy = user;
                    detail.CreateDate = DateTime.Now;

                    db.actDs.Add(detail);
                }
                await db.SaveChangesAsync();
            }

            return Content("Ok");
        }

        public List<ActDView> getPekerjaanDetailImport(int header, List<ImportPekerjaanDetail> detail)
        {
            List<ActDView> model = new List<ActDView>();
            foreach (var data in detail.Where(a => Convert.ToInt64(a.header) == header))
            {
                model.Add(new ActDView
                {
                    Id = data.Id == "" ? 0 : (int)Convert.ToInt64(data.Id),
                    kegiatan = data.kegiatan,
                    uom = data.satuan,
                    volume = data.volume
                });
            }
            return model;
        }

        public IActionResult modalPekerjaan(int id)
        {
            listAction model = new listAction();
            List<ActView> data = new List<ActView>();

            // Temp Data
            var header = db.actHs.FirstOrDefault(a => a.Id == id);
            var _actD = db.actDs.Where(a => a.header == header.Id).OrderBy(a => a.Id).ToList();
            
            data.Add(new ActView
            {
                id = header.Id,
                idProyek = header.idProyek,
                Seq = header.Seq,
                kegiatan = header.kegiatan,
                details = getPekerjaanDetailDataAnalisa(_actD)
            });
            model.data = data;
            return PartialView("modalPekerjaan", model);
        }

        public List<ActDView> getPekerjaanDetailDataAnalisa(List<ActD> actDs)
        {
            List<ActDView> model = new List<ActDView>();
            foreach (var data in actDs)
            {
                model.Add(new ActDView
                {
                    header = data.header,
                    Id = data.Id,
                    kegiatan = data.kegiatan,
                    uom = data.uom,
                    volume = data.volume,
                    harga = calcHargaSatuanPekerjaanDetail(data.Id, data.profit),
                    profit = data.profit
                });
            }
            return model;
        }

        public double calcHargaSatuanPekerjaanDetail(int id, double profit)
        {
            var data = from a in db.analisas
                       join b in db.materials on a.idMaterial equals b.Id
                       where a.idActD == id
                       select new
                       {
                           harga = b.harga,
                           koefisien = a.koefisien
                       };
            return data.Sum(a => a.harga * a.koefisien) + data.Sum(a => a.harga * a.koefisien) * profit / 100;
        }

        public async Task<IActionResult> deletePekerjaanDetail(int id)
        {
            try
            {
                db.actDs.Remove(db.actDs.FirstOrDefault(a => a.Id == id));
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public async Task<IActionResult> submitActH(string idProyek, int idActH, string seq, string pekerjaan, string pekerjaanDetail)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                if (idActH != 0)
                {
                    var dataHeader = db.actHs.FirstOrDefault(a => a.Id == idActH);
                    dataHeader.Seq = (int)Convert.ToInt32(seq);
                    dataHeader.kegiatan = pekerjaan;
                    dataHeader.idProyek = idProyek;
                    dataHeader.UpdateBy = user;
                    dataHeader.UpdateDate = DateTime.Now;
                    db.actHs.Update(dataHeader);
                    await db.SaveChangesAsync();
                    await submitActD(idProyek, idActH, pekerjaanDetail);
                }
                else
                {
                    ActH dataHeader = new ActH();
                    dataHeader.Seq = (int)Convert.ToInt32(seq);
                    dataHeader.kegiatan = pekerjaan;
                    dataHeader.idProyek = idProyek;
                    dataHeader.CreateBy = user;
                    dataHeader.CreateDate = DateTime.Now;
                    db.actHs.Add(dataHeader);
                    db.SaveChanges();
                    await db.SaveChangesAsync();
                    await submitActD(idProyek, dataHeader.Id, pekerjaanDetail);
                }
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public async Task<IActionResult> submitActD(string idProyek, int idActH, string pekerjaanDetail)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                var dataDetail = JsonConvert.DeserializeObject<List<ActDParam>>(pekerjaanDetail);

                // insert update detail                
                foreach (var data in dataDetail)
                {
                    if (data.Id == "0")
                    {
                        ActD details = new ActD();
                        details.header = idActH;
                        details.idProyek = idProyek;
                        details.kegiatan = data.Kegiatan;
                        details.uom = data.Satuan;
                        details.volume = data.Volume;
                        details.CreateBy = user;
                        details.CreateDate = DateTime.Now;
                        db.actDs.Add(details);
                    }
                    else
                    {
                        var details = db.actDs.FirstOrDefault(a => a.Id == Convert.ToInt64(data.Id));
                        details.header = idActH;
                        details.idProyek = idProyek;
                        details.kegiatan = data.Kegiatan;
                        details.uom = data.Satuan;
                        details.volume = data.Volume;
                        details.UpdateBy = user;
                        details.UpdateDate = DateTime.Now;
                        db.actDs.Update(details);
                    }
                }
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public async Task<IActionResult> deletePekerjaan(int id)
        {
            try
            {
                // drop header
                db.actHs.Remove(db.actHs.FirstOrDefault(a => a.Id == id));
                await db.SaveChangesAsync();

                // drop detail
                db.actDs.RemoveRange(db.actDs.Where(a => a.header == id));
                await db.SaveChangesAsync();

                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
        #endregion

        #region "Material"

        [Authorize(Roles = "1,3")]
        public IActionResult Material(string idProyek)
        {
            // direct if idProyek empty
            if (string.IsNullOrEmpty(idProyek))
            {
                return RedirectToAction("index", "Project");
            }

            List<MatView> model = new List<MatView>();
            foreach (var data in db.materials.Where(a => a.idProyek == idProyek).OrderBy(a => a.Id).ToList())
            {
                model.Add(new MatView
                {
                    id = data.Id,
                    jenis = data.jenis,
                    item = data.item,
                    satuan = data.uom,
                    harga = data.harga
                });
            }
            ViewData["idProyek"] = idProyek;
            listMat list = new listMat();
            list.data = model;
            return PartialView("Material", list);
        }

        public IActionResult exportMaterial(string idProyek)
        {
            try
            {
                DataTable dtmaterial = new DataTable("material");
                dtmaterial.Columns.Add("Ref", typeof(int));
                dtmaterial.Columns.Add("Item", typeof(string));
                dtmaterial.Columns.Add("Satuan", typeof(string));
                dtmaterial.Columns.Add("Harga", typeof(double));

                DataTable dtjasa = new DataTable("jasa");
                dtjasa.Columns.Add("Ref", typeof(int));
                dtjasa.Columns.Add("Item", typeof(string));
                dtjasa.Columns.Add("Satuan", typeof(string));
                dtjasa.Columns.Add("Harga", typeof(double));

                if (!string.IsNullOrEmpty(idProyek))
                {
                    var material = db.materials.Where(a => a.idProyek == idProyek && a.jenis == "M").ToList();
                    var jasa = db.materials.Where(a => a.idProyek == idProyek && a.jenis == "J").ToList();

                    foreach (var m in material)
                    {
                        DataRow dr = dtmaterial.NewRow();
                        dr["Ref"] = m.Id;
                        dr["Item"] = m.item;
                        dr["Satuan"] = m.uom;
                        dr["Harga"] = m.harga;
                        dtmaterial.Rows.Add(dr);
                    }

                    foreach (var m in jasa)
                    {
                        DataRow dr = dtjasa.NewRow();
                        dr["Ref"] = m.Id;
                        dr["Item"] = m.item;
                        dr["Satuan"] = m.uom;
                        dr["Harga"] = m.harga;
                        dtjasa.Rows.Add(dr);
                    }
                }

                var stream = new MemoryStream();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var pkg = new OfficeOpenXml.ExcelPackage(stream))
                {
                    ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Material");
                    ws.Cells.LoadFromDataTable(dtmaterial, true);
                    ws.Cells["A1"].Style.Font.Bold = true;
                    ws.Cells["B1"].Style.Font.Bold = true;
                    ws.Cells["C1"].Style.Font.Bold = true;
                    ws.Cells["D1"].Style.Font.Bold = true;

                    ws.View.FreezePanes(2, 1);
                    ws.Cells.AutoFitColumns();

                    ExcelWorksheet ws1 = pkg.Workbook.Worksheets.Add("Jasa");
                    ws1.Cells.LoadFromDataTable(dtjasa, true);
                    ws1.Cells["A1"].Style.Font.Bold = true;
                    ws1.Cells["B1"].Style.Font.Bold = true;
                    ws1.Cells["C1"].Style.Font.Bold = true;
                    ws1.Cells["D1"].Style.Font.Bold = true;

                    ws1.View.FreezePanes(2, 1);
                    ws1.Cells.AutoFitColumns();

                    pkg.Save();
                }
                stream.Position = 0;
                string filename = string.Concat("HargaSatuan_", idProyek ?? "Baru", ".xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        public IActionResult importMaterialModal(string idProyek)
        {
            upload model = new upload();
            model.idProyek = idProyek;
            return PartialView("importMaterialModal", model);
        }

        public async Task<IActionResult> importMaterial(upload file)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);

                // grab xlsx
                List<MatView> model = new List<MatView>();

                using (var stream = new MemoryStream())
                {
                    await file.data.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var pkg = new OfficeOpenXml.ExcelPackage(stream))
                    {
                        ExcelWorksheet ws = pkg.Workbook.Worksheets["Material"];
                        var rowCount = ws.Dimension.Rows;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var Ref = ws.Cells[row, 1].Text;
                            var Item = ws.Cells[row, 2].Text.Trim();
                            var Satuan = ws.Cells[row, 3].Text;
                            var Harga = ws.Cells[row, 4].Text;

                            model.Add(new MatView
                            {
                                id = Ref == "" ? 0 : (int)Convert.ToInt64(Ref),
                                item = Item,
                                satuan = Satuan,
                                harga = (double)Convert.ToDouble(Harga),
                                jenis = "M"
                            });
                        }

                        ExcelWorksheet ws1 = pkg.Workbook.Worksheets["Jasa"];
                        var rowCount1 = ws1.Dimension.Rows;
                        for (int row = 2; row <= rowCount1; row++)
                        {
                            var Ref = ws1.Cells[row, 1].Text;
                            var Item = ws1.Cells[row, 2].Text.Trim();
                            var Satuan = ws1.Cells[row, 3].Text;
                            var Harga = ws1.Cells[row, 4].Text;

                            model.Add(new MatView
                            {
                                id = Ref == "" ? 0 : (int)Convert.ToInt64(Ref),
                                item = Item,
                                satuan = Satuan,
                                harga = (double)Convert.ToDouble(Harga),
                                jenis = "J"
                            });
                        }
                    }
                    stream.Close();
                }

                // insert update data
                foreach (var data in model)
                {
                    // kalau ref ada, update!
                    if (data.id != 0)
                    {
                        var dheader = db.materials.FirstOrDefault(a => a.Id == data.id);
                        dheader.item = data.item;
                        dheader.idProyek = file.idProyek;
                        dheader.item = data.item;
                        dheader.harga = data.harga;
                        dheader.jenis = data.jenis;
                        dheader.uom = data.satuan;
                        dheader.UpdateBy = user;
                        dheader.UpdateDate = DateTime.Now;
                        db.materials.Update(dheader);
                    }
                    else
                    {
                        Material dheader = new Material();
                        dheader.item = data.item;
                        dheader.idProyek = file.idProyek;
                        dheader.item = data.item;
                        dheader.harga = data.harga;
                        dheader.jenis = data.jenis;
                        dheader.uom = data.satuan;
                        dheader.CreateBy = user;
                        dheader.CreateDate = DateTime.Now;
                        db.materials.Add(dheader);
                    }
                }
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
                throw;
            }
        }

        public async Task<IActionResult> submitMaterial(string idProyek, string xdata)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);

                // convert to object M
                var dataMat = JsonConvert.DeserializeObject<List<MatParam>>(xdata);
                foreach (var data in dataMat)
                {
                    if (data.id == "0")
                    {
                        Material model = new Material();
                        model.jenis = "M";
                        model.item = data.item;
                        model.uom = data.satuan;
                        model.harga = data.harga;
                        model.idProyek = idProyek;
                        model.CreateBy = user;
                        model.CreateDate = DateTime.Now;
                        db.materials.Add(model);
                    }
                    else
                    {
                        var model = db.materials.FirstOrDefault(a => a.Id == Convert.ToInt64(data.id));
                        model.item = data.item;
                        model.uom = data.satuan;
                        model.harga = data.harga;
                        model.idProyek = idProyek;
                        model.UpdateBy = user;
                        model.UpdateDate = DateTime.Now;
                        db.materials.Update(model);
                    }
                }
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public async Task<IActionResult> submitJasa(string idProyek, string xdata)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);

                // convert to object J
                var dataMat = JsonConvert.DeserializeObject<List<MatParam>>(xdata);
                foreach (var data in dataMat)
                {
                    if (data.id == "0")
                    {
                        Material model = new Material();
                        model.jenis = "J";
                        model.item = data.item;
                        model.uom = data.satuan;
                        model.harga = data.harga;
                        model.idProyek = idProyek;
                        model.CreateBy = user;
                        model.CreateDate = DateTime.Now;
                        db.materials.Add(model);
                    }
                    else
                    {
                        var model = db.materials.FirstOrDefault(a => a.Id == Convert.ToInt64(data.id));
                        model.item = data.item;
                        model.uom = data.satuan;
                        model.harga = data.harga;
                        model.idProyek = idProyek;
                        model.UpdateBy = user;
                        model.UpdateDate = DateTime.Now;
                        db.materials.Update(model);
                    }
                }
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public async Task<IActionResult> deleteMaterial(int id)
        {
            try
            {
                var data = db.materials.FirstOrDefault(a => a.Id == id);
                db.Remove(data);
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region "Personil"
        [Authorize(Roles = "1,3")]
        public IActionResult Personil(string idProyek)
        {
            // direct if idProyek empty
            if (string.IsNullOrEmpty(idProyek))
            {
                return RedirectToAction("index", "Projects");
            }

            var data = from a in db.personils
                       join b in db.materials on a.role equals b.Id
                       where b.jenis == "J" && b.idProyek == idProyek
                       select new PersonilView
                       {
                           Id = a.Id,
                           Nama = a.nama,
                           Telepon = a.telepon,
                           Rekening = a.akunBank,
                           Role = b.item
                       };

            ViewData["idProyek"] = idProyek;
            listTukang list = new listTukang();
            list.data = data.ToList();
            return PartialView("Personil", list);
        }

        public async Task<IActionResult> submitPersonil(Personil data)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);

                if (data.Id == 0)
                {
                    Personil model = new Personil();
                    model.nama = data.nama;
                    model.telepon = data.telepon;
                    model.akunBank = data.telepon;
                    model.idProyek = data.idProyek;
                    model.CreateBy = user;
                    model.role = data.role;
                    model.CreateDate = DateTime.Now;
                    db.personils.Add(model);
                }
                else
                {
                    var model = db.personils.FirstOrDefault(a => a.Id == Convert.ToInt64(data.Id));
                    model.nama = data.nama;
                    model.telepon = data.telepon;
                    model.akunBank = data.akunBank;
                    model.idProyek = data.idProyek;
                    model.role = data.role;
                    model.UpdateBy = user;
                    model.UpdateDate = DateTime.Now;
                    db.personils.Update(model);
                }

                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public IActionResult modalPersonil(int idPersonil, string idProyek)
        {
            Personil model = new Personil();
            if (idPersonil != 0)
            {
                var data = db.personils.FirstOrDefault(a => a.Id == idPersonil);
                model.Id = data.Id;
                model.nama = data.nama;
                model.telepon = data.telepon;
                model.akunBank = data.akunBank;
            }
            model.idProyek = idProyek;
            return PartialView("modalPersonil", model);
        }

        public IActionResult getRolePersonil(int idPersonil, string idProyek)
        {
            var data = from a in db.materials
                       join b in db.personils
                       on new { x1 = a.idProyek, x2 = a.Id, x3 = idPersonil }
                       equals new { x1 = b.idProyek, x2 = b.role, x3 = b.Id }
                       into gp
                       from x in gp.DefaultIfEmpty()
                       where a.idProyek == idProyek && a.jenis == "J"
                       select new
                       {
                           id = a.Id,
                           text = a.item,
                           selected = x != null
                       };
            return Json(data);
        }

        public async Task<IActionResult> deletePersonil(int id)
        {
            try
            {
                var data = db.personils.FirstOrDefault(a => a.Id == id);
                db.Remove(data);
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region "Analisa"
        [Authorize(Roles = "1,3")]
        public IActionResult Analisa(string idProyek)
        {
            // direct if idProyek empty
            if (string.IsNullOrEmpty(idProyek))
            {
                return RedirectToAction("index", "Projects");
            }

            List<ActView> model = new List<ActView>();
            foreach (var data in db.actHs.Where(a => a.idProyek == idProyek).OrderBy(a => a.Seq).ToList())
            {
                var _actD = db.actDs.Where(a => a.header == data.Id).ToList();
                var _material = db.materials.Where(a => a.idProyek == data.idProyek).OrderBy(a => a.Id).ToList();
                var _analisa = (from a in db.analisas
                                join b in db.actDs on a.idActD equals b.Id
                                join c in db.materials on a.idMaterial equals c.Id
                                where b.idProyek == data.idProyek
                                select new analisa
                                {
                                    Id = a.Id,
                                    idActD = a.idActD,
                                    koefisien = a.koefisien,
                                    idMaterial = a.idMaterial
                                }).ToList();

                List<ActDView> detail = new List<ActDView>();
                detail = getActDetailDataAnalisa(_actD, _analisa, _material, data.Id);

                model.Add(new ActView
                {
                    id = data.Id,
                    idProyek = data.idProyek,
                    Seq = data.Seq,
                    kegiatan = data.kegiatan,
                    total = detail.Sum(a => a.harga * a.volume),
                    details = detail
                });
            }

            var ProyekInfo = db.proyeks.FirstOrDefault(a => a.idProyek == idProyek);
            ViewData["judul"] = ProyekInfo.judul;
            ViewData["Alamat"] = ProyekInfo.alamat;
            ViewData["tglMulai"] = ProyekInfo.tglMulai.ToString("dd-MMM-yyyy");
            ViewData["tglSelesai"] = ProyekInfo.tglSelesai.ToString("dd-MMM-yyyy");
            ViewData["namaKlien"] = ProyekInfo.namaKlien;

            listAction list = new listAction();
            list.data = model;
            return PartialView("Analisa", list);
        }

        public List<ActDView> getActDetailDataAnalisa(List<ActD> actDs, List<analisa> analisas, List<Material> materials, int idActH)
        {
            List<ActDView> model = new List<ActDView>();
            foreach (var data in actDs.Where(a => a.header == idActH).OrderBy(a => a.Id).ToList())
            {
                model.Add(new ActDView
                {
                    Id = data.Id,
                    kegiatan = data.kegiatan,
                    uom = data.uom,
                    volume = data.volume,
                    harga = calcHargaSatuanActDetail(analisas, materials, data.Id, data.profit),
                    profit = data.profit
                });
            }
            return model;
        }

        public double calcHargaSatuanActDetail(List<analisa> analisas, List<Material> materials, int idActD, double profit)
        {
            var data = (from a in analisas
                       join b in materials on a.idMaterial equals b.Id
                       where a.idActD == idActD
                       select new
                       {
                           harga = b.harga,
                           koefisien = a.koefisien
                       }).ToList();

            double harga = data.Sum(a => a.harga * a.koefisien);
            double hargaProfit = data.Sum(a => a.harga * a.koefisien) * profit / 100;

            return harga + hargaProfit;
        }

        public IActionResult AnalisaDetail(int id)
        {
            var data = (from a in db.analisas
                       join b in db.materials on a.idMaterial equals b.Id
                       where a.idActD == id
                       select new analisaView
                       {
                           id = a.Id,
                           idActD = a.idActD,
                           jenisItem = b.jenis,
                           item = b.item,
                           satuan = b.uom,
                           harga = b.harga,
                           koefisien = a.koefisien,
                           jumlah = b.harga * a.koefisien
                       }).ToList();

            ViewData["profit"] = db.actDs.Where(a => a.Id == id).Select(a => a.profit).FirstOrDefault();
            ViewData["idActD"] = id;

            listAnalisa list = new listAnalisa();
            list.data = data;
            return PartialView("AnalisaDetail", list);
        }

        public IActionResult modalAnalisa(int id, int idActD)
        {
            analisaParam model = new analisaParam();
            if (id != 0)
            {
                var data = db.analisas.FirstOrDefault(a => a.Id == id);
                model.id = data.Id;
                model.koefisien = data.koefisien;
                model.idMaterial = data.idMaterial;
            }
            model.idActD = idActD;
            return PartialView("modalAnalisa", model);
        }

        public IActionResult getItem(string idProyek, string jenis, int idActD, int idAnalisa)
        {
            var data = from a in db.materials
                       join b in db.analisas
                       on new { x1 = a.Id, x2 = idActD, x3 = idAnalisa }
                       equals new { x1 = b.idMaterial, x2 = b.idActD, x3 = b.Id }
                       into gp
                       from x in gp.DefaultIfEmpty()
                       where a.idProyek == idProyek && a.jenis == jenis
                       select new
                       {
                           id = a.Id,
                           text = string.Concat(a.item, " @ ", a.harga.ToString("n2"), "/", a.uom),
                           selected = x != null
                       };
            return Json(data);
        }

        public async Task<IActionResult> submitAnalisa(analisaParam data)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                if (data.id != 0)
                {
                    var model = db.analisas.FirstOrDefault(a => a.Id == data.id);
                    model.idActD = data.idActD;
                    model.idMaterial = data.idMaterial;
                    model.koefisien = data.koefisien;
                    model.UpdateBy = user;
                    model.UpdateDate = DateTime.Now;
                    db.analisas.Update(model);
                }
                else
                {
                    analisa model = new analisa();
                    model.idActD = data.idActD;
                    model.idMaterial = data.idMaterial;
                    model.koefisien = data.koefisien;
                    model.CreateBy = user;
                    model.CreateDate = DateTime.Now;
                    db.analisas.Add(model);
                }
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        public async Task<IActionResult> delAnalisa(int id)
        {
            try
            {
                var data = db.analisas.FirstOrDefault(a => a.Id == id);
                db.Remove(data);
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        public async Task<IActionResult> calcAnalisa(int idActD, double profit)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                var data = db.actDs.FirstOrDefault(a => a.Id == idActD);
                data.profit = profit;
                data.UpdateBy = user;
                data.UpdateDate = DateTime.Now;
                db.actDs.Update(data);
                await db.SaveChangesAsync();
                return Content("Ok");
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }
        #endregion

        #region "Jadwal"
        [Authorize(Roles = "1,3")]
        public IActionResult Jadwal(string idProyek)
        {
            // direct if idProyek empty
            if (string.IsNullOrEmpty(idProyek))
            {
                return RedirectToAction("index", "Project");
            }

            List<ActForJadwalView> model = new List<ActForJadwalView>();
            foreach (var data in db.actHs.Where(a => a.idProyek == idProyek).OrderBy(a => a.Seq).ToList())
            {
                model.Add(new ActForJadwalView
                {
                    id = data.Id,
                    idProyek = data.idProyek,
                    Seq = data.Seq,
                    kegiatan = data.kegiatan,
                    details = getPekerjaanOriginalId(data.Id)
                });
            }

            listActionForJadwal list = new listActionForJadwal();
            list.data = model;

            DateTime tglM = db.proyeks.Where(a => a.idProyek == idProyek).Select(a => a.tglMulai).FirstOrDefault();
            DateTime tglS = db.proyeks.Where(a => a.idProyek == idProyek).Select(a => a.tglSelesai).FirstOrDefault();

            int bulan = ((tglS.Year - tglM.Year) * 12) + tglS.Month - tglM.Month;

            ViewData["target"] = string.Concat(tglM.ToString("dd MMMM yyyy"), " s/d ", tglS.ToString("dd MMMM yyyy"));
            ViewData["bulan"] = bulan <= 0 ? 1 : bulan;
            ViewData["idProyek"] = idProyek;
            return PartialView("Jadwal", list);
        }

        public List<ActDForJadwalView> getPekerjaanOriginalId(int id)
        {
            int i = 1;
            List<ActDForJadwalView> model = new List<ActDForJadwalView>();
            var dataDetail = from a in db.actDs
                             join b in db.jadwals on a.Id equals b.idActD into gp
                             from x in gp.DefaultIfEmpty()
                             where a.header == id
                             orderby a.Id
                             select new
                             {
                                 a.Id,
                                 x.blocked,
                                 a.header,
                                 a.kegiatan
                             };

            foreach (var data in dataDetail.ToList())
            {
                model.Add(new ActDForJadwalView
                {
                    Id = i,
                    originalId = data.Id,
                    blocked = data.blocked,
                    header = data.header,
                    kegiatan = data.kegiatan
                });
                i++;
            }
            return model;
        }

        public async Task<IActionResult> submitJadwal(string idProyek, string jadwal)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                var jadwalDetail = JsonConvert.DeserializeObject<List<jadwalParam>>(jadwal);

                // drop 
                db.jadwals.RemoveRange(db.jadwals.Where(a => a.idProyek == idProyek));
                await db.SaveChangesAsync();

                // insert
                foreach (var data in jadwalDetail)
                {
                    if (!string.IsNullOrEmpty(data.Id) && data.Header != "0")
                    {
                        Jadwal model = new Jadwal();
                        model.idProyek = idProyek;
                        model.idActD = (int)Convert.ToInt64(data.Id);
                        model.header = (int)Convert.ToInt64(data.Header);
                        model.blocked = data.Blocked;
                        model.CreateBy = user;
                        model.CreateDate = DateTime.Now;

                        db.jadwals.Add(model);
                    }
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

        #region "Form Absensi"

        [Authorize(Roles = "1,2,3")]
        public IActionResult FormAbsen(int id)
        {
            try
            {
                var proyek = db.proyeks.FirstOrDefault(a => a.Id == id);
                var mandor = db.Users.FirstOrDefault(a => a.role == 2 && a.id == proyek.mandor);
                var absen = db.personils.Where(a => a.idProyek == proyek.idProyek).OrderBy(a => a.nama).ToList();

                var stream = new MemoryStream();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var pkg = new OfficeOpenXml.ExcelPackage(stream))
                {
                    ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Sheet1");
                    ws.Row(1).Style.Font.Bold = true;
                    ws.Row(2).Style.Font.Bold = true;
                    ws.Row(3).Style.Font.Bold = true;
                    ws.Row(5).Style.Font.Bold = true;

                    ws.Cells["A1:B1"].Merge = true;
                    ws.Cells["A2:B2"].Merge = true;
                    ws.Cells["A3:B3"].Merge = true;

                    ws.Cells[1, 1].Value = "NAMA PROYEK:";
                    ws.Cells[2, 1].Value = "MANDOR:";
                    ws.Cells[3, 1].Value = "PERIODE ABSENSI :";

                    ws.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ws.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ws.Cells["A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;


                    ws.Cells["C1:E1"].Merge = true;
                    ws.Cells["C2:E2"].Merge = true;
                    ws.Cells["C3:E3"].Merge = true;

                    ws.Cells[1, 3].Value = proyek.judul.ToUpper();
                    ws.Cells[2, 3].Value = mandor.fullname.ToUpper();
                    ws.Cells[3, 3].Value = "";
                    ws.Cells["C1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ws.Cells["C2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ws.Cells["C3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;


                    ws.Cells[5, 1].Value = "No";
                    ws.Cells[5, 2].Value = "Nama";
                    ws.Cells[5, 3].Value = "Telepon";
                    ws.Cells[5, 4].Value = "Mandays";

                    int row = 6;
                    int no = 1;
                    foreach (var data in absen)
                    {
                        ws.Cells[row, 1].Value = no;
                        ws.Cells[row, 2].Value = data.nama;
                        ws.Cells[row, 3].Value = data.telepon;
                        ws.Cells[row, 4].Value = string.Empty;
                        row++;
                        no++;
                    }

                    // note absen

                    pkg.Save();
                }
                stream.Position = 0;
                string filename = string.Concat("FormAbsensi_", proyek.idProyek ?? "Baru", ".xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }
        #endregion

        #region "Rekap"

        [Authorize(Roles = "1,3")]
        public IActionResult Rekap(int id)
        {
            var proyek = db.proyeks.FirstOrDefault(a => a.Id == id);
            ViewData["idProyek"] = proyek.idProyek;
            return View();
        }

        public IActionResult getPeriodRekap(string idProyek)
        {
            List<periodRekap> model = new List<periodRekap>();

            DateTime tglM = db.proyeks.Where(a => a.idProyek == idProyek).Select(a => a.tglMulai).FirstOrDefault();
            DateTime tglS = db.proyeks.Where(a => a.idProyek == idProyek).Select(a => a.tglSelesai).FirstOrDefault();

            int bulan = ((tglS.Year - tglM.Year) * 12) + tglS.Month - tglM.Month;

            for (int i = 0; i <= bulan; i++)
            {
                string prd = tglM.AddMonths(i).ToString("MM-yyyy");
                model.Add(new periodRekap
                {
                    id = prd,
                    text = prd,
                    selected = i == 0
                });
            }

            return Json(model);
        }



        // Biaya
        public IActionResult RekapBiaya(string idProyek, string period)
        {
            var proyek = db.proyeks.FirstOrDefault(a => a.idProyek == idProyek);

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


            List<rekapView> model = new List<rekapView>();
            List<rekapDView> detail = new List<rekapDView>();
            detail = getRekapActHBaseAtcD(_actH, _actD, _analisa, _material, _laporan, _laporanUsage, _laporanOverheadProfit, period);

            model.Add(new rekapView
            {
                judul = proyek.judul,
                alamat = proyek.alamat,
                pemilik = proyek.namaKlien,
                period = period,
                totalNilai = detail.Sum(a => a.nilai),
                totalProgressNilaiPerPeriod = detail.Sum(a => a.progressNilaiPerPeriod),
                totalProgressNilaiOverall = detail.Sum(a => a.progressNilaiOverall),
                totalBobot = detail.Sum(a => a.bobot),
                totalProgressBobotPerPeriod = detail.Sum(a => a.progressBobotPerPeriod),
                totalProgressBobotOverall = detail.Sum(a => a.progressBobotOverall),
                details = detail
            });

            listRekap list = new listRekap();
            list.data = model;

            double conv = Convert.ToInt64(model.Select(a => a.totalNilai).FirstOrDefault() / 10000) * 10000;

            long amt = Convert.ToInt64(conv);

            string terbilang = Terbilang(amt);

            ViewData["Terbilang"] = terbilang;

            return PartialView("RekapBiaya", list);
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
                    pekerjaan = data.kegiatan,
                    nilai = calcHargaRekapPerActH(actD, analisa, material, data.Id),
                    progressNilaiPerPeriod = calcProgressNilaiPerPeriod(laporan, laporanUsages, laporanOverheadProfits, data.Id, period),
                    progressNilaiOverall = calcProgressNilaiOverallActH(laporan, laporanUsages, laporanOverheadProfits, data.Id, period),
                    bobot = calcBobot(calcHargaRekapPerActH(actD, analisa, material, data.Id), calcHargaRekapPerIdProyek(actH,actD, analisa, material)),
                    progressBobotPerPeriod = calcBobot(calcProgressNilaiPerPeriod(laporan, laporanUsages, laporanOverheadProfits, data.Id, period), calcHargaRekapPerIdProyek(actH, actD, analisa, material)),
                    progressBobotOverall = calcBobot(calcProgressNilaiOverallActH(laporan, laporanUsages, laporanOverheadProfits, data.Id, period), calcHargaRekapPerIdProyek(actH, actD, analisa, material))
                });
                x++;
            }
            return model;
        }


        public double calcBobot(double harga, double totalHarga)
        {
            return harga / totalHarga * 100;
        }

        public double calcHargaRekapPerActH(List<ActD> actDs, List<analisa> analisas, List<Material> materials, int idActH)
        {
            double result = 0;
            foreach (var data in actDs.Where(a => a.header == idActH).ToList())
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

        public double calcHargaRekapPerActD(List<ActD> actDs,List<analisa> analisas, List<Material> materials, int idActD)
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

        public double calcHargaRekapPerIdProyek(List<ActH> actHs, List<ActD> actDs, 
            List<analisa> analisas, List<Material>  materials)
        {
            double result = 0;
            foreach (var header in actHs)
            {
                var ActD = actDs.Where(a => a.header == header.Id).ToList();
                foreach (var detail in ActD)
                {
                    var xdata = (from a in analisas
                                 join b in materials on a.idMaterial equals b.Id
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

        public double calcProgressNilaiOverallActD(List<Laporan> laporan, List<LaporanUsage> laporanUsages, 
            List<LaporanOverheadProfit> laporanOverheadProfits, int idActD, string period)
        {

            string[] periodx = period.Split('-');
            var EoD = DateTime.DaysInMonth(Convert.ToInt32(periodx[1]), Convert.ToInt32(periodx[0]));
            DateTime periodxx = DateTime.Parse(string.Concat(periodx[1].ToString(), "-", periodx[0].ToString(), "-", EoD.ToString()));

            // laporan detail id with ActD
            var laporans = from a in laporan
                           where a.idActD == idActD
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

        public double calcProgressNilaiPerPeriod(List<Laporan> laporan,  List<LaporanUsage> laporanUsages, 
            List<LaporanOverheadProfit> laporanOverheadProfits, int idActH, string period)
        {
            string[] periodx = period.Split('-');
            // laporan header id
            var laporans = from a in laporan
                           where a.idActH == idActH
                           && a.reportDate.Month.ToString() == periodx[0].ToString()
                           && a.reportDate.Year.ToString() == periodx[1].ToString()
                           select new
                           {
                               a.Id
                           };
            int[] headers = laporans.Select(a => a.Id).ToArray();

            var usage = laporanUsages.Where(a => headers.Contains(a.header)).ToList();
            var overhead = laporanOverheadProfits.Where(a => headers.Contains(a.header)).ToList();

            return usage.Sum(a => a.amount) + overhead.Sum(a => a.amount);
        }


        // WIP
        public IActionResult RekapWip(string idProyek, string period)
        {
            var proyek = db.proyeks.FirstOrDefault(a => a.idProyek == idProyek);
            List<rekapWipView> model = new List<rekapWipView>();

            // Temp Data
            var _actH = db.actHs.Where(a => a.idProyek == proyek.idProyek).OrderBy(a => a.Id).ToList();
            
            model.Add(new rekapWipView
            {
                judul = proyek.judul,
                alamat = proyek.alamat,
                pemilik = proyek.namaKlien,
                period = period,
                ActWip = getWipAct(_actH,idProyek,period)
            });

            listRekapWip list = new listRekapWip();
            list.data = model;
            return PartialView("RekapWip", list);
        }

        public async Task<IActionResult> RekapMaterial(string idProyek, string period)
        {
            var proyek = db.proyeks.FirstOrDefault(a => a.idProyek == idProyek);
            List<rekapMaterialView> model = new List<rekapMaterialView>();            
            model.Add(new rekapMaterialView
            {
                judul = proyek.judul,
                alamat = proyek.alamat,
                pemilik = proyek.namaKlien,
                period = period,
                ActMat = await getActMat(proyek.idProyek, period)
            });

            listRekapMaterial list = new listRekapMaterial();
            list.data = model;
            return PartialView("RekapMaterial", list);
        }

        public List<rekapWipActHView> getWipAct(List<ActH> actHs, string idProyek, string period)
        {
            int no = 1;
            List<rekapWipActHView> model = new List<rekapWipActHView>();

            // Temp Data
            var _actH = db.actHs.Where(a => a.idProyek == idProyek).OrderBy(a => a.Id).ToList();
            var _actD = db.actDs.Where(a => a.idProyek == idProyek).OrderBy(a => a.Id).ToList();
            var _laporan = db.laporans.Where(a => a.idProyek == idProyek).OrderBy(a => a.Id).ToList();
            var _laporanUsage = db.laporanUsages.Where(a => a.idProyek == idProyek).OrderBy(a => a.Id).ToList();
            var _laporanOverheadProfit = db.laporanOverheadProfits.Where(a => a.idProyek == idProyek).OrderBy(a => a.Id).ToList();
            var _material = db.materials.Where(a => a.idProyek == idProyek).OrderBy(a => a.Id).ToList();
            var _analisa = (from a in db.analisas
                            join b in db.actDs on a.idActD equals b.Id
                            join c in db.materials on a.idMaterial equals c.Id
                            where b.idProyek == idProyek
                            select new analisa
                            {
                                Id = a.Id,
                                idActD = a.idActD,
                                koefisien = a.koefisien,
                                idMaterial = a.idMaterial
                            }).ToList();

            foreach (var header in actHs)
            {
                List<rekapWipActDView> detail = new List<rekapWipActDView>();
                detail = getWipActD(_actH, _actD, _analisa, _material, _laporan, header.Id, period);

                model.Add(new rekapWipActHView
                {
                    nomor = no,
                    kegiatan = header.kegiatan,
                    totalJumlahHarga = detail.Sum(a => a.jumlahHarga),
                    totalBobot = detail.Sum(a => a.bobot),
                    totalWip = detail.Sum(a => a.wip),
                    totalAmount = detail.Sum(a => a.amount),
                    details = detail
                });

                no++;
            }
            return model;
        }

        public List<rekapWipActDView> getWipActD(List<ActH> actHs, List<ActD> actDs, List<analisa> analisas, 
            List<Material> materials, List<Laporan> laporans, int header, string period)
        {
            int no = 1;
            List<rekapWipActDView> model = new List<rekapWipActDView>();
            foreach (var detail in actDs.Where(a => a.header == header).OrderBy(a => a.Id).ToList())
            {
                model.Add(new rekapWipActDView
                {
                    nomor = no,
                    kegiatan = detail.kegiatan,
                    satuan = detail.uom,
                    volume = detail.volume,
                    hargaSatuan = calcHargaSatuanActDetail(analisas,materials, detail.Id, detail.profit),
                    jumlahHarga = calcHargaRekapPerActD(actDs,analisas,materials,detail.Id),
                    bobot = WipData1(laporans, detail.Id, period),                    
                    wip = WipData(laporans, detail.Id, calcHargaRekapPerActD(actDs, analisas, materials, detail.Id), 
                                    calcHargaRekapPerIdProyek(actHs,actDs,analisas,materials), period),
                    amount = WipAmount(calcHargaRekapPerIdProyek(actHs, actDs, analisas, materials),
                                    WipData(laporans, detail.Id, calcHargaRekapPerActD(actDs, analisas, materials, detail.Id),
                                            calcHargaRekapPerIdProyek(actHs, actDs, analisas, materials), period))
                });
                no++;
            }

            return model;
        }

        public double WipData1(List<Laporan> laporans, int idActD, string period)
        {
            string[] periodx = period.Split('-');
            var EoD = DateTime.DaysInMonth(Convert.ToInt32(periodx[1]), Convert.ToInt32(periodx[0]));
            DateTime periodxx = DateTime.Parse(string.Concat(periodx[1].ToString(), "-", periodx[0].ToString(), "-", EoD.ToString()));
            
            var wip = laporans.Where(a => a.idActD == idActD && a.reportDate <= periodxx.Date).OrderByDescending(a => a.reportDate).ToList();

            return wip.Count() > 0 ? wip.Select(a => a.wip).FirstOrDefault() : 0;
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

        public double WipAmount(double total, double bobot)
        {
            return total*bobot/100;
        }

        public async Task<List<rekapMaterialActHView>> getActMat(string idProyek, string period)
        {
            int no = 1;
            List<rekapMaterialActHView> model = new List<rekapMaterialActHView>();
            foreach (var header in await db.actHs.Where(a => a.idProyek == idProyek).OrderBy(a => a.Seq).ToListAsync())
            {
                model.Add(new rekapMaterialActHView
                {
                    nomor = no,
                    kegiatan = header.kegiatan,
                    details = await getActDMat(idProyek, header.Id, period)
                });

                no++;
            }
            return model;
        }

        public async Task<List<rekapMaterialActDView>> getActDMat(string idProyek, int header, string period)
        {
            int no = 1;
            List<rekapMaterialActDView> model = new List<rekapMaterialActDView>();
            foreach (var detail in await db.actDs.Where(a => a.idProyek == idProyek && a.header == header).OrderBy(a => a.Id).ToListAsync())
            {
                model.Add(new rekapMaterialActDView
                {
                    nomor = no,
                    kegiatan = detail.kegiatan,
                    items = await getActItemMat(idProyek, detail.Id, period)
                });

                no++;
            }
            return model;
        }

        public async Task<List<rekapMaterialItemView>> getActItemMat(string idProyek, int idActD, string period)
        {
            List<rekapMaterialItemView> model = new List<rekapMaterialItemView>();

            var data = await (from a in db.analisas
                       join b in db.actDs on a.idActD equals b.Id
                       join c in db.materials on a.idMaterial equals c.Id
                       where a.idActD == idActD && b.idProyek == idProyek
                       select new rekapMaterialItemView
                       {
                           idMaterial = a.idMaterial,
                           idActD = a.idActD,
                           item = c.item,
                           uom = c.uom,
                           qtyBudget = a.koefisien * b.volume,
                           amountBudget = (a.koefisien * c.harga) * b.volume                           
                       }).ToListAsync();

            foreach (var d in data)
            {
                model.Add(new rekapMaterialItemView
                {
                    item = d.item,
                    uom = d.uom,
                    qtyBudget = d.qtyBudget,
                    amountBudget = d.amountBudget,
                    qtyUsage = usageDataQty(d.idMaterial, d.idActD, period),
                    amountUsage = usageDataAmt(d.idMaterial, d.idActD, period)
                });
            }

            return model;
        }

        // set use pariod
        public double usageDataQty(int idMaterial, int idActD, string period)
        {

            string[] periodx = period.Split('-');
            var EoD = DateTime.DaysInMonth(Convert.ToInt32(periodx[1]), Convert.ToInt32(periodx[0]));
            DateTime periodxx = DateTime.Parse(string.Concat(periodx[1].ToString(), "-", periodx[0].ToString(), "-", EoD.ToString()));
 
            var data = (from a in db.laporanUsages
                       join b in db.laporans on a.header equals b.Id
                       where b.idActD == idActD && a.idMaterial == idMaterial
                       && b.reportDate.Date <= periodxx.Date
                       select new
                       {
                           qty = a.qty
                       }).ToList();

            return data.Sum(a => a.qty);
        }

        public double usageDataAmt(int idMaterial, int idActD, string period)
        {
            string[] periodx = period.Split('-');
            var EoD = DateTime.DaysInMonth(Convert.ToInt32(periodx[1]), Convert.ToInt32(periodx[0]));
            DateTime periodxx = DateTime.Parse(string.Concat(periodx[1].ToString(), "-", periodx[0].ToString(), "-", EoD.ToString()));

            var data = (from a in db.laporanUsages
                       join b in db.laporans on a.header equals b.Id
                       where b.idActD == idActD && a.idMaterial == idMaterial
                       && b.reportDate.Date <= periodxx.Date
                       select new
                       {
                           amount = a.amount
                       }).ToList();

            return data.Sum(a => a.amount);
        }

        public IActionResult RekapProgressBiaya(string idProyek, string period)
        {
            var proyek = db.proyeks.FirstOrDefault(a => a.idProyek == idProyek);

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

            List<rekapView> model = new List<rekapView>();
            List<rekapDView> detail = new List<rekapDView>();
            detail = getRekapActHBaseAtcD(_actH,_actD,_analisa,_material,_laporan,_laporanUsage,_laporanOverheadProfit,period);

            model.Add(new rekapView
            {
                judul = proyek.judul,
                alamat = proyek.alamat,
                pemilik = proyek.namaKlien,
                period = period,
                totalNilai = detail.Sum(a => a.nilai),
                totalProgressNilaiPerPeriod = detail.Sum(a => a.progressNilaiPerPeriod),
                totalProgressNilaiOverall = detail.Sum(a => a.progressNilaiOverall),
                totalBobot = detail.Sum(a => a.bobot),
                totalProgressBobotPerPeriod = detail.Sum(a => a.progressBobotPerPeriod),
                totalProgressBobotOverall = detail.Sum(a => a.progressBobotOverall),
                details = detail
            });

            listRekap list = new listRekap();
            list.data = model;
            return PartialView("RekapProgressBiaya", list);
        }


        public static string Terbilang(long a)
        {
            string[] bilangan = new string[] { "", "Satu", "Dua", "Tiga", "Empat", "Lima", "Enam", "Tujuh", "Delapan", "Sembilan", "Sepuluh", "Sebelas" };
            var kalimat = "";
            // 1 - 11
            if (a < 12)
            {
                kalimat = bilangan[a];
            }
            // 12 - 19
            else if (a < 20)
            {
                kalimat = bilangan[a - 10] + " Belas";
            }
            // 20 - 99
            else if (a < 100)
            {
                var utama = a / 10;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 1));
                var belakang = a % 10;
                kalimat = bilangan[depan] + " Puluh " + bilangan[belakang];
            }
            // 100 - 199
            else if (a < 200)
            {
                kalimat = "Seratus " + Terbilang(a - 100);
            }
            // 200 - 999
            else if (a < 1000)
            {
                var utama = a / 100;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 1));
                var belakang = a % 100;
                kalimat = bilangan[depan] + " Ratus " + Terbilang(belakang);
            }
            // 1,000 - 1,999
            else if (a < 2000)
            {
                kalimat = "Seribu " + Terbilang(a - 1000);
            }
            // 2,000 - 9,999
            else if (a < 10000)
            {
                var utama = a / 1000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 1));
                var belakang = a % 1000;
                kalimat = bilangan[depan] + " Ribu " + Terbilang(belakang);
            }
            // 10,000 - 99,999
            else if (a < 100000)
            {
                var utama = a / 100;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 2));
                var belakang = a % 1000;
                kalimat = Terbilang(depan) + " Ribu " + Terbilang(belakang);
            }
            // 100,000 - 999,999
            else if (a < 1000000)
            {
                var utama = a / 1000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 3));
                var belakang = a % 1000;
                kalimat = Terbilang(depan) + " Ribu " + Terbilang(belakang);
            }
            // 1,000,000 - 	99,999,999
            else if (a < 100000000)
            {
                var utama = a / 1000000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 1));//Substring(0, 4));
                var belakang = a % 1000000;
                kalimat = Terbilang(depan) + " Juta " + Terbilang(belakang);
            }
            else if (a < 1000000000)
            {
                var utama = a / 1000000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 3));
                var belakang = a % 1000000;
                kalimat = Terbilang(depan) + " Juta " + Terbilang(belakang);
            }
            else if (a < 10000000000)
            {
                var utama = a / 1000000000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 1));
                var belakang = a % 1000000000;
                kalimat = Terbilang(depan) + " Milyar " + Terbilang(belakang);
            }
            else if (a < 100000000000)
            {
                var utama = a / 1000000000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 2));
                var belakang = a % 1000000000;
                kalimat = Terbilang(depan) + " Milyar " + Terbilang(belakang);
            }
            else if (a < 1000000000000)
            {
                var utama = a / 1000000000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 3));
                var belakang = a % 1000000000;
                kalimat = Terbilang(depan) + " Milyar " + Terbilang(belakang);
            }
            else if (a < 10000000000000)
            {
                var utama = a / 10000000000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 1));
                var belakang = a % 10000000000;
                kalimat = Terbilang(depan) + " Triliun " + Terbilang(belakang);
            }
            else if (a < 100000000000000)
            {
                var utama = a / 1000000000000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 2));
                var belakang = a % 1000000000000;
                kalimat = Terbilang(depan) + " Triliun " + Terbilang(belakang);
            }

            else if (a < 1000000000000000)
            {
                var utama = a / 1000000000000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 3));
                var belakang = a % 1000000000000;
                kalimat = Terbilang(depan) + " Triliun " + Terbilang(belakang);
            }

            else if (a < 10000000000000000)
            {
                var utama = a / 1000000000000000;
                var depan = Convert.ToInt32(utama.ToString().Substring(0, 1));
                var belakang = a % 1000000000000000;
                kalimat = Terbilang(depan) + " Kuadriliun " + Terbilang(belakang);
            }

            var pisah = kalimat.Split(' ');
            List<string> full = new List<string>();// = [];
            for (var i = 0; i < pisah.Length; i++)
            {
                if (pisah[i] != "") { full.Add(pisah[i]); }
            }
            return CombineTerbilang(full.ToArray());// full.Concat(' '); .join(' ');
        }
        static string CombineTerbilang(string[] arr)
        {
            return string.Join(" ", arr);
        }

        #endregion

        #region "Monitoring"
        [Authorize(Roles = "1,2,3")]
        public IActionResult Monitoring(int id)
        {
            var proyek = db.proyeks.FirstOrDefault(a => a.Id == id);
            List<ActForJadwalView> model = new List<ActForJadwalView>();
            foreach (var data in db.actHs.Where(a => a.idProyek == proyek.idProyek).OrderBy(a => a.Seq).ToList())
            {
                model.Add(new ActForJadwalView
                {
                    id = data.Id,
                    idProyek = data.idProyek,
                    Seq = data.Seq,
                    kegiatan = data.kegiatan,
                    details = getPekerjaanOriginalId(data.Id)
                });
            }

            listActionForJadwal list = new listActionForJadwal();
            list.data = model;

            DateTime tglM = db.proyeks.Where(a => a.idProyek == proyek.idProyek).Select(a => a.tglMulai).FirstOrDefault();
            DateTime tglS = db.proyeks.Where(a => a.idProyek == proyek.idProyek).Select(a => a.tglSelesai).FirstOrDefault();

            int bulan = ((tglS.Year - tglM.Year) * 12) + tglS.Month - tglM.Month;

            ViewData["target"] = string.Concat(tglM.ToString("dd MMMM yyyy"), " s/d ", tglS.ToString("dd MMMM yyyy"));
            ViewData["bulan"] = bulan <= 0 ? 1 : bulan;
            ViewData["idProyek"] = proyek.idProyek;
            ViewData["judulProyek"] = proyek.judul;
            return View(list);
        }

        public IActionResult ProgressActD(int idActD)
        {
            var data = db.actDs.Where(a => a.Id == idActD).FirstOrDefault();
            ViewData["pekerjaan"] = data.kegiatan;

            // Temp Data
            var _actH = db.actHs.Where(a => a.idProyek == data.idProyek).OrderBy(a => a.Id).ToList();
            var _actD = db.actDs.Where(a => a.idProyek == data.idProyek).OrderBy(a => a.Id).ToList();
            var _laporan = db.laporans.Where(a => a.idProyek == data.idProyek).OrderBy(a => a.Id).ToList();
            var _laporanUsage = db.laporanUsages.Where(a => a.idProyek == data.idProyek).OrderBy(a => a.Id).ToList();
            var _laporanOverheadProfit = db.laporanOverheadProfits.Where(a => a.idProyek == data.idProyek).OrderBy(a => a.Id).ToList();
            var _material = db.materials.Where(a => a.idProyek == data.idProyek).OrderBy(a => a.Id).ToList();
            var _analisa = (from a in db.analisas
                            join b in db.actDs on a.idActD equals b.Id
                            join c in db.materials on a.idMaterial equals c.Id
                            where b.idProyek == data.idProyek
                            select new analisa
                            {
                                Id = a.Id,
                                idActD = a.idActD,
                                koefisien = a.koefisien,
                                idMaterial = a.idMaterial
                            }).ToList();

            // RAB
            ViewData["NilaiRAB"] = calcHargaRekapPerActD(_actD,_analisa,_material,idActD).ToString("n2");
            ViewData["BobotRAB"] = string.Concat((calcBobot(calcHargaRekapPerActD(_actD, _analisa, _material, idActD), 
                                            calcHargaRekapPerIdProyek(_actH,_actD,_analisa,_material))).ToString("n2"), "%");

            // progress 
            // get max periode
            DateTime tglM = db.proyeks.Where(a => a.idProyek == data.idProyek).Select(a => a.tglMulai).FirstOrDefault();
            DateTime tglS = db.proyeks.Where(a => a.idProyek == data.idProyek).Select(a => a.tglSelesai).FirstOrDefault();
            Dictionary<int, string> periodTemp = new Dictionary<int, string>();

            int bulan = ((tglS.Year - tglM.Year) * 12) + tglS.Month - tglM.Month;

            for (int i = 0; i <= bulan; i++)
            {
                string prd = tglM.AddMonths(i).ToString("MM-yyyy");
                periodTemp.Add(i, prd);
            }

            string period = periodTemp[bulan];
            ViewData["NilaiProgress"] = calcProgressNilaiOverallActD(_laporan,_laporanUsage,_laporanOverheadProfit,idActD, period).ToString("n2");
            ViewData["BobotProgress"] = string.Concat((calcBobot(calcProgressNilaiOverallActD(_laporan, _laporanUsage, _laporanOverheadProfit, idActD, period),
                calcHargaRekapPerIdProyek(_actH, _actD, _analisa, _material))).ToString("n2"), "%");

            return PartialView("ProgressActD");
        }
        #endregion

        #region "Duplicate"
        [Authorize(Roles = "1,3")]
        public async Task<IActionResult> Duplicate(int id)
        {
            try
            {
                int user = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value);
                List<ActHTemp> ActHTemp = new List<ActHTemp>();
                List<ActDTemp> ActDTemp = new List<ActDTemp>();
                List<MaterialTemp> MaterialTemp = new List<MaterialTemp>();

                string idProyek = string.Concat(DateTime.Now.ToString("yyyyMM"), Guid.NewGuid().ToString().ToUpper().Substring(1, 5));

                // Proyek
                var proyeks = db.proyeks.FirstOrDefault(a => a.Id == id);
                Proyek modelProyek = new Proyek
                {
                    idProyek = idProyek,
                    judul = string.Concat(proyeks.judul, "_Copy"),
                    mandor = proyeks.mandor,
                    tglMulai = proyeks.tglMulai,
                    tglSelesai = proyeks.tglSelesai,
                    kontrak = proyeks.kontrak,
                    desain = proyeks.desain,
                    isActive = false,
                    alamat = proyeks.alamat,
                    namaKlien = proyeks.namaKlien,
                    CreateBy = user,
                    CreateDate = DateTime.Now
                };
                db.proyeks.Add(modelProyek);
                await db.SaveChangesAsync();

                // ActH
                var actHs = db.actHs.Where(a => a.idProyek == proyeks.idProyek).OrderBy(a => a.Seq).ToList();
                foreach (var actH in actHs)
                {
                    ActH modelActH = new ActH
                    {
                        idProyek = idProyek,
                        kegiatan = actH.kegiatan,
                        Seq = actH.Seq,
                        CreateBy = user,
                        CreateDate = DateTime.Now
                    };
                    db.actHs.Add(modelActH);
                    await db.SaveChangesAsync();

                    // ActH Temp
                    ActHTemp.Add(new ActHTemp
                    {
                        idActH_Old = actH.Id,
                        idActH_New = modelActH.Id
                    });
                }

                // ActD (use list temp to compare id)
                var actDs = db.actDs.Where(a => a.idProyek == proyeks.idProyek).OrderBy(a => a.Id).ToList();
                foreach (var actD in actDs)
                {
                    ActD modelActD = new ActD
                    {
                        idProyek = idProyek,
                        kegiatan = actD.kegiatan,
                        volume = actD.volume,
                        uom = actD.uom,
                        header = ActHTemp.FirstOrDefault(a => a.idActH_Old == actD.header).idActH_New,
                        profit = actD.profit,
                        CreateBy = user,
                        CreateDate = DateTime.Now
                    };
                    db.actDs.Add(modelActD);
                    await db.SaveChangesAsync();

                    // ActD Temp
                    ActDTemp.Add(new ActDTemp
                    {
                        idActD_Old = actD.Id,
                        idActD_New = modelActD.Id
                    });
                }

                // Material
                var materials = db.materials.Where(a => a.idProyek == proyeks.idProyek).ToList();
                foreach (var material in materials)
                {
                    Material modelMat = new Material
                    {
                        idProyek = idProyek,
                        item = material.item,
                        uom = material.uom,
                        harga = material.harga,
                        jenis = material.jenis,
                        CreateBy = user,
                        CreateDate = DateTime.Now
                    };
                    db.materials.Add(modelMat);
                    await db.SaveChangesAsync();

                    // Material Temp
                    MaterialTemp.Add(new MaterialTemp
                    {
                        idMaterial_Old = material.Id,
                        idMaterial_New = modelMat.Id
                    });
                }

                // Personil (use list temp to compare id)
                var personils = db.personils.Where(a => a.idProyek == proyeks.idProyek).ToList();
                var xPersonils = from a in personils
                                 join b in MaterialTemp on new { x1 = a.role, x2 = a.idProyek } equals new { x1 = b.idMaterial_Old, x2 = proyeks.idProyek }                                 
                                select new
                                {
                                    idProyek = a.idProyek,
                                    akunBank = a.akunBank,
                                    nama = a.nama,
                                    telepon = a.telepon,
                                    role = b.idMaterial_New,
                                    CreateBy = user,
                                    CreateDate = DateTime.Now
                                };

                foreach (var personil in xPersonils.ToList())
                {
                    Personil modelPersonil = new Personil
                    {
                        idProyek = personil.idProyek,
                        akunBank = personil.akunBank,
                        nama = personil.nama,
                        telepon = personil.telepon,
                        role = personil.role,
                        CreateBy = personil.CreateBy,
                        CreateDate = personil.CreateDate
                    };
                    db.personils.Add(modelPersonil);                    
                }
                await db.SaveChangesAsync();


                // Analisa (use list temp to compare id)
                var Analisas = (from a in db.analisas
                               join b in db.actDs on a.idActD equals b.Id
                               where b.idProyek == proyeks.idProyek
                               select new
                               {
                                   a.idActD,
                                   a.idMaterial,
                                   a.koefisien
                               }).ToList();

                var xAnalisas = from a in Analisas
                               join b in ActDTemp on a.idActD equals b.idActD_Old
                               join c in MaterialTemp on a.idMaterial equals c.idMaterial_Old
                               select new
                               {
                                   idActD = b.idActD_New,
                                   idMaterial = c.idMaterial_New,
                                   koefisien = a.koefisien,
                                   CreateBy = user,
                                   CreateDate = DateTime.Now
                               };

                foreach (var analisa in xAnalisas.ToList())
                {
                    analisa modelAnalisa = new analisa
                    {
                        idActD = analisa.idActD,
                        idMaterial = analisa.idMaterial,
                        koefisien = analisa.koefisien,
                        CreateBy = analisa.CreateBy,
                        CreateDate = analisa.CreateDate
                    };
                    db.analisas.Add(modelAnalisa);
                }
                await db.SaveChangesAsync();


                // Jadwal (use list temp to compare id)
                var jadwals = db.jadwals.Where(a => a.idProyek == proyeks.idProyek).ToList();
                var xjadwals = from a in jadwals
                              join b in ActHTemp on a.header equals b.idActH_Old
                              join c in ActDTemp on a.idActD equals c.idActD_Old
                              select new
                              {
                                  idProyek = a.idProyek,
                                  blocked = a.blocked,
                                  header = b.idActH_New,
                                  idActD = c.idActD_New,
                                  CreateBy = user,
                                  CreateDate = DateTime.Now
                              };

                foreach (var jadwal in xjadwals.ToList())
                {
                    Jadwal modelJad = new Jadwal
                    {
                        idProyek = jadwal.idProyek,
                        blocked = jadwal.blocked,
                        header = jadwal.header,
                        idActD = jadwal.idActD,
                        CreateBy = jadwal.CreateBy,
                        CreateDate = jadwal.CreateDate
                    };
                    db.jadwals.Add(modelJad);
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
    }
}
