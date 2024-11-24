using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlightManagement.Controllers
{
    public class PaymentController : Controller
    {
        DAPMLThuyetEntities database = new DAPMLThuyetEntities();

        // GET: Aircrafts
        public ActionResult Index()
        {
            var payAD = database.Payments.ToList();
            return View(payAD);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.BookingID = new SelectList(database.Bookings, "bookingID", "bookingDate"); // Assuming 'airlines' table has 'airlineID' and 'airlineName'
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "paymentID,amount,paymentDate,paymentMethod,status,bookingID")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                // Ensure ID is not set to any value
                payment.paymentID = 0; // Not necessary if IDENTITY_INSERT is set to OFF

                database.Payments.Add(payment);
                database.SaveChanges();
                TempData["SuccessMessage"] = "Tạo thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.BookingID = new SelectList(database.Bookings, "bookingID", "bookingDate", payment.bookingID); // Ensure correct ViewBag key
            return View(payment);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Payment payment = database.Payments.FirstOrDefault(p => p.paymentID == id);
            if (payment != null)
            {
                ViewBag.BookingID = new SelectList(database.Bookings, "bookingID", "bookingDate", payment.bookingID); // Ensure correct ViewBag key
                return View(payment);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "paymentID,amount,paymentDate,paymentMethod,status,bookingID")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                var paymentDB = database.Payments.FirstOrDefault(p => p.paymentID == payment.paymentID);
                if (paymentDB != null)
                {
                    paymentDB.amount = payment.amount;
                    paymentDB.paymentDate = payment.paymentDate;
                    paymentDB.paymentMethod = payment.paymentMethod;
                    paymentDB.status = payment.status;
                    paymentDB.bookingID = payment.bookingID;

                    database.SaveChanges();
                    TempData["SuccessMessage"] = "Chỉnh sửa thành công!";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.BookingID = new SelectList(database.Bookings, "bookingID", "bookingDate", payment.bookingID); // Ensure correct ViewBag key
            return View(payment);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Payment payment = database.Payments.FirstOrDefault(p => p.paymentID == id);
            if (payment != null)
            {
                return View(payment);
            }
            else return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            int i = int.Parse(id);
            var payDB = database.Payments.FirstOrDefault(p => p.paymentID == i);
            if (payDB != null)
            {
                database.Payments.Remove(payDB);
                TempData["SuccessMessage"] = "Xóa thành công!";
                database.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
