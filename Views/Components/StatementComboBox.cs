using System.Collections.Generic;
using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Class is a generic statement ComboBox object
    /// </summary>
    class StatementComboBox : ComboBox
    {
        public StatementComboBox(FlowLayoutPanel statementsPanel, List<string> positiveNegativeFluents) : base()
        {
            Font = StatementConstants.FONT;
            Width = statementsPanel.Width - StatementConstants.OFFSET;
            Items.Clear();
            MouseClick += new MouseEventHandler(this.openDropdown);
            Items.AddRange(positiveNegativeFluents.ToArray());
        }

        private void openDropdown(object sender, MouseEventArgs e)
        {
            DroppedDown = true;
        }

    }
}
