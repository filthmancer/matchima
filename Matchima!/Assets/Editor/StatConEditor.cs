using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(StatCon))]
public class StatConEditor : PropertyDrawer {

	public bool showStat = false;
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{

		SerializedProperty Current = property.FindPropertyRelative("Current");
		SerializedProperty Max = property.FindPropertyRelative("Max");
		SerializedProperty Min = property.FindPropertyRelative("Min");
		SerializedProperty Gain = property.FindPropertyRelative("Gain");
		SerializedProperty Leech = property.FindPropertyRelative("Leech");
		SerializedProperty Regen = property.FindPropertyRelative("Regen");
		SerializedProperty Multiplier = property.FindPropertyRelative("Multiplier");
		SerializedProperty ThisTurn = property.FindPropertyRelative("ThisTurn");

		SerializedProperty Level_Current = property.FindPropertyRelative("Level_Current");
		SerializedProperty Level_Required = property.FindPropertyRelative("Level_Required");
		SerializedProperty Level_Multiplier = property.FindPropertyRelative("Level_Multiplier");





		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		Rect l = new Rect(position.x, position.y, 75, 20);
		showStat = EditorGUI.PropertyField(l, property, label); //EditorGUI.Foldout (position, showStat, label);
		
		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		//Top Level Info - Current, Max, This Turn
	
		int fieldsize = 100;
		position.x += fieldsize;
		Rect c = new Rect(position.x, position.y, fieldsize, 20);
		EditorGUI.LabelField(c, new GUIContent("" + Current.intValue + "/" + Max.intValue + "  +" + ThisTurn.floatValue));
		position.x += fieldsize + 20;

		c = new Rect(position.x, position.y, 100, 20);
		if(GUI.Button (c, "Default"))
		{
			//(property.serializedObject.targetObject as StatCon).Reset();
		}
			//SetToDefault(property);

		if(showStat)
		{

			int title_width = 5;
			int value_width = 15;
			int value_offset = 60;

			int title_indent = 1;
			int value_indent = -1;

			
			fieldsize = 150;
			position.y += 25;
			position.x = 0;

			Rect field = position;
			field.height = 20;

			EditorGUI.indentLevel = title_indent;
			field.width = title_width;
			field = EditorGUI.PrefixLabel(field, new GUIContent("Curr"));
			EditorGUI.indentLevel = value_indent;
			field.x -= value_offset;
			field.width = value_width;
			EditorGUI.PropertyField (field, Current, GUIContent.none);
		
			//position.x += fieldsize+20;
			Rect r = new Rect (position.x + fieldsize+20, position.y, 100, 20);

			EditorGUI.indentLevel = title_indent;
			field.x += value_width;
			field.width = title_width;
			field = EditorGUI.PrefixLabel(field, new GUIContent("Min"));
			EditorGUI.indentLevel = value_indent;
			field.x -= value_offset;
			field.width = value_width;
			EditorGUI.PropertyField (field, Min, GUIContent.none);


			EditorGUI.indentLevel = title_indent;
			field.x += value_width;
			field.width = title_width;
			field = EditorGUI.PrefixLabel(field, new GUIContent("Max"));
			EditorGUI.indentLevel = value_indent;
			field.x -= value_offset;
			field.width = value_width;
			EditorGUI.PropertyField (field, Max, GUIContent.none);


			field.y += 20;
			field.x = 0;
			value_offset = 35;

			EditorGUI.indentLevel = title_indent;
			field.x += value_width;
			field.width = title_width;
			field = EditorGUI.PrefixLabel(field, new GUIContent("Gain"));
			EditorGUI.indentLevel = value_indent;
			field.x -= value_offset;
			field.width = value_width;
			EditorGUI.PropertyField (field, Gain, GUIContent.none);

			EditorGUI.indentLevel = title_indent;
			field.x += value_width;
			field.width = title_width;
			field = EditorGUI.PrefixLabel(field, new GUIContent("Mult"));
			EditorGUI.indentLevel = value_indent;
			field.x -= value_offset;
			field.width = value_width;
			EditorGUI.PropertyField (field, Multiplier, GUIContent.none);

			field.y += 20;
			field.x = 0;
			value_offset = 35;

			EditorGUI.indentLevel = title_indent;
			field.x += value_width;
			field.width = title_width;
			field = EditorGUI.PrefixLabel(field, new GUIContent("Leech"));
			EditorGUI.indentLevel = value_indent;
			field.x -= value_offset;
			field.width = value_width;
			EditorGUI.PropertyField (field, Leech, GUIContent.none);

			EditorGUI.indentLevel = title_indent;
			field.x += value_width;
			field.width = title_width;
			field = EditorGUI.PrefixLabel(field, new GUIContent("Regen"));
			EditorGUI.indentLevel = value_indent;
			field.x -= value_offset;
			field.width = value_width;
			EditorGUI.PropertyField (field, Regen, GUIContent.none);

			field.y += 20;
			field.x = 0;

			value_offset = 35;

			EditorGUI.indentLevel = title_indent;
			field.x += value_width;
			field.width = title_width;
			field = EditorGUI.PrefixLabel(field, new GUIContent("Lvl Curr"));
			EditorGUI.indentLevel = value_indent;
			field.x -= value_offset;
			field.width = value_width;
			EditorGUI.PropertyField (field, Level_Current, GUIContent.none);

			EditorGUI.indentLevel = title_indent;
			field.x += value_width;
			field.width = title_width;
			field = EditorGUI.PrefixLabel(field, new GUIContent("Lvl Req"));
			EditorGUI.indentLevel = value_indent;
			field.x -= value_offset;
			field.width = value_width;
			EditorGUI.PropertyField (field, Level_Required, GUIContent.none);

			EditorGUI.indentLevel = title_indent;
			field.x += value_width;
			field.width = title_width;
			field = EditorGUI.PrefixLabel(field, new GUIContent("Lvl Mult"));
			EditorGUI.indentLevel = value_indent;
			field.x -= value_offset;
			field.width = value_width;
			EditorGUI.PropertyField (field, Level_Multiplier, GUIContent.none);


		}
		EditorGUI.indentLevel = indent;

		// Set indent back to what it was
		
		EditorGUI.EndProperty ();
	}

	 public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
     {
         if (showStat)
         {
             return base.GetPropertyHeight(property, label) + 150;
         }
         else
         {
             return base.GetPropertyHeight(property, label);
         }
     }

     public void SetToDefault(SerializedProperty property)
     {
     	//(property.serializedObject.targetObject as StatCon).Reset();
     }

}
