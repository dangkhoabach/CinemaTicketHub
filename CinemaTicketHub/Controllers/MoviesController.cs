using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaTicketHub.Controllers
{
    public class MoviesController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();

        // GET: Movies
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NowShowing()
        {
            return View(_dbContext.Phim.ToList());
        }
    }
}