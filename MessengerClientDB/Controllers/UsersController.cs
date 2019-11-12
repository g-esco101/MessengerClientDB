using MessengerClientDB.Models;
using MessengerClientDB.Models.ViewModels;
using MessengerClientDB.Restful;
using MessengerClientDB.Unit;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace MessengerClientDB.Controllers
{
    public class UsersController : Controller
    {
        private IUnitOfWork _unitOfWork;

        private IUsersRest _usersRest;

        public UsersController(IUsersRest usersRest)
        {
            _unitOfWork = new UnitOfWork();
            _usersRest = usersRest;
        }


        // Displays all users & their roles. 
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            List<UserRolesVM> viewModels = new List<UserRolesVM>();
            var users = await _unitOfWork.usersRolesRepo.GetAllUsersAsync();
            foreach (var user in users)
            {
                string roles = RolesToCSV(user);
                UserRolesVM userRolesViewModel = new UserRolesVM() { Id = user.ID, MyRoles = roles, Username = user.Username };
                viewModels.Add(userRolesViewModel);
            }
            if (viewModels == null)
            {
                return HttpNotFound();
            }
            return View(viewModels);
        }

        // Get
        // Sets up the view model to add roles, namely RolesUpdateVM.
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateRoles(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = await _unitOfWork.usersRolesRepo.GetAsync((int)id);
            var roleMap = user.UserRolesMapping;
            List<CheckBoxRoles> roles = new List<CheckBoxRoles>();
            var roleMasters = await _unitOfWork.rolesMappingRepo.GetRoleMastersAsync();
            bool IsChecked;
            foreach (var roleMaster in roleMasters)
            {
                CheckBoxRoles cbRoles = new CheckBoxRoles() { ID = roleMaster.ID, Role = roleMaster.RoleName, IsChecked = false };
                roles.Add(cbRoles);
            }
            RolesUpdateVM model = new RolesUpdateVM()
            {
                Id = (int)id,
                Username = user.Username,
                MyRoles = RolesToCSV(user),
                RoleMasters = roles
            };
            TempData["roles"] = roles;
            return View(model);
        }

        // Adds a UserRoleMapping to the database & then initiates are rest call to the service to update
        // its database with the user's new role.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateRoles([Bind(Include = "Id,Username,MyRoles,RoleMasters")]RolesUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (TempData["roles"] == null)
            {
                return View(model);
            }
            List<CheckBoxRoles> roles = (List<CheckBoxRoles>)TempData["roles"];
            Users user = await _unitOfWork.usersRolesRepo.GetAsync(model.Id);
            var userRoleMaps = user.UserRolesMapping;

            // Gets the role names to be updated. 
            var rolesAdding = new List<string>();
            var rolesRemoving = new List<string>();
            foreach (var role in model.RoleMasters)
            {
                // assigns the role name to role.Role. 
                role.Role = roles.Where(r => r.ID == role.ID).Select(r => r.Role).FirstOrDefault();
                // check if user is in role.
                UserRolesMapping map = userRoleMaps.Where(r => r.RoleID == role.ID).FirstOrDefault();
                if (role.IsChecked && map == null)
                {
                    map = new UserRolesMapping() { UserID = user.ID, RoleID = role.ID };
                    _unitOfWork.rolesMappingRepo.Add(map);
                    rolesAdding.Add(role.Role);
                }
                else if (!role.IsChecked && map != null)
                {
                    _unitOfWork.rolesMappingRepo.Remove(map);
                    rolesRemoving.Add(role.Role);
                }
            }
            // Updates user roles in the service's database. 
            if (rolesAdding.Any())
            {
                await _usersRest.AddRolesServiceAsync(user.Username, rolesAdding);
            }
            if (rolesRemoving.Any())
            {
                await _usersRest.RemoveRolesServiceAsync(user.Username, rolesRemoving);
            }
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Index");
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = await _unitOfWork.usersRolesRepo.GetAsync((int)id);
            string roles = RolesToCSV(user);
            UserRolesVM model = new UserRolesVM() { MyRoles = roles, Username = user.Username };
            return View(model);
        }

        // Deletes user from client database & calls a method to delete user from the service database.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //        var user = _unitOfWork.usersRolesRepo.Get((int)id);
            var user = await _unitOfWork.usersRolesRepo.SingleOrDefaultAsync(u => u.ID == id);
            string username = user.Username;
            var roleMaps = user.UserRolesMapping.ToList();
            foreach (var rolemap in roleMaps)
            {
                _unitOfWork.rolesMappingRepo.Remove(rolemap);
            }
            // Must update the UserRolesMapping table before the user can be removed.
            await _unitOfWork.SaveAsync();
            _unitOfWork.usersRolesRepo.Remove(user);
            var removeMsgs = await _unitOfWork.messagesRepo.FindAsync(m => (m.ReceiverID == username && m.Queued == true) || ((m.SenderID == username) && m.Queued == false));
            _unitOfWork.messagesRepo.RemoveRange(removeMsgs);
            await _unitOfWork.SaveAsync();
            _usersRest.DeleteFromService(username);
            return RedirectToAction("Index");
        }

        private string RolesToCSV(Users user)
        {
            var roleMaps = user.UserRolesMapping;
            int roleMapsLen = roleMaps.Count;
            int iter = 0;
            string roles = "";
            foreach (var map in roleMaps)
            {
                string role = map.RoleMaster.RoleName;
                roles += role;
                if ((iter + 1) < roleMapsLen)
                {
                    roles += ", ";
                }

                iter++;
            }
            return roles;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
