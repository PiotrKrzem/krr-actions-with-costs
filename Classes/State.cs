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

    }
}
