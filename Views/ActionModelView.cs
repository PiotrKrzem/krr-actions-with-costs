using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.WinForms.ListView;

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
        /// <param name="allStatements">list of all currently existing statements</param>
        /// <returns>boolean indicating if the statement was added</returns>
        public virtual bool addStatementToCollection(string statementText, ref List<Statement> allStatements) { return true; }

        /// <summary>
        /// Abstract method that clears the state of statement object fields after statement is committed.
        /// </summary>
        public virtual void clearStatementObjectState() { }

        /// <summary>
        /// Abstract method that verifies if the given statement can be added.
        /// </summary>
        /// <param name="statementForm">statement object formatted to Statement</param>
        /// <param name="allStatements">list of all currently existing statements</param>
        /// <returns>boolean indicating if the statement can be added</returns>
        public virtual bool verifyStatementCorrectness(Statement statementForm, List<Statement> allStatements) 
        {
            if (allStatements.Any(statement => statementForm.Text == statement.Text))
            {
                string message = "You cannot add the same statement twice!";
                MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true; 
        }
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

        override public bool addStatementToCollection(string statementText, ref List<Statement> allStatements)
        {
            InitiallyStatement statement = new InitiallyStatement(statementText, initiallyComboBox.Text);

            if (verifyStatementCorrectness(statement, allStatements))
            {
                allStatements.Add(statement);
                return true;
            }

            return false;
        }

        override public void clearStatementObjectState()
        {
            initiallyComboBox.Text = "";
        }

        override public bool verifyStatementCorrectness(Statement statementForm, List<Statement> allStatements)
        {
            if (base.verifyStatementCorrectness(statementForm, allStatements))
            {
                if (initiallyComboBox.Text == "")
                {
                    string message = "Empty initially statement cannot be added!";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!statementForm.checkModelConsistency(allStatements))
                {
                    string message = "\"Initially\" statement cannot be added since it will cause MODEL INCONSISTENCY! Please verify your model.";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }
            return false;
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

        override public bool addStatementToCollection(string statementText, ref List<Statement> allStatements)
        {
            AfterStatement statement = new AfterStatement(statementText, afterPostCondition.Text, afterActions.Text);

            if (verifyStatementCorrectness(statement, allStatements))
            {
                allStatements.Add(statement);
                return true;
            }

            return false;
        }

        override public void clearStatementObjectState()
        {
            afterPostCondition.Text = "";
            afterActions.SelectedItems.Clear();
        }

        override public bool verifyStatementCorrectness(Statement statementForm, List<Statement> allStatements)
        {
            if (base.verifyStatementCorrectness(statementForm, allStatements))
            {
                if (afterActions.Text == "")
                {
                    string message = "Please specify action for the \"after\" statement or use \"initially\" abbreviation!";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (afterPostCondition.Text == "")
                {
                    string message = "Please specify the fluent post-condition for the \"after\" statement!";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!statementForm.checkModelConsistency(allStatements))
                {
                    string message = "\"After\" statement cannot be added since it will cause MODEL INCONSISTENCY! Please verify your model.";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
                }
            return false;
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

        override public bool addStatementToCollection(string statementText, ref List<Statement> allStatements)
        {
            CausesStatement statement = new CausesStatement(
                statementText, 
                causesAction.Text, 
                causesPostcondition.Text,
                causesPrecondition.Text, 
                Convert.ToInt32(causesCost.Value)
                );

            if (verifyStatementCorrectness(statement, allStatements))
            {
                allStatements.Add(statement);
                return true;
            }
            return false;
        }

        override public void clearStatementObjectState()
        {
            causesAction.Text = "";
            causesPostcondition.Text = "";
            causesPrecondition.SelectedItems.Clear();
            causesCost.Value = 0;
        }

        override public bool verifyStatementCorrectness(Statement statementForm, List<Statement> allStatements)
        {
            if (base.verifyStatementCorrectness(statementForm, allStatements))
            {
                if (causesAction.Text == "")
                {
                    string message = "Please specify action for the \"causes\" statement!";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (causesPostcondition.Text == "")
                {
                    string message = "Please specify the fluent post-condition for the \"causes\" statement!";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!statementForm.checkModelConsistency(allStatements))
                {
                    string message = "\"Causes\" statement cannot be added since it will cause MODEL INCONSISTENCY! Please verify your model.";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }
            return false;
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
        private CheckedListBox allStatementsCheckBox;
        private Button statementRemoveButton;
        private Button statementAddButton;

        // Elements of layout for specifying program to be executed
        public SfComboBox programExecuteComboBox;
        public SfComboBox programInitialStateComboBox;
        public Button programExecuteButton;

        private Label inconsistentDomainLabel;

        public ActionModelView(
            ref FlowLayoutPanel statementsPanel,
            ref ComboBox statementsComboBox,
            ref List<string> positiveNegativeFluents,
            ref CheckedListBox allStatementsCheckBox,
            ref Label inconsistentDomainLabel,
            ref Button statementRemoveButton,
            ref Button statementAddButton,
            ref SfComboBox programExecuteComboBox,
            ref SfComboBox programInitialStateComboBox,
            ref Button programExecuteButton)
        {
            this.statementsPanel = statementsPanel;
            this.statementsComboBox = statementsComboBox;
            this.allStatementsCheckBox = allStatementsCheckBox;
            this.statementAddButton = statementAddButton;
            this.statementRemoveButton = statementRemoveButton;
            this.programExecuteComboBox = programExecuteComboBox;
            this.programInitialStateComboBox = programInitialStateComboBox;
            this.programExecuteButton = programExecuteButton;
            this.inconsistentDomainLabel = inconsistentDomainLabel;

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
        /// <param name="fluents">list of all fluents</param>
        public void addStatement(ref List<Statement> allStatements, List<string> fluents)
        {
            StatementObject statementObject = getStatementObjectForType();
            string statementText = statementObject.createStatementText(statementsPanel);
            
            if(statementObject.addStatementToCollection(statementText, ref allStatements))
            {
                allStatementsCheckBox.Items.Add(statementText);
                statementObject.clearStatementObjectState();
                inconsistentDomainLabel.Visible = !verifyGlobalModelConsistency(allStatements, fluents);
            }
        }
        /// <summary>
        /// Method updates the state of RemoveButton of fluents/actions.
        /// </summary>
        /// <param name="e">arguments of the event that triggered the method</param>
        public void updateRemoveButtonState(ItemCheckEventArgs e)
        {
            statementRemoveButton.Enabled = e.NewValue == CheckState.Checked || (allStatementsCheckBox.CheckedItems.Count > 1);
        }

        /// <summary>
        /// Method removes the statement from the collection.
        /// </summary>
        /// <param name="elements">list of all statements</param>
        public void deleteModelElement(ref List<Statement> elements)
        {
            List<string> itemsToRemove = allStatementsCheckBox.CheckedItems.Cast<string>().ToList();
            foreach (string item in itemsToRemove)
            {
                allStatementsCheckBox.Items.Remove(item);
                List<Statement> statementsToRemove = elements.ToList();
                foreach (Statement statement in statementsToRemove)
                {
                    if (statement.Text.Equals(item))
                    {
                        elements.Remove(statement);
                    }
                }
            }
            statementRemoveButton.Enabled = false;
        }

        private bool verifyGlobalModelConsistency(List<Statement> allStatements, List<string> fluents)
        {
            // Mapping sets of statements to specific types
            List<CausesStatement> causesStatements = allStatements
              .FindAll(statement => statement.Type == StatementType.CAUSES)
              .Cast<CausesStatement>()
              .ToList();
            List<AfterStatement> afterStatements = allStatements
              .FindAll(statement => statement.Type == StatementType.AFTER)
              .Cast<AfterStatement>()
              .ToList();
            List<InitiallyStatement> initiallyStatements = allStatements
              .FindAll(statement => statement.Type == StatementType.INITIALLY)
              .Cast<InitiallyStatement>()
              .ToList();

            
            // Set of all possible initial states of different models
            List<State> allPossibleStartingStates = getInitialStates(initiallyStatements, fluents);
            List<State> allConsistentInitialStates = new List<State>();
            bool isRestrictedByAfter = afterStatements.Count > 0;
            bool currentConsistencyState = true;

            foreach(var state in allPossibleStartingStates)
            {
                // Verifying if there are no causes statements and if the final state does not correspond to the initial one
                if (!allStatements.Any(statement => statement.Type == StatementType.CAUSES))
                {
                    return !state.Literals.Any(st => afterStatements.Any(ast => ast.Postcondition.isComplementary(st)));
                }

                State currentState = new State(state.Literals);
                List<CausesStatement> causesWithEffects = causesStatements.FindAll(causes => causes.doesMeetPreconditions(currentState.Literals));

                // Verifying if there is no inconsistency between causes statements
                var groupedCauses = causesWithEffects.GroupBy(statement => statement.Action).ToList();

                foreach(var causeGroup in groupedCauses)
                {
                    List<Literal> effectsOfAction = causeGroup.Select(statement => statement.Postcondition).ToList();

                    if(effectsOfAction.Any(effect => effectsOfAction.Any(otherEffect => otherEffect.isComplementary(effect))))
                    {
                        if(!isRestrictedByAfter)
                        {
                            return false;
                        }
                        else
                        {
                            currentConsistencyState = false;
                            break;
                        }
                    } 
                    else if (isRestrictedByAfter)
                    {
                        currentConsistencyState = true;
                    }
                }

                if(currentConsistencyState == false)
                {
                    continue;
                }

                // Verifying if there is no inconsistency between after statetments and causes executed upon given initial state
                foreach (var afterStatement in afterStatements)
                {
                    foreach (var action in afterStatement.Actions)
                    {
                        List<Literal> effectsOfAction = causesWithEffects.Select(statement => statement.Postcondition).ToList();
                        List<Literal> remainingLiterals = currentState.Literals.FindAll(literal => !effectsOfAction.Select(effect => effect.Fluent).Contains(literal.Fluent));
                        List<Literal> newState = effectsOfAction.Union(remainingLiterals).ToList();

                        if (newState.Any(stateLiteral => stateLiteral.isComplementary(afterStatement.Postcondition)))
                        {
                            if (!isRestrictedByAfter)
                            {
                                return false;
                            }
                            else
                            {
                                currentConsistencyState = false;
                                goto AfterVerification;
                            }
                        }
                        else if (isRestrictedByAfter)
                        {
                            currentConsistencyState = true;
                        }
                        currentState = new State(newState);
                    }
                }

                AfterVerification:
                if (currentConsistencyState == true)
                {
                    allConsistentInitialStates.Add(state);
                }
            }

            return isRestrictedByAfter ? allConsistentInitialStates.Count > 0 : currentConsistencyState;
        }


        private List<State> getInitialStates(List<InitiallyStatement> initiallyStatements, List<string> allFluents)
        {
            List<Literal> initialState = initiallyStatements.Select(st => st.Condition).ToList();
            List<string> distinctFluents = initialState.GroupBy(literal => literal.Fluent).Select(group => group.Key).ToList();
            List<List<Literal>> missingFluents = allFluents
                .FindAll(fluent => !distinctFluents.Contains(fluent))
                .Select(fluent => new[] { new Literal(fluent, true), new Literal(fluent, false) }.ToList())
                .ToList();
            List<State> allPossibleStartingStates = new List<State>();
            generateAllPossibleInitialStates(initialState, missingFluents, 0, new List<Literal>(), allPossibleStartingStates);

            return allPossibleStartingStates;
        }

        private static void generateAllPossibleInitialStates(List<Literal> initialState, List<List<Literal>> missingFluents, int depth, List<Literal> partialOutput, List<State> finalOutput)
        {
            if (depth < missingFluents.Count)
            {
                foreach(Literal value in missingFluents[depth])
                {
                    List<Literal> literals = new List<Literal>();
                    literals.AddRange(partialOutput);
                    literals.Add(value);
                    if (literals.Count == missingFluents.Count)
                    {
                        finalOutput.Add(new State(literals.Union(initialState).ToList()));
                    }
                    generateAllPossibleInitialStates(initialState, missingFluents, depth + 1, literals, finalOutput);
                }
            }
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
