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

        private string _axiom;

        private Vector3 _direction = Vector3.forward;
        private Vector3 _ru = Vector3.up;
        private Vector3 _rl = -Vector3.right;
        private Vector3 _rh = Vector3.forward;
        private float _distance;
        private float _dL;
        private float _angle;

        private List<DeterministicProduction> _deterministicProductions = new List<DeterministicProduction>();
        private Dictionary<string, string> _deterministicProductionsDic = new Dictionary<string, string>();
        private List<StochasticProduction> _stochasticProductions = new List<StochasticProduction>();

        private GameObject _turtleObj;
        private Turtle _turtle;

        private readonly List<State> run = new List<State>();
        public List<State> Run { get { return run; } set { } }

        private readonly Stack<State> storedStates = new Stack<State>();
        private State _currentState;

        private int _generation = 0;

        public UnityEvent onGenerationOver;

        private void Awake()
        {
            LoadConfig();
            Random.InitState(randomSeed);

            _turtleObj = new GameObject("Turtle");
            _turtle = _turtleObj.gameObject.AddComponent<Turtle>();

            _turtle.transform.parent = transform;

            Init();
        }

        private void Init()
        {
            _turtle.transform.position = transform.position;
            _turtle.transform.rotation = transform.rotation;

            _currentState = new State
            {
                previousPosition = _turtle.transform.position,
                position = _turtle.transform.position,
                rotation = _turtle.transform.rotation,
                isDrawn = false
            };

            run.Clear();
            run.Add(_currentState.Clone());
        }

        private void LoadConfig()
        {
            _axiom = config.axiom;

            _direction = config.direction;
            _ru = config.RU;
            _rl = config.RL;
            _rh = config.RH;
            _distance = config.distance;
            _dL = config.dL;
            _angle = config.angle;

            _deterministicProductions = config.deterministicProductions;
            foreach (DeterministicProduction rule in _deterministicProductions)
            {
                _deterministicProductionsDic.Add(rule.predecessor, rule.successor);
            }

            _stochasticProductions = config.stochasticProductions;
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
            _generation++;
            _axiom = Derivation(_axiom);

            Init();

            foreach (char c in _axiom)
            {
                switch (c)
                {
                    case 'F':
                        _currentState.isDrawn = true;
                        Translate(_direction * _distance * ((_generation > 1) ? _dL : 1f));
                        run.Add(_currentState.Clone());
                        break;
                    case 'f':
                        _currentState.isDrawn = false;
                        Translate(_direction * _distance * ((_generation > 1) ? _dL : 1f));
                        break;
                    case '+':
                        Rotate(_ru, _angle);
                        break;
                    case '-':
                        Rotate(_ru, -_angle);
                        break;
                    case '&':
                        Rotate(_rl, _angle);
                        break;
                    case '^':
                        Rotate(_rl, -_angle);
                        break;
                    case '\\':
                        Rotate(_rh, _angle);
                        break;
                    case '/':
                        Rotate(_rh, -_angle);
                        break;
                    case '|':
                        Rotate(_ru, 180);
                        break;
                    case '[':
                        storedStates.Push(_currentState.Clone());
                        break;
                    case ']':
                        _currentState = storedStates.Pop();
                        _turtle.SetStateToTurtle(_currentState);
                        break;
                    default:
                        break;
                }
            }

            onGenerationOver.Invoke();
        }

        private void Translate(Vector3 to)
        {
            _currentState.previousPosition = _turtle.transform.position;
            _turtle.transform.Translate(to);
            _currentState.position = _turtle.transform.position;
        }

        private void Rotate(Vector3 axis, float angle)
        {
            _turtle.transform.Rotate(axis * angle);
            _currentState.rotation = _turtle.transform.rotation;
        }

        private string Derivation(string s)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in s)
            {
                List<StochasticProduction> appliedRules = _stochasticProductions.FindAll(el => el.predecessor == c.ToString());
                if (appliedRules.Count > 0)
                {
                    float randomSelect = Random.Range(0f, 1f);
                    int idx;
                    float cumulativeProbability = 0f;
                    for (idx = 0; idx < appliedRules.Count; idx++)
                    {
                        cumulativeProbability += appliedRules[idx].probability;
                        if (randomSelect < cumulativeProbability) break;
                    } 
                    sb.Append(appliedRules[idx].successor);
                }
                else if (_deterministicProductionsDic.ContainsKey(c.ToString()))
                {
                    sb.Append(_deterministicProductionsDic[c.ToString()]);
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

