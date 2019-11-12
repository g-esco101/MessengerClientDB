using MessengerClientDB.Models;
using MessengerClientDB.Models.ViewModels;
using MessengerClientDB.Restful;
using MessengerClientDB.Unit;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;


namespace MessengerClientDB.Controllers
{
    public class AccountsController : Controller
    {
        private IUnitOfWork _unitOfWork;

        private IAccountRest _accountRest;

        private string _userRole;

        private int _roleId;

        public AccountsController(IAccountRest accountRest)

        {
            _unitOfWork = new UnitOfWork();
            _accountRest = accountRest;
            _userRole = "Admin";
            _roleId = 1;
        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            string username = loginVM.Username;
            Users user = await _unitOfWork.usersRolesRepo.GetAsync(username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User does not exist. Please register.");
                return View(loginVM);
            }
            if (Hasher.RightPassword(user.HashedPassword, loginVM.HashedPassword))
            {

                bool validToken = await LoginHelper(username, user.HashedPassword.Split(':')[0]);
                if (validToken)
                {
                    return RedirectToAction("Inbox", "Messages");
                }
                ModelState.AddModelError(string.Empty, "Unable to authenticate.");
                return View(loginVM);
            }
            ModelState.AddModelError(string.Empty, "Invalid Username or Password");
            return View(loginVM);
        }

        // Creates authentication ticket for the user & gets the bearer token from the service
        // & stores it in httpClient.DefaultRequestHeaders.Authorization.
        private async Task<bool> LoginHelper(string username, string hashSaltIter)
        {
            FormsAuthentication.SetAuthCookie(username, false);
            return await _accountRest.GetServiceTokenAsync(username, hashSaltIter);
        }

        public ActionResult Register()
        {
            return View();
        }

        // Registers the user with the service & the client. 
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterVM registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please try again.");
                return View(registerViewModel);
            }
            string username = registerViewModel.Username;
            Users user = await _unitOfWork.usersRolesRepo.GetAsync(username);
            if (user != null)
            {
                ModelState.AddModelError(string.Empty, "User already exists");
                return View(registerViewModel);
            }
            string hashInfo = Hasher.HashGenerator(registerViewModel.HashedPassword);
            string myHash = hashInfo.Split(':')[0]; // returns the hash
            user = new Users() { Username = username, HashedPassword = hashInfo };
            _unitOfWork.usersRolesRepo.Add(user);
            UserRolesMapping roleMap = new UserRolesMapping() { UserID = user.ID, RoleID = _roleId };
            _unitOfWork.rolesMappingRepo.Add(roleMap);
            bool registerOk = await _accountRest.RegisterServiceAsync(username, myHash, _userRole);
            if (registerOk)
            {
                int saved = await _unitOfWork.SaveAsync();
                if (saved < 0)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save User to client.");
                    return View(registerViewModel);
                }
                bool tokenOk = await LoginHelper(username, myHash);
                if (tokenOk)
                {
                    return RedirectToAction("Inbox", "Messages");
                }
            }
            ModelState.AddModelError(string.Empty, "Unable to register user.");
            return View(registerViewModel);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
