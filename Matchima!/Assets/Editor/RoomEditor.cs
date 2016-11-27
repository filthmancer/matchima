using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class RoomEditor : EditorWindow {

	[MenuItem("Tools/Room Editor")]
	static void Init()
	{
		window = (RoomEditor)EditorWindow.GetWindow(typeof(RoomEditor));
		window.Show();
	}

	int tab = 0;
	static RoomEditor window;
	public Camera m_Cam;
	private Rect sceneRect, editorRect;
	private GameData Data;

	void OnGUI()	
	{
		if(Data == null)
		{
			Data = GameObject.Find("GameManager").GetComponent<GameData>();
		}
		if(TargetRoom == null && Selection.objects.Length > 0)
		{
			ClearPoints();
			for (int i = 0; i < Selection.objects.Length; i++)
			{
				if(!Selection.objects[i] is GameObject) continue;
				GridInfo inf = (Selection.objects[i] as GameObject).GetComponent<GridInfo>();
				if(inf != null)
				{
					TargetRoom = inf;
					break;
				}
			}
		}
		if(m_Cam == null)
		{
			m_Cam = GameObject.Find("Room Editor Cam").GetComponent<Camera>();//new GameObject("Room Editor Cam").AddComponent<Camera>();
			m_Cam.hideFlags = HideFlags.None;
			//m_Cam.GetComponent("FlareLayer").enabled = false;
		} 
		

		if(Event.current.type == EventType.Repaint)
		{
			if(window == null) window = (RoomEditor)EditorWindow.GetWindow(typeof(RoomEditor));
			else editorRect = window.position;
		}

		EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Room Editor", EditorStyles.boldLabel);	
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			tab = GUILayout.Toolbar(tab, new string[] {"Basic", "Influences", "Tile"});
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(editorRect.width/2));
			TabMenu(tab);
			EditorGUILayout.EndVertical();

			GUILayout.FlexibleSpace();
			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(editorRect.width/2));
			VisualMenu();
			EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		
		if(CurrentPoint != null) tab = 2;
	}

	void TabMenu(int t)
	{
		int indent = EditorGUI.indentLevel;
		if(t == 0)
		{
			ClearPoints();
			if(TargetRoom == null)
			{
				EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(editorRect.width/2));
					TargetInfo.Name = EditorGUILayout.TextField("Name: ", TargetInfo.Name);
					GUILayout.Label("Index: " + TargetInfo.Index);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(editorRect.width/2));
					IntVector size = new IntVector(TargetInfo.Size);
					GUILayout.Label("Size:", GUILayout.Width(50));
					size.x = EditorGUILayout.IntSlider(size.x, 0, 20);
					size.y = EditorGUILayout.IntSlider(size.y, 0, 20);
					if(size.x != TargetInfo.Size.x || size.y != TargetInfo.Size.y)
					{
						TargetInfo.Size = new IntVector(size);
					}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			else 
			{
				EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(editorRect.width/2));
					TargetRoom.Info.Name = EditorGUILayout.TextField("Name: ", TargetRoom.Info.Name);
					GUILayout.Label("Index: " + TargetRoom.Info.Index);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(editorRect.width/2));
					IntVector size = new IntVector(TargetRoom.Info.Size);
					GUILayout.Label("Size:", GUILayout.Width(50));
					size.x = EditorGUILayout.IntSlider(size.x, 0, 20);
					size.y = EditorGUILayout.IntSlider(size.y, 0, 20);
					if(size.x != TargetRoom.Info.Size.x || size.y != TargetRoom.Info.Size.y)
					{
						
						TargetRoom.ChangeGridSizeTo(size);
					}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			
		}
		else if(t == 1)
		{
			ClearPoints();
			EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(editorRect.width/2));
				for(int i = 0; i < TargetInfo.GenusToIgnore.Length; i++)
				{
					TargetInfo.GenusToIgnore[i].Active = EditorGUILayout.ToggleLeft(TargetInfo.GenusToIgnore[i].Name, TargetInfo.GenusToIgnore[i].Active, 	GUILayout.	Width(50));
				}
			EditorGUILayout.EndHorizontal();
		}
		else if(t == 2 && TargetRoom != null)
		{
			if(CurrentPoint == null) CurrentPoint = PrevPoint;
			if(CurrentPoint == null) CurrentPoint = TargetRoom[0,0];
			if(CurrentPoint == null) return;

			EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(editorRect.width/2));
			GUILayout.Label("Point " + (CurrentPoint != null ? CurrentPoint.num.x + ":" + CurrentPoint.num.y : ""), EditorStyles.boldLabel, GUILayout.Width(60));
			GUILayout.Label("Pos " + CurrentPoint.Pos.x + "x." + CurrentPoint.Pos.y + "y", EditorStyles.boldLabel, GUILayout.Width(100));
			GUILayout.Label("Tile ", EditorStyles.boldLabel,  GUILayout.Width(60));
			EditorGUILayout.ObjectField(CurrentPoint._Tile, typeof(Tile), true, GUILayout.Width(100));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			Rect r = EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(editorRect.width/2));

				EditorGUILayout.BeginVertical(GUILayout.MaxWidth(editorRect.width/2));
					bool empty = EditorGUILayout.ToggleLeft("Empty", CurrentPoint.Empty, GUILayout.Width(100));
					bool infgenus =  EditorGUILayout.ToggleLeft("Room Inf. Genus", CurrentPoint.RoomInfluencedGenus, GUILayout.Width(100));
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label("Genus Override", GUILayout.Width(100));
					GENUS overgenus = (GENUS) EditorGUILayout.EnumPopup(CurrentPoint.GenusOverride, GUILayout.Width(100));
					GUILayout.Label("Entry Point", GUILayout.Width(100));
					EntryPoint entry = (EntryPoint) EditorGUILayout.EnumPopup(CurrentPoint.Entry, GUILayout.Width(100));
					bool door =  EditorGUILayout.ToggleLeft("Door", CurrentPoint.Doorway, GUILayout.Width(100));
					EditorGUILayout.EndHorizontal();

					if(SelectedPoints.Count > 0)
					{
						for(int i = 0; i < SelectedPoints.Count; i++)
						{
							SelectedPoints[i].Empty = empty;
							SelectedPoints[i].RoomInfluencedGenus = infgenus;
							SelectedPoints[i].GenusOverride = overgenus;
							SelectedPoints[i].Entry = entry;
							SelectedPoints[i].Doorway = door;
						}
					}
					
				EditorGUILayout.EndVertical();

				EditorGUILayout.Space();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(editorRect.width/2));

				int startspawns = CurrentPoint.HasStartSpawns() ? CurrentPoint.StartSpawns.Length : 0;
				GUILayout.Label("Start Spawns", GUILayout.Width(100));
				newspawns = EditorGUILayout.IntField(newspawns, GUILayout.Width(150));
				if(startspawns != newspawns && newspawns >= 0)
				{
					for(int s = 0; s < SelectedPoints.Count; s++)
					{
						TileShortInfo [] old = (SelectedPoints[s].HasStartSpawns() ? SelectedPoints[s].StartSpawns : null);
						
						SelectedPoints[s].StartSpawns = new TileShortInfo[newspawns];
						for(int i = 0 ; i < newspawns; i++)
						{
							if(old != null && old.Length > i) SelectedPoints[s].StartSpawns[i] = old[i];
							else SelectedPoints[s].StartSpawns[i] = new TileShortInfo();
						}
					}
					
				}
			
			EditorGUILayout.EndHorizontal();

				if(CurrentPoint.HasStartSpawns())
				{
					EditorGUI.indentLevel = 0;
					
					for(int i = 0; i < CurrentPoint.StartSpawns.Length; i++)
					{
						TileShortInfo ss = CurrentPoint.StartSpawns[i];
						if(ss.OnGUI())
						{
							for(int x = 0; x < SelectedPoints.Count; x++)
							{
								if(SelectedPoints[x] == CurrentPoint) continue;
								SelectedPoints[x].StartSpawns[i] = new TileShortInfo(ss);
							}
						}

						
					}
				}
			EditorGUI.indentLevel = indent;	


			EditorGUILayout.Space();
			
		}

		GUILayout.FlexibleSpace();
		GUILayout.Label("Room Options", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();

		GUI.color = Color.Lerp(Color.green, Color.white, 0.5F);
		if(GUILayout.Button("Create", GUILayout.Height(40)))
		{
			CreateRoom();
		}
		if(GUILayout.Button("Save", GUILayout.Height(40)))
		{
			SaveRoom();
		}
		GUI.color = Color.Lerp(Color.blue, Color.white, 0.5F);
		if(GUILayout.Button("Test", GUILayout.Height(40)))
		{
			TestRoom();
		}
		if(GUILayout.Button("Play", GUILayout.Height(40)))
		{
			PlayRoom();
		}
		
		GUI.color = Color.Lerp(Color.red, Color.white, 0.5F);
		if(GUILayout.Button("Delete", GUILayout.Height(40)))
		{
			DestroyRoom();
		}
		GUI.color = Color.white;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
	}

	void VisualMenu()
	{
		float ratio_height = editorRect.height- 150;
		float ratio_width = (ratio_height/16) * 9;

		Rect r = EditorGUILayout.BeginHorizontal(GUILayout.Height(ratio_height));
		float offset_x = r.x + 10;
		float offset_y = r.y;

		if(m_Cam != null)
		{
			float screensize = 0.4F + ratio_height / 5000;
			Rect screen = new Rect(offset_x, offset_y, ratio_width, ratio_height);

				if(Event.current.type == EventType.Repaint)
				{
					sceneRect = screen;
					//m_Cam.pixelRect = sceneRect;
					//m_Cam.Render();
					
				}
				
				bool targeted = false;
				int tx = 0, ty = 0;
				if(TargetRoom != null)
				{
					Handles.SetCamera(m_Cam);
					Handles.DrawCamera(sceneRect, m_Cam, DrawCameraMode.Normal);
					for(int x = 0; x < TargetRoom.Size[0]; x++)
					{
						for(int y = 0; y < TargetRoom.Size[1]; y++)
						{

							GridPoint t = TargetRoom[x,y];
							Vector3 gridpoint = t.Pos;
							Color targ = Color.Lerp(Color.grey, Color.black, 0.4F);

							if(t == CurrentPoint || SelectedPoints.Contains(t)) targ = Color.white;
							else if(t.Empty) targ =  Color.Lerp(Color.black, Color.white, 0.05F);
							else if(t.GenusOverride != GENUS.NONE) 
							{
								targ = Data.GetGENUSColour(t.GenusOverride);
							}
							//else if(t.HasStartSpawns()) targ = Color.Lerp(Color.red, Color.white, 0.2F);

							Vector3 rotation = Vector3.zero;
							float finalhitsize = screensize;
							float finalsize = screensize;
							Handles.color = targ;
							Handles.DrawCapFunction f = Handles.DotCap;
							if(t.HasStartSpawns()) f = Handles.CircleCap;
							else if(t.Doorway)
							{
								f = Handles.ArrowCap;
								finalsize *= 5;
							} 
							else if(t.Entry != EntryPoint.None)
							{
								f = Handles.ConeCap;
								finalsize *= 2;
								switch(t.Entry)
								{
									case EntryPoint.North:
									rotation.x = 90;
									break;
									case EntryPoint.South:
									rotation.x = 270;
									break;
									case EntryPoint.East:
									rotation.y = 270;
									break;
									case EntryPoint.West:
									rotation.y = 90;
									break;
								}
							}

							if(Handles.Button(gridpoint, Quaternion.Euler(rotation), finalsize, finalhitsize, f))
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

						if(SelectedPoints.Contains(TargetRoom[tx,ty]))
						{
							SelectedPoints.Remove(TargetRoom[tx,ty]);
							if(CurrentPoint == TargetRoom[tx,ty])
							{
								if(SelectedPoints.Count > 0) 
								{
									CurrentPoint = SelectedPoints[SelectedPoints.Count-1];
									newspawns = CurrentPoint.HasStartSpawns() ? CurrentPoint.StartSpawns.Length : 0;
								}
								else CurrentPoint = null;
							}
						} 
						else 
						{
							if(!group_select) SelectedPoints.Clear();

							AddPoint(TargetRoom[tx,ty]);
						}

						Repaint();
					}	

					Event e = Event.current;
					switch (e.type)
					 {
					     case EventType.keyDown:
					     {
					         if (Event.current.keyCode == (KeyCode.Q))
					         {
					         	bool setempty = !CurrentPoint.Empty;
					            for(int i = 0; i < SelectedPoints.Count; i++)
					            {
					            	SelectedPoints[i].Empty = setempty;
					            }
					         }
					         else if(Event.current.keyCode == (KeyCode.LeftControl)) group_select = true;
					         break;
					     }
					     case EventType.keyUp:
					     if(Event.current.keyCode == (KeyCode.LeftControl)) group_select = false;
					     break;
					 }
				}				
		}


		EditorGUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
	}

	void AddPoint(GridPoint g)
	{
		CurrentPoint = g;
		SelectedPoints.Add(g);
		newspawns = CurrentPoint.HasStartSpawns() ? CurrentPoint.StartSpawns.Length : 0;
	}

	GameObject RoomObj;
	GameObject GridObj;
	void OnEnable()
	{
		if(RoomObj == null) RoomObj = (GameObject) Resources.Load("RoomObj");
		if(GridObj == null) GridObj = (GameObject) Resources.Load("GridPointObj");
		if(TargetInfo == null) TargetInfo = new RoomInfo();
		hideFlags = HideFlags.HideAndDontSave;
		
	}

	GridInfo TargetRoom;
	RoomInfo TargetInfo;
	public void CreateRoom()
	{
		DestroyRoom();
		CheckInfo();
		//if(TargetInfo == null) TargetInfo = new RoomInfo();
		TargetRoom = Instantiate(RoomObj).GetComponent<GridInfo>();
		TargetRoom.GridPointObj = GridObj.GetComponent<GridPoint>();
		TargetRoom.Setup(TargetInfo);
		TargetInfo = new RoomInfo();
		TargetInfo.Size = new IntVector(6,6);
		TargetRoom.transform.position = m_Cam.transform.position + m_Cam.transform.forward;

		GameObject [] select = new GameObject[1];
		select[0] = TargetRoom.transform.gameObject;
		Selection.objects = select;
	}

	public void PlayRoom()
	{
		Data.TestRoom = TargetRoom;
		TargetRoom.SetActive(false);
		Debug.Log(Data.TestRoom);
		EditorApplication.ExecuteMenuItem("Edit/Play");
	}

	public void TestRoom()
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

	int newspawns = 0;
	bool lockinspector;
	bool group_select = false;
	IntVector size;
	
	private GridPoint CurrentPoint;
	private GridPoint PrevPoint;
	
	private List<GridPoint> SelectedPoints = new List<GridPoint>();

	public void SaveRoom()
	{
		CreatePrefab(TargetRoom);
	}

	static void CreatePrefab (GridInfo r)
	{
		GameObject obj = r.transform.gameObject;
		string name = r.Info.Name;
	
		Object prefab = Resources.Load("rooms/" + name);
		if(prefab == null) prefab = 	EditorUtility.CreateEmptyPrefab("Assets/Resources/rooms/" + name + ".prefab");
		EditorUtility.ReplacePrefab(obj, prefab);
		AssetDatabase.Refresh();
	}

	public void ClearPoints()
	{
		PrevPoint = CurrentPoint;
		CurrentPoint = null;
		SelectedPoints.Clear();
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

		serializedObject.ApplyModifiedProperties();

		GUILayout.FlexibleSpace();

	//	DrawDefaultInspector();
	}

	GridPoint CurrentPoint;
	//SerializedObject CurrentPoint_Obj;
	/*public void OnSceneGUI()
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

	public void PlayRoom()
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
	}*/
}

