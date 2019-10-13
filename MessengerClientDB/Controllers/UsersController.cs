using MessengerClientDB.Models;
using MessengerClientDB.Restful;
using MessengerClientDB.Services;
using MessengerClientDB.Unity;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MessengerClientDB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private IUnitOfWork _unitOfWork;

        private IUsersService _usersService;

        private IUsersRest _usersRest;

        public UsersController(IUsersService usersService, IUsersRest usersRest)
        {
            _unitOfWork = new UnitOfWork(new MessengerClient_DBEntities());
            _usersService = usersService;
            _usersRest = usersRest;
        }

        // GET: Users
        public ActionResult Index()
        {
            var users = _unitOfWork.usersRolesRepo.GetAll();
            var usersRoles = _usersService.GetAllUsersRolesVM(users);
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
            Users user = _unitOfWork.usersRolesRepo.Get((int)id);
            UpdateRolesViewModel model = _usersService.GetUpdateRolesVM(user, _unitOfWork.usersRolesRepo.GetAllRoles());
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
            bool addRolesToService = false;
            var updatedRoles = new List<string>();
            string[] allRoles = _unitOfWork.usersRolesRepo.GetAllRoles();
            string username = _unitOfWork.usersRolesRepo.Get(model.Id).Username;
            int checkedLength = model.Checked.Length;
            for (int i = 0; i < checkedLength; i++)
            {
                if (model.Checked[i])
                {
                    updatedRoles.Add(allRoles[i]);
                }
            }
            string[] myRoles = updatedRoles.ToArray();
            addRolesToService = await _usersRest.AddRolesServiceAsync(username, _usersService.stringArrToCSV(myRoles));
            if (addRolesToService)
            {
                _unitOfWork.usersRolesRepo.AddRoles(username, myRoles);
                _unitOfWork.Save();
            }
            return RedirectToAction("Index", "Users");
        }

        // GET: Users/Edit/5
        public ActionResult Remove(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = _unitOfWork.usersRolesRepo.Get((int)id);
            UpdateRolesViewModel update = _usersService.GetUpdateRolesVM(user, _unitOfWork.usersRolesRepo.GetAllRoles());
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
            bool removedRolesFromService = false;
            List<string> updatedRoles = new List<string>();
            string[] allRoles = _unitOfWork.usersRolesRepo.GetAllRoles();
            string username = _unitOfWork.usersRolesRepo.Get(model.Id).Username;
            int checkedLength = model.Checked.Length;
            for (int i = 0; i < checkedLength; i++)
            {
                if (model.Checked[i])
                {
                    updatedRoles.Add(allRoles[i]);
                }
            }
            string[] myRoles = updatedRoles.ToArray();
            removedRolesFromService = await RemoveRolesService(username, _usersService.stringArrToCSV(myRoles));
            if (removedRolesFromService)
            {
                _unitOfWork.usersRolesRepo.RemoveRoles(username, myRoles);
                _unitOfWork.Save();
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
            Users user = _unitOfWork.usersRolesRepo.Get((int)id);
            UserRolesViewModel userVM = _usersService.GetUserRolesVM(user);
            if (userVM == null)
            {
                return HttpNotFound();
            }
            return View(userVM);
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
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }


        // Deletes user from client database & calls a method to delete user from the service database.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            bool deletedFromService = await DeleteFromService(1032);
            if (deletedFromService)
            {
                _unitOfWork.usersRolesRepo.Remove(_unitOfWork.usersRolesRepo.Get(1032));
                _unitOfWork.Save();
            }
            return RedirectToAction("Index");
        }

        // Rest call to the service to delete user from the database. 
        private async Task<bool> DeleteFromService(int Id)
        {
            try
            {
                string username = "Tester111";
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

    }
}