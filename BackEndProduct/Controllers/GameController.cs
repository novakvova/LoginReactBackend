using BackEndProduct.Models;
using BackEndProduct.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BackEndProduct.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class GameController : ApiController
    {
        private readonly ApplicationDbContext _context;
        public GameController()
        {
            _context = new ApplicationDbContext();
        }
        // GET api/values
        public IHttpActionResult Get()
        {
            var model = _context.Games
                .Select(g => new GameItemViewModel
                {
                    id = g.Id,
                    title = g.Title,
                    image = g.Image,
                    description = g.Description
                }).ToList();

            return Content(HttpStatusCode.OK, model);
        }
        public IHttpActionResult PostAdd(GameCreateViewModel model)
        {
            if(ModelState.IsValid)
            {
                string uniqueName = String.Empty;
                string imagePath = String.Empty;
                uniqueName = Guid.NewGuid().ToString() + ".jpeg";
                imagePath = HttpContext.Current.Server
                    .MapPath(ConfigurationManager.AppSettings["ImagePath"])
                    + uniqueName;
                string base64 = model.image.Split(',')[1];
                byte[] imageBytes = Convert.FromBase64String(base64);
                File.WriteAllBytes(imagePath, imageBytes);
            }

            return Content(HttpStatusCode.BadRequest, new { success=true });
        }
        public IHttpActionResult Put(GameCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                return Content(HttpStatusCode.OK, new { success = true });
            }

            return Content(HttpStatusCode.BadRequest, new { success = true });
        }
    }
}
