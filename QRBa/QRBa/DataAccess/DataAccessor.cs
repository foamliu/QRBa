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
        private readonly IdGenerator idGen;

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
            idGen = new IdGenerator(current => GetNextAccountIdRange(128));
        }

        #endregion

        #region accessors

        private static IAccountRepository _accountRepositoryOverrided;

        public static IAccountRepository AccountRepository
        {
            get { return _accountRepositoryOverrided ?? Instance; }
            set { _accountRepositoryOverrided = value; }
        }

        private static ICodeRepository _codeRepositoryOverrided;

        public static ICodeRepository CodeRepository
        {
            get { return _codeRepositoryOverrided ?? Instance; }
            set { _codeRepositoryOverrided = value; }
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

        private IdGenerator.IdRange GetNextAccountIdRange(int size)
        {
            var result = QueryStoreProcedure("GetNextAccountIdRange", new Dictionary<string, object>
                                                                            {
                                                                                {
                                                                                    "requester",
                                                                                    Environment.MachineName
                                                                                },
                                                                                {"rangeLength", size},
                                                                            });
            if (result.Tables.Count > 1 || result.Tables[1].Rows.Count > 0)
            {
                var row = result.Tables[1].Rows[0];
                return new IdGenerator.IdRange(
                    row.GetInt64Field("StartId"),
                    row.GetInt64Field("EndId"),
                    row.GetInt64Field("StartId"));
            }

            return null;
        }

        public class IdGenerator
        {
            private readonly Func<IdRange, IdRange> _nextBatchFunc;
            private IdRange _currentRange;
            //TODO: aync retrieve next batch?
            private readonly object _lock = new object();

            public class IdRange
            {
                public long Start { get; set; }
                public long End { get; set; }
                public long Pos { get; set; }

                public IdRange(long start, long end, long pos)
                {
                    this.Start = start;
                    this.End = end;
                    this.Pos = pos;
                }
            }

            public IdGenerator(Func<IdRange, IdRange> nextBatchFunc)
            {
                _nextBatchFunc = nextBatchFunc;
                _currentRange = null;
            }

            public long NextId
            {
                get
                {
                    lock (_lock)
                    {
                        if (_currentRange == null || _currentRange.Pos >= _currentRange.End)
                        {
                            _currentRange = _nextBatchFunc(_currentRange);
                        }
                        return _currentRange.Pos++;
                    }
                }
            }
        }
    }
}
