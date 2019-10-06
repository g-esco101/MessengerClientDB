using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MessengerClientDB.Models;
using MessengerClientDB.Repositories;
using MessengerClientDB.Services;
using Microsoft.AspNet.Identity;

namespace MessengerClientDB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private IUsersRepository _usersRepo;

        private IUsersService _userService;

        public UsersController(IUsersRepository usersRepository, IUsersService usersService)
        {
            this._usersRepo = usersRepository;
            this._userService = usersService;
        }

        // GET: Users
        public ActionResult Index()
        {
            var users = _usersRepo.GetAllUsersRoles();
            List<UserRolesViewModel> usersRoles = _userService.GetUserRolesVMAll(users);
            if (usersRoles == null)
            {
                return HttpNotFound();

            }
            return View(usersRoles);
        }

        // Add user roles - get method.
        public ActionResult Add(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UpdateRolesViewModel model = _userService.GetUpdateRolesVMById(_usersRepo.GetUsersRoles((int)id), _usersRepo.GetAllRoles());
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // Adds roles to users in the client database & calls a method to add user roles in the service database.
        [HttpPost]
        //    [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add([Bind(Include = "Id,Checked")]UpdateRolesViewModel model)
        {
            string[] myRoles, allRoles; bool addRolesFromService = false;
            int checkedLength; string username;
            List<string> updatedRoles = new List<string>();
            allRoles = _usersRepo.GetAllRoles();
            username = _usersRepo.GetUserName(model.Id);
            checkedLength = model.Checked.Length;
            for (int i = 0; i < checkedLength; i++)
            {
                if (model.Checked[i])
                {
                    updatedRoles.Add(allRoles[i]);
                }
            }
            myRoles = updatedRoles.ToArray();
            addRolesFromService = await AddRolesService(username, _userService.stringArrToCSV(myRoles));
            if (addRolesFromService)
            {
                _usersRepo.AddRoles(username, myRoles);
            }
            return RedirectToAction("Index", "Users");
        }

        // Rest call to service to add user roles.
        private async Task<bool> AddRolesService(string username, string rolesCSV)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/users/addroles?username=" + username + "&roles=" + rolesCSV);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
        // GET: Users/Edit/5
        public ActionResult Remove(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UpdateRolesViewModel update = _userService.GetUpdateRolesVMById(_usersRepo.GetUsersRoles((int)id), _usersRepo.GetAllRoles());
            if (update == null)
            {
                return HttpNotFound();
            }
            return View(update);
        }

        // Removes user roles from users in the client database & calls a method to remove user roles in the service database.
        [HttpPost]
        //    [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Remove(UpdateRolesViewModel model)
        {
            string[] myRoles, allRoles; bool removedRolesFromService = false;
            int checkedLength; string username;
            List<string> updatedRoles = new List<string>();
            allRoles = _usersRepo.GetAllRoles();
            username = _usersRepo.GetUserName(model.Id);
            checkedLength = model.Checked.Length;
            for (int i = 0; i < checkedLength; i++)
            {
                if (model.Checked[i])
                {
                    updatedRoles.Add(allRoles[i]);
                }
            }
            myRoles = updatedRoles.ToArray();
            removedRolesFromService = await RemoveRolesService(username, _userService.stringArrToCSV(myRoles));
            if (removedRolesFromService)
            {
                _usersRepo.RemoveRoles(username, myRoles);
            }
            return RedirectToAction("Index", "Users");
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRolesViewModel user = _userService.GetUserRolesVMById(_usersRepo.GetUsersRoles((int)id));
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // Deletes user from client database & calls a method to delete user from the service database.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            bool deletedFromService = await DeleteFromService(id);
            if (deletedFromService)
            {
                _usersRepo.DeleteUser(id);
            }
            return RedirectToAction("Index");
        }

        // Rest call to the service to delete user from the database. 
        private async Task<bool> DeleteFromService(int Id)
        {
            try
            {
                string username = _usersRepo.GetUserName(Id);
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/users/delete?username=" + username);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        // Rest call to service to remove user roles.
        private async Task<bool> RemoveRolesService(string username, string rolesCSV)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/users/removeroles?username=" + username + "&roles=" + rolesCSV);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _usersRepo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}