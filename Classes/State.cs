using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace actions_with_costs
{
    public class State
    {
        public State(List<Literal> literals)
        {
            Literals = new List<Literal>();
            Literals = literals.ToList();
        }
        public List<Literal> Literals { get; set; }

        public override bool Equals(object obj)
        {
            return ((State)obj).Literals.Equals(Literals);
        }

        /// <summary>
        /// Method returns the text displayed to describe the state.
        /// </summary>
        /// <returns>text describing the state</returns>
        public string getStateDescription()
        {
            List<Literal> orderedLiterals = Literals.OrderBy(literal => literal.Fluent).ToList();
            string text = "";
            for(int i=0; i< orderedLiterals.Count; i++)
            {
                text += orderedLiterals[i].ToString();

                if(i != orderedLiterals.Count - 1)
                {
                    text += "\n";
                }
            }
            return text;

        }

        /// <summary>
        /// Method generates all possible states of the system.
        /// </summary>
        /// <param name="allFluents">set of all available fluents</param>
        /// <returns>list of states</returns>
        public static List<State> getAllInitialStates(List<string> allFluents)
        {
            List<List<Literal>> complementaryFluents = allFluents.Select(fluent => new[] { new Literal(fluent, true), new Literal(fluent, false) }.ToList()).ToList();
            List<State> allPossibleStartingStates = new List<State>();
            generateAllPossibleInitialStates(complementaryFluents, 0, new List<Literal>(), allPossibleStartingStates);

            return allPossibleStartingStates;
        }

        private static void generateAllPossibleInitialStates(List<List<Literal>> fluents, int depth, List<Literal> partialOutput, List<State> finalOutput)
        {
            if (depth < fluents.Count)
            {
                foreach (Literal value in fluents[depth])
                {
                    List<Literal> literals = new List<Literal>();
                    literals.AddRange(partialOutput);
                    literals.Add(value);
                    if (literals.Count == fluents.Count)
                    {
                        finalOutput.Add(new State(literals));
                    }
                    generateAllPossibleInitialStates(fluents, depth + 1, literals, finalOutput);
                }
            }
        }
    }
}
