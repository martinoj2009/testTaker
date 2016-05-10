using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cityCodeTest
{
    class question
    {
        public string QuestionText { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
        public string CorrectAnswer { get; set; }


        public question(string Question, string Answer)
        {
            QuestionText = Question;
            CorrectAnswer = Answer;
        }
    }
}
