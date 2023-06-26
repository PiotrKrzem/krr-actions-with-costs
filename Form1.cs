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
        // View sections
        private FluentActionView fluentActionView;
        private ActionModelView actionModelView;
        private QueryView queryView;

        // Stored state
        public List<string> allFluents;
        public List<string> positiveNegativeFluents;
        public List<List<Literal>> allInitialStates;
        public List<string> allActions;
        public List<Statement> allStatements;

        public Form1()
        {
            InitializeComponent();
            allFluents = new List<string>();
            allActions = new List<string>();
            allStatements = new List<Statement>();
            positiveNegativeFluents = new List<string>();
            allInitialStates = new List<List<Literal>>();


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

            // Initializing part of the view responsible for creating action model
            actionModelView = new ActionModelView(
                ref statementsPanel,
                ref statementsComboBox,
                ref positiveNegativeFluents,
                ref allStatementsCheckBox,
                ref inconsistentDomainLabel,
                ref deleteStatementButton);

            queryView = new QueryView(
                ref queryPanel,
                ref historyPanel,
                ref queryTypeComboBox,
                ref positiveNegativeFluents,
                ref allInitialStates);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addFluentButton.Enabled = false;
            addActionButton.Enabled = false;
            //addStatementButton.Enabled = false;
            deleteFluentButton.Enabled = false;
            deleteActionButton.Enabled = false;
            deleteStatementButton.Enabled = false;

            List<Item> items = new List<Item>();
            items.Add(new Item() { Text = "Initially statement", Value = "initially" });
            items.Add(new Item() { Text = "Value statement", Value = "value" });
            items.Add(new Item() { Text = "Effect statement", Value = "effect" });

            statementsComboBox.DataSource = items;
            statementsComboBox.DisplayMember = "Text";
            statementsComboBox.ValueMember = "Value";

            actionModelView.initiallyStatementObject.createStatementObject(statementsPanel);

            List<Item> queryItems = new List<Item>();
            queryItems.Add(new Item() { Text = "Value query", Value = "value" });
            queryItems.Add(new Item() { Text = "Cost query", Value = "cost" });

            queryTypeComboBox.DataSource = queryItems;
            queryTypeComboBox.DisplayMember = "Text";
            queryTypeComboBox.ValueMember = "Value";

            queryView.valueQuery.createQueryObject(ref queryPanel);

            
        }

        // ----------------------------- FORM METHODS OF FLUENT/ACTION SECTION ---------------------------------------------
        private void addFluentTextBox_TextChanged(object sender, EventArgs e) => 
            fluentActionView.updateAddButtonState(ModelElementType.FLUENT);
        private void addActionTextBox_TextChanged(object sender, EventArgs e) => 
            fluentActionView.updateAddButtonState(ModelElementType.ACTION);

        private void addFluentTextBox_KeyPress(object sender, KeyPressEventArgs e) => 
            fluentActionView.addModelItemAfterEnter(ref e, ModelElementType.FLUENT, allFluents, buildPositiveNegativeFluents);
        private void addActionTextBox_KeyPress(object sender, KeyPressEventArgs e) => 
            fluentActionView.addModelItemAfterEnter(ref e, ModelElementType.ACTION, allActions, updateCausesDropdownAndQueryActionDropdown);

        private void addFluentButton_Click(object sender, EventArgs e) =>
            fluentActionView.addFluent(buildPositiveNegativeFluents, allFluents);
        private void addActionButton_Click(object sender, EventArgs e) =>
            fluentActionView.addAction(updateCausesDropdownAndQueryActionDropdown, allActions);

        private void deleteFluentButton_Click(object sender, EventArgs e) =>
            fluentActionView.deleteModelElement(ModelElementType.FLUENT, ref allFluents, buildPositiveNegativeFluents);
        private void deleteActionButton_Click(object sender, EventArgs e) =>
            fluentActionView.deleteModelElement(ModelElementType.ACTION, ref allActions, updateCausesDropdownAndQueryActionDropdown);

        private void removeAllFluents_Click(object sender, EventArgs e) =>
            fluentActionView.deleteAllModelElementsOfType(ModelElementType.FLUENT, allFluents, buildPositiveNegativeFluents);
        private void removeAllActions_Click(object sender, EventArgs e) =>
            fluentActionView.deleteAllModelElementsOfType(ModelElementType.ACTION, allActions, updateCausesDropdownAndQueryActionDropdown);

        private void allFluentsCheckBox_ItemChecked(object sender, ItemCheckEventArgs e) => 
            fluentActionView.updateRemoveButtonState(ModelElementType.FLUENT, e);
        private void allActionsCheckBox_ItemChecked(object sender, ItemCheckEventArgs e) => 
            fluentActionView.updateRemoveButtonState(ModelElementType.ACTION, e);

        // -------------------------------------------------------------------------------------------------------------------


        // ----------------------------- FORM METHODS OF ACTION MODEL --------------------------------------------------------

        private void statementsComboBox_SelectionChangeCommitted(object sender, EventArgs e) =>
            actionModelView.createStatementObject();

        private void addStatementButton_Click(object sender, EventArgs e) =>
            actionModelView.addStatement(ref allStatements, allFluents);

        private void allStatementsCheckBox_ItemCheck(object sender, ItemCheckEventArgs e) =>
            actionModelView.updateRemoveButtonState(e);

        private void deleteStatementButton_Click(object sender, EventArgs e) => 
            actionModelView.deleteModelElement(ref allStatements);

        // -------------------------------------------------------------------------------------------------------------------


        // ----------------------------- COMMON HELPER METHODS ---------------------------------------------------------------
        private void updateCausesDropdownAndQueryActionDropdown()
        {
            actionModelView.effectStatementObject.causesAction.Items.Clear();
            actionModelView.effectStatementObject.causesAction.Items.AddRange(allActions.ToArray());
            actionModelView.afterStatementObject.afterActions.DataSource = allActions.ToList();

            queryView.costQuery.queryActions.DataSource = allActions.ToList();
            queryView.valueQuery.queryActions.DataSource = allActions.ToList();

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
            actionModelView.initiallyStatementObject.initiallyComboBox.Items.Clear();
            actionModelView.initiallyStatementObject.initiallyComboBox.Items.AddRange(positiveNegativeFluents.ToArray());

            actionModelView.afterStatementObject.afterPostCondition.Items.Clear();
            actionModelView.afterStatementObject.afterPostCondition.Items.AddRange(positiveNegativeFluents.ToArray());

            actionModelView.effectStatementObject.causesPostcondition.Items.Clear();
            actionModelView.effectStatementObject.causesPostcondition.Items.AddRange(positiveNegativeFluents.ToArray());

            actionModelView.effectStatementObject.causesPrecondition.DataSource = positiveNegativeFluents.ToList();
            actionModelView.effectStatementObject.causesPrecondition.SelectedItems.Clear();

            queryView.valueQuery.fluentSelectBox.Items.Clear();
            queryView.valueQuery.fluentSelectBox.Items.AddRange(positiveNegativeFluents.ToArray());
        }

        private void queryTypeComboBox_SelectionChangeCommitted(object sender, EventArgs e) =>
            queryView.createQueryObject();

        private void executeQueryButton_Click(object sender, EventArgs e) =>
            queryView.executeQuery();

        private void clearHistoryButton_Click(object sender, EventArgs e) =>
            historyPanel.Controls.Clear();
    }

    public class Item
    {
        public Item() { }
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
