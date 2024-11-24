using FlightManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FlightManagement.Controllers
{
    public class BookingController : Controller
    {
        private readonly DAPMLThuyetEntities _context;
        private static readonly HttpClient _httpClient = new HttpClient();
        public BookingController(DAPMLThuyetEntities context)
        {
            _context = context;
        }

        // GET: Bookings
        public ActionResult Index()
        {
            var bookAD = _context.Bookings.ToList();
            return View(bookAD);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.AccountID = new SelectList(_context.Accounts, "accountID", "firstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bookingID,bookingDate,totalAmount,status,accountID")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.bookingID = 0;
                _context.Bookings.Add(booking);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Tạo thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.AccountID = new SelectList(_context.Accounts, "accountID", "firstName", booking.accountID);
            return View(booking);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Booking booking = _context.Bookings.FirstOrDefault(p => p.bookingID == id);
            if (booking != null)
            {
                ViewBag.AccountID = new SelectList(_context.Accounts, "accountID", "firstName", booking.accountID);
                return View(booking);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "bookingID,bookingDate,totalAmount,status,accountID")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var bookingDB = _context.Bookings.FirstOrDefault(p => p.bookingID == booking.bookingID);
                if (bookingDB != null)
                {
                    bookingDB.bookingDate = booking.bookingDate;
                    bookingDB.totalAmount = booking.totalAmount;
                    bookingDB.status = booking.status;
                    bookingDB.accountID = booking.accountID;

                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Chỉnh sửa thành công!";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.AccountID = new SelectList(_context.Accounts, "accountID", "firstName", booking.accountID);
            return View(booking);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Booking booking = _context.Bookings.FirstOrDefault(p => p.bookingID == id);
            if (booking != null)
            {
                return View(booking);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var bookingDB = _context.Bookings.FirstOrDefault(p => p.bookingID == id);
            if (bookingDB != null)
            {
                _context.Bookings.Remove(bookingDB);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Xóa thành công!";
            }
            return RedirectToAction("Index");
        }

        public ActionResult BookFlight(int flightID)
        {
            var flight = _context.Flights.Find(flightID);
            if (flight == null)
            {
                return HttpNotFound();
            }

            var booking = new Booking
            {
                bookingDate = DateTime.Now,
                totalAmount = flight.flightPrice,
                status = "Pending",
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return RedirectToAction("CustomerInfo", new { bookingID = booking.bookingID });
        }

        [HttpPost]
        public ActionResult BookFlight(Booking booking, int flightID)
        {
            if (ModelState.IsValid)
            {
                booking.bookingDate = DateTime.Now;
                booking.status = "Pending";
                booking.Flight = _context.Flights.Find(flightID);

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                return RedirectToAction("CustomerInfo", new { bookingID = booking.bookingID });
            }
            return View(booking);
        }
        public BookingController()
        {
            _context = new DAPMLThuyetEntities();
        }
        public async Task<ActionResult> Payment(int bookingID)
        {
            var booking = _context.Bookings.Find(bookingID);
            if (booking == null || booking.status == "Paid")
            {
                return HttpNotFound();
            }

            // Tạo bản ghi MoMoPayment
            var momoPayment = new MoMoPayment
            {
                PartnerCode = ConfigurationManager.AppSettings["MoMoPartnerCode"],
                AccessKey = ConfigurationManager.AppSettings["MoMoAccessKey"],
                RequestId = Guid.NewGuid().ToString(),
                OrderId = bookingID.ToString(),
                Amount = booking.totalAmount,
                OrderInfo = "Thanh toán đặt vé máy bay",
                RedirectUrl = Url.Action("PaymentCallback", "Booking", new { bookingID }, Request.Url.Scheme),
                IpnUrl = Url.Action("PaymentCallback", "Booking", new { bookingID }, Request.Url.Scheme),
                RequestType = "captureMoMoWallet",
                CreatedAt = DateTime.Now,
                Status = "Pending"
            };

            // Tạo chữ ký SHA-256 cho yêu cầu
            momoPayment.Signature = SignSHA256(momoPayment.ToString(), ConfigurationManager.AppSettings["MoMoSecretKey"]);

            try
            {
                // Gửi yêu cầu thanh toán đến MoMo
                var response = await _httpClient.PostAsync(
                    ConfigurationManager.AppSettings["MoMoEndpoint"],
                    new StringContent(JsonConvert.SerializeObject(momoPayment), Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode)
                {
                    return View("Error", new { message = "Không thể kết nối với cổng thanh toán MoMo." });
                }

                dynamic result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                // Cập nhật URL thanh toán vào bản ghi
                momoPayment.RedirectUrl = result.payUrl;
                momoPayment.Status = "Sent"; // Trạng thái đã gửi yêu cầu đến MoMo
                _context.SaveChanges();

                // Chuyển hướng người dùng đến URL thanh toán
                return Redirect(result.payUrl.ToString());
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và ghi log nếu cần
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi xử lý yêu cầu thanh toán.";
                return View("Error", new { message = ex.Message });
            }
        }

        private static string SignSHA256(string data, string secretKey)
        {
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashBytes = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public ActionResult PaymentCallback(int bookingID)
        {
            var booking = _context.Bookings.Find(bookingID);
            if (booking != null && booking.status != "Paid")
            {
                booking.status = "Paid"; // Cập nhật trạng thái
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = bookingID });
        }
        [HttpGet]
        public ActionResult CustomerInfo(int bookingID)
        {
            ViewBag.BookingID = bookingID;
            return View(new CustomerInfo());
        }

        // Action để xử lý thông tin người dùng nhập vào
        [HttpPost]
        public ActionResult ConfirmBooking(CustomerInfo model, string PaymentMethod)
        {
            if (ModelState.IsValid)
            {
                _context.CustomerInfoes.Add(model);
                _context.SaveChanges();

                // Xử lý dựa trên phương thức thanh toán đã chọn
                if (PaymentMethod == "MoMo")
                {
                    return RedirectToAction("Payment", new { bookingID = model.BookingID });
                }
                else if (PaymentMethod == "Thẻ Tín Dụng")
                {
                    // Logic cho thanh toán bằng thẻ tín dụng
                }
                else if (PaymentMethod == "Thẻ ATM")
                {
                    // Logic cho thanh toán bằng thẻ ATM
                }

                return RedirectToAction("Details", new { id = model.BookingID });
            }

            return View("CustomerInfo", model);
        }
    }
}
