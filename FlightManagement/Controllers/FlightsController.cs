using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlightManagement.Controllers
{
    public class FlightsController : Controller
    {
        DAPMLThuyetEntities database = new DAPMLThuyetEntities();
        // GET: Flights
        public ActionResult Index()
        {
            var flyAD = database.Flights.ToList();
            return View(flyAD);
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.AircraftID = new SelectList(database.Aircraft, "aircraftID", "model");
            ViewBag.AirlineID = new SelectList(database.Airlines, "airlineID", "airlineName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "flightID,departureCity,arrivalCity,departureTime,flightDuration,flightPrice,aircraftID,airlineID")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                database.Flights.Add(flight);
                database.SaveChanges();
                TempData["SuccessMessage"] = "Tạo thành công!";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, cập nhật lại các SelectList để hiển thị lại dropdown đúng cách
            ViewBag.AircraftID = new SelectList(database.Aircraft, "aircraftID", "model", flight.aircraftID);
            ViewBag.AirlineID = new SelectList(database.Airlines, "airlineID", "airlineName", flight.airlineID);
            return View(flight);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            Flight flight = database.Flights.FirstOrDefault(p => p.flightID == id);
            if (flight != null)
            {
                ViewBag.AircraftID = new SelectList(database.Aircraft, "aircraftID", "model", flight.aircraftID);
                ViewBag.AirlineID = new SelectList(database.Airlines, "airlineID", "airlineName", flight.airlineID);
                return View(flight);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "flightID,departureCity,arrivalCity,departureTime,flightDuration,flightPrice,aircraftID,airlineID")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                var flightDB = database.Flights.FirstOrDefault(p => p.flightID == flight.flightID);
                if (flightDB != null)
                {
                    flightDB.departureCity = flight.departureCity;
                    flightDB.arrivalCity = flight.arrivalCity;
                    flightDB.departureTime = flight.departureTime;
                    flightDB.flightDuration = flight.flightDuration;
                    flightDB.flightPrice = flight.flightPrice;
                    flightDB.aircraftID = flight.aircraftID;
                    flightDB.airlineID = flight.airlineID;

                    database.SaveChanges();
                    TempData["SuccessMessage"] = "Chỉnh sửa thành công!";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.AircraftID = new SelectList(database.Aircraft, "aircraftID", "model", flight.aircraftID);
            ViewBag.AirlineID = new SelectList(database.Airlines, "airlineID", "airlineName", flight.airlineID);
            return View(flight);
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            Flight flight = database.Flights.FirstOrDefault(p => p.flightID == id);
            if (flight != null)
                return View(flight);
            else return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            int i = int.Parse(id);
            var FlightDB = database.Flights.FirstOrDefault(p => p.flightID == i);
            if (FlightDB != null)
            {
                database.Flights.Remove(FlightDB);
                TempData["SuccessMessage"] = "Xóa thành công!";
                database.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        // Hiển thị trang form tìm kiếm
        [HttpGet]
        public ActionResult Search()
        {
            // Khởi tạo ViewModel với dữ liệu ban đầu
            var model = new SearchFlight
            {
                DepartureCity = null,
                ArrivalCity = null,
                DepartureTime = null,
                Flights = new List<Flight>() // Danh sách rỗng
            };

            return View(model); // Hiển thị form tìm kiếm
        }

        // Thực hiện tìm kiếm và hiển thị kết quả
        [HttpGet]
        public ActionResult ResultSearch(string departureCity, string arrivalCity, DateTime? departureTime)
        {
            // Truy vấn danh sách chuyến bay
            var flights = database.Flights.AsQueryable();

            // Lọc theo điểm đi
            if (!string.IsNullOrEmpty(departureCity))
            {
                flights = flights.Where(f => f.departureCity.Contains(departureCity));
            }

            // Lọc theo điểm đến
            if (!string.IsNullOrEmpty(arrivalCity))
            {
                flights = flights.Where(f => f.arrivalCity.Contains(arrivalCity));
            }

            // Lọc theo ngày khởi hành
            if (departureTime.HasValue)
            {
                DateTime startOfDay = departureTime.Value.Date;
                DateTime endOfDay = startOfDay.AddDays(1);

                flights = flights.Where(f => f.departureTime >= startOfDay && f.departureTime < endOfDay);
            }

            // Chuẩn bị ViewModel
            var model = new SearchFlight
            {
                DepartureCity = departureCity,
                ArrivalCity = arrivalCity,
                DepartureTime = departureTime,
                Flights = flights.ToList() // Chuyển đổi kết quả thành danh sách
            };

            return View(model); // Hiển thị trang kết quả
        }

    }
}