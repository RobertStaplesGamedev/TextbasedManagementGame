using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Colony {
    [CustomEditor(typeof(CommodityData))]
    public class CommodityDataEditor : Editor {

        private Editor editor = null;
        public bool oreFoldout = true;

        public override void OnInspectorGUI()
        {
            CommodityData commodityData = (CommodityData)target;
            DrawDefaultInspector();

            if (commodityData.resource == CommodityData.Resource.Ore && commodityData.type != CommodityData.Type.Building) {
                EditorGUILayout.BeginHorizontal();
                GUIStyle style = new GUIStyle(EditorStyles.foldout);
                //style.fixedWidth = 5;
                oreFoldout = EditorGUILayout.Foldout(oreFoldout, "Ore Data",style);
                commodityData.OreData = (OreData)EditorGUILayout.ObjectField("",commodityData.OreData, typeof(OreData), allowSceneObjects: false);
                EditorGUILayout.EndHorizontal();
                if (commodityData.OreData != null) {
                    if (oreFoldout) {
                        editor = Editor.CreateEditor(commodityData.OreData);
                        editor.DrawDefaultInspector();
                    }
                }
            }
        }
    }
}