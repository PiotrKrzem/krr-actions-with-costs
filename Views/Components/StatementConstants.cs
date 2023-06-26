using System.Drawing;
using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Class contains constant values/static method which are used by different statement types
    /// </summary>
    class StatementConstants
    {
        static public readonly Font FONT = new Font("Calibri Light", 10);
        static public readonly int OFFSET = 20;

        /// <summary>
        /// Method creates default statement label
        /// </summary>
        /// <param name="text">text that is to be displayed on the label</param>
        /// <returns>Label object</returns>
        public static Label createStatementLabel(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = FONT;
            label.AutoSize = true;

            return label;
        }
    }
}
