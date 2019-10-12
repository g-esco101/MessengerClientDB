using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using MessengerClientDB.Repositories;
using MessengerClientDB.Roles;
using MessengerClientDB.Services;
using MessengerClientDB.Unity;

namespace MessengerClientDB
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IMessagesRepository, MessagesRepository>();
            container.RegisterType<IUsersRolesRepository, UsersRolesRepository>();
            container.RegisterType<IMessageService, MessageService>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IUsersService, UsersService>();          

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}