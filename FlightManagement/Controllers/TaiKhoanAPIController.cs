using FlightManagement.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlightManagement.Controllers
{
    public class TaiKhoanAPIController : Controller
    {
        // GET: TaiKhoanAPI
        public JsonResult Index()
        {
            DataModel db = new DataModel();
            ArrayList a = db.get("EXEC TTTaiKhoan");
            return Json(a, JsonRequestBehavior.AllowGet);
        }
    }
}