using MessengerClientDB.Helpers;
using MessengerClientDB.Models;
using MessengerClientDB.Restful;
using MessengerClientDB.Unity;
using System;
using System.Threading.Tasks;
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

        private IAccountRest _accountRest;

        public AccountsController(IAccountRest accountRest)
        {
            _unitOfWork = new UnitOfWork(new MessengerClient_DBEntities());
            _accountRest = accountRest;
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
                bool tokenOk = await _accountRest.GetServiceTokenAsync(username, hashSaltIter);
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
                //           _unitOfWork.BeginTransaction();
                user = new Users() { Username = registerViewModel.Username, HashedPassword = hashInfo };
                _unitOfWork.usersRolesRepo.Add(user);
                _unitOfWork.Save(); // Must save here so that user is in database when AddRoles executes two lines below.
                string[] role = { "User" };
                _unitOfWork.usersRolesRepo.AddRoles(registerViewModel.Username, role);
                _unitOfWork.Save();
                bool registerOk = await _accountRest.RegisterServiceAsync(registerViewModel.Username, hashInfo, role[0]);
                if (registerOk)
                {
                    bool tokenOk = await _accountRest.GetServiceTokenAsync(registerViewModel.Username, hashInfo);
                    _unitOfWork.CommitTransaction();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                //          _unitOfWork.Rollback();
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
