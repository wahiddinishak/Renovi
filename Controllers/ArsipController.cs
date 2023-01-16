using Microsoft.AspNetCore.Mvc;
using renovi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Controllers
{
    public class ArsipController : Controller
    {
        private readonly AppDbContext db;
       
        public ArsipController(AppDbContext context)
        {
            db = context;
        }

        public IActionResult ArsipList(int id)
        {

            var ActD = db.actDs.FirstOrDefault(a => a.Id == id);
            var laporan = db.laporans.Where(a => a.idActD == id).OrderBy(a => a.reportDate).ToList();

            List<ArsipView> model = new List<ArsipView>();
            foreach (var data in laporan)
            {
                // Temp Data
                var _actH = db.actHs.Where(a => a.idProyek == data.idProyek).OrderBy(a => a.Id).ToList();
                var _actD = db.actDs.Where(a => a.idProyek == data.idProyek).OrderBy(a => a.Id).ToList();
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


                model.Add(new ArsipView
                {
                    idReport = data.Id,
                    reportDate = data.reportDate,
                    nilai = calcProgressNilai(_laporanUsage, _laporanOverheadProfit, data.Id),
                    bobot = calcBobot(calcProgressNilai(_laporanUsage, _laporanOverheadProfit, data.Id), 
                                        calcHargaRekapPerIdProyek(_actH, _actD, _analisa, _material))
                });

            }

            listArsip list = new listArsip();
            list.data = model;
            list.totalNiali = model.Sum(a => a.nilai);
            list.totalBobot = model.Sum(a => a.bobot);
            list.judulProyek = db.proyeks.FirstOrDefault(a => a.idProyek == ActD.idProyek).judul;
            list.pekerjaan = db.actHs.FirstOrDefault(a => a.Id == ActD.header).kegiatan;
            list.detailPekerjaan = ActD.kegiatan;
            list.aiProyek = db.proyeks.FirstOrDefault(a => a.idProyek == ActD.idProyek).Id;
            return View(list);
        }

        public IActionResult ReportDetail(int idReport)
        {
            List<LaporanUsageView> model = new List<LaporanUsageView>();
            var data = from c in db.materials
                       join d in db.laporanUsages on c.Id equals d.idMaterial
                       join e in db.laporans on d.header equals e.Id
                       where e.Id == idReport
                       orderby e.reportDate ascending
                       select new LaporanUsageView
                       {
                           materialInfo = string.Concat(c.item, " @ ", c.harga.ToString("n2"), "/", c.uom),
                           qty = d.qty,
                           amount = d.amount
                       };

            foreach (var d in data.ToList())
            {
                model.Add(new LaporanUsageView
                {
                    materialInfo = d.materialInfo,
                    qty = d.qty,
                    amount = d.amount
                });
            }

            ViewBag.listLaporanUsage = model;

            var overhead = db.laporanOverheadProfits.Where(a => a.header == idReport).FirstOrDefault();
            ViewData["info"] = overhead != null ? overhead.info : "";
            ViewData["amount"] = overhead != null ?  overhead.amount.ToString("n2") : "0.00";

            ViewBag.Nota = db.laporanAttachments.Where(a => a.header == idReport && a.jenis == "N").OrderBy(a => a.Id).ToList();
            ViewBag.Progress = db.laporanAttachments.Where(a => a.header == idReport && a.jenis == "P").OrderBy(a => a.Id).ToList();

            return PartialView("ReportDetail");
        }

        public double calcBobot(double harga, double totalHarga)
        {
            return harga / totalHarga * 100;
        }

        public double calcHargaRekapPerIdProyek(List<ActH> actHs, List<ActD> actDs, 
            List<analisa> analisas, List<Material> materials)
        {
            double result = 0;
            foreach (var header in actHs)
            {
                foreach (var detail in actDs)
                {
                    var xdata = from a in analisas
                                join b in materials on a.idMaterial equals b.Id
                                where a.idActD == detail.Id
                                select new
                                {
                                    harga = b.harga,
                                    koefisien = a.koefisien
                                };

                    result += (xdata.Sum(a => a.harga * a.koefisien) + xdata.Sum(a => a.harga * a.koefisien) * detail.profit / 100) * detail.volume;
                }
            }
            return result;
        }

        public double calcProgressNilai(List<LaporanUsage> laporanUsages, 
            List<LaporanOverheadProfit> laporanOverheadProfits, 
            int idReport)
        {
            var usage = laporanUsages.Where(a => a.header == idReport).ToList();
            var overhead = laporanOverheadProfits.Where(a => a.header == idReport).ToList();

            return usage.Sum(a => a.amount) + overhead.Sum(a => a.amount);
        }


    }
}
