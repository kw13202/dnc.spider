using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dnc.efcontext;
using Microsoft.AspNetCore.Mvc;

namespace dnc.spider.webapi.Controllers
{
    public class UserController : Controller
    {
        private EfContext _context;
        public UserController(EfContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}