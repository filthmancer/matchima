using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class RoomEditor : EditorWindow {

	[MenuItem("Tools/Room Editor")]
	static void Init()
	{
		RoomEditor window = (RoomEditor)EditorWindow.GetWindow(typeof(RoomEditor));
		window.Show();
	}

	int tab = 0;

	void OnGUI()
	{
		//if(GUI.changed) CheckInfo();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Room Editor", EditorStyles.boldLabel);

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		tab = GUILayout.Toolbar(tab, new string[] {"Basic", "Ignores"});

		//if(TargetInfo == null) return;

		if(tab == 0)
		{
			EditorGUILayout.BeginHorizontal();
			TargetInfo.Name = EditorGUILayout.TextField("Room Name: ", TargetInfo.Name, GUILayout.Width(400));
			GUILayout.Label("Room Index: " + TargetInfo.Index);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();

			IntVector size = new IntVector(TargetInfo.Size);
			GUILayout.Label("Size:", GUILayout.Width(70));
			size.x = EditorGUILayout.IntSlider(size.x, 0, 20, GUILayout.Width(200));
			size.y = EditorGUILayout.IntSlider(size.y, 0, 20, GUILayout.Width(200));
			if(size.x != TargetInfo.Size.x || size.y != TargetInfo.Size.y)
			{
				TargetInfo.Size = new IntVector(size);
			}

			EditorGUILayout.EndHorizontal();
		}
		else if(tab == 1)
		{
			EditorGUILayout.BeginHorizontal();
			for(int i = 0; i < TargetInfo.GenusToIgnore.Length; i++)
			{
				TargetInfo.GenusToIgnore[i].Active = EditorGUILayout.ToggleLeft(TargetInfo.GenusToIgnore[i].Name, TargetInfo.GenusToIgnore[i].Active, GUILayout.Width(50));
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.EndHorizontal();
		}
		

		GUILayout.FlexibleSpace();
		
		GUILayout.Label("Room Options", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();

		GUI.color = Color.Lerp(Color.green, Color.white, 0.5F);
		if(GUILayout.Button("Create", GUILayout.Width(170), GUILayout.Height(40)))
		{
			CreateRoom();
		}
		GUI.color = Color.Lerp(Color.blue, Color.white, 0.5F);
		if(GUILayout.Button("Test", GUILayout.Width(170), GUILayout.Height(40)))
		{
			TestRoom();
		}
		GUI.color = Color.Lerp(Color.yellow, Color.white, 0.5F);
		if(GUILayout.Button("Save", GUILayout.Width(170), GUILayout.Height(40)))
		{
			SaveRoom();
		}
		GUI.color = Color.Lerp(Color.red, Color.white, 0.5F);
		if(GUILayout.Button("Delete", GUILayout.Width(170), GUILayout.Height(40)))
		{
			DestroyRoom();
		}
		GUI.color = Color.white;
		EditorGUILayout.EndHorizontal();
	}


	GameObject RoomObj;
	void OnEnable()
	{
		RoomObj = (GameObject) Resources.Load("RoomObj");
		if(TargetInfo == null) TargetInfo = new RoomInfo();
	}

	GridInfo TargetRoom;
	RoomInfo TargetInfo;
	public void CreateRoom()
	{
		DestroyRoom();
		CheckInfo();
		//if(TargetInfo == null) TargetInfo = new RoomInfo();
		TargetRoom = Instantiate(RoomObj).GetComponent<GridInfo>();
		TargetRoom.Setup(TargetInfo);
		TargetInfo = new RoomInfo();
		TargetInfo.Size = new IntVector(6,6);
		TargetRoom.transform.position = new Vector3(26, 0,0);

		GameObject [] select = new GameObject[1];
		select[0] = TargetRoom.transform.gameObject;
		Selection.objects = select;
	}

	public void TestRoom()
	{

	}


	public void SaveRoom()
	{

	}

	public void DestroyRoom()
	{
		if(TargetRoom != null) DestroyImmediate(TargetRoom.gameObject);
	}


	public void CheckInfo()
	{

		if(TargetRoom != null && TargetInfo != null)
		{
			TargetRoom.CompareInfo(TargetInfo);
		}
	}
}

[CustomEditor(typeof(GridInfo))]
public class GridInfoEditor : Editor
{
	int newspawns = 0;
	bool lockinspector;
	IntVector size;
	public override void OnInspectorGUI()
	{
		GridInfo grid = target as GridInfo;
		RoomInfo Info = grid.Info;

		var indent = EditorGUI.indentLevel;

		
		serializedObject.Update();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Name:", GUILayout.Width(70));
		EditorGUI.indentLevel = 0;
		Info.Name = EditorGUILayout.TextField("", Info.Name, GUILayout.Width(150));
		GUILayout.Label("Index: " + Info.Index, GUILayout.Width(100));
		lockinspector = EditorGUILayout.ToggleLeft("Lock Inspector", lockinspector);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		size = new IntVector(Info.Size);
		GUILayout.Label("Size:", GUILayout.Width(70));
		size.x = EditorGUILayout.IntSlider(size.x, 0, 20, GUILayout.Width(200));
		GUILayout.Label("", GUILayout.Width(70));
		size.y = EditorGUILayout.IntSlider(size.y, 0, 20, GUILayout.Width(200));
		if(size.x != Info.Size.x || size.y != Info.Size.y)
		{
			grid.ChangeGridSizeTo(size);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		grid.GridPointObj = (GridPoint) EditorGUILayout.ObjectField("Grid Point Obj: ", grid.GridPointObj, typeof(GridPoint), true);

		EditorGUI.indentLevel = indent;

		SerializedProperty inf = serializedObject.FindProperty("Info");
		EditorGUILayout.PropertyField(inf, true);

		GUI.color = Color.grey;
		Rect cr = EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(200));
		GUI.color = Color.white;

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Target Point - " + (CurrentPoint != null ? CurrentPoint.num.x + ":" + CurrentPoint.num.y : ""), EditorStyles.boldLabel, GUILayout.Width(150));

		if(CurrentPoint != null) 
		{
			EditorGUILayout.Vector3Field("", CurrentPoint.Pos, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			CurrentPoint.Empty = EditorGUILayout.ToggleLeft("Empty", CurrentPoint.Empty, GUILayout.Width(100));
			CurrentPoint.RoomInfluencedGenus = EditorGUILayout.ToggleLeft("Room Inf. Genus", CurrentPoint.RoomInfluencedGenus, GUILayout.Width(100));
			GUILayout.Label("Genus Override", GUILayout.Width(100));
			CurrentPoint.GenusOverride = (GENUS) EditorGUILayout.EnumPopup(CurrentPoint.GenusOverride, GUILayout.Width(100));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Tile: ", GUILayout.Width(100));
			EditorGUILayout.ObjectField(CurrentPoint._Tile, typeof(Tile), true, GUILayout.Width(150));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			int startspawns = CurrentPoint.HasStartSpawns() ? CurrentPoint.StartSpawns.Length : 0;
			GUILayout.Label("Start Spawns", GUILayout.Width(100));
			newspawns = EditorGUILayout.IntField(newspawns, GUILayout.Width(150));
			if(startspawns != newspawns && newspawns >= 0)
			{
				TileShortInfo [] old = CurrentPoint.StartSpawns;

				CurrentPoint.StartSpawns = new TileShortInfo[newspawns];
				for(int i = 0 ; i < newspawns; i++)
				{
					if(old != null && old.Length > i) CurrentPoint.StartSpawns[i] = old[i];
					else CurrentPoint.StartSpawns[i] = new TileShortInfo();
				}
				return;
			}
			EditorGUILayout.EndHorizontal();

			if(CurrentPoint.HasStartSpawns())
			{
				EditorGUI.indentLevel = indent + 2;
				
				for(int i = 0; i < CurrentPoint.StartSpawns.Length; i++)
				{
					TileShortInfo ss = CurrentPoint.StartSpawns[0];
					
					//ss.OnGUI();

					/*EditorGUILayout.BeginHorizontal();
					GUILayout.Label("Start ", GUILayout.Width(70));
					ss._Type = EditorGUILayout.TextField("", ss._Type, GUILayout.Width(150));
					ss._Genus = (GENUS) EditorGUILayout.EnumPopup(ss._Genus, GUILayout.Width(100));

					Vector2 value = ss._Value.ToVector2;
					GUILayout.Label("Value:", GUILayout.Width(70));
					value = EditorGUILayout.Vector2Field("", value, GUILayout.Width(200));
					ss._Value = new IntVector(value);

					EditorGUILayout.EndHorizontal();*/
				}
			}
			EditorGUI.indentLevel = indent;	
		}
		else EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();

		GUILayout.FlexibleSpace();
		EditorGUILayout.BeginHorizontal();
		GUI.color = Color.Lerp(Color.blue, Color.white, 0.5F);
		if(GUILayout.Button("Test", GUILayout.Width(170), GUILayout.Height(40)))
		{
			TestRoom();
		}
		GUI.color = Color.Lerp(Color.yellow, Color.white, 0.5F);
		if(GUILayout.Button("Save", GUILayout.Width(170), GUILayout.Height(40)))
		{
			SaveRoom();
		}
		EditorGUILayout.EndHorizontal();


		if(GUI.changed)
		{
			for(int i = 0; i < SelectedPoints.Count; i++)
			{
				SelectedPoints[i].SetInfo(CurrentPoint);
			}
		}
	//	DrawDefaultInspector();
	}

	GridPoint CurrentPoint;
	//SerializedObject CurrentPoint_Obj;
	public void OnSceneGUI()
	{
		GridInfo grid = target as GridInfo;
		Transform trans = grid.transform;

		if(grid == null) return;
		int tx = 0, ty = 0;
		bool targeted = false;
		for(int x = 0; x < grid.Size[0]; x++)
		{
			for(int y = 0; y < grid.Size[1]; y++)
			{
				Vector3 gridpoint = grid[x,y].Pos;
				Color targ = Color.Lerp(Color.blue, Color.white, 0.2F);
				if(grid[x,y] == CurrentPoint || SelectedPoints.Contains(grid[x,y])) targ =  Color.Lerp(Color.green, Color.white, 0.2F);
				else if(grid[x,y].Empty) targ =  Color.Lerp(Color.black, Color.white, 0.05F);
				else if(grid[x,y].HasStartSpawns()) targ = Color.Lerp(Color.red, Color.white, 0.2F);

				Handles.color = targ;

				if(Handles.Button(gridpoint, Quaternion.identity, 0.4F, 0.4F, Handles.DotCap))
				{
					targeted = true;
					tx = x;
					ty = y;
					break;
				}
			}
		}

		if(targeted)
		{
			targeted = false;
			Debug.Log(group_select);
			if(!group_select) 
			{
				SelectedPoints = new List<GridPoint>();
			}
			CurrentPoint = grid[tx,ty];
			SelectedPoints.Add(grid[tx,ty]);
			newspawns = CurrentPoint.HasStartSpawns() ? CurrentPoint.StartSpawns.Length : 0;
			Repaint();
		}	

		if(lockinspector)
		{
			if(Selection.objects.Length == 0 || Selection.objects[0] != trans.gameObject)
			{
				GameObject [] sel = new GameObject[1];
				sel[0] = trans.gameObject;
				Selection.objects = sel;
				CurrentPoint = null;
			}
		}	

		Event e = Event.current;
		switch (e.type)
		 {
		     case EventType.keyDown:
		     {
		         if (Event.current.keyCode == (KeyCode.Q))
		         {
		             if(CurrentPoint != null) CurrentPoint.Empty = !CurrentPoint.Empty;
		         }
		         else if(Event.current.keyCode == (KeyCode.LeftControl)) group_select = true;
		         break;
		     }
		     case EventType.keyUp:
		     if(Event.current.keyCode == (KeyCode.LeftControl)) group_select = false;
		     break;
		 }
	}
	bool group_select = false;
	List<GridPoint> SelectedPoints = new List<GridPoint>();

	public void OnEnable()
	{
		hideFlags = HideFlags.HideAndDontSave;
	}

	public void TestRoom()
	{

	}

	public void SaveRoom()
	{
		CreatePrefab(target as GridInfo);
	}

	static void CreatePrefab (GridInfo r)
	{
		GameObject obj = r.transform.gameObject;
		string name = r.Info.Name;
	
		Object prefab = EditorUtility.CreateEmptyPrefab("Assets/Resources/rooms/" + name + ".prefab");
		EditorUtility.ReplacePrefab(obj, prefab);
		AssetDatabase.Refresh();
	}
}

