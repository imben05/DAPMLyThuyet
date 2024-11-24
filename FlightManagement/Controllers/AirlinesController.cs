using FlightManagement.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlightManagement.Controllers
{
    public class AirlinesController : Controller
    {
        DAPMLThuyetEntities database = new DAPMLThuyetEntities();

        // GET: Airlines
        public ActionResult Index()
        {
            var airAD = database.Airlines.ToList();
            return View(airAD);
        }

        // GET: Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "airlineID,airlineName,code,logoUrl")] Airline airline, HttpPostedFileBase logoUrl)
        {
            if (ModelState.IsValid)
            {
                // Đảm bảo ID không bị thiết lập
                airline.airlineID = 0; // Không cần thiết nếu IDENTITY_INSERT là OFF

                // Kiểm tra nếu người dùng không cung cấp logo
                if (logoUrl != null && logoUrl.ContentLength > 0)
                {
                    // Tạo tên file duy nhất
                    var fileName = Path.GetFileName(logoUrl.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Image"), fileName);

                    // Lưu ảnh vào thư mục
                    logoUrl.SaveAs(path);

                    // Lưu đường dẫn vào thuộc tính logoUrl của mô hình
                    airline.logoUrl = "~/Content/Image/" + fileName;
                }

                // Thêm airline vào cơ sở dữ liệu và lưu thay đổi
                database.Airlines.Add(airline);
                database.SaveChanges();
                TempData["SuccessMessage"] = "Tạo thành công!";
                return RedirectToAction("Index");
            }
            return View(airline);
        }

        // GET: Edit
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Airline airline = database.Airlines.FirstOrDefault(p => p.airlineID == id);
            if (airline != null)
            {
                return View(airline);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "airlineID,airlineName,code,logoUrl")] Airline airline, HttpPostedFileBase logoUrl)
        {
            if (ModelState.IsValid)
            {
                var airlineDB = database.Airlines.FirstOrDefault(p => p.airlineID == airline.airlineID);
                if (airlineDB != null)
                {
                    airlineDB.airlineName = airline.airlineName;
                    airlineDB.code = airline.code;

                    // Nếu người dùng tải lên logo mới
                    if (logoUrl != null && logoUrl.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(logoUrl.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Image"), fileName);

                        // Lưu ảnh vào thư mục
                        logoUrl.SaveAs(path);

                        // Cập nhật đường dẫn logoUrl trong cơ sở dữ liệu
                        airlineDB.logoUrl = "~/Content/Image/" + fileName;
                        database.SaveChanges();
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    database.SaveChanges();
                    TempData["SuccessMessage"] = "Chỉnh sửa thành công!";
                    return RedirectToAction("Index");
                }
            }
            return View(airline);
        }

        // GET: Delete
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Airline airline = database.Airlines.FirstOrDefault(p => p.airlineID == id);
            if (airline != null)
                return View(airline);
            else return RedirectToAction("Index");
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            int i = int.Parse(id);
            var airlineDB = database.Airlines.FirstOrDefault(p => p.airlineID == i);
            if (airlineDB != null)
            {
                database.Airlines.Remove(airlineDB);
                TempData["SuccessMessage"] = "Xóa thành công!";
                database.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
