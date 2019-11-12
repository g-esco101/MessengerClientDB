using MessengerClientDB.Models;
using MessengerClientDB.Restful;
using MessengerClientDB.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MessengerClientDB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRolesMappingsController : Controller
    {

        private IUnitOfWork _unitOfWork;

        private IUsersRest _usersRest;

        public UserRolesMappingsController(IUsersRest usersRest)
        {
            _unitOfWork = new UnitOfWork(new MessengerClient_DBEntities());
            _usersRest = usersRest;
        }

        // GET:
        public async Task<ActionResult> Index()
        {
            var userRolesMapping = _unitOfWork.rolesMappingRepo.GetAllRoleMaps();

            // Converts UserRoleMappings into their corresponding view models.
            List<RoleMappingVM> roleMapVMs = new List<RoleMappingVM>();
            foreach (var roleMap in userRolesMapping)
            {
                roleMapVMs.Add(CreateRoleMapVM(roleMap));
            }
            return View(roleMapVMs);
        }

        // Get
        // Sets up the view model to add roles, namely RolesUpdateVM.
        public async Task<ActionResult> Add(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRolesMapping roleMap = _unitOfWork.rolesMappingRepo.Get((int)id);
            Users user = roleMap.Users;
            List<CheckBoxRoles> rolesAdding = new List<CheckBoxRoles>();
            var roleMasters = _unitOfWork.rolesMappingRepo.GetRoleMasters();

            foreach (var roleMaster in roleMasters)
            {
                rolesAdding.Add(new CheckBoxRoles()
                {
                    ID = roleMaster.ID,
                    Role = roleMaster.RoleName,
                    // If the use is in a particular role, that role will not appear as an option to add.
                    IsChecked = user.UserRolesMapping.Where(x => x.RoleID == roleMaster.ID).Any()
                });
            }
            RolesUpdateVM model = new RolesUpdateVM()
            {
                Id = (int)id,
                Username = user.Username,
                MyRoles = GetMyRoles(user),
                RoleMasters = rolesAdding
            };
            return View(model);
        }

        // Adds a UserRoleMapping to the database & then initiates are rest call to the service to update
        // its database with the user's new role.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(RolesUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            UserRolesMapping roleMap = _unitOfWork.rolesMappingRepo.Get(model.Id);
            Users user = roleMap.Users;
            int userId = user.ID;
            string username = user.Username;
            var myRoleMaps = user.UserRolesMapping;

            // Gets the role names to be updated. 
            var rolesAdding = new List<string>();
            foreach (var role in model.RoleMasters)
            {
                UserRolesMapping prospectiveRoleMap = myRoleMaps.Where(r => r.RoleID == role.ID).FirstOrDefault();
                if (role.IsChecked && prospectiveRoleMap == null)
                {
                    prospectiveRoleMap = new UserRolesMapping() { UserID = userId, RoleID = role.ID };
                    _unitOfWork.rolesMappingRepo.Add(prospectiveRoleMap);
                    rolesAdding.Add(role.Role);
                }
            }
            // Updates user roles in the service's database. 
            if (rolesAdding.Any())
            {
                _usersRest.AddRolesServiceAsync(username, rolesAdding);
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        // GET: UserRolesMappings/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRolesMapping userRolesMapping = _unitOfWork.rolesMappingRepo.Get((int)id);
            if (userRolesMapping == null)
            {
                return HttpNotFound();
            }
            return View(CreateRoleMapVM(userRolesMapping));
        }

        // POST: UserRolesMappings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UserRolesMapping userRolesMapping = _unitOfWork.rolesMappingRepo.Get(id);
            // Removes role from the service's database
            await _usersRest.RemoveRolesServiceAsync(userRolesMapping.Users.Username, new List<string> { userRolesMapping.RoleMaster.RoleName });
            _unitOfWork.rolesMappingRepo.Remove(userRolesMapping);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        // Creates the UserRoleMapping view model.
        private RoleMappingVM CreateRoleMapVM(UserRolesMapping roleMap)
        {
            return new RoleMappingVM() { Id = roleMap.ID, Username = roleMap.Users.Username, Role = roleMap.RoleMaster.RoleName };
        }

        // Puts the roles in CSV form.
        private string GetMyRoles(Users user)
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

    }
}
/*
 

        // GET: UserRolesMappings/Create
        public ActionResult Create()
        {
            ViewBag.RoleID = new SelectList(_unitOfWork.DbContext.RoleMaster, "ID", "RoleName");
            ViewBag.UserID = new SelectList(_unitOfWork.DbContext.Users, "ID", "Username");
            return View();
        }

        // POST: UserRolesMappings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,UserID,RoleID")] UserRolesMapping userRolesMapping)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.DbContext.UserRolesMapping.Add(userRolesMapping);
                await _unitOfWork.DbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(_unitOfWork.DbContext.RoleMaster, "ID", "RoleName", userRolesMapping.RoleID);
            ViewBag.UserID = new SelectList(_unitOfWork.DbContext.Users, "ID", "Username", userRolesMapping.UserID);
            return View(userRolesMapping);
        }




        // POST: UserRolesMappings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,UserID,RoleID")] UserRolesMapping userRolesMapping)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.DbContext.Entry(userRolesMapping).State = EntityState.Modified;
                await _unitOfWork.DbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.RoleID = new SelectList(_unitOfWork.DbContext.RoleMaster, "ID", "RoleName", userRolesMapping.RoleID);
            ViewBag.UserID = new SelectList(_unitOfWork.DbContext.Users, "ID", "Username", userRolesMapping.UserID);
            return View(userRolesMapping);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
        // Get
        public async Task<ActionResult> UpdateRoles(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRolesMapping roleMap = _unitOfWork.DbContext.UserRolesMapping.Find((int)id);
            Users user = _unitOfWork.usersRolesRepo.Get(roleMap.UserID);
            var myRoleMaps = user.UserRolesMapping;
            var roleMaster = _unitOfWork.rolesMappingRepo.GetRoleMasters();
            List<CheckBoxRoles> roles = new List<CheckBoxRoles>();
            foreach (var role in roleMaster)
            {
                bool check = myRoleMaps.Contains(new UserRolesMapping { RoleID = role.ID }, new RoleMapComparer());
                roles.Add(new CheckBoxRoles()
                {
                    ID = role.ID,
                    Role = role.RoleName,
                    IsChecked = check
                });
            }
            RolesUpdateVM model = new RolesUpdateVM()
            {
                Id = user.ID,
                Username = user.Username,
                MyRoles = GetMyRoles(user),
                RoleMasters = roles
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> UpdateRoles(RolesUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }
            Users user = _unitOfWork.usersRolesRepo.Get(model.Id);
            int userId = user.ID;
            string username = user.Username;
            var myRoleMaps = user.UserRolesMapping;

            // Use TempData to avoid querying the database. 
            UpdateRolesVM myModel;
            if (TempData["model"] != null)
            {
                myModel = (UpdateRolesVM)TempData["model"];
            }
            else
            {
                myModel = GetUserRolesVM(userId);
            }

            // Gets the role names to be updated. 
            var rolesAdding = new List<string>();
            var rolesRemoving = new List<string>();

            foreach (var role in model.RoleMasters)
            {
                UserRolesMapping roleMap = user.UserRolesMapping.Where(r => r.RoleID == role.ID).FirstOrDefault();

                if (role.IsChecked && roleMap == null)
                {
                    UserRolesMapping newRole = new UserRolesMapping() { UserID = userId, RoleID = role.ID };
                    _unitOfWork.rolesMappingRepo.Add(newRole);
                    rolesAdding.Add(role.Role);
                }
                if (!role.IsChecked && roleMap != null)
                {
                    _unitOfWork.DbContext.UserRolesMapping.Attach(roleMap);
                    _unitOfWork.rolesMappingRepo.Remove(roleMap);
                    rolesRemoving.Add(role.Role);
                }
            }
            // Updates user roles in the service's database. 
            if (rolesAdding.Count > 0)
            {
                _usersRest.AddRolesServiceAsync(username, rolesAdding);
            }
            if (rolesRemoving.Count > 0)
            {
                _usersRest.RemoveRolesServiceAsync(username, rolesRemoving);
            }
            _unitOfWork.Save();
            return true;
        }



        private UpdateRolesVM GetUserRolesVM(int id)
        {
            Users user = _unitOfWork.usersRolesRepo.Get(id);
            string[] allRoles = _unitOfWork.rolesMappingRepo.AllRoleNames();
            UpdateRolesVM model = new UpdateRolesVM()
            {
                Id = user.ID,
                Username = user.Username,
                MyRoles = GetMyRoles(user),
                Roles = allRoles,
                Checked = new bool[allRoles.Length]
            };
            return model;
        }

        private string GetMyRoles(Users user)
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
    
    // GET: UserRolesMappings/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRolesMapping userRolesMapping = await db.UserRolesMapping.FindAsync(id);
            if (userRolesMapping == null)
            {
                return HttpNotFound();
            }
            return View(userRolesMapping);
        }



    
    
    protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


            // GET: UserRolesMappings/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRolesMapping userRolesMapping = await db.UserRolesMapping.FindAsync(id);
            if (userRolesMapping == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleID = new SelectList(db.RoleMaster, "ID", "RoleName", userRolesMapping.RoleID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", userRolesMapping.UserID);
            return View(userRolesMapping);
        }
*/
