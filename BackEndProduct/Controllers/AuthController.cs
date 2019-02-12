using BackEndProduct.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BackEndProduct.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class AuthController : ApiController
    {
        private readonly ApplicationDbContext _context;
        public AuthController()
        {
            _context = new ApplicationDbContext();
        }
        public IHttpActionResult PostLogin(LoginBindingModel model)
        {
            var request = HttpContext.Current.Request;
            var url = request.Url.GetLeftPart(UriPartial.Authority) +
                request.ApplicationPath + "/Token";
            Dictionary<string, string> p = new Dictionary<string, string>();
            p.Add("username", model.Identifier);
            p.Add("password", model.Password);
            p.Add("grant_type", "password");
            string token = "HelloToken";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.PostAsync(url,
                    new FormUrlEncodedContent(p)).Result;
                var json = response.Content.ReadAsStringAsync().Result;
                TokenModel Token = JsonConvert.DeserializeObject<TokenModel>(json);
                token = Token.access_token;
            }
            if (token == null)
            {
                return Content(HttpStatusCode.Unauthorized, new
                {
                    invalid = "Не коректно вказано логін або парль"
                });
            }
            return Content(HttpStatusCode.OK, new
            {
                token
            });
        }
    }
    public class LoginBindingModel
    {
        public string Identifier { get; set; }
        public string Password { get; set; }
    }
    public class TokenModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string Token { get { return $"{token_type} {access_token}"; } }
    }
}
