using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FlightManagement.Controllers
{
    public class HotelController : Controller
    {
        // GET: Hotel
        DAPMLThuyetEntities database = new DAPMLThuyetEntities();
        // GET: TrangChu
        public ActionResult Index()
        {
            //var Sach = database.Hotels.Include(c => c.TacGia).Include(c => c.TheLoai).ToList();
            return View(database.Hotels.ToList());
        }
        public ActionResult Them()
        {
            //lấy dữ liệu các bảng khác để thêm
            //ViewBag.MaTacGia = new SelectList(database.TacGias, "MaTacGia", "TenTacGia");
            //ViewBag.MaTheLoai = new SelectList(database.TheLoais, "MaTheLoai", "TenTheLoai");
            //ViewBag.MaNhaCungCap = new SelectList(database.NhaCungCaps, "MaNhaCungCap", "TenNhaCungCap");
            /*Thả danh sách các tác giả có sẵn*/
            return View();
        }
        [HttpPost] //ghi nhận dữ liệu
        [ValidateAntiForgeryToken]
        public ActionResult Them(Hotel Ks, HttpPostedFileBase HinhAnh)
        {
            if (ModelState.IsValid)
            {
                if (HinhAnh != null && HinhAnh.ContentLength > 0)
                {
                    // Tạo tên file ảnh duy nhất
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Image"), fileName);

                    // Lưu ảnh vào thư mục
                    HinhAnh.SaveAs(path);

                    // Lưu đường dẫn vào thuộc tính HinhAnh của mô hình
                    Ks.HinhAnh = "~/Content/Image/" + fileName;
                }
                // Thêm sách vào cơ sở dữ liệu và lưu thay đổi
                database.Hotels.Add(Ks);
                database.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(Ks);
            //database.Saches.Add(sach); //Hàm thêm data 
            //database.SaveChanges(); //Lưu vào data
            //return RedirectToAction("Index"); //Lưu xong điều hướng 
        }

        public ActionResult Details(int id)
        {
            return View(database.Hotels.Where(s => s.IdHotel == id).FirstOrDefault());
        }
        [HttpGet]
        public ActionResult Edit(int id)

        {
            Hotel Ks = database.Hotels.FirstOrDefault(p => p.IdHotel == id);
            if (Ks != null)
            {
                return View(Ks);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase HinhAnh, [Bind(Include = "IdHotel,NameHotel,Address,Phone,StarRange,Decripstion,RoomPrice,Location,HinhAnh")] Hotel Ks)
        {
            if (ModelState.IsValid)
            {
                var KSdb = database.Hotels.FirstOrDefault(p => p.IdHotel == Ks.IdHotel);

                if (KSdb != null)
                {
                    // Cập nhật thông tin khách sạn
                    KSdb.NameHotel = Ks.NameHotel;
                    KSdb.Address = Ks.Address;
                    KSdb.Phone = Ks.Phone;
                    KSdb.StarRange = Ks.StarRange;
                    KSdb.Decripstion = Ks.Decripstion;
                    KSdb.Location = Ks.Location;
                    KSdb.RoomPrice = Ks.RoomPrice;

                    // Kiểm tra nếu có ảnh mới được tải lên
                    if (HinhAnh != null && HinhAnh.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(HinhAnh.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Image"), fileName);

                        // Tạo thư mục nếu chưa tồn tại
                        var imagePath = Server.MapPath("~/Content/Image");
                        if (!Directory.Exists(imagePath))
                        {
                            Directory.CreateDirectory(imagePath);
                        }

                        // Lưu ảnh vào thư mục
                        HinhAnh.SaveAs(path);

                        // Cập nhật đường dẫn hình ảnh
                        KSdb.HinhAnh = "~/Content/Image/" + fileName;
                    }

                    // Lưu tất cả thay đổi vào cơ sở dữ liệu
                    database.SaveChanges();
                    TempData["SuccessMessage"] = "Chỉnh sửa thành công!";
                    return RedirectToAction("Index");
                }
            }
            return View(Ks);
        }


        public ActionResult Delete(int id)
        {
            return View(database.Hotels.Where(s => s.IdHotel == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Delete(int id, Hotel Ks)
        {
            Ks = database.Hotels.Where((s) => s.IdHotel == id).FirstOrDefault();
            database.Hotels.Remove(Ks);
            database.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}