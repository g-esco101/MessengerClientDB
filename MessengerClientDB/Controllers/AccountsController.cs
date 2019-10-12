using MessengerClientDB.Helpers;
using MessengerClientDB.Models;
using MessengerClientDB.Unity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

/*
Add your Unity registrations in the RegisterComponents method of the UnityConfig class. All components that implement IDisposable should be 
registered with the HierarchicalLifetimeManager to ensure that they are properly disposed at the end of the request.
*/

namespace MessengerClientDB.Controllers
{
    public class AccountsController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public AccountsController()
        {
            _unitOfWork = new UnitOfWork(new MessengerClient_DBEntities());
        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            if (ValidationHelper.IsNullEmptyWhiteSpace(loginVM.Username, loginVM.HashedPassword))
            {
                return View();
            }
            try
            {
                var user = _unitOfWork.usersRolesRepo.SingleOrDefault(u => u.Username.ToLower() == loginVM.Username.ToLower());
                if (user == null)
                {
                    ModelState.AddModelError("Username", "User does not exist. Please register.");
                    return View(loginVM);
                }
                if (Hasher.RightPassword(user.HashedPassword, loginVM.HashedPassword))
                {
                    bool validToken = await LoginHelper(loginVM.Username, user.HashedPassword);
                    if (validToken)
                    {
                        return RedirectToAction("Index", "Messages");
                    }
                }
                ModelState.AddModelError("", "Invalid Username or Password");
            }
            catch { }
            return View();
        }

        private async Task<bool> LoginHelper(string username, string hashSaltIter)
        {           
            try
            {
                FormsAuthentication.SetAuthCookie(username, false);
                bool tokenOk = await getServiceTokenAsync(username, hashSaltIter);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Login Helper - error: " + ex.Message);
            }
            return false;
        }

        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            if (ValidationHelper.IsNullEmptyWhiteSpace(registerViewModel.Username, registerViewModel.HashedPassword))
            {
                return View();
            }
            var user = _unitOfWork.usersRolesRepo.SingleOrDefault(r => r.Username.ToLower() == registerViewModel.Username.ToLower());
            if (user != null)
            {
                ModelState.AddModelError("Username", "User already exists");
                return View(registerViewModel);
            }
            try
            {
                string hashInfo = Hasher.HashGenerator(registerViewModel.HashedPassword);
                _unitOfWork.BeginTransaction();
                user = new Users() { Username = registerViewModel.Username, HashedPassword = hashInfo };
                _unitOfWork.usersRolesRepo.Add(user);
                string[] role = { "User" };
                _unitOfWork.usersRolesRepo.AddRoles(registerViewModel.Username, role);
                bool registerOk = await RegisterService(registerViewModel.Username, hashInfo, role[0]);
                if (registerOk)
                {
                    bool tokenOk = await getServiceTokenAsync(registerViewModel.Username, hashInfo);
                    _unitOfWork.CommitTransaction();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                _unitOfWork.Rollback();
            }
            return View();
        }

    //    [HttpPost]
        private async Task<bool> RegisterService(string username, string password, string role)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/users/register?username=" + HttpUtility.UrlEncode(username) + "&password=" + HttpUtility.UrlEncode(password) + "&roles=" + role);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

   //     [HttpPost]
        private async Task<bool> getServiceTokenAsync(string username, string hashedPwd)
        {
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("username", username));
            formData.Add(new KeyValuePair<string, string>("password", hashedPwd));
            formData.Add(new KeyValuePair<string, string>("grant_type", "password"));
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/token");
                request.Content = new FormUrlEncodedContent(formData);
                var response = await MvcApplication._httpClient.SendAsync(request);
                HttpContent requestContent = response.Content;
                string jsonContent = requestContent.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(jsonContent);
                string token = json.GetValue("access_token").ToString();
                MvcApplication._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
