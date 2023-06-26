using System.Collections.Generic;
using System.Linq;

namespace actions_with_costs
{
    /// <summary>
    /// Enum representing types of statements
    /// </summary>
    public enum StatementType
    {
        INITIALLY, AFTER, CAUSES
    }


    /// <summary>
    /// Class represents a given literal. It contains the information about fluent name and the boolean indicating if it is negation or simple fluent.
    /// </summary>
    public class Literal
    {
        public Literal(string fluent, bool ifHolds)
        {
            Fluent = fluent;
            IfHolds = !ifHolds;
        }
        public Literal(Literal literal)
        {
            Fluent = literal.Fluent;
            IfHolds = literal.IfHolds;
        }
        public string Fluent { get; set; }
        public bool IfHolds { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Literal literal &&
                   Fluent == literal.Fluent &&
                   IfHolds == literal.IfHolds;
        }

        public override string ToString()
        {
            return IfHolds ? Fluent : "~" + Fluent;
        }
        public bool ExistsInCollection(List<Literal> literals)
        {
            bool ifExists = false;
            foreach(Literal l in literals)
            {
                if (this.Equals(l))
                {
                    ifExists = true;
                }
            }
            return ifExists;
        }

        /// <summary>
        /// Method verifies if given literal is complementary to this one.
        /// </summary>
        /// <param name="literal">second literal used in the comparision</param>
        /// <returns>boolean indicating if the literals are complementary</returns>
        public bool isComplementary(Literal literal)
        {
            return literal.Fluent == Fluent && literal.IfHolds != IfHolds;
        }
    }

    /// <summary>
    /// Class represents a generic statement.
    /// </summary>
    public class Statement
    {
        public Statement(StatementType type, string text)
        {
            Type = type;
            Text = text;
            Delimeter = "~";
        }
        public StatementType Type { get; set; }
        public string Text { get; set; }
        public string Delimeter { get; private set; }

        /// <summary>
        /// Abstract method responsible for verifying the consistency of the model given a set of statements.
        /// </summary>
        /// <param name="allStatements"></param>
        /// <returns></returns>
        public virtual bool checkModelConsistency(List<Statement> allStatements) { return true; }
    }

    /// <summary>
    /// Class represents an initially statement.
    /// </summary>
    public class InitiallyStatement : Statement
    {
        public InitiallyStatement(string text, string condition) : base(StatementType.INITIALLY, text)
        {
            Condition = new Literal(condition.Replace("~", ""), condition.Contains(Delimeter));
        }
        public Literal Condition { get; set; }

        public override bool checkModelConsistency(List<Statement> allStatements)
        {
            // Verifying if there exist initially statement that has complementary literal
            if (allStatements.Any(statement => statement.Type == StatementType.INITIALLY && ((InitiallyStatement)statement).Condition.isComplementary(Condition)))
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Class represents an after statement.
    /// </summary>
    public class AfterStatement : Statement
    {
        public AfterStatement(string text, string postcondition, string actions) : base(StatementType.AFTER, text)
        {
            Postcondition = new Literal(postcondition.Replace("~", ""), postcondition.Contains(Delimeter));
            Actions = new List<string>();
            string[] words = actions.Split(',');
            foreach(string w in words)
            {
                Actions.Add(w);
            }
        }
        public Literal Postcondition { get; set; }
        public List<string> Actions { get; set; }

        public override bool checkModelConsistency(List<Statement> allStatements)
        {
            // Verifying if there exist after statement that has complementary literal
            bool complementaryAfter = allStatements
                .FindAll(statement => statement.Type == StatementType.AFTER)
                .Cast<AfterStatement>()
                .ToList()
                .Any(statement => statement.Actions.Equals(Actions) && statement.Postcondition.isComplementary(Postcondition));

            if (complementaryAfter)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Class represents a causes statement.
    /// </summary>
    public class CausesStatement : Statement
    {
        public CausesStatement(string text, string action, string postcondition, string precondition, int cost) : base(StatementType.CAUSES, text)
        {
            Action = action;
            Postcondition = new Literal(postcondition.Replace("~", ""), postcondition.Contains(Delimeter)); ;
            Precondition = new List<Literal>();
            if (precondition != string.Empty)
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

        public override bool checkModelConsistency(List<Statement> allStatements)
        {
            List<CausesStatement> causesStatements = allStatements
               .FindAll(statement => statement.Type == StatementType.CAUSES)
               .Cast<CausesStatement>()
               .ToList();

            // Verifying if there exist causes statement WITH pre-conditions for the same action that results in complementary literal
            bool complementaryCausesWith = causesStatements
                .Any(statement => statement.Action == Action && statement.Precondition.Equals(Precondition) && statement.Postcondition.isComplementary(Postcondition));

            // Verifying if there exist causes statement WITHOUT pre-conditions for the same action that results in complementary literal
            bool complementaryCausesWithout = causesStatements
                .Any(statement => statement.Action == Action && statement.Precondition.Count == 0 && statement.Postcondition.isComplementary(Postcondition));

            if (complementaryCausesWith || complementaryCausesWithout)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method verifies if the causes statement meets given preconditions.
        /// </summary>
        /// <param name="preconditions">list of preconditions to be met</param>
        /// <returns>boolean indicating if the preconditions are met</returns>
        public bool doesMeetPreconditions(List<Literal> preconditions)
        {
            if(Precondition.Count == 0)
            {
                return true;
            }

            foreach(var precondition in Precondition)
            {
               if(!preconditions.Contains(precondition))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
