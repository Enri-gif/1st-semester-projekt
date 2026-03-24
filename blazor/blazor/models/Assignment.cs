using System;
using System.Collections.Generic;

namespace blazor.models
{
    public class Assignment
    {
        public string Answer { get; set; } = "";
        public string Education { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Level { get; set; } = "";
        public int Year { get; set; } = DateTime.Today.Year;
        public DateTime Date { get; set; } = DateTime.Today;
        public string Subquestion { get; set; } = "";
        public int Subtest { get; set; } = 1;
        public string Topic { get; set; } = "";
        public string Owner { get; set; } = "Prøvebank";
        public int Number { get; set; } = 1;
        public int Points { get; set; } = 0;
        public List<string> Tags { get; set; } = new List<string>();
    }
}
