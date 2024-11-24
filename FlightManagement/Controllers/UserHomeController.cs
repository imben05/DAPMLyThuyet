using FlightManagement.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static FlightManagement.Models.SearchHotel;
using static FlightManagement.Models.SearchFlight;

namespace FlightManagement.Controllers
{
    public class UserHomeController : Controller
    {
        private readonly DAPMLThuyetEntities _context;
        DAPMLThuyetEntities database = new DAPMLThuyetEntities();
        // GET: UserHome
        public ActionResult Index()
        {
            return View();
        }
        //code chưa gọn, còn hao tốn dữ liệu, làm select từng cái khi sử dụng chứ không phải select hết tất cả list rồi chọn cụ thể
        //Cô Thùy An
        public ActionResult SearchHotel(string NameHotel, string Location, DateTime? depatureTime, DateTime? returnTime,decimal? RoompPrice)
            {
            // Lấy danh sách chuyến bay từ cơ sở dữ liệu
            var HoteltList = database.Hotels.AsQueryable(); // AsQueryable() cho phép linh hoạt với các truy vấn

            // Lọc theo vị trí
            if (!string.IsNullOrEmpty(Location))
            {
                HoteltList = HoteltList.Where(x => x.Location.Contains(Location));
            }

            // Lọc theo tên hotel
            if (!string.IsNullOrEmpty(NameHotel))
            {
                HoteltList = HoteltList.Where(x => x.NameHotel.Contains(NameHotel));
            }

            // Lọc theo thời gian book
            if (depatureTime.HasValue)
            {
                DateTime startOfDay = depatureTime.Value.Date;
                DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);
                HoteltList = HoteltList.Where(x => x.depatureTime >= startOfDay && x.depatureTime <= endOfDay);
            }

            // Lọc theo thời gian trả
            if (returnTime.HasValue)
            {
                DateTime startOfDay = returnTime.Value.Date;
                DateTime endOfDay = startOfDay.AddDays(1).AddTicks(-1);
                HoteltList = HoteltList.Where(x => x.returnTime >= startOfDay && x.returnTime <= endOfDay);
            }
            if (!HoteltList.Any())
            {
                ViewBag.Message = "Không tìm thấy khách sạn nào phù hợp với tiêu chí!";
            }
            return View("SearchHotel", HoteltList.ToList());
        }
        public ActionResult BookHotel(int? hotelId)
        {
            if (hotelId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingHotel booking = database.BookingHotels.Find(hotelId);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdHotel = new SelectList(database.Hotels, "IdHotel", "NameHotel");

            return View(booking);
        }

        [HttpPost]
        public ActionResult BookHotel(int? hotelId, DateTime checkInDate, DateTime checkOutDate, int roomCount)
        {
            //var userId = // Lấy ID người dùng;
            var booking = new BookingHotel
            {
                IdHotel = hotelId,
                CheckinDate = checkInDate,
                CheckoutDate = checkOutDate,
                RoomCount = roomCount,
                BookingDate = DateTime.Now
            };

            database.BookingHotels.Add(booking);
            database.SaveChanges();

            ViewBag.Message = "Đặt phòng thành công!";
            return View("BookingSuccess");
        }
    }

}