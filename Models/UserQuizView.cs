using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Live_Quiz.Models
{
    public class UserQuizView
    {
        public int Score { get; set; }
        public string QuizName { get; set; }
        public int QuizId { get; set; }
    }
    public class UserQuizModal
    {
        public string uid { get; set; }
        public string Uname { get; set; }
        public int score { get; set; }
        public string country { get; set; }
        public string league { get; set; }
    }
}