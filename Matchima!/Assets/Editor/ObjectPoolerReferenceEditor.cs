using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ObjectPoolerReference))]
public class ObjectPoolerReferenceEditor : Editor
{
	private SerializedProperty Pool;

	private void OnEnable ()
	{
		Pool = serializedObject.FindProperty("Pool");
	}

	public override void OnInspectorGUI ()
	{
		GameObject go = Selection.activeGameObject;
		if(go)
		{
			EditorGUILayout.HelpBox("This is a pooled object.", MessageType.None);
			EditorGUILayout.HelpBox("Pool: " + Pool, MessageType.None);
			return;
		}
	}
}