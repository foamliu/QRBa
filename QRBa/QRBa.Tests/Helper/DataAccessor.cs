using MySql.Data.MySqlClient;
using QRBa.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QRBa.Tests.Helper
{
    public class DataAccessor : ITestability
    {
        public static readonly string ConnectionString = "Server=120.27.41.26;Database=QRBaDB;Uid=root;Pwd=Bugs4fun;";

        #region singleton instance

        private static DataAccessor _instance;

        private static DataAccessor Instance
        {
            get
            {
                if (_instance == null)
                {
                    Interlocked.CompareExchange(ref _instance, new DataAccessor(), null);
                }
                return _instance;
            }
        }

        private DataAccessor()
        {

        }

        #endregion

        #region accessors

        private static ITestability _testabilityOverrided;

        public static ITestability Testability
        {
            get { return _testabilityOverrided ?? Instance; }
            set { _testabilityOverrided = value; }
        }

        #endregion

        #region helper functions

        [SuppressMessage("Microsoft.Security", "CA2100:Sql Injection", Justification = "Only parametered stored procedures will be used here internally")]
        private DataSet QueryStoreProcedure(string spName, IDictionary<string, object> parameters)
        {
            var ds = new DataSet();

            using (var cnn = new MySqlConnection(ConnectionString))
            using (var cmd = new MySqlCommand())
            using (var adapter = new MySqlDataAdapter(cmd))
            {
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                if (parameters != null && parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(parameters.Select(p => new MySqlParameter(p.Key, p.Value)).ToArray());
                }
                cnn.Open();
                adapter.Fill(ds);
            }

            return ds;
        }

        public void Cleanup()
        {
            QueryStoreProcedure("Cleanup", new Dictionary<string, object>());
        }
        #endregion
    }
}
