using BackEndProduct.Models;
using BackEndProduct.Models.Entitites;
using BackEndProduct.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;

namespace BackEndProduct.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class GameController : ApiController
    {
        private readonly ApplicationDbContext _context;
        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET api/values
        public IHttpActionResult Get()
        {
            var folder = Url
                .Content(ConfigurationManager.AppSettings["ImagePath"]);
            var model = _context.Games
                .Select(g => new GameItemViewModel
                {
                    id = g.Id,
                    title = g.Title,
                    image = folder+g.Image,
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
                Game game = new Game()
                {
                    Image= uniqueName,
                    Description = model.description,
                    Title=model.title
                };
                _context.Games.Add(game);
                _context.SaveChanges();

                //var request = HttpContext.Current.Request;
                //var url = request.Url.GetLeftPart(UriPartial.Authority) +
                //    request.ApplicationPath;
                var folder = Url.Content(ConfigurationManager.AppSettings["ImagePath"]);
                var image = folder + uniqueName;
                //folder += uniqueName;
                GameItemViewModel responseModel = new GameItemViewModel()
                {
                    id=game.Id,
                    title=game.Title,
                    description=game.Description,
                    image=image
                };
                return Content(HttpStatusCode.OK, responseModel);
            }

            //var errors = new ExpandoObject() as IDictionary<string, object>;
            var errors = new Dictionary<string, string>();

            var errorList = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()[0]
                );
            foreach (var item in errorList)
            {
                var key = item.Key.Split('.')[1];
                errors.Add(key, item.Value.ToString());
            }
            //dynamic slavic = new ExpandoObject();
            //slavic.firstName = "Петро";
            //errors.title = "Обов'язкове поле";
            return Content(HttpStatusCode.BadRequest, errors);
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
