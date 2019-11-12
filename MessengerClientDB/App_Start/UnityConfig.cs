using MessengerClientDB.Repositories;
using MessengerClientDB.Restful;
using MessengerClientDB.Unit;
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
            container.RegisterType<IUsersRepository, UsersRepository>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IMessagesRest, MessagesRest>();
            container.RegisterType<IAccountRest, AccountRest>();
            container.RegisterType<IUsersRest, UsersRest>();
            container.RegisterType<IRolesRepository, RolesRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}