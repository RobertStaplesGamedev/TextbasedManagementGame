using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Colony {

    [CustomEditor(typeof(TechnologyCard))]
    public class TechnologyCardEditor : Editor
    {

        private Editor commodityEditor = null;
        public bool commodityFoldout = true;

        public override void OnInspectorGUI()
        {
            TechnologyCard technologyCard = (TechnologyCard)target;
            DrawDefaultInspector();
            if (technologyCard.techType == TechnologyCard.TechType.Commodity || technologyCard.techType == TechnologyCard.TechType.CommodityModifier) {
                if (technologyCard.techType == TechnologyCard.TechType.CommodityModifier) {
                    technologyCard.efficencymod = EditorGUILayout.IntField("Modifier:", technologyCard.efficencymod);
                }
                EditorGUILayout.BeginHorizontal();
                GUIStyle style = new GUIStyle(EditorStyles.foldout);
                //style.fixedWidth = 25;
                commodityFoldout = EditorGUILayout.Foldout(commodityFoldout, "Commodity",style);
                technologyCard.commodity = (CommodityData)EditorGUILayout.ObjectField("",technologyCard.commodity, typeof(CommodityData), allowSceneObjects: false);
                EditorGUILayout.EndHorizontal();
                if (technologyCard.commodity != null) {
                    if (commodityFoldout) {
                        commodityEditor = Editor.CreateEditor(technologyCard.commodity);
                        commodityEditor.OnInspectorGUI();
                    }
                }
            }
        }
    }
}