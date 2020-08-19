using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Pile.db;
using Pile.Models;
using System.Reflection;

namespace Pile.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customers
        public ActionResult Index()
        {
            return View();
        }


        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: Customers/Edit
        public ActionResult Edit(int id)
        {
            return View();
        }

    }

    public class RoutesController : Controller
    {
        public ActionResult Index()
        {
            return View("RoutesIndex");
        }
    }

    public class ServiceDetailsController : Controller
    {
        public ActionResult Edit(int id)
        {
            return View();
        }
    }

    public class ServiceHistoriesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }

    public class EmployeesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
