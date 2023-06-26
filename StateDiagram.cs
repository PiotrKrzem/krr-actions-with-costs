using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Northwoods.Go;
using Northwoods.Go.Layouts;
using Northwoods.Go.Models;
using Northwoods.Go.Tools;

namespace actions_with_costs
{
    public partial class StateDiagram : Form
    {
        List<State> initialStates;
        List<State> finalStates;
        List<State> allStates;
        List<string> allActions;
        List<Statement> allStatements;

        Diagram stateDiagram;

        public StateDiagram(List<string> allActions, List<Statement> allStatements, List<State> initialStates)
        {
            InitializeComponent();

            this.initialStates = initialStates;
            this.allActions = allActions;
            this.allStatements = allStatements;
            allStates = new List<State>(initialStates);
            finalStates = new List<State>();

            stateDiagram = stateVisualization.Diagram;
            createModel();
        }

        public void createModel()
        {
            stateDiagram.Layout = new ForceDirectedLayout
            {
                DefaultSpringLength = 5,
                MaxIterations = 300
            };
            stateDiagram.NodeTemplate = new Node("Auto")
              .Add(new Shape("Circle").Bind("Fill", "Color"),
                new TextBlock { Margin = 5, Stroke = "black", Font = new Font("Segoe UI", 12, FontWeight.Bold) }.Bind("Text", "Key")
                );;

            stateDiagram.LinkTemplate = new Link {
                Curve = LinkCurve.Bezier,
                Adjusting = LinkAdjusting.Stretch,
                Reshapable = true,
                RelinkableFrom = false,
                RelinkableTo = false,
                ToShortLength = 5
            }.Add(
                new Shape(),
                new Shape { ToArrow = "Standard" },
                new TextBlock { 
                    TextAlign = TextAlign.Center, 
                    SegmentOffset = new Point(0, -25), 
                    Font = new Font("Segoe UI", 15, FontWeight.Bold) }.Bind("Text")
                );

            List<LinkData> stateTransitions = generateStateArrows();
            stateDiagram.Model = new StateModel
            {
                NodeDataSource = allStates.Select(state => new NodeData { 
                    Key = state.getStateDescription(),
                    Color = initialStates.Select(s => s.getStateDescription()).Contains(state.getStateDescription()) ? "lightgreen" : "lightgray" 
                }).ToList(),
                LinkDataSource = stateTransitions
            };
        }

        public List<LinkData> generateStateArrows()
        {
            List<LinkData> links = new List<LinkData>();

            // Finding next states per each initial state
            for(int i = 0; i < allStates.Count; i++)
            {
                State state = allStates[i];
                State finalState = new State(state.Literals);

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

                    // cost of execution of statements
                    int cost = statementsPerAction.Select(st => st.Cost).Sum();

                    State newState = new State(statementEffects.Union(unModifiedLiterals).ToList());
                    string newStateDescription = newState.getStateDescription();

                    if(!allStates.Select(s => s.getStateDescription()).Contains(newState.getStateDescription()))
                    {
                        allStates.Add(newState);
                    }

                    finalState = newState;
                    links.Add(new LinkData { From = state.getStateDescription(), To = newStateDescription, Text = action + "\n cost:" + cost });
                }
                finalStates.Add(finalState);
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
