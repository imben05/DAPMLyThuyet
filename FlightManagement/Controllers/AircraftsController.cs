using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlightManagement.Controllers
{
    public class AircraftsController : Controller
    {
        DAPMLThuyetEntities database = new DAPMLThuyetEntities();

        // GET: Aircrafts
        public ActionResult Index()
        {
            var aircraftAD = database.Aircraft.ToList();
            return View(aircraftAD);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.AirlineID = new SelectList(database.Airlines, "airlineID", "airlineName"); // Assuming 'airlines' table has 'airlineID' and 'airlineName'
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "aircraftID,model,capacity,airlineID")] Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                // Ensure ID is not set to any value
                aircraft.aircraftID = 0; // Not necessary if IDENTITY_INSERT is set to OFF

                database.Aircraft.Add(aircraft);
                database.SaveChanges();
                TempData["SuccessMessage"] = "Tạo thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.AirlineID = new SelectList(database.Airlines, "airlineID", "airlineName", aircraft.airlineID); // Ensure correct ViewBag key
            return View(aircraft);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Aircraft aircraft = database.Aircraft.FirstOrDefault(p => p.aircraftID == id);
            if (aircraft != null)
            {
                ViewBag.AirlineID = new SelectList(database.Airlines, "airlineID", "airlineName", aircraft.airlineID); // Ensure correct ViewBag key
                return View(aircraft);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "aircraftID,model,capacity,airlineID")] Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                var aircraftDB = database.Aircraft.FirstOrDefault(p => p.aircraftID == aircraft.aircraftID);
                if (aircraftDB != null)
                {
                    aircraftDB.model = aircraft.model;
                    aircraftDB.capacity = aircraft.capacity;
                    aircraftDB.airlineID = aircraft.airlineID;

                    database.SaveChanges();
                    TempData["SuccessMessage"] = "Chỉnh sửa thành công!";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.AirlineID = new SelectList(database.Airlines, "airlineID", "airlineName", aircraft.airlineID); // Ensure correct ViewBag key
            return View(aircraft);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Aircraft aircraft = database.Aircraft.FirstOrDefault(p => p.aircraftID == id);
            if (aircraft != null)
            {
                return View(aircraft);
            }
            else return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            int i = int.Parse(id);
            var aircraftDB = database.Aircraft.FirstOrDefault(p => p.aircraftID == i);
            if (aircraftDB != null)
            {
                database.Aircraft.Remove(aircraftDB);
                TempData["SuccessMessage"] = "Xóa thành công!";
                database.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
