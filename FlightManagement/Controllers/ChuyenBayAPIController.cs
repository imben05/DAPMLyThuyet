using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlightManagement.Models;

namespace FlightManagement.Controllers
{
    public class ChuyenBayAPIController : Controller
    {
        // GET: ChuyenBayAPI
        public JsonResult Index()
        {
            DataModel db = new DataModel();
            ArrayList a = db.get("EXEC TTChuyenBay");
            return Json(a,JsonRequestBehavior.AllowGet);
        }
        
    }
}