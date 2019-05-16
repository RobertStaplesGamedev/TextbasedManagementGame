using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Colony {

    [CustomEditor(typeof(TechnologyCard))]
    public class TechnologyCardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI ();

            TechnologyCard technologyCard = (TechnologyCard)target;

        }
    }
}