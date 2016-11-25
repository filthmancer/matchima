using UnityEngine;
using System.Collections;

public class UIObjects2 : MonoBehaviour {

	public CrewMenuUIObj [] _CrewButtons;
	public static CrewMenuUIObj [] CrewButtons{get{return UIManager.instance._CrewButtons;}}

	public UIObj FullVersionAlert, AdAlert, AdAlertMini;
	public static UIMenu Menu;
	public GameObject _Cheats, _Menu, _Main;

	public Tile Tooltip_Target;
	public UIObjtk Tooltip_Parent;
	public UIObj Tooltip_DescObj;
}
