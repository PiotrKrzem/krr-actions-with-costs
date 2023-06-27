using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Class is a generic statement NumericUpDown object
    /// </summary>
    class QueryNumericUpDown : NumericUpDown
    {
        public QueryNumericUpDown(FlowLayoutPanel queryPanel) : base()
        {
            Font = StatementConstants.FONT;
            Width = queryPanel.Width - StatementConstants.OFFSET;
            Value = 0;
        }
    }
}