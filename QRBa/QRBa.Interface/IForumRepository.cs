using QRBa.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Interface
{
    public interface IForumRepository
    {
        Comment AddComment(Comment newComment);

        List<Comment> GetComments();
    }
}
