using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace Enviroself.Infrastructure.DependencyManagement
{
    /// <summary>
    /// DependencyRegister class
    /// 
    /// nuGet packages: 1. Autofac.Extensions.DependencyInjection
    ///                 2. Autofac
    /// </summary>
    public static class DependencyRegister
    {
        /// <summary>
        /// Register services manually
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IContainer Register_Manual(this ContainerBuilder builder, IServiceCollection services)
        {
            // Populate the Autofac container builder
            builder.Populate(services);

            // Register Services
            #region Services

            //builder.RegisterType<TestService>().As<ITestService>();

            #endregion

            // Return IContainer
            return builder.Build();
        }

        /// <summary>
        /// Auto register all the services and repositories.
        /// 
        /// Notes:
        /// 1. All the services that will be injected must end with "Service".
        /// 2. All the repositories that will be injected must end with "Repository".
        /// 3. Don't let any interface unimplemented.
        /// 4. If one interface is implemented by many clases(>1), only the last class will be injected for that particular interface.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IContainer Register_Automatic(this ContainerBuilder builder, IServiceCollection services)
        {
            // Populate the Autofac container builder
            builder.Populate(services);

            //Get assemply
            var dataAccess = Assembly.GetExecutingAssembly();

            // Register Services
            builder.RegisterAssemblyTypes(dataAccess)
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();

            // Register Repositories

            //builder.RegisterAssemblyTypes(dataAccess)                 // Uncomment these lines after creating and 
            //       .Where(t => t.Name.EndsWith("Repository"))         // implementing IRepository interface
            //       .As<IRepository>();                                //

            // Return IContainer
            return builder.Build();
        }
    }
}
