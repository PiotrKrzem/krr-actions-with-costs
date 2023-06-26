using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Northwoods.Go;
using Northwoods.Go.Models;
using Northwoods.Go.Tools;

namespace actions_with_costs
{
    public partial class StateDiagram : Form
    {
        List<State> initialStates;
        List<string> allActions;
        List<Statement> allStatements;

        Diagram stateDiagram;

        public StateDiagram(List<string> allFluents, List<string> allActions, List<Statement> allStatements)
        {
            InitializeComponent();

            initialStates = State.getAllInitialStates(allFluents);
            this.allActions = allActions;
            this.allStatements = allStatements;

            stateDiagram = stateVisualization.Diagram;
            createModel();
        }

        public void createModel()
        {
            stateDiagram.NodeTemplate = new Node("Auto")
              .Add(
                new Shape("Circle").Bind("Fill", "Color"), 
                new TextBlock { Margin = 5, Stroke = "black", Font = new Font("Segoe UI", 12, FontWeight.Bold) }.Bind("Text", "Key")
                );

            stateDiagram.Model = new StateModel
            {
                NodeDataSource = initialStates.Select(state => new NodeData { Key = state.getStateDescription(), Color = "lightblue" }).ToList(),
                LinkDataSource = generateStateArrows()
            };
        }

        public List<LinkData> generateStateArrows()
        {
            List<LinkData> links = new List<LinkData>();

            // Finding next states per each initial state
            foreach(var state in initialStates)
            {
                // Finding next states per each action
                foreach(var action in allActions)
                {
                    List<CausesStatement> statementsPerAction = allStatements
                        .FindAll(statement => statement.Type == StatementType.CAUSES)
                        .Cast<CausesStatement>()
                        .ToList()
                        .FindAll(statement => statement.Action == action && statement.doesMeetPreconditions(state.Literals))
                        .ToList();
                    List<Literal> statementEffects = statementsPerAction.Select(statement => statement.Postcondition).ToList();
                    List<string> modifiedFluents = statementEffects.Select(literal => literal.Fluent).ToList();
                    List<Literal> unModifiedLiterals = state.Literals.FindAll(literal => !modifiedFluents.Contains(literal.Fluent)).ToList();

                    string newStateDescription = new State(statementEffects.Union(unModifiedLiterals).ToList()).getStateDescription();
                    links.Add(new LinkData { From = state.getStateDescription(), To = newStateDescription, Text = action });
                }
            }
            return links;
        }

        public class StateModel : GraphLinksModel<NodeData, string, object, LinkData, string, string> { }
        public class NodeData : StateModel.NodeData
        {
            public string Color { get; set; }
        }
        public class LinkData : StateModel.LinkData { }
    }
}
