using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NodeEditorFramework.Utilities;

namespace NodeEditorFramework.Standard
{
	[Node (false, "Technology/Technology Node")]
	public class TechnologyNode : Node 
	{
		public const string ID = "TechnologyNode";
		public override string GetID { get { return ID; } }

		public override string Title { get { return title; } }
		public override Vector2 DefaultSize { get { return new Vector2 (300, 200); } }
		
		private string title = "New Technology"; 
        private string description = "";
        private Sprite sprite;
        private int researchCost;

        private int dependancies = 0;
        private int dependants = 0;

        public List<string> labels = new List<string>();

        //private float propertyLength = 500;

		private ValueConnectionKnobAttribute inputKnob = new ValueConnectionKnobAttribute("Output", Direction.In, "System.String");
        private ValueConnectionKnobAttribute outputKnob = new ValueConnectionKnobAttribute("Output", Direction.Out, "System.String");

		private ValueConnectionKnob[] inputKnobs = new ValueConnectionKnob[100];
		
		public override void NodeGUI () 
		{
            //EditorStyles.textField.wordWrap = true;

            // GUILayout.BeginHorizontal ();
            //     GUILayout.Label("Title:");
			//     title = EditorGUILayout.TextField(title);
            // GUILayout.EndHorizontal();
            // GUILayout.BeginHorizontal ();
            //     GUILayout.Label("Description:");
			//     description = EditorGUILayout.TextField(description, GUILayout.MaxHeight(40));
            // GUILayout.EndHorizontal();
            // GUILayout.BeginHorizontal ();
            //     GUILayout.Label("Sprite:");
			//     sprite = (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), allowSceneObjects: true);
            // GUILayout.EndHorizontal();
            // GUILayout.BeginHorizontal ();
            //     GUILayout.Label("Cost:");
			//     researchCost = EditorGUILayout.IntField(researchCost);
            // GUILayout.EndHorizontal();
            
            // GUILayout.BeginHorizontal ();
            //     if (GUILayout.Button("Add Dependancy")) {
            //         labels.Add("");
			// 	    inputKnobs[labels.Count] = CreateValueConnectionKnob(inputKnob);
            //     }
			    
            //     if (GUILayout.Button("Add Dependant")) {
            //         labels.Add("");
			// 	    inputKnobs[labels.Count] = CreateValueConnectionKnob(outputKnob);
            //     }
            // GUILayout.EndHorizontal ();
            // for (int i = 0; i < labels.Count; i++)
			// { // Display label and delete button
			// 	GUILayout.BeginHorizontal();
			// 	GUILayout.Label(labels[i]);
			// 	((ValueConnectionKnob)dynamicConnectionPorts[i]).SetPosition();
			// 	if(GUILayout.Button("x", GUILayout.ExpandWidth(false)))
			// 	{ // Remove current label
			// 		labels.RemoveAt (i);
			// 		inputKnobs[i] = null;
			// 		DeleteConnectionPort(i);
			// 		i--;
			// 	}
			// 	GUILayout.EndHorizontal();
			// }
		}
		
		// public override bool Calculate () 
		// {
		// 	for (int i = 0; i < labels.Count; i++) {
		// 		inputKnobs[i]
		// 	}
		// 	return true;
		// }
	}
}