﻿using Microsoft.AspNetCore.Mvc;

namespace Jewellis.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class CartController : Controller
    {

        [Route("/cart")]
        public IActionResult Index()
        {
            return View();
        }

    }
}