using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Helpers
{
    public static class AppServicesHelper
    {
        private static IServiceProvider services = null;
        /// <summary>
        /// Provides static access to the framework's services provider
        /// </summary>
        public static IServiceProvider Services
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                services = value;
            }
        }

        /// <summary>
        /// Provides static access to framework's configurations provider
        /// </summary>
        public static IConfiguration Configuration { get; set; }


        /// <summary>
        /// Configuration settings from appsetting.json.
        /// </summary>
        public static TModel GetOptions<TModel>(string section) where TModel : new()
        {
            var model = new TModel();
            Configuration.GetSection(section).Bind(model);
            return model;
        }
    }
}
