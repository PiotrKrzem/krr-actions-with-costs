﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Class is a generic statement ComboBox object
    /// </summary>
    class StatementComboBox : ComboBox
    {
        public StatementComboBox(FlowLayoutPanel statementsPanel, List<string> items) : base()
        {
            Font = StatementConstants.FONT;
            Width = statementsPanel.Width - StatementConstants.OFFSET;
            Items.Clear();
            Items.AddRange(items.ToArray());
        }
    }
}
