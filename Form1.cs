using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

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
            deleteFluentButton.Enabled = false;
            deleteActionButton.Enabled = false;

            List<Item> items = new List<Item>();
            items.Add(new Item() { Text = "Initially statement", Value = "initially" });
            items.Add(new Item() { Text = "Value statement", Value = "value" });
            items.Add(new Item() { Text = "Effect statement", Value = "effect" });

            statementsComboBox.DataSource = items;
            statementsComboBox.DisplayMember = "Text";
            statementsComboBox.ValueMember = "Value";

            createInitialStatements();
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

        private void deleteFluentButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in allFluentsListView.CheckedItems)
            {
                allFluentsListView.Items.Remove(item);
            }
            allFluents = allFluentsListView.Items.Cast<ListViewItem>()
                                 .Select(item => item.Text)
                                 .ToList();
            deleteFluentButton.Enabled = false;
        }

        private void deleteActionButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in allActionsListView.CheckedItems)
            {
                allActionsListView.Items.Remove(item);
            }
            allActions = allActionsListView.Items.Cast<ListViewItem>()
                                 .Select(item => item.Text)
                                 .ToList();
            deleteActionButton.Enabled = false;
        }

        private void allFluentsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            deleteFluentButton.Enabled = allFluentsListView.CheckedItems.Count > 0;
        }

        private void allActionsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            deleteActionButton.Enabled = allActionsListView.CheckedItems.Count > 0;
        }

        private void statementsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if(statementsComboBox.)

        }
        private void createInitialStatements()
        {
            Label label = new Label();
            label.Text = "initially";
            label.Font = new Font("Calibri Light", 13);
            TextBox textBox = new TextBox();
            statementsPanel.Controls.AddRange(new Control[] { label, textBox });
        }
        private void createValueStatements()
        {
         
        }
        private void createEffectStatements()
        {
                
        }
    }

    public class Item
    {
        public Item() { }
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
