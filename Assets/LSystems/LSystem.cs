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

        private List<Edge> edges = new List<Edge>();
        public List<Edge> Edges { get { return edges; } set { } }

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
            turtle.transform.position = transform.position;
            turtle.transform.rotation = transform.rotation;

            currentState = new State
            {
                distance = distance,
                angle = angle,
                dL = dL
            };
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

            edges.Clear();
            turtle.transform.position = transform.position;
            turtle.transform.rotation = transform.rotation;

            foreach (char c in axiom)
            {
                switch (c)
                {
                    case 'F':
                        Vector3 from = turtle.transform.position;
                        turtle.transform.Translate(direction * currentState.distance * ((generation > 1) ? currentState.dL : 1f));
                        edges.Add(new Edge { from = from, to = turtle.transform.position });
                        break;
                    case 'f':
                        turtle.transform.Translate(direction * currentState.distance * ((generation > 1) ? currentState.dL : 1f));
                        break;
                    case '+':
                        turtle.transform.Rotate(RU * currentState.angle);
                        break;
                    case '-':
                        turtle.transform.Rotate(RU * -currentState.angle);
                        break;
                    case '&':
                        turtle.transform.Rotate(RL * currentState.angle);
                        break;
                    case '^':
                        turtle.transform.Rotate(RL * -currentState.angle);
                        break;
                    case '\\':
                        turtle.transform.Rotate(RH * currentState.angle);
                        break;
                    case '/':
                        turtle.transform.Rotate(RH * -currentState.angle);
                        break;
                    case '|':
                        turtle.transform.Rotate(RU * 180);
                        break;
                    case '[':
                        storedStates.Push(new State
                        {
                            position = turtle.transform.position,
                            rotation = turtle.transform.rotation,
                            distance = currentState.distance,
                            angle = currentState.angle
                        });
                        break;
                    case ']':
                        turtle.SetStateToTurtle(storedStates.Pop());
                        break;
                    default:
                        break;
                }
            }

            onGenerationOver.Invoke();
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

