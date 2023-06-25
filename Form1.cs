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
        public List<string> positiveNegativeFluents;
        public List<string> allActions;
        private ComboBox initiallyCondition;
        private TextBox afterPostcondition;
        private TextBox afterActions;
        private TextBox causesAction;
        private TextBox causesPostcondition;
        private TextBox causesPrecondition;
        private TextBox causesCost;
        private int fontSize = 10;
        private string fontType = "Calibri Light";
        private int offset = 20;
        private Font font;

        public Form1()
        {
            InitializeComponent();
            allFluents = new List<string>();
            allActions = new List<string>();
            positiveNegativeFluents = new List<string>();
            font = new Font(fontType, fontSize);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addFluentButton.Enabled = false;
            addActionButton.Enabled = false;
            //addStatementButton.Enabled = false;
            deleteFluentButton.Enabled = false;
            deleteActionButton.Enabled = false;

            List<Item> items = new List<Item>();
            items.Add(new Item() { Text = "Initially statement", Value = "initially" });
            items.Add(new Item() { Text = "Value statement", Value = "value" });
            items.Add(new Item() { Text = "Effect statement", Value = "effect" });

            statementsComboBox.DataSource = items;
            statementsComboBox.DisplayMember = "Text";
            statementsComboBox.ValueMember = "Value";

            initializeComboBoxes();
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
                buildPositiveNegativeFluents();
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
            buildPositiveNegativeFluents();
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
            if (statementsComboBox.SelectedValue.ToString() == "initially")
            {
                createInitialStatements();
            }
            else if (statementsComboBox.SelectedValue.ToString() == "value")
            {
                createValueStatements();
            }
            else
            {
                createEffectStatements();
            }
        }
        private void initializeComboBoxes()
        {
            initiallyCondition = new ComboBox();
            initiallyCondition.Font = font;
            initiallyCondition.Width = statementsPanel.Width - offset;
            initiallyCondition.Items.Clear();
            initiallyCondition.Items.AddRange(positiveNegativeFluents.ToArray());
        }
        private void createInitialStatements()
        {
            statementsPanel.Controls.Clear();
            Label label = new Label();
            label.Text = "initially";
            label.Font = font;
            //initiallyCondition.SelectionChangeCommitted += new EventHandler(initiallyCondition_SelectionChangeCommitted);

            statementsPanel.Controls.AddRange(new Control[] { label, initiallyCondition });
        }
        private void initiallyCondition_SelectionChangeCommitted(object sender, EventArgs e)
        {
            
        }
        private void createValueStatements()
        {
            statementsPanel.Controls.Clear();
            Label label = new Label();
            label.Text = "after";
            label.Font = font;

            afterPostcondition = new TextBox();
            afterPostcondition.Font = font;
            afterPostcondition.Width = statementsPanel.Width - offset;
            afterActions = new TextBox();
            afterActions.Font = font;
            afterActions.Width = statementsPanel.Width - offset;

            statementsPanel.Controls.AddRange(new Control[] { afterPostcondition, label, afterActions });
        }
        private void createEffectStatements()
        {
            statementsPanel.Controls.Clear();
            Label labelCauses = new Label();
            labelCauses.Text = "causes";
            labelCauses.Font = font;
            Label labelIf = new Label();
            labelIf.Text = "if";
            labelIf.Font = font;
            Label labelCost = new Label();
            labelCost.Text = "cost";
            labelCost.Font = font;

            causesAction = new TextBox();
            causesAction.Font = font;
            causesAction.Width = statementsPanel.Width - offset;
            causesPrecondition = new TextBox();
            causesPrecondition.Font = font;
            causesPrecondition.Width = statementsPanel.Width - offset;
            causesPostcondition = new TextBox();
            causesPostcondition.Font = font;
            causesPostcondition.Width = statementsPanel.Width - offset;
            causesCost = new TextBox();
            causesCost.Font = font;
            causesCost.Width = statementsPanel.Width - offset;

            statementsPanel.Controls.AddRange(new Control[] { causesAction, labelCauses, causesPostcondition, 
                                                labelIf, causesPrecondition, labelCost, causesCost });
        }
        private void buildPositiveNegativeFluents()
        {
            positiveNegativeFluents = new List<string>();
            foreach (string item in allFluents)
            {
                positiveNegativeFluents.Add(item);
                string negated = "~" + item;
                positiveNegativeFluents.Add(negated);
            }
            initiallyCondition.Items.Clear();
            initiallyCondition.Items.AddRange(positiveNegativeFluents.ToArray());
        }

        private void addStatementButton_Click(object sender, EventArgs e)
        {
            if (statementsComboBox.SelectedValue.ToString() == "initially")
            {
                string statement = "initially " + initiallyCondition.Text;
                allStatementsListView.Items.Add(statement);
                initiallyCondition.Text = "";
            }
        }
    }

    public class Item
    {
        public Item() { }
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
