using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.Utilities
{
    public class DapperUtilities<T>
    {
        static IDbConnection _dbConnection;

        public static async Task<T> GetSingleObjectAsync(IDbConnection dbConnection, IDictionary<string, DatabaseParameterWrappers> paras, string procName, CommandType commandType)
        {
            try
            {
                _dbConnection = dbConnection;
                T retVal;
                using (var con = _dbConnection)
                {
                    DynamicParameters dbParams = new DynamicParameters();
                    foreach (var para in paras)
                    {
                        var dictionaryValue = para.Value;
                        dbParams.Add(para.Key, dictionaryValue.ParameterValue, dictionaryValue.DbParameterType, dictionaryValue.DbParameterDirection);
                    }

                    retVal = await SqlMapper.QueryFirstOrDefaultAsync<T>(_dbConnection, procName, dbParams, commandType: commandType);
                }
                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<T> RunQueryorProcAsync_NoDbWrappers(DbConnection dbConnection, IDictionary<string, string> parameters, string procName, CommandType commandType)
        {
            _dbConnection = dbConnection;
            T retVal;
            using (var con = _dbConnection)
            {
                DynamicParameters dbParams = new DynamicParameters();
                foreach (var para in parameters)
                {
                    dbParams.Add(para.Key, para.Value);
                }
                var val = await SqlMapper.QueryAsync<T>(con, procName, dbParams, commandType: commandType);
                retVal = val.FirstOrDefault();
            }

            return retVal;
        }
    }
}
