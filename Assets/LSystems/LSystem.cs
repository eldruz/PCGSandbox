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

        private List<State> run = new List<State>();
        public List<State> Run { get { return run; } set { } }

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
                isDrawn = false
            };

            run.Clear();
            run.Add(currentState.Clone());
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
            foreach (DeterministicProduction rule in deterministicProductions)
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

        private void Generate()
        {
            generation++;
            axiom = Derivation(axiom);

            Init();

            foreach (char c in axiom)
            {
                switch (c)
                {
                    case 'F':
                        currentState.isDrawn = true;
                        Translate(direction * distance * ((generation > 1) ? dL : 1f));
                        run.Add(currentState.Clone());
                        break;
                    case 'f':
                        currentState.isDrawn = false;
                        Translate(direction * distance * ((generation > 1) ? dL : 1f));
                        break;
                    case '+':
                        Rotate(RU, angle);
                        break;
                    case '-':
                        Rotate(RU, -angle);
                        break;
                    case '&':
                        Rotate(RL, angle);
                        break;
                    case '^':
                        Rotate(RL, -angle);
                        break;
                    case '\\':
                        Rotate(RH, angle);
                        break;
                    case '/':
                        Rotate(RH, -angle);
                        break;
                    case '|':
                        Rotate(RU, 180);
                        break;
                    case '[':
                        storedStates.Push(currentState.Clone());
                        break;
                    case ']':
                        currentState = storedStates.Pop();
                        turtle.SetStateToTurtle(currentState);
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
                    float randomSelect = Random.Range(0f, 1f);
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

