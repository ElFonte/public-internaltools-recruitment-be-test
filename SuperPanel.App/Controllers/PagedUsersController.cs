using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SuperPanel.App.Data;
using SuperPanel.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace SuperPanel.App.Controllers
{
    public class PagedUsersController : Controller
    {
        private readonly ILogger<PagedUsersController> _logger;
        private readonly IUserRepository _userRepository;

        public PagedUsersController(ILogger<PagedUsersController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }
        [Route("index")]
        [Route("")]
        [Route("~/")]
        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            List<User> _users = _userRepository.QueryAll().ToList();
            List<UserView> _userViews = new List<UserView>();
            foreach (User user in _users)
            {
                _userViews.Add(new UserView(user));
            }
            ViewBag.UserView = _userViews.ToPagedList(pageNumber, 10);
            return View();
        }
        public async Task<IActionResult> GDPR(long id)
        {
            HttpClient ApiClient = new HttpClient();
            ApiClient.BaseAddress = new Uri("http://localhost:61695");
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = new HttpResponseMessage();

            response = await ApiClient.PutAsync("v1/contacts/" + id.ToString() + "/gdpr", null);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                return Ok("User contact anonimized");
            }
            else
            {
                if (response.ReasonPhrase == "Method Not Allowed")
                    return Ok("Server error, retry");
                else if (response.ReasonPhrase == "Not Found")
                    return Ok("User not found as contact");
                else
                    return Ok("Unknown error");
            }
        }
    }
}
