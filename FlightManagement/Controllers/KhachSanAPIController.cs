using FlightManagement.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlightManagement.Controllers
{
    public class KhachSanAPIController : Controller
    {
        // GET: KhachSanAPI
        public JsonResult Index()
        {
            DataModel db = new DataModel();
            ArrayList a = db.get("EXEC TTKhachSan");
            return Json(a, JsonRequestBehavior.AllowGet);
        }
    }
}