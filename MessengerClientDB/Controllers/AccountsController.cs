using MessengerClientDB.Models;
using MessengerClientDB.Repositories;
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
        private IAccountsRepository _accountsRepo;

        public AccountsController(IAccountsRepository accountsRepository)
        {
            this._accountsRepo = accountsRepository;
        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            bool validUser, validToken; string hashSaltIterProvided;
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
            try
            {

                if (!_accountsRepo.UserExists(loginViewModel.Username))
                {
                    ModelState.AddModelError("Username", "User does not exist. Please register.");
                    return View(loginViewModel);
                }
                hashSaltIterProvided = _accountsRepo.ReprodcueHash(loginViewModel.Username, loginViewModel.HashedPassword);
                validUser = _accountsRepo.ValidLogin(loginViewModel.Username, loginViewModel.HashedPassword);
                if (validUser)
                {
                    validToken = await LoginHelper(loginViewModel.Username, hashSaltIterProvided);
                    if (validToken)
                    {
                        return RedirectToAction("Index", "Messages");
                    }
                }
                ModelState.AddModelError("", "Invalid Username or Password");
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Login - error: " + ex.Message);
            }
            return View();
        }

        private async Task<bool> LoginHelper(string username, string hashSaltIter)
        {
            bool tokenOk;
            try
            {
                FormsAuthentication.SetAuthCookie(username, false);
       //         await RegisterService(username, hashSaltIter);
                tokenOk = await getServiceTokenAsync(username, hashSaltIter);
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
        //        public async Task<ActionResult> Register(RegisterViewModel registerViewModel)
        public async Task<ActionResult> Register(RegisterViewModel registerViewModel, IUsersRepository usersRepository)
        {
            string hashedPwd; bool tokenOk, registerOk;
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            if (_accountsRepo.UserExists(registerViewModel.Username))
            {
                ModelState.AddModelError("Username", "User already exists");
                return View(registerViewModel);
            }
            hashedPwd = Hasher.HashGenerator(registerViewModel.HashedPassword);
            if(_accountsRepo.AddUser(registerViewModel.Username, hashedPwd, usersRepository))
            {
                registerOk = await RegisterService(registerViewModel.Username, hashedPwd);
                if (registerOk)
                {
                    tokenOk = await getServiceTokenAsync(registerViewModel.Username, hashedPwd);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [HttpPost]
        private async Task<bool> RegisterService(string username, string password)
        {
            string user, pwd, roles = "User";
            if (String.IsNullOrEmpty(username) || String.IsNullOrWhiteSpace(username) || String.IsNullOrEmpty(password) || String.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            try
            {
                user = HttpUtility.UrlEncode(username);
                pwd = HttpUtility.UrlEncode(password);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/users/register?username=" + user + "&password=" + pwd + "&roles=" + roles);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("DeleteFromService - End - true");
                    return true;
                }
            }
            catch { }
            return false;
        }

        [HttpPost]
        private async Task<bool> getServiceTokenAsync(string username, string hashedPwd)
        {
            JObject json;
            string jsonContent, token = "User not found";
            HttpContent requestContent;
            if (String.IsNullOrEmpty(username) || String.IsNullOrWhiteSpace(username) || String.IsNullOrEmpty(hashedPwd) || String.IsNullOrWhiteSpace(hashedPwd))
            {
                return false;
            }
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("username", username));
            formData.Add(new KeyValuePair<string, string>("password", hashedPwd));
            formData.Add(new KeyValuePair<string, string>("grant_type", "password"));
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/token");
                request.Content = new FormUrlEncodedContent(formData);
                var response = await MvcApplication._httpClient.SendAsync(request);
                requestContent = response.Content;
                jsonContent = requestContent.ReadAsStringAsync().Result;
                json = JObject.Parse(jsonContent);
                token = json.GetValue("access_token").ToString();
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
