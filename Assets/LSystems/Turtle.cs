using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystems
{
    public class Turtle : MonoBehaviour
    {
        public void SetStateToTurtle(State st)
        {
            transform.position = st.position;
            transform.rotation = st.rotation;
        }
    }
}

