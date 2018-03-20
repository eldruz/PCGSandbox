using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystems
{
    [System.Serializable]
    public class State
    {
        public Vector3 position;
        public Quaternion rotation;
        public float distance;
        public float dL;
        public float angle;

        public State Clone()
        {
            return (State)this.MemberwiseClone();
        }
    }
}

