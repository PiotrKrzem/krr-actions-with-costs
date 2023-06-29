using Syncfusion.WinForms.ListView;
using Syncfusion.WinForms.ListView.Enums;
using System.Collections.Generic;
using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Class is a generic statement SfComboBox object
    /// </summary>
    class QuerySFComboBox : TextBox
    {
        public List<string> Actions { get; set; }
        public QuerySFComboBox(FlowLayoutPanel queryPanel) : base()
        {
            Font = StatementConstants.FONT;
            Width = queryPanel.Width - StatementConstants.OFFSET;
            // ComboBoxMode = ComboBoxMode.MultiSelection;
        }
    }
}