
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace Westwind.Globalization.AspnetCore
{
    /// <summary>
    /// A Hosting Environment Abstraction for ASP.NET Core that
    /// can be used to provide a single .Host instance that works
    /// for both .NET Core 3.x and 2.x
    ///
    /// Basically abstracts away the fucked up way Microsoft
    /// 
    ///
    /// You still need to dual target for this to work.
    /// </summary>
    public class HostingAbstraction
    {
        private IHostingEnvironment env;

        private IServiceProvider Provider { get; }

        public HostingAbstraction(IServiceProvider provider)
        {
            if (CurrentHost == null)
            {
                Provider = provider;
                InitializeHost(Provider);
            }
        }
#if NETCORE2
        public static IHostingEnvironment CurrentHost { get; set; }

        public IHostingEnvironment Host
        {
            get { return CurrentHost; }
        }
#else
        public static IWebHostEnvironment CurrentHost {get; private set;}

        public IWebHostEnvironment Host
        {
            get { return CurrentHost; }
        }
#endif

        public static void InitializeHost(IServiceProvider serviceProvider)
        {

#if NETCORE2
            CurrentHost = serviceProvider.GetService<IHostingEnvironment>();
#else
            CurrentHost = serviceProvider.GetService<IWebHostEnvironment>();
#endif
        }

    }

   
}
