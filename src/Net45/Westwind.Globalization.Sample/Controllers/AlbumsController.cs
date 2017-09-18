using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Westwind.Globalization.Sample.Controllers
{
    public class AlbumsController : Controller
    {
        // GET: Albums
        public ActionResult Index()
        {
            var albumBus = new AlbumsBusiness();

            var albums = albumBus.GetAlbums();


            return View(albums);
        }
    }

    
}