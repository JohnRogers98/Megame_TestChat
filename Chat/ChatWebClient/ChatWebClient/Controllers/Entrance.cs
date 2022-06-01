using ChatWebClient.Models;
using ChatWebClient.Models.Http;
using ChatWebClient.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatWebClient.Controllers
{
    public class Entrance : Controller
    {
        #region Fields

        private readonly HttpQueriesToServer _httpQueries;

        #endregion

        public Entrance()
        {
            _httpQueries = new HttpQueriesToServer(); 
        }

        #region Actions

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(String email, String password)
        {
            var response = await _httpQueries.Login(email, password);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content?.ReadAsStringAsync();

                var responseObj = JsonConvert.DeserializeObject<LoginResponse>(responseString);

                this.HttpContext.Session.SetString("token", responseObj.Token);
                Authentication.Token = responseObj.Token;
                Authentication.Id = responseObj.Id;

                return Ok(responseString);
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(String email, String password, String name)
        {
            var response = await _httpQueries.Register(email, password, name);
            
            if(response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(Login));
            }

            return RedirectToAction(nameof(Register));
        }

        #endregion
    }
}