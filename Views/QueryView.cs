using Syncfusion.WinForms.ListView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace actions_with_costs
{
    class Query
    {
        public Label afterLabel;
        public QuerySFComboBox queryActions;
        public Label fromLabel;
        public QueryInitStateBox initialStateSelectBox;

        public Query(ref FlowLayoutPanel queryPanel, ref List<List<Literal>> allPossibleStartingStates) {
            this.afterLabel = StatementConstants.createStatementLabel("after");
            this.queryActions = new QuerySFComboBox(queryPanel);
            this.fromLabel = StatementConstants.createStatementLabel("from initial state");
            this.initialStateSelectBox = new QueryInitStateBox(queryPanel, allPossibleStartingStates);
        }
        public virtual void createQueryObject(ref FlowLayoutPanel queryPanel) { }
        public virtual string createQueryText() { return "<error query>"; }
    }
    class ValueQuery : Query
    {
        public QueryComboBox fluentSelectBox;

        public ValueQuery(ref FlowLayoutPanel queryPanel, ref List<string> positiveNegativeFluents, ref List<List<Literal>> allPossibleStartingStates) : base(ref queryPanel, ref allPossibleStartingStates)
        {
            this.fluentSelectBox = new QueryComboBox(queryPanel, positiveNegativeFluents);
        }
        public override void createQueryObject(ref FlowLayoutPanel queryPanel)
        {
            queryPanel.Controls.Clear();
            queryPanel.Controls.AddRange(new Control[] {
                fluentSelectBox,
                afterLabel,
                queryActions,
                fromLabel,
                initialStateSelectBox});
        }

        public override string createQueryText()
        {
            return fluentSelectBox.Text + " after " + queryActions.Text + " from " + initialStateSelectBox.Text;
        }
    }
    class CostQuery : Query
    {
        public QueryNumericUpDown costSelectBox;
        public CostQuery(ref FlowLayoutPanel queryPanel, ref List<List<Literal>> allPossibleStartingStates) : base(ref queryPanel, ref allPossibleStartingStates)
        {
            this.costSelectBox = new QueryNumericUpDown(queryPanel);
        }
        public override void createQueryObject(ref FlowLayoutPanel queryPanel)
        {
            queryPanel.Controls.Clear();
            queryPanel.Controls.AddRange(new Control[] {
                costSelectBox,
                afterLabel,
                queryActions,
                fromLabel,
                initialStateSelectBox});
        }

        public override string createQueryText()
        {
            return costSelectBox.Text + " after " + queryActions.Text + " from " + initialStateSelectBox.Text;
        }
    }

    
    class QueryView
    {
        private FlowLayoutPanel queryPanel;
        private FlowLayoutPanel historyPanel;
        private ComboBox queryTypeComboBox;
        private Button executeQueryButton;
        private Button clearHistoryButton;

        public ValueQuery valueQuery;
        public CostQuery costQuery;


        public QueryView(ref FlowLayoutPanel queryPanel, ref FlowLayoutPanel historyPanel, ref ComboBox queryTypeComboBox, ref List<string> positiveNegativeFluents, ref List<List<Literal>> allPossibleStartingStates)
        {
            this.queryPanel = queryPanel;
            this.historyPanel = historyPanel;
            this.queryTypeComboBox = queryTypeComboBox;

            valueQuery = new ValueQuery(ref queryPanel, ref positiveNegativeFluents, ref allPossibleStartingStates);
            costQuery = new CostQuery(ref queryPanel, ref allPossibleStartingStates);
        }

        public void createQueryObject()
        {
            Query query = getQueryForType();
            query.createQueryObject(ref queryPanel);
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

        public void executeQuery()
        {
            List<Literal> literal_state;
            Query q;
            int final_cost = 0;
            if (queryTypeComboBox.SelectedValue.ToString() == "value")
            {
                q = valueQuery;
            }
            else
            {
                q = costQuery;
            }
            /*
            literal_state = q.initialStateSelectBox;
            State state = new State(literal_state);
            List<CausesStatement> causesStatements = allStatements
                .FindAll(statement => statement.Type == StatementType.CAUSES)
                .Cast<CausesStatement>()
                .ToList();

            
            while(true) {
                List<CausesStatement> causesWithEffects = null;// causesStatements.FindAll(causes => causes.doesMeetPreconditions(currentState.Literals));
                if (causesWithEffects.Count() == 0) break;
                if (causesWithEffects.Count() > 1) throw new Exception("Non-deterministic execution of the query");
                final_cost += causesWithEffects.First().Cost;
                var groupedCauses = causesWithEffects.GroupBy(statement => statement.Action).ToList();
                foreach (var causeGroup in groupedCauses)
                {
                    List<Literal> effectsOfAction = causeGroup.Select(statement => statement.Postcondition).ToList();
                    foreach(Literal effect in effectsOfAction)
                    {
                        Literal negatedEffect = new Literal(effect);
                        negatedEffect.IfHolds = !effect.IfHolds;
                        if (state.Literals.Contains(effect))
                        {
                            continue;
                        } else if(state.Literals.Contains(negatedEffect))
                        {
                            state.Literals.Remove(negatedEffect);
                            state.Literals.Add(effect);
                        } else
                        {
                            state.Literals.Add(effect);
                        }
                    }
                }
            }
            */
            String output = q.createQueryText() + " -> ";
            if (queryTypeComboBox.SelectedValue.ToString() == "value")
            {
                /* 
                if (valueQuery.fluentSelectBox.Text.Contains(state))
                {
                    output += "QUERY CORRECT";
                } else
                {
                    output += "QUERY INCORRECT";
                }
                */
            } else
            {
                if ((int)costQuery.costSelectBox.Value >= final_cost)
                {
                    output += "QUERY CORRECT";
                }
                else
                {
                    output += "QUERY INCORRECT";
                }
            }
            Label l = new Label();
            l.Text = output;
            historyPanel.Controls.Add(l);
        }
    }
}
