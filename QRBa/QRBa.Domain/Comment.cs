﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Domain
{
    public class Comment
    {
        public int AccountId { get; set; }

        public int CommentId { get; set; }

        public string Content { get; set; }

        public DateTime InsertedDateTime { get; set; }
    }
}
