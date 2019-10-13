using MessengerClientDB.Repositories;
using MessengerClientDB.Restful;
using MessengerClientDB.Services;
using MessengerClientDB.Unity;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

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
            container.RegisterType<IMessagesRest, MessagesRest>();
            container.RegisterType<IAccountRest, AccountRest>();
            container.RegisterType<IUsersRest, UsersRest>();


            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}