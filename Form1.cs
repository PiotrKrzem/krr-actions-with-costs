using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace actions_with_costs
{
    public partial class Form1 : Form
    {
        public List<string> allFluents;
        public List<string> allActions;
        public Form1()
        {
            InitializeComponent();
            allFluents = new List<string>();
            allActions = new List<string>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addFluentButton.Enabled = false;
            addActionButton.Enabled = false;
            addStatementButton.Enabled = false;
        }

        private void addFluentTextBox_TextChanged(object sender, EventArgs e)
        {
            addFluentButton.Enabled = addFluentTextBox.Text.Length > 0;
        }

        private void addActionTextBox_TextChanged(object sender, EventArgs e)
        {
            addActionButton.Enabled = addActionTextBox.Text.Length > 0;
        }

        private void addFluentButton_Click(object sender, EventArgs e)
        {
            if(!allFluents.Contains(addFluentTextBox.Text))
            {
                allFluents.Add(addFluentTextBox.Text);
                allFluentsListView.Items.Add(addFluentTextBox.Text);
                addFluentTextBox.Text = String.Empty;
            }
            else
            {
                string message = "This fluent was already added";
                MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void addActionButton_Click(object sender, EventArgs e)
        {
            if (!allActions.Contains(addActionTextBox.Text))
            {
                allActions.Add(addActionTextBox.Text);
                allActionsListView.Items.Add(addActionTextBox.Text);
                addActionTextBox.Text = String.Empty;
            }
            else
            {
                string message = "This action was already added";
                MessageBox.Show(message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
