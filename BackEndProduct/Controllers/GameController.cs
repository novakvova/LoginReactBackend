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
        private Dictionary<string, string> GetErrorsByModel(ModelStateDictionary modelErrors)
        {
            var errors = new Dictionary<string, string>();

            var errorList = modelErrors
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()[0]
                );
            foreach (var item in errorList)
            {
                var key = item.Key.Split('.')[1];
                errors.Add(key, item.Value);
            }
            return errors;
        }
        // GET api/values
        public IHttpActionResult Get()
        {
            var folder = Url.Content(ConfigurationManager
                .AppSettings["ImagePath"]);
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
        public IHttpActionResult Get(int id)
        {
            var model = new GameItemViewModel();
            var game=_context.Games.SingleOrDefault(g => g.Id == id);
            if(game!=null)
            {
                var folder = Url.Content(ConfigurationManager
                .AppSettings["ImagePath"]);
                model.id = game.Id;
                model.title = game.Title;
                model.image = folder + game.Image;
                model.description = game.Description;
            }
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
               
                var folder = Url.Content(ConfigurationManager.AppSettings["ImagePath"]);
                var image = folder + uniqueName;
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
            var errors = GetErrorsByModel(ModelState);
            return Content(HttpStatusCode.BadRequest, errors);
        }
        public IHttpActionResult Put(GameItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var game = _context.Games
                    .SingleOrDefault(g => g.Id == model.id);
                if(game!=null)
                {
                    game.Title = model.title;
                    game.Description = model.description;
                    _context.SaveChanges(); 
                }
                return Content(HttpStatusCode.OK, model);
            }

            var errors = GetErrorsByModel(ModelState);

            return Content(HttpStatusCode.BadRequest, errors);
        }
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var game = _context.Games
                    .SingleOrDefault(g => g.Id == id);
                if (game != null)
                {
                    _context.Games.Remove(game);
                    _context.SaveChanges();
                }
                return Content(HttpStatusCode.OK, new { success = true });
            }
            catch(Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError,  
                    new { errors = new { global = ex.Message } } );
            }
                
            

            
        }
    }
}
