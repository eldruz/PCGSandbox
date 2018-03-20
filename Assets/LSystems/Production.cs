using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystems
{
    public abstract class Production
    {
        public string predecessor;
        public string successor;

        public Production() { }

        public Production(string l, string r)
        {
            this.predecessor = l;
            this.successor = r;
        }
    }

    [System.Serializable]
    public class DeterministicProduction : Production
    {
        public DeterministicProduction() { }

        public DeterministicProduction(string l, string r) : base(l, r) { }
    }

    [System.Serializable]
    public class StochasticProduction : Production
    {
        public float probability;

        public StochasticProduction() { }

        public StochasticProduction(string l, string r, float p) : base(l, r)
        {
            this.probability = Mathf.Clamp(p, 0f, 1f);
        }
    }
}
