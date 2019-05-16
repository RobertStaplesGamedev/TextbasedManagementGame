using UnityEngine;
using NodeEditorFramework.Utilities;

namespace NodeEditorFramework.Standard
{
	[Node (false, "Example/Example Node Copy")]
	public class ExampleNodeCopy : Node 
	{
		public const string ID = "exampleNodeCopy";
		public override string GetID { get { return ID; } }

		public override string Title { get { return "Example Node Copy"; } }
		public override Vector2 DefaultSize { get { return new Vector2 (150, 60); } }
		
		private string exampleText = ""; 

		[ValueConnectionKnob("Dependancies", Direction.In, "Float")]
		public ValueConnectionKnob inputKnob;
		[ValueConnectionKnob("Output", Direction.Out, "Float")]
		public ValueConnectionKnob outputKnob;
		
		public override void NodeGUI () 
		{
			exampleText = RTEditorGUI.TextField(GUIContent.none, exampleText);

			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();

			inputKnob.DisplayLayout ();

			GUILayout.EndVertical ();
			GUILayout.BeginVertical ();
			
			outputKnob.DisplayLayout ();
			
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
			
		}
		
		public override bool Calculate () 
		{
			outputKnob.SetValue<float> (inputKnob.GetValue<float> () * 5);
			return true;
		}
	}
}