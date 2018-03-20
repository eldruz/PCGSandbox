using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystems
{
    [CreateAssetMenu(fileName = "Shape", menuName = "L System Configuration")]
    public class LSystemConfig : ScriptableObject
    {
        public Vector3 direction = Vector3.forward;
        public Vector3 RU = Vector3.up;
        public Vector3 RL = -Vector3.right;
        public Vector3 RH = Vector3.forward;
        public float distance = 1f;
        public float dL = 1f;
        public float angle = 90f;

        public string axiom;
        [HideInInspector, SerializeField]
        public List<StochasticProduction> stochasticProductions = new List<StochasticProduction>();
        [HideInInspector, SerializeField]
        public List<DeterministicProduction> deterministicProductions = new List<DeterministicProduction>();

    }
}