using QRBa.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRBa.Domain;
using System.Data;

namespace QRBa.DataAccess
{
    public partial class DataAccessor : IForumRepository
    {
        public Comment AddComment(Comment newComment)
        {
            var result = QueryStoreProcedure("AddComment", new Dictionary<string, object>
                                                          {
                                                              {"param_accountId", newComment.AccountId},
                                                              {"param_content", newComment.Content},
                                                          });
            if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
            {
                var comment = new Comment().FromRow(result.Tables[1].Rows[0]);
                return comment;
            }
            return null;
        }

        public List<Comment> GetComments()
        {
            var list = new List<Comment>();
            var result = QueryStoreProcedure("GetComments", new Dictionary<string, object>());
            if (result.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    var comment = new Comment().FromRow(row);
                    list.Add(comment);
                }
            }
            return list;
        }
    }
}
