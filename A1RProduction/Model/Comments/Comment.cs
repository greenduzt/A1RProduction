using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Comments
{
    public class Comment : User
    {
        public int CommentID { get; set; }
        public int QuoteID { get; set; }
        public int UserID { get; set; }
        public string UserComment { get; set; }
        public string Date { get; set; }

    }
}
