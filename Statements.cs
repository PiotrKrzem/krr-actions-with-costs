using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace actions_with_costs
{
    public class Literal
    {
        public Literal(string fluent, bool ifHolds)
        {
            Fluent = fluent;
            IfHolds = !ifHolds;
        }
        public string Fluent { get; set; }
        public bool IfHolds { get; set; }
    }
    public class Statement
    {
        public Statement(string type, string text)
        {
            Type = type;
            Text = text;
            Delimeter = "~";
        }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Delimeter { get; private set; }
    }
    public class InitiallyStatement : Statement
    {
        public InitiallyStatement(string type, string text, string condition) : base(type, text)
        {
            Condition = new Literal(condition.Replace("~", ""), condition.Contains(Delimeter));
        }
        public Literal Condition { get; set; } 
    }
    public class AfterStatement : Statement
    {
        public AfterStatement(string type, string text, string postcondition, string actions) : base(type, text)
        {
            Postcondition = new Literal(postcondition.Replace("~", ""), postcondition.Contains(Delimeter)); ;
            Actions = new List<string>();
            string[] words = actions.Split(',');
            foreach(string w in words)
            {
                Actions.Add(w);
            }
        }
        public Literal Postcondition { get; set; }
        public List<string> Actions { get; set; }
    }
    public class CausesStatement : Statement
    {
        public CausesStatement(string type, string text, string action, string postcondition, 
                                                        string precondition, int cost) : base(type, text)
        {
            Action = action;
            Postcondition = new Literal(postcondition.Replace("~", ""), postcondition.Contains(Delimeter)); ;
            Precondition = new List<Literal>();
            if (precondition != String.Empty)
            {
                string[] words = precondition.Split(',');
                foreach (string w in words)
                {
                    Precondition.Add(new Literal(w.Replace("~", ""), w.Contains(Delimeter)));
                }
            }
            Cost = cost;
        }
        public string Action { get; set; }
        public Literal Postcondition { get; set; }
        public List<Literal> Precondition { get; set; }
        public int Cost { get; set; }
    }
}
