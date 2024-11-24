using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using System.Security.Principal;

namespace FlightManagement.Controllers
{
    public class LoginController : Controller
    {
        DAPMLThuyetEntities database = new DAPMLThuyetEntities();

        // GET: Admin
        public ActionResult Index()
        {
            var userAD = database.Accounts.Include(a => a.AccountType).ToList();
            return View(userAD);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account _user)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem có người nào đã đăng kí với tên đăng nhập này hay chưa                  
                var existingCustomer = database.Accounts.FirstOrDefault(k => k.username == _user.username);
                if (existingCustomer != null)
                {

                    ViewBag.CanhBao = "Tài khoản đã tồn tại!";
                    return View();
                }

                // Nếu mọi điều kiện hợp lệ, thêm mới khách hàng
                database.Accounts.Add(new Account
                {
                    lastName = _user.lastName,
                    firstName = _user.firstName,
                    username = _user.username,
                    password = _user.password,
                    accountTypeID = 2,
                    // Thêm các trường khác nếu cần
                });

                database.SaveChanges();
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Account model)
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = database.Accounts.FirstOrDefault(k => k.username == model.username && k.password == model.password);

                if (existingCustomer != null)
                {
                    Session["idUser"] = existingCustomer.accountID;
                    Session["Username"] = existingCustomer.username;
                    Session["Password"] = existingCustomer.password;
                    Session["Role"] = existingCustomer.accountTypeID;

                    if (existingCustomer.AccountType.typeName == "Admin")
                    {
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        return RedirectToAction("Index", "UserHome");
                    }
                }
                else
                {
                    ViewBag.ThongBao = "Sai tài khoản";
                }
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear(); // Clear all sessions
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.AccountTypes = new SelectList(database.AccountTypes, "accountTypeID", "typeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "accountID,lastname,firstname, username, password, dateOfBirth, address, email, phone,accountTypeID")] Account customer)
        {
            if (ModelState.IsValid)
            {
                // Ensure ID is not set to any value
                customer.accountID = 0; // Not necessary if IDENTITY_INSERT is set to OFF

                database.Accounts.Add(customer);
                database.SaveChanges();
                TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.AccountTypes = new SelectList(database.AccountTypes, "accountTypeID", "typeName", customer.accountTypeID);
            return View(customer);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Account customer = database.Accounts.FirstOrDefault(p => p.accountID == id);
            if (customer != null)
            {
                ViewBag.AccountTypes = new SelectList(database.AccountTypes, "accountTypeID", "typeName", customer.accountTypeID);
                return View(customer);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "accountID,lastname,firstname, username, password, dateOfBirth, address, email, phone,accountTypeID")] Account account)
        {
            if (ModelState.IsValid)
            {
                var customerDB = database.Accounts.FirstOrDefault(p => p.accountID == account.accountID);
                if (customerDB != null)
                {
                    customerDB.lastName = account.lastName;
                    customerDB.firstName = account.firstName;
                    customerDB.username = account.username;
                    customerDB.password = account.password;
                    customerDB.accountTypeID = account.accountTypeID;

                    database.SaveChanges();
                    TempData["SuccessMessage"] = "Sửa tài khoản thành công!";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.AccountTypes = new SelectList(database.AccountTypes, "accountTypeID", "typeName", account.accountTypeID);
            return View(account);
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            Account customer = database.Accounts.FirstOrDefault(p => p.accountID == id);
            if (customer != null)
                return View(customer);
            else return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            int i = int.Parse(id);
            var customerDB = database.Accounts.FirstOrDefault(p => p.accountID == i);
            if (customerDB != null)
            {
                database.Accounts.Remove(customerDB);
                TempData["SuccessMessage"] = "Xóa tài khoản thành công!";
                database.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Details(int id)
        {
            Account customer = database.Accounts.FirstOrDefault(p => p.accountID == id);
            if (customer != null)
            {
                ViewBag.AccountTypes = new SelectList(database.AccountTypes, "accountTypeID", "typeName", customer.accountTypeID);
                return View(customer);
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "accountID,lastname,firstname,dateOfBirth, address, email, phone,accountTypeID")] Account account)
        {
            if (ModelState.IsValid)
            {
                var customerDB = database.Accounts.FirstOrDefault(p => p.accountID == account.accountID);
                if (customerDB != null)
                {
                    customerDB.lastName = account.lastName;
                    customerDB.firstName = account.firstName;
                    customerDB.dateOfBirth = account.dateOfBirth;
                    customerDB.address = account.address;
                    customerDB.email = account.email;
                    customerDB.phone = account.phone;

                    database.SaveChanges();
                    TempData["SuccessMessage"] = "Sửa tài khoản thành công!";
                    return RedirectToAction("Details", new { id = account.accountID });
                }
            }
            ViewBag.AccountTypes = new SelectList(database.AccountTypes, "accountTypeID", "typeName", account.accountTypeID);
            return View(account);
        }
    }
}
