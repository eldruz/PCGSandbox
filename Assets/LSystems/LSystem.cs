using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace LSystems
{
    [System.Serializable]
    public class Edge
    {
        public Vector3 from;
        public Vector3 to;
    }

    public class LSystem : MonoBehaviour
    {
        public LSystemConfig config;

        public int randomSeed = 0;

        private string axiom;

        private Vector3 direction = Vector3.forward;
        private Vector3 RU = Vector3.up;
        private Vector3 RL = -Vector3.right;
        private Vector3 RH = Vector3.forward;
        private float distance;
        private float dL;
        private float angle;

        private List<DeterministicProduction> deterministicProductions = new List<DeterministicProduction>();
        private Dictionary<string, string> productionsDic = new Dictionary<string, string>();
        private List<StochasticProduction> stochasticProductions = new List<StochasticProduction>();

        private GameObject turtleObj;
        private Turtle turtle;

        //private List<Edge> edges = new List<Edge>();
        //public List<Edge> Edges { get { return edges; } set { } }

        private List<State> run = new List<State>();
        public List<State> Run { get { return run; } set { } }

        //private Tree<State> tree;
        //public Tree<State> Tree { get { return tree; } set { } }
        //private Tree<State> currentTree;

        //private Stack<Tree<State>> storedTree = new Stack<Tree<State>>();
        private Stack<State> storedStates = new Stack<State>();
        private State currentState;

        private int generation = 0;

        public UnityEvent onGenerationOver;

        private void Awake()
        {
            LoadConfig();
            Random.InitState(randomSeed);

            turtleObj = new GameObject("Turtle");
            turtle = turtleObj.gameObject.AddComponent<Turtle>();

            turtle.transform.parent = transform;

            Init();
        }

        private void Init()
        {
            turtle.transform.position = transform.position;
            turtle.transform.rotation = transform.rotation;

            currentState = new State
            {
                previousPosition = turtle.transform.position,
                position = turtle.transform.position,
                rotation = turtle.transform.rotation,
                distance = distance,
                angle = angle,
                dL = dL
            };

            //edges.Clear();
            run.Clear();
            run.Add(currentState.Clone());
            //tree = new Tree(null, currentState.Clone());
            //currentTree = tree;
        }

        private void LoadConfig()
        {
            axiom = config.axiom;

            direction = config.direction;
            RU = config.RU;
            RL = config.RL;
            RH = config.RH;
            distance = config.distance;
            dL = config.dL;
            angle = config.angle;

            deterministicProductions = config.deterministicProductions;
            foreach (Production rule in deterministicProductions)
            {
                productionsDic.Add(rule.predecessor, rule.successor);
            }

            stochasticProductions = config.stochasticProductions;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Generate();
            }
        }

        public void Generate()
        {
            generation++;
            axiom = Derivation(axiom);

            Init();

            foreach (char c in axiom)
            {
                switch (c)
                {
                    case 'F':
                        //Vector3 from = turtle.transform.position;
                        currentState.isDrawn = true;
                        // Moves forward and updates the state
                        Translate(direction * currentState.distance * ((generation > 1) ? currentState.dL : 1f));
                        //// Create a new tree with the updated state
                        //Tree branch = new Tree(currentTree, currentState.Clone());
                        //// Add the tree as a child of the currentTree
                        //currentTree.AddChild(branch);
                        //// Set the currentTree to the newly created tree
                        //currentTree = branch;
                        run.Add(currentState.Clone());
                        //edges.Add(new Edge { from = from, to = turtle.transform.position });
                        break;
                    case 'f':
                        currentState.isDrawn = false;
                        Translate(direction * currentState.distance * ((generation > 1) ? currentState.dL : 1f));
                        break;
                    case '+':
                        Rotate(RU, currentState.angle);
                        break;
                    case '-':
                        Rotate(RU, -currentState.angle);
                        break;
                    case '&':
                        Rotate(RL, currentState.angle);
                        break;
                    case '^':
                        Rotate(RL, -currentState.angle);
                        break;
                    case '\\':
                        Rotate(RH, currentState.angle);
                        break;
                    case '/':
                        Rotate(RH, -currentState.angle);
                        break;
                    case '|':
                        Rotate(RU, 180);
                        break;
                    case '[':
                        storedStates.Push(currentState.Clone());
                        //storedTree.Push(currentTree);
                        break;
                    case ']':
                        currentState = storedStates.Pop();
                        turtle.SetStateToTurtle(currentState);
                        //currentTree = storedTree.Pop();
                        break;
                    default:
                        break;
                }
            }

            onGenerationOver.Invoke();
        }

        private void Translate(Vector3 to)
        {
            currentState.previousPosition = turtle.transform.position;
            turtle.transform.Translate(to);
            currentState.position = turtle.transform.position;
        }

        private void Rotate(Vector3 axis, float angle)
        {
            turtle.transform.Rotate(axis * angle);
            currentState.rotation = turtle.transform.rotation;
        }

        private string Derivation(string s)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in s)
            {
                List<StochasticProduction> appliedRules = stochasticProductions.FindAll(el => el.predecessor == c.ToString());
                if (appliedRules.Count > 0)
                {
                    // apply stochastic rules
                    // TODO: apply provided probabilities
                    sb.Append(appliedRules[Random.Range(0, appliedRules.Count)].successor);
                }
                else if (productionsDic.ContainsKey(c.ToString()))
                {
                    // apply deterministic rules
                    sb.Append(productionsDic[c.ToString()]);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}

