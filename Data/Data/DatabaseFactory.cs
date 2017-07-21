using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rk3.Data
{
    public sealed class DatabaseFactory
    {
        public static DbFactoryConfigHandler sectionHandler = (DbFactoryConfigHandler)ConfigurationManager.GetSection("DatabaseFactoryConfiguration");

        public static Database CreateDatabase()
        {
            if (string.IsNullOrEmpty(sectionHandler.Name))
                throw new Exception("Database name not defined in configuration file.");

            try
            {
                Type tDb = Type.GetType(sectionHandler.Name);

                ConstructorInfo constructor = tDb.GetConstructor(Type.EmptyTypes);

                Database createdDb = (Database)constructor.Invoke(null);

                createdDb.connectionString = sectionHandler.ConnectionString;

                return createdDb;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error instantiating database {0}. {1}", sectionHandler.Name, ex.Message));
            }
        }
    }
}
