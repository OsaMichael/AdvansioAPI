using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.Utilities
{
    public class DatabaseConnection
    {
        public static SqlConnection SqlDatabaseCreateConnection(string connectionString, bool openConnection = false)
        {
            var conn = new SqlConnection(connectionString);

            if (openConnection)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                else if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return conn;
        }

        public static void SqlDatabaseCloseConnection(SqlConnection conn)
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        public static OracleConnection OracleDatabaseCreateConnection(string connectionString, bool openConnection = false)
        {
            var conn = new OracleConnection(connectionString);

            if (openConnection)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
            }

            return conn;
        }

        public static OracleConnection OracleDatabaseOpenConnection(OracleConnection conn)
        {
            //if(conn)
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            return conn;
        }

        internal static DbConnection GetSQLConnection(string v)
        {
            throw new NotImplementedException();
        }

        public static void OracleDatabaseCloseConnection(IDbConnection conn)
        {
            if (conn.State == ConnectionState.Open || conn.State == ConnectionState.Broken)
            {
                conn.Close();
            }
        }

        public static void OracleDatabaseDisposeConnection(IDbConnection conn)
        {
            conn.Dispose();
        }
    }
}
