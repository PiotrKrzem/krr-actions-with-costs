using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Class is a generic statement ComboBox object
    /// </summary>
    class QueryInitStateBox : ComboBox
    {
        public QueryInitStateBox(FlowLayoutPanel queryPanel, List<List<Literal>> items) : base()
        {
            Font = StatementConstants.FONT;
            Width = queryPanel.Width - StatementConstants.OFFSET;
            Items.Clear();
            Items.AddRange(items.ToArray());

            List<string> states = new List<string>();
            foreach (List<Literal> state in items)
            {
                states.Add(string.Join(" ", state.Select(x => (x.IfHolds ? "": "~") + x.Fluent)));
            }
            Items.AddRange(states.ToArray());
        }
    }
}
