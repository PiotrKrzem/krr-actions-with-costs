using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.WinForms.ListView;
using Syncfusion.Windows.Forms.Tools;
using System.Drawing;
using Syncfusion.DataSource.Extensions;

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
        /// Abstract method that verifies if the selection fields contain valid values of fluents or actions.
        /// </summary>
        /// <param name="allFluents">list of all acceptable fluent names</param>
        /// <param name="allActions">list of all acceptable action names</param>
        /// <returns>boolean indicating if the statement can be added</returns>
        public virtual bool verifyFluentsAndActionsCorrectness(List<string> allFluents, List<string> allActions) { return true; }

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

        public override bool verifyFluentsAndActionsCorrectness(List<string> allFluents, List<string> allActions)
        {
            string providedText = initiallyComboBox.Text;
            List<string> acceptableTexts = allFluents.SelectMany(fluent => new[] { fluent, "~" + fluent }).ToList();

            if(!acceptableTexts.Contains(providedText))
            {
                string message = "Literal provided in initially statement does not correspond to any existing fluents! Please use valid fluent names or add new fluents.";
                MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
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

        public override bool verifyFluentsAndActionsCorrectness(List<string> allFluents, List<string> allActions)
        {
            string providedText = afterPostCondition.Text;
            List<string> acceptableTexts = allFluents.SelectMany(fluent => new[] { fluent, "~" + fluent }).ToList();

            if (!acceptableTexts.Contains(providedText))
            {
                string message = "Literal provided in post-condition statement does not correspond to any existing fluents! Please use valid fluent names or add new fluents.";
                MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
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

        public override bool verifyFluentsAndActionsCorrectness(List<string> allFluents, List<string> allActions)
        {
            string providedText = causesPostcondition.Text;
            List<string> acceptableTexts = allFluents.SelectMany(fluent => new[] { fluent, "~" + fluent }).ToList();

            if (!acceptableTexts.Contains(providedText))
            {
                string message = "Literal provided in post-condition statement does not correspond to any existing fluents! Please use valid fluent names or add new fluents.";
                MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if(!allActions.Contains(causesAction.Text))
            {
                string message = "Provided action does not correspond to any existing actions! Please use valid action name or add new actions.";
                MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
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

    class Query
    {
        public QuerySFComboBox queryActions;

        public Query(FlowLayoutPanel queryPanel)
        {
            this.queryActions = new QuerySFComboBox(queryPanel);
        }
        public virtual void createQueryObject(FlowLayoutPanel queryPanel) { }
        public virtual string createQueryText() { return "<error query>"; }
    }
    class ValueQuery : Query
    {
        public QueryComboBox fluentSelectBox;

        public ValueQuery(FlowLayoutPanel queryPanel, List<string> positiveNegativeFluents) : base(queryPanel)
        {
            this.fluentSelectBox = new QueryComboBox(queryPanel, positiveNegativeFluents);
        }
        public override void createQueryObject(FlowLayoutPanel queryPanel)
        {
            queryPanel.Controls.Clear();
            queryPanel.Controls.AddRange(new Control[] {
                fluentSelectBox,
                StatementConstants.createStatementLabel("after"),
                queryActions});
        }

        public override string createQueryText()
        {
            return fluentSelectBox.Text + " after " + queryActions.Text;
        }
    }
    class CostQuery : Query
    {
        public QueryNumericUpDown costSelectBox;
        public CostQuery(FlowLayoutPanel queryPanel) : base(queryPanel)
        {
            this.costSelectBox = new QueryNumericUpDown(queryPanel);
        }
        public override void createQueryObject(FlowLayoutPanel queryPanel)
        {
            queryPanel.Controls.Clear();
            queryPanel.Controls.AddRange(new Control[] {
                StatementConstants.createStatementLabel("sufficient"),
                costSelectBox,
                StatementConstants.createStatementLabel("after"),
                queryActions});
        }

        public override string createQueryText()
        {
            return "sufficient " + costSelectBox.Text + " after " + queryActions.Text;
        }
    }

    /// <summary>
    /// Class contains functionalities used in the Action Model section.
    /// </summary>
    class ActionModelView
    {
        public List<State> initialStatesOfModels;

        // Statement object field groups
        public InitiallyStatementObject initiallyStatementObject;
        public AfterStatementObject afterStatementObject;
        public EffectStatementObject effectStatementObject;

        // Elements of the layout on which statement values are displayed
        private FlowLayoutPanel statementsPanel;
        private ComboBox statementsComboBox;
        private CheckedListBox allStatementsCheckBox;
        private Button statementRemoveButton;
        private Button statementRemoveAllButton;

        // Elements of layout for specifying program to be executed
        public TextBoxExt programExecuteTextBox;
        public Button programExecuteButton;
        public Label stateFinal;
        public Label costFinal;
        public Label stateFinalLabel;
        public Label costFinalLabel;


        // Elements of layout relevant to visualization
        public Button displayVisualizationButton;

        // Elements displayed with regard to model consistency
        public Label inconsistentDomainLabel;

        // Elements of query
        public FlowLayoutPanel queryPanel;
        public ComboBox queryTypeComboBox;
        public Button queryExecuteButton;

        // Queries objects
        public ValueQuery valueQuery;
        public CostQuery costQuery;

        public ActionModelView(
            ref FlowLayoutPanel statementsPanel,
            ref ComboBox statementsComboBox,
            ref List<string> positiveNegativeFluents,
            ref CheckedListBox allStatementsCheckBox,
            ref Label inconsistentDomainLabel,
            ref Button statementRemoveButton,
            ref Button statementRemoveAllButton,
            ref TextBoxExt programExecuteTextBox,
            ref Button programExecuteButton,
            ref Button displayVisualizationButton,
            ref FlowLayoutPanel queryPanel,
            ref ComboBox queryTypeComboBox,
            ref Button queryExecuteButton,
            ref Label stateFinal,
            ref Label costFinal,
            ref Label stateFinalLabel,
            ref Label costFinalLabel)
        {
            this.statementsPanel = statementsPanel;
            this.statementsComboBox = statementsComboBox;
            this.allStatementsCheckBox = allStatementsCheckBox;
            this.statementRemoveButton = statementRemoveButton;
            this.programExecuteTextBox = programExecuteTextBox;
            this.programExecuteButton = programExecuteButton;
            this.stateFinal = stateFinal;
            this.costFinal = costFinal;
            this.stateFinalLabel = stateFinalLabel;
            this.costFinalLabel = costFinalLabel;
            this.statementRemoveAllButton = statementRemoveAllButton;
            this.inconsistentDomainLabel = inconsistentDomainLabel;
            this.displayVisualizationButton = displayVisualizationButton;
            this.queryPanel = queryPanel;
            this.queryTypeComboBox = queryTypeComboBox;
            this.queryExecuteButton = queryExecuteButton;

            this.initialStatesOfModels = new List<State>();

            initiallyStatementObject = new InitiallyStatementObject(statementsPanel, positiveNegativeFluents);
            afterStatementObject = new AfterStatementObject(statementsPanel, positiveNegativeFluents);
            effectStatementObject = new EffectStatementObject(statementsPanel, positiveNegativeFluents);

            valueQuery = new ValueQuery(queryPanel, positiveNegativeFluents);
            costQuery = new CostQuery(queryPanel);

        }

        /// <summary>
        /// Method initialized fields for a selected statement.
        /// </summary>
        public void createStatementObject()
        {
            StatementObject statementObject = getStatementObjectForType();
            statementObject.createStatementObject(statementsPanel);
        }

        public void createQueryObject()
        {
            Query query = getQueryForType();
            query.createQueryObject(queryPanel);
        }

        /// <summary>
        /// Method adds new statement to the collection.
        /// </summary>
        /// <param name="allStatements">collection of all current statements</param>
        /// <param name="fluents">list of all fluents</param>
        /// <returns>list of initial states of all models</returns>
        public List<State> addStatement(ref List<Statement> allStatements, List<string> fluents, List<string> actions)
        {
            StatementObject statementObject = getStatementObjectForType();
            string statementText = statementObject.createStatementText(statementsPanel);
            
            if(statementObject.verifyFluentsAndActionsCorrectness(fluents, actions) && statementObject.addStatementToCollection(statementText, ref allStatements))
            {
                allStatementsCheckBox.Items.Add(statementText);
                statementObject.clearStatementObjectState();
                inconsistentDomainLabel.Visible = !verifyGlobalModelConsistency(allStatements, fluents);
                statementRemoveAllButton.Enabled = true;
                updateFunctionsStateBasedOnModelConsistency();
            }
            return initialStatesOfModels;
        }
        /// <summary>
        /// Method updates the state of RemoveButton of statement.
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
        public List<State> deleteStatementElement(ref List<Statement> allStatements, List<string> fluents)
        {
            List<string> itemsToRemove = allStatementsCheckBox.CheckedItems.Cast<string>().ToList();
            foreach (string item in itemsToRemove)
            {
                allStatementsCheckBox.Items.Remove(item);
                List<Statement> statementsToRemove = allStatements.ToList();
                foreach (Statement statement in statementsToRemove)
                {
                    if (statement.Text.Equals(item))
                    {
                        allStatements.Remove(statement);
                    }
                }
            }
            statementRemoveButton.Enabled = false;
            if (statementRemoveAllButton.Enabled && allStatements.Count == 0)
            {
                statementRemoveAllButton.Enabled = false;
            }
            inconsistentDomainLabel.Visible = !verifyGlobalModelConsistency(allStatements, fluents);
            stateFinalLabel.Visible = false;
            stateFinal.Text = "";
            costFinalLabel.Visible = false;
            costFinal.Text = "";
            updateFunctionsStateBasedOnModelConsistency();

            return initialStatesOfModels;
        }

        /// <summary>
        /// Method removes all statements.
        /// </summary>
        /// <param name="allStatements">list of all statements</param>
        /// <returns>boolean indicating if the statements were successfuly removed</returns>
        public bool deleteAllStatements(ref List<Statement> allStatements)
        {
           allStatementsCheckBox.Items.Clear();
            allStatements.Clear();
            statementRemoveAllButton.Enabled = false;
            statementRemoveButton.Enabled = false;
            inconsistentDomainLabel.Visible = false;
            stateFinal.Text = "";
            costFinal.Text = "";
            updateFunctionsStateBasedOnModelConsistency();
            return true;
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
                if(isRestrictedByAfter)
                {
                    currentConsistencyState = true;
                }

                // Verifying if there are no causes statements and if the final state does not correspond to the initial one
                if (causesStatements.Count == 0)
                {
                    if (state.Literals.Any(st => afterStatements.Any(ast => ast.Postcondition.isComplementary(st))))
                    {
                        currentConsistencyState = false;
                        continue;
                    }
                    else
                    {
                        allConsistentInitialStates.Add(state);
                    }
                }
                else
                {

                    State currentState = new State(state.Literals);
                    List<CausesStatement> causesWithEffects = causesStatements.FindAll(causes => causes.doesMeetPreconditions(currentState.Literals));

                    // Verifying if there is no inconsistency between causes statements
                    var groupedCauses = causesWithEffects.GroupBy(statement => statement.Action).ToList();

                    foreach (var causeGroup in groupedCauses)
                    {
                        List<Literal> effectsOfAction = causeGroup.Select(statement => statement.Postcondition).ToList();

                        if (effectsOfAction.Any(effect => effectsOfAction.Any(otherEffect => otherEffect.isComplementary(effect))))
                        {
                            if (!isRestrictedByAfter)
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

                    if (currentConsistencyState == false)
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
            }
            initialStatesOfModels = allConsistentInitialStates;
            return isRestrictedByAfter ? allConsistentInitialStates.Count > 0 : currentConsistencyState;
        }

        public List<State> getInitialStates(List<InitiallyStatement> initiallyStatements, List<string> allFluents)
        {
            List<Literal> initialState = initiallyStatements.Select(st => st.Condition).ToList();
            List<string> distinctFluents = initialState.GroupBy(literal => literal.Fluent).Select(group => group.Key).ToList();
            List<List<Literal>> missingFluents = allFluents
                .FindAll(fluent => !distinctFluents.Contains(fluent))
                .Select(fluent => new[] { new Literal(fluent, true), new Literal(fluent, false) }.ToList())
                .ToList();
            List<State> allPossibleStartingStates = new List<State>();

            if (missingFluents.Count == 0)
            {
                allPossibleStartingStates.Add(new State(initialState));
            }
            else
            {
                generateAllPossibleInitialStates(initialState, missingFluents, 0, new List<Literal>(), allPossibleStartingStates);
            }
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

        private void updateFunctionsStateBasedOnModelConsistency()
        {
            bool functionsState = !inconsistentDomainLabel.Visible && allStatementsCheckBox.Items.Count > 0;
            
            programExecuteButton.Enabled = functionsState;
            programExecuteTextBox.Enabled = functionsState;
            displayVisualizationButton.Enabled = functionsState;
            queryExecuteButton.Enabled = functionsState;
            programExecuteTextBox.Text = "Type in actions";
            programExecuteTextBox.ForeColor = SystemColors.ScrollBar;
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

        private Query getQueryForType()
        {
            if (queryTypeComboBox.SelectedValue.ToString() == "value")
            {
                return valueQuery;
            }
            else
            {
                return costQuery;
            }
        }

        public bool ValidateValueQuery()
        {
            string message = string.Empty;
            bool validation_succesful = false;
            if (valueQuery.fluentSelectBox.Text == "")
            {
                message = "Value Query's fluent should not be empty";
            }
            else if (!valueQuery.fluentSelectBox.Items.Contains(valueQuery.fluentSelectBox.Text))
            {
                message = "Value Query's fluent " + valueQuery.fluentSelectBox.Text + " does not exist";
                // }
                // else if (valueQuery.queryActions.Text == "")
                // {
                //     message = "Value Query's action selection should not be empty";
            } else if(!valueQuery.queryActions.Text.Split(',').Select(qa => valueQuery.queryActions.Actions.Contains(qa.Trim())).All(found => found == true))
            {
                message = "Value Query's actions " + valueQuery.queryActions.Text + " contain at least one unknown action";
            } 
            else
            {
                validation_succesful = true;
            }

            if(!validation_succesful)
                MessageBox.Show(message, "Query Execution Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return validation_succesful;
        }

        public bool ValidateCostQuery()
        {
            string message = string.Empty;
            bool validation_succesful = false;
            if (costQuery.costSelectBox.Text == "")
            {
                message = "Cost Query's cost should not be empty";
            // }
            // else if (costQuery.queryActions.Text == "")
            // {
            //     message = "Cost Query's action selection should not be empty";
            }
            else if (!costQuery.queryActions.Text.Split(',').Select(qa => costQuery.queryActions.Actions.Contains(qa.Trim())).All(found => found == true))
            {
                message = "Cost Query's actions " + costQuery.queryActions.Text + " contain at least one unknown action";
            }
            else
            {
                validation_succesful = true;
            }

            if (!validation_succesful)
                MessageBox.Show(message, "Query Execution Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return validation_succesful;
        }
    }
}
