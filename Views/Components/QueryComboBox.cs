using System.Collections.Generic;
using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Class is a generic statement ComboBox object
    /// </summary>
    class QueryComboBox : ComboBox
    {
        public QueryComboBox(FlowLayoutPanel queryPanel, List<string> items) : base()
        {
            Font = StatementConstants.FONT;
            Width = queryPanel.Width - StatementConstants.OFFSET;
            Items.Clear();
            Items.AddRange(items.ToArray());
        }
    }
}
