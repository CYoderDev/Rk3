using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rk3.Data
{
    public abstract class Database
    {
        public string connectionString;

        public abstract IDbConnection CreateConnection();
        public abstract IDbCommand CreateCommand();
        public abstract IDbConnection CreateOpenConnection();
        public abstract IDbCommand CreateCommand(string commandText, IDbConnection connection);
        public abstract IDbCommand CreateStoredProcedure(string procedureName, IDbConnection connection);
        public abstract IDataParameter CreateParameter(string parameterName, object parameterValue);
    }

    public class MSSQLDatabase : Database
    {
        public override IDbConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }

        public override IDbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public override IDbConnection CreateOpenConnection()
        {
            SqlConnection connection = (SqlConnection)CreateConnection();
            connection.Open();

            return connection;
        }

        public override IDbCommand CreateCommand(string commandText, IDbConnection connection)
        {
            SqlCommand command = (SqlCommand)CreateCommand(commandText, connection);
            command.CommandType = CommandType.Text;

            return command;
        }

        public override IDbCommand CreateStoredProcedure(string procedureName, IDbConnection connection)
        {
            SqlCommand command = (SqlCommand)CreateCommand(procedureName, connection);
            command.CommandType = CommandType.StoredProcedure;

            return command;
        }

        public override IDataParameter CreateParameter(string parameterName, object parameterValue)
        {
            return new SqlParameter(parameterName, parameterValue);
        }
    }
}
