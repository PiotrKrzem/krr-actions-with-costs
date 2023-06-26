using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Class is a generic statement NumericUpDown object
    /// </summary>
    class StatementNumericUpDown : NumericUpDown
    {
        public StatementNumericUpDown(FlowLayoutPanel statementsPanel) : base()
        {
            Font = StatementConstants.FONT;
            Width = statementsPanel.Width - StatementConstants.OFFSET;
            Value = 0;
        }
    }
}
