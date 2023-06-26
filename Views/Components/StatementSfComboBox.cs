using Syncfusion.WinForms.ListView;
using Syncfusion.WinForms.ListView.Enums;
using System.Collections.Generic;
using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Class is a generic statement SfComboBox object
    /// </summary>
    class StatementSFComboBox : SfComboBox
    {
        public StatementSFComboBox(FlowLayoutPanel statementsPanel) : base()
        {
            Font = StatementConstants.FONT;
            Width = statementsPanel.Width - StatementConstants.OFFSET;
            ComboBoxMode = ComboBoxMode.MultiSelection;
        }
    }
}
