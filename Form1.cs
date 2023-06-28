using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace actions_with_costs
{
    public partial class Form1 : Form
    {
        // View sections
        private FluentActionView fluentActionView;
        private ActionModelView actionModelView;

        // Stored state
        public List<string> allFluents;
        public List<string> positiveNegativeFluents;
        public List<string> allActions;
        public List<Statement> allStatements;
        public List<State> initialStates;
        public List<string> allInitialStatesStringified;

        public Form1()
        {
            InitializeComponent();
            allFluents = new List<string>();
            allActions = new List<string>();
            allStatements = new List<Statement>();
            positiveNegativeFluents = new List<string>();
            allInitialStatesStringified = new List<string>();
            initialStates = new List<State>();

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
                ref deleteStatementButton,
                ref deleteAllStatementsButton,
                ref executeProgramTextBox,
                ref executeProgramButton,
                ref visualizationButton,
                ref queryPanel,
                ref queryTypeSelectBox,
                ref executeQueryButton,
                ref finalState,
                ref finalCost,
                ref finalStateLabel,
                ref finalCostLabel);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addFluentButton.Enabled = false;
            addActionButton.Enabled = false;
            //addStatementButton.Enabled = false;
            deleteFluentButton.Enabled = false;
            deleteActionButton.Enabled = false;
            deleteStatementButton.Enabled = false;
            executeProgramButton.Enabled = false;
            executeProgramTextBox.Enabled = false;
            executeQueryButton.Enabled = false;

            List<Item> items = new List<Item>();
            items.Add(new Item() { Text = "Initially statement", Value = "initially" });
            items.Add(new Item() { Text = "Value statement", Value = "value" });
            items.Add(new Item() { Text = "Effect statement", Value = "effect" });

            statementsComboBox.DataSource = items;
            statementsComboBox.DisplayMember = "Text";
            statementsComboBox.ValueMember = "Value";

            List<Item> queryItems = new List<Item>();
            queryItems.Add(new Item() { Text = "Value query", Value = "value" });
            queryItems.Add(new Item() { Text = "Cost query", Value = "cost" });

            queryTypeSelectBox.DataSource = queryItems;
            queryTypeSelectBox.DisplayMember = "Text";
            queryTypeSelectBox.ValueMember = "Value";

            actionModelView.initiallyStatementObject.createStatementObject(statementsPanel);
            actionModelView.valueQuery.createQueryObject(queryPanel);

        }

        // ----------------------------- FORM METHODS OF FLUENT/ACTION SECTION ---------------------------------------------
        private void addFluentTextBox_TextChanged(object sender, EventArgs e) =>
            fluentActionView.updateAddButtonState(ModelElementType.FLUENT);
        private void addActionTextBox_TextChanged(object sender, EventArgs e) =>
            fluentActionView.updateAddButtonState(ModelElementType.ACTION);

        private void addFluentTextBox_KeyPress(object sender, KeyPressEventArgs e) =>
            fluentActionView.addModelItemAfterEnter(ref e, ModelElementType.FLUENT, allFluents, buildPositiveNegativeFluents);
        private void addActionTextBox_KeyPress(object sender, KeyPressEventArgs e) =>
            fluentActionView.addModelItemAfterEnter(ref e, ModelElementType.ACTION, allActions, updateActionSelections);

        private void addFluentButton_Click(object sender, EventArgs e) =>
            fluentActionView.addFluent(buildPositiveNegativeFluents, allFluents);
        private void addActionButton_Click(object sender, EventArgs e) =>
            fluentActionView.addAction(updateActionSelections, allActions);

        private void deleteFluentButton_Click(object sender, EventArgs e) =>
            fluentActionView.deleteModelElement(ModelElementType.FLUENT, ref allFluents, allStatements, buildPositiveNegativeFluents, clearStatements);
        private void deleteActionButton_Click(object sender, EventArgs e) =>
            fluentActionView.deleteModelElement(ModelElementType.ACTION, ref allActions, allStatements, updateActionSelections, clearStatements);

        private void removeAllFluents_Click(object sender, EventArgs e) =>
            fluentActionView.deleteAllModelElementsOfType(ModelElementType.FLUENT, allFluents, allStatements, buildPositiveNegativeFluents, clearStatements);
        private void removeAllActions_Click(object sender, EventArgs e) =>
            fluentActionView.deleteAllModelElementsOfType(ModelElementType.ACTION, allActions, allStatements, updateActionSelections, clearStatements);

        private void allFluentsCheckBox_ItemChecked(object sender, ItemCheckEventArgs e) =>
            fluentActionView.updateRemoveButtonState(ModelElementType.FLUENT, e);
        private void allActionsCheckBox_ItemChecked(object sender, ItemCheckEventArgs e) =>
            fluentActionView.updateRemoveButtonState(ModelElementType.ACTION, e);
        private void executeProgramButton_Click(object sender, EventArgs e)
        {
            List<CausesStatement> causesStatements = allStatements
              .FindAll(statement => statement.Type == StatementType.CAUSES)
              .Cast<CausesStatement>()
              .ToList();

            List<(State, int, State)> currentStatesCosts = new List<(State, int, State)>();
            List<string> actionsList = new List<string>();
            if (executeProgramTextBox.Text == "")
            {
                string message = "There are no actions specified";
                MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string[] actions = executeProgramTextBox.Text.Split(',');
            foreach (string a in actions)
            {
                if (a.Trim().Length == 0)
                {
                    string message = "Actions have to be separated by a single comma";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (!allActions.Contains(a.Trim()))
                {
                    string message = "Action '" + a + "' was not added to the list of actions";
                    MessageBox.Show(message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    actionsList.Add(a.Trim());
                }
            }

            foreach (State s in initialStates)
            {
                int currentCost = 0;
                List<Literal> currentState = new List<Literal>();
                foreach(Literal l in s.Literals)
                {
                    currentState.Add(new Literal(l));
                }
                foreach (string action in actionsList)
                {
                    List<Literal> allPostconditions = new List<Literal>();
                    List<CausesStatement> matchedCausesStatements = causesStatements
                        .FindAll(statement => statement.Action == action)
                        .ToList();

                    foreach (CausesStatement statement in matchedCausesStatements)
                    {
                        if (statement.Precondition.Count == 0)
                        {
                            allPostconditions.Add(statement.Postcondition);
                            currentCost += statement.Cost;
                        }
                        else
                        {
                            bool ifPreconditionHolds = statement.Precondition.All(l => l.ExistsInCollection(currentState));
                            if (ifPreconditionHolds)
                            {
                                allPostconditions.Add(statement.Postcondition);
                                currentCost += statement.Cost;
                            }
                        }
                    }
                    foreach (Literal literal in allPostconditions)
                    {
                        currentState
                           .Where(l => l.Fluent == literal.Fluent)
                           .Select(l => l.IfHolds = literal.IfHolds)
                           .ToList();
                    }
                }
                currentStatesCosts.Add((new State(currentState), currentCost, new State(s.Literals)));
            }
            string state = String.Empty;
            if (currentStatesCosts.Count == 1)
            {
                state = string.Join(",", currentStatesCosts[0].Item1.Literals.Select(l => l.ToString()));
                finalStateLabel.Visible = true;
                finalState.Text = state;
                finalCostLabel.Visible = true;
                finalCost.Text = currentStatesCosts[0].Item2.ToString();
            }
            else
            {
                foreach ((State, int, State) stateCost in currentStatesCosts)
                {
                    state += "Initial state: {";
                    allInitialStatesStringified.Add(state);
                    state += string.Join(",", stateCost.Item3.Literals.Select(l => l.ToString()));
                    state += "} => Final state: {";
                    state += string.Join(",", stateCost.Item1.Literals.Select(l => l.ToString()));
                    state += "} cost: ";
                    state += stateCost.Item2.ToString();
                    state += " \n\n";
                }
                MessageBox.Show(state, "Result of program execution", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            //string aa = "Initail state: loaded, alive => Final state: loaded,alive \n\nInitail state: loaded, alive Final state: loaded,~alive";
            //MessageBox.Show(aa, "Result of program execution", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
        }

        private void executeQueryButton_Click(object sender, EventArgs e)
        {
            Query query;
            if (queryTypeSelectBox.SelectedValue.ToString() == "value")
            {
                if (!actionModelView.ValidateValueQuery()) return;
                query = actionModelView.valueQuery;
            }   
            else
            {
                if (!actionModelView.ValidateCostQuery()) return;
                query = actionModelView.costQuery;
            }
                

            List<CausesStatement> causesStatements = allStatements
              .FindAll(statement => statement.Type == StatementType.CAUSES)
              .Cast<CausesStatement>()
              .ToList();

            int currentCost = 0;
            List<Literal> initialState = new List<Literal>();
            List<Literal> currentState = new List<Literal>();
            string[] fluents = query.initialStateSelectBox.Text.Split(',');
            foreach (string f in fluents)
            {
                initialState.Add(new Literal(f.Replace("~", ""), f.Contains("~")));
                currentState.Add(new Literal(f.Replace("~", ""), f.Contains("~")));
            }

            List<string> actionsList = new List<string>();
            string[] actions = query.queryActions.Text.Split(',');
            foreach (string a in actions)
            {
                actionsList.Add(a);
            }

            foreach (string action in actionsList)
            {
                List<Literal> allPostconditions = new List<Literal>();
                List<CausesStatement> matchedCausesStatements = causesStatements
                    .FindAll(statement => statement.Action == action)
                    .ToList();

                foreach (CausesStatement statement in matchedCausesStatements)
                {
                    if (statement.Precondition.Count == 0)
                    {
                        allPostconditions.Add(statement.Postcondition);
                        currentCost += statement.Cost;
                    }
                    else
                    {
                        bool ifPreconditionHolds = statement.Precondition.All(l => l.ExistsInCollection(currentState));
                        if (ifPreconditionHolds)
                        {
                            allPostconditions.Add(statement.Postcondition);
                            currentCost += statement.Cost;
                        }
                    }
                }
                foreach (Literal literal in allPostconditions)
                {
                    currentState
                       .Where(l => l.Fluent == literal.Fluent)
                       .Select(l => l.IfHolds = literal.IfHolds)
                       .ToList();
                }
            }
            string state = string.Join(",", currentState.Select(l => l.ToString()));
            string output;
            MessageBoxIcon icon;
            if (query is ValueQuery)
            {
                List<string> query_fluents = (query as ValueQuery).fluentSelectBox.Text.Split(',').ToList();
                List<string> current_state_fluents = currentState.Select(l => l.ToString()).ToList();
                bool all_match = true;
                output = "FINAL STATE: " + state + " ";
                foreach (string query_fluent in query_fluents)
                {
                    if (!current_state_fluents.Contains(query_fluent))
                    {
                        all_match = false;
                        break;
                    }
                }
                if (all_match)
                {
                    output += "QUERY CORRECT";
                    icon = MessageBoxIcon.Asterisk;
                }
                else
                {
                    output += "QUERY INCORRECT";
                    icon = MessageBoxIcon.Error;
                }
            }
            else
            {
                output = "FINAL COST: " + currentCost.ToString() + " ";
                if ((int)((query as CostQuery).costSelectBox.Value) >= currentCost)
                {
                    output += "QUERY CORRECT";
                    icon = MessageBoxIcon.Asterisk;
                }
                else
                {
                    output += "QUERY INCORRECT";
                    icon = MessageBoxIcon.Error;
                }
            }
            MessageBox.Show(output, "Query Result", MessageBoxButtons.OK, icon);
        }

        // -------------------------------------------------------------------------------------------------------------------


        // ----------------------------- FORM METHODS OF ACTION MODEL --------------------------------------------------------

        private void statementsComboBox_SelectionChangeCommitted(object sender, EventArgs e) =>
            actionModelView.createStatementObject();

        private void addStatementButton_Click(object sender, EventArgs e)
        {
            initialStates = actionModelView.addStatement(ref allStatements, allFluents, allActions);
            getInitialStatesStringified();
        }

        private void allStatementsCheckBox_ItemCheck(object sender, ItemCheckEventArgs e) =>
            actionModelView.updateRemoveButtonState(e);

        private void deleteStatementButton_Click(object sender, EventArgs e)
        {
            initialStates = actionModelView.deleteStatementElement(ref allStatements, allFluents);
            getInitialStatesStringified();
        }     
        private void deleteAllStatementsButton_Click(object sender, EventArgs e)
        {
            actionModelView.deleteAllStatements(ref allStatements);
            initialStates = new List<State>();
            getInitialStatesStringified();
        }
        private void executeProgramTextBox_Click(object sender, EventArgs e)
        {
            if(executeProgramTextBox.Text == "Type in actions")
            {
                executeProgramTextBox.Text = "";
                executeProgramTextBox.ForeColor = SystemColors.WindowText;
            }
        }

        // -------------------------------------------------------------------------------------------------------------------

        // ----------------------------- FORM METHODS OF VISUALIZATION --------------------------------------------------------

        private void visualizationButton_Click(object sender, EventArgs e)
        {
            StateDiagram stateDiagramForm = new StateDiagram(allActions, allStatements, initialStates);
            stateDiagramForm.Show();
        }

        // -------------------------------------------------------------------------------------------------------------------

        // ----------------------------- FORM MAIN METHODS -------------------------------------------------------------------
        private void clearMenuOption_Click(object sender, EventArgs e)
        {
            actionModelView.deleteAllStatements(ref allStatements); 
            fluentActionView.deleteAllModelElementsOfType(ModelElementType.ACTION, allActions, updateActionSelections);
            fluentActionView.deleteAllModelElementsOfType(ModelElementType.FLUENT, allFluents, buildPositiveNegativeFluents);
        }
        // -------------------------------------------------------------------------------------------------------------------

        // ----------------------------- COMMON HELPER METHODS ---------------------------------------------------------------

        private void clearStatements() => actionModelView.deleteAllStatements(ref allStatements);

        private void updateActionSelections()
        {
            actionModelView.effectStatementObject.causesAction.Items.Clear();
            actionModelView.effectStatementObject.causesAction.Items.AddRange(allActions.ToArray());
            actionModelView.afterStatementObject.afterActions.DataSource = allActions.ToList();

            actionModelView.valueQuery.queryActions.DataSource = allActions.ToList();
            actionModelView.valueQuery.queryActions.SelectedItems.Clear();

            actionModelView.costQuery.queryActions.DataSource = allActions.ToList();
            actionModelView.costQuery.queryActions.SelectedItems.Clear();
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

            actionModelView.valueQuery.fluentSelectBox.Items.Clear();
            actionModelView.valueQuery.fluentSelectBox.Items.AddRange(positiveNegativeFluents.ToArray());

            if (allStatements.Count > 0)
            {
                getInitialStatesStringified();
                executeProgramTextBox.Text = "Type in actions";
                finalState.Text = "";
                finalCost.Text = "";
            }
        }
        private void getInitialStatesStringified()
        {
            allInitialStatesStringified = new List<string>();
            foreach (State s in initialStates)
            {
                string state = String.Empty;
                foreach (Literal l in s.Literals)
                {
                    state += ",";
                    state += l.ToString();
                }
                state = state.Remove(0, 1);
                allInitialStatesStringified.Add(state);
            }

            actionModelView.valueQuery.initialStateSelectBox.Items.Clear();
            actionModelView.valueQuery.initialStateSelectBox.Items.AddRange(allInitialStatesStringified.ToArray());

            actionModelView.costQuery.initialStateSelectBox.Items.Clear();
            actionModelView.costQuery.initialStateSelectBox.Items.AddRange(allInitialStatesStringified.ToArray());
        }

        private void queryTypeSelectBox_SelectionChangeCommitted(object sender, EventArgs e) =>
            actionModelView.createQueryObject();

    }

    public class Item
    {
        public Item() { }
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
