using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Creature))]
public class CreatureEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		Creature myScript = (Creature)target;

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Eat Blue")) {
			myScript.EatColor(GameColors.Blue);
		}

		if (GUILayout.Button("Eat Red")) {
			myScript.EatColor(GameColors.Red);
		}

		if (GUILayout.Button("Eat Yellow")) {
			myScript.EatColor(GameColors.Yellow);
		}

		EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Clear Colors")) {
			myScript.ClearColors();
		}
	}
}