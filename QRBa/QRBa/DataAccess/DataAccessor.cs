using MySql.Data.MySqlClient;
using QRBa.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QRBa.DataAccess
{
    public partial class DataAccessor
    {
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["QRBaDB"].ConnectionString;

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

        private static IAccountRepository _accountRepositoryOverrided;

        public static IAccountRepository AccountRepository
        {
            get { return _accountRepositoryOverrided ?? Instance; }
            set { _accountRepositoryOverrided = value; }
        }

        #endregion

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
    }
}
