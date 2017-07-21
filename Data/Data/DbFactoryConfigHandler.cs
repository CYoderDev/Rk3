using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rk3.Data
{
    public class DbFactoryConfigHandler : ConfigurationSection
    {
        [ConfigurationProperty("Name")]
        public string Name { get { return (string)base["Name"]; } }

        [ConfigurationProperty("ConnectionStringName")]
        public string ConnectionStringName { get { return (string)base["ConnectionStringName"]; } }

        public string ConnectionString
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Connection string {0} was not found in the web.config file. {1}", ConnectionStringName, ex.Message));
                }
            }
        }
    }
}
