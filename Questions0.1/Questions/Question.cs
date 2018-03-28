using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Questions
{
    class Question
    {
        public string Text { get; set; }
        public List<Answer> AnswerList { get; set; }
        public Question()
        {
            AnswerList = new List<Answer>();
        }
    }
}
