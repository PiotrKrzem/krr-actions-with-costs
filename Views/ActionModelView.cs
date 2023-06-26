using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace actions_with_costs
{
    /// <summary>
    /// Class is a generic statement object (more preceisely: it contains all fields displayed in the panel when a given statement type is selected.
    /// </summary>
    class StatementObject
    {
        public StatementObject() { }

        /// <summary>
        /// Abstract method that defines how new statement object is to be initialized in the pannel.
        /// </summary>
        /// <param name="statementsPanel">panel on which statement fields will be displayed</param>
        public virtual void createStatementObject(FlowLayoutPanel statementsPanel) { }

        /// <summary>
        /// Abstract method that creates statement text.
        /// </summary>
        /// <param name="statementsPanel">panel on which statement fields will be displayed</param>
        /// <returns>text displayed on the panel for the user</returns>
        public virtual string createStatementText(FlowLayoutPanel statementsPanel) { return ""; }

        /// <summary>
        /// Abstract method that adds new statement to the collection.
        /// </summary>
        /// <param name="statementsPanel">text of the statement that is displayed for the user</param>
        /// <param name="allStatements">list of all existing currently statements</param>
        /// <returns>created Statement object</returns>
        public virtual Statement addStatementToCollection(string statementText, ref List<Statement> allStatements) { return null; }

        /// <summary>
        /// Abstract method that clears the state of statement object fields after statement is committed.
        /// </summary>
        public virtual void clearStatementObjectState() { }
    }

    /// <summary>
    /// Class represents "initially" statement object.
    /// </summary>
    class InitiallyStatementObject: StatementObject
    {
        public StatementComboBox initiallyComboBox;

        public InitiallyStatementObject(FlowLayoutPanel statementsPanel, List<string> positiveNegativeFluents)
        {
            initiallyComboBox = new StatementComboBox(statementsPanel, positiveNegativeFluents);
        }

        override public void createStatementObject(FlowLayoutPanel statementsPanel)
        {
            statementsPanel.Controls.Clear();
            statementsPanel.Controls.AddRange(new Control[] { StatementConstants.createStatementLabel("initially"), initiallyComboBox });
        }

        override public string createStatementText(FlowLayoutPanel statementsPanel)
        {
            return "initially " + initiallyComboBox.Text;
        }

        override public Statement addStatementToCollection(string statementText, ref List<Statement> allStatements)
        {
            InitiallyStatement statement = new InitiallyStatement("initially", statementText, initiallyComboBox.Text);
            allStatements.Add(statement);

            return statement;
        }

        override public void clearStatementObjectState()
        {
            initiallyComboBox.Text = "";
        }
    }

    /// <summary>
    /// Class represents "after" statement object.
    /// </summary>
    class AfterStatementObject : StatementObject
    {
        public StatementComboBox afterPostCondition;
        public StatementSFComboBox afterActions;

        public AfterStatementObject(FlowLayoutPanel statementsPanel, List<string> positiveNegativeFluents) 
        {
            afterPostCondition = new StatementComboBox(statementsPanel, positiveNegativeFluents);
            afterActions = new StatementSFComboBox(statementsPanel);
        }

        override public void createStatementObject(FlowLayoutPanel statementsPanel)
        {
            statementsPanel.Controls.Clear();
            Label label = StatementConstants.createStatementLabel("after");
            statementsPanel.Controls.AddRange(new Control[] { afterPostCondition, label, afterActions });
        }

        override public string createStatementText(FlowLayoutPanel statementsPanel)
        {
            return afterPostCondition.Text + " after " + afterActions.Text;
        }

        override public Statement addStatementToCollection(string statementText, ref List<Statement> allStatements)
        {
            AfterStatement statement = new AfterStatement("after", statementText, afterPostCondition.Text, afterActions.Text);
            allStatements.Add(statement);

            return statement;
        }

        override public void clearStatementObjectState()
        {
            afterPostCondition.Text = "";
            afterActions.SelectedItems.Clear();
        }
    }

    /// <summary>
    /// Class represents "effect" statement object.
    /// </summary>
    class EffectStatementObject : StatementObject
    {
        public StatementComboBox causesAction;
        public StatementComboBox causesPostcondition;
        public StatementSFComboBox causesPrecondition;
        public NumericUpDown causesCost;

        public EffectStatementObject(FlowLayoutPanel statementsPanel, List<string> positiveNegativeFluents)
        {
            causesAction = new StatementComboBox(statementsPanel, positiveNegativeFluents);
            causesPostcondition = new StatementComboBox(statementsPanel, positiveNegativeFluents);
            causesPrecondition = new StatementSFComboBox(statementsPanel);
            causesCost = new StatementNumericUpDown(statementsPanel);
        }

        override public void createStatementObject(FlowLayoutPanel statementsPanel)
        {
            statementsPanel.Controls.Clear();
            Label labelCauses = StatementConstants.createStatementLabel("causes");
            Label labelIf = StatementConstants.createStatementLabel("if");
            Label labelCost = StatementConstants.createStatementLabel("cost");

            statementsPanel.Controls.AddRange(new Control[] { 
                causesAction, 
                labelCauses, 
                causesPostcondition,
                labelIf, 
                causesPrecondition, 
                labelCost, 
                causesCost 
            });
        }

        override public string createStatementText(FlowLayoutPanel statementsPanel)
        {
            return causesPrecondition.Text != string.Empty 
                ? (causesAction.Text + " causes " + causesPostcondition.Text + " if " + causesPrecondition.Text + " cost " + causesCost.Text)
                : (causesAction.Text + " causes " + causesPostcondition.Text + " cost " + causesCost.Text);
        }

        override public Statement addStatementToCollection(string statementText, ref List<Statement> allStatements)
        {
            CausesStatement statement = new CausesStatement(
                "causes", 
                statementText, 
                causesAction.Text, 
                causesPostcondition.Text,
                causesPrecondition.Text, 
                Convert.ToInt32(causesCost.Value)
                );
            allStatements.Add(statement);

            return statement;
        }

        override public void clearStatementObjectState()
        {
            causesAction.Text = "";
            causesPostcondition.Text = "";
            causesPrecondition.SelectedItems.Clear();
            causesCost.Value = 0;
        }
    }


    /// <summary>
    /// Class contains functionalities used in the Action Model section.
    /// </summary>
    class ActionModelView
    {
        // Statement object field groups
        public InitiallyStatementObject initiallyStatementObject;
        public AfterStatementObject afterStatementObject;
        public EffectStatementObject effectStatementObject;

        // Elements of the layout on which statement values are displayed
        private FlowLayoutPanel statementsPanel;
        private ComboBox statementsComboBox;
        private CheckedListBox allStatementsListView;

        public ActionModelView(
            ref FlowLayoutPanel statementsPanel, 
            ref ComboBox statementsComboBox, 
            ref List<string> positiveNegativeFluents,
            ref CheckedListBox allStatementsListView
            )
        {
            this.statementsPanel = statementsPanel;
            this.statementsComboBox = statementsComboBox;
            this.allStatementsListView = allStatementsListView;

            initiallyStatementObject = new InitiallyStatementObject(statementsPanel, positiveNegativeFluents);
            afterStatementObject = new AfterStatementObject(statementsPanel, positiveNegativeFluents);
            effectStatementObject = new EffectStatementObject(statementsPanel, positiveNegativeFluents);
        }

        /// <summary>
        /// Method initialized fields for a selected statement.
        /// </summary>
        public void createStatementObject()
        {
            StatementObject statementObject = getStatementObjectForType();
            statementObject.createStatementObject(statementsPanel);
        }

        /// <summary>
        /// Method adds new statement to the collection.
        /// </summary>
        /// <param name="allStatements">collection of all current statements</param>
        public void addStatement(ref List<Statement> allStatements)
        {
            StatementObject statementObject = getStatementObjectForType();
            string statementText = statementObject.createStatementText(statementsPanel);
            allStatementsListView.Items.Add(statementText);
            statementObject.addStatementToCollection(statementText, ref allStatements);
            statementObject.clearStatementObjectState();
        }

        private StatementObject getStatementObjectForType()
        {
            if (statementsComboBox.SelectedValue.ToString() == "initially")
            {
                return initiallyStatementObject;
            }
            else if (statementsComboBox.SelectedValue.ToString() == "value")
            {
                return afterStatementObject;
            }
            else
            {
                return effectStatementObject;
            }
        }
    }
}
