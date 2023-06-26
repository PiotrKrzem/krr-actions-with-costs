using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows;
using Syncfusion.WinForms.ListView;
using Syncfusion.WinForms.ListView.Enums;

namespace actions_with_costs
{
    public partial class Form1 : Form
    {
        private FluentActionView fluentActionView;
        public List<string> allFluents;
        public List<string> positiveNegativeFluents;
        public List<string> allActions;
        public List<Statement> allStatements;
        private ComboBox initiallyCondition;
        private ComboBox afterPostcondition;
        private SfComboBox afterActions;
        private ComboBox causesAction;
        private ComboBox causesPostcondition;
        private SfComboBox causesPrecondition;
        private NumericUpDown causesCost;
        private int fontSize = 10;
        private string fontType = "Calibri Light";
        private int offset = 20;
        private Font font;

        public Form1()
        {
            InitializeComponent();
            allFluents = new List<string>();
            allActions = new List<string>();
            allStatements = new List<Statement>();
            positiveNegativeFluents = new List<string>();

            // Initializing part of the view responsible for actions and fluents
            fluentActionView = new FluentActionView(
                ref addFluentTextBox,
                ref addActionTextBox,
                ref addFluentButton,
                ref addActionButton,
                ref deleteFluentButton,
                ref deleteActionButton,
                ref removeAllFluents,
                ref removeAllActions,
                ref allFluentsCheckBox,
                ref allActionsCheckBox);

            font = new Font(fontType, fontSize);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addFluentButton.Enabled = false;
            addActionButton.Enabled = false;
            //addStatementButton.Enabled = false;
            deleteFluentButton.Enabled = false;
            deleteActionButton.Enabled = false;
            //deleteStatementButton.Enabled = false;

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

        // ----------------------------- FORM METHODS OF FLUENT/ACTION SECTION ---------------------------------------------
        private void addFluentTextBox_TextChanged(object sender, EventArgs e) => 
            fluentActionView.updateAddButtonState(ModelElementType.FLUENT);
        private void addActionTextBox_TextChanged(object sender, EventArgs e) => 
            fluentActionView.updateAddButtonState(ModelElementType.ACTION);

        private void addFluentTextBox_KeyPress(object sender, KeyPressEventArgs e) => 
            fluentActionView.addModelItemAfterEnter(ref e, ModelElementType.FLUENT, allFluents, buildPositiveNegativeFluents);
        private void addActionTextBox_KeyPress(object sender, KeyPressEventArgs e) => 
            fluentActionView.addModelItemAfterEnter(ref e, ModelElementType.ACTION, allActions, updateCausesDropdown);

        private void addFluentButton_Click(object sender, EventArgs e) =>
            fluentActionView.addFluent(buildPositiveNegativeFluents, allFluents);
        private void addActionButton_Click(object sender, EventArgs e) =>
            fluentActionView.addAction(updateCausesDropdown, allActions);

        private void deleteFluentButton_Click(object sender, EventArgs e) =>
            fluentActionView.deleteModelElement(ModelElementType.FLUENT, ref allFluents, buildPositiveNegativeFluents);
        private void deleteActionButton_Click(object sender, EventArgs e) =>
            fluentActionView.deleteModelElement(ModelElementType.ACTION, ref allActions, updateCausesDropdown);

        private void removeAllFluents_Click(object sender, EventArgs e) =>
            fluentActionView.deleteAllModelElementsOfType(ModelElementType.FLUENT, allFluents, buildPositiveNegativeFluents);
        private void removeAllActions_Click(object sender, EventArgs e) =>
            fluentActionView.deleteAllModelElementsOfType(ModelElementType.ACTION, allActions, updateCausesDropdown);

        private void allFluentsCheckBox_ItemChecked(object sender, ItemCheckEventArgs e) => 
            fluentActionView.updateRemoveButtonState(ModelElementType.FLUENT, e);
        private void allActionsCheckBox_ItemChecked(object sender, ItemCheckEventArgs e) => 
            fluentActionView.updateRemoveButtonState(ModelElementType.ACTION, e);

        // -------------------------------------------------------------------------------------------------------------------
        private void deleteStatementButton_Click(object sender, EventArgs e)
        {
            //foreach(ListViewItem item in allStatementsListView.CheckedItems)
            //{
            //    allStatementsListView.Items.Remove(item);
            //    foreach (Statement statement in allStatements)
            //    {
            //        if (statement.Text.Equals(item.Text))
            //        {
            //            allStatements.Remove(statement);
            //        }
            //    }
            //}
            //MessageBox.Show(allStatements.Count.ToString());
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

            afterPostcondition = new ComboBox();
            afterPostcondition.Font = font;
            afterPostcondition.Width = statementsPanel.Width - offset;
            afterPostcondition.Items.Clear();
            afterPostcondition.Items.AddRange(positiveNegativeFluents.ToArray());

            afterActions = new SfComboBox();
            afterActions.Font = font;
            afterActions.Width = statementsPanel.Width - offset;
            afterActions.ComboBoxMode = ComboBoxMode.MultiSelection;

            causesAction = new ComboBox();
            causesAction.Font = font;
            causesAction.Width = statementsPanel.Width - offset;
            causesAction.Items.Clear();
            causesAction.Items.AddRange(allActions.ToArray());

            causesPostcondition = new ComboBox();
            causesPostcondition.Font = font;
            causesPostcondition.Width = statementsPanel.Width - offset;
            causesPostcondition.Items.Clear();
            causesPostcondition.Items.AddRange(positiveNegativeFluents.ToArray());

            causesPrecondition = new SfComboBox();
            causesPrecondition.Font = font;
            causesPrecondition.Width = statementsPanel.Width - offset;
            causesPrecondition.ComboBoxMode = ComboBoxMode.MultiSelection;
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

            causesCost = new NumericUpDown();
            causesCost.Font = font;
            causesCost.Width = statementsPanel.Width - offset;
            causesCost.Value = 0;

            statementsPanel.Controls.AddRange(new Control[] { causesAction, labelCauses, causesPostcondition, 
                                                labelIf, causesPrecondition, labelCost, causesCost });
        }

        private void updateCausesDropdown()
        {
            causesAction.Items.Clear();
            causesAction.Items.AddRange(allActions.ToArray());
            afterActions.DataSource = allActions.ToList();
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
            afterPostcondition.Items.Clear();
            afterPostcondition.Items.AddRange(positiveNegativeFluents.ToArray());
            causesPostcondition.Items.Clear();
            causesPostcondition.Items.AddRange(positiveNegativeFluents.ToArray());
            causesPrecondition.DataSource = positiveNegativeFluents.ToList();
            causesPrecondition.SelectedItems.Clear();
        }

        private void addStatementButton_Click(object sender, EventArgs e)
        {
            if (statementsComboBox.SelectedValue.ToString() == "initially")
            {
                string statementText = "initially " + initiallyCondition.Text;
                allStatementsListView.Items.Add(statementText);
                InitiallyStatement statement = new InitiallyStatement("initially", statementText, initiallyCondition.Text);
                allStatements.Add(statement);
                initiallyCondition.Text = "";
            }
            else if (statementsComboBox.SelectedValue.ToString() == "value")
            {
                string statementText = afterPostcondition.Text + " after " + afterActions.Text;
                allStatementsListView.Items.Add(statementText);
                AfterStatement statement = new AfterStatement("after", statementText, afterPostcondition.Text, afterActions.Text);
                allStatements.Add(statement);
                afterPostcondition.Text = "";
                afterActions.SelectedItems.Clear();
            }
            else
            {
                string statementText;
                if (causesPrecondition.Text != String.Empty)
                {
                    statementText = causesAction.Text + " causes " + causesPostcondition.Text +
                                 " if " + causesPrecondition.Text + " cost " + causesCost.Text;
                }
                else
                {
                    statementText = causesAction.Text + " causes " + causesPostcondition.Text + " cost " + causesCost.Text;
                }        
                allStatementsListView.Items.Add(statementText);
                CausesStatement statement = new CausesStatement("causes", statementText, causesAction.Text, causesPostcondition.Text, 
                                                                            causesPrecondition.Text, Convert.ToInt32(causesCost.Value));
                allStatements.Add(statement);
                causesAction.Text = "";
                causesPostcondition.Text = "";
                causesPrecondition.SelectedItems.Clear();
                causesCost.Value = 0;
                //MessageBox.Show(statement.Postcondition.Fluent);
                //MessageBox.Show(statement.Postcondition.IfHolds.ToString());
                //MessageBox.Show(statement.Precondition[0].Fluent);
                //MessageBox.Show(statement.Precondition[0].IfHolds.ToString());
                //MessageBox.Show(statement.Cost.ToString());
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
