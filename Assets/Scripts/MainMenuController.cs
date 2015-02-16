using UnityEngine;
using System.Collections;
using GameFacilities;

public class MainMenuController : MonoBehaviour {

    private GUIStyle style;
    void OnGUI()
    {
        if (SharedControllerGame.Shared.AlreadyPlayed)
        {
            style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 20;
            style.fontStyle = FontStyle.Bold;

            GUI.Label(new Rect(625, 420, 320, 60), 
                      new GUIContent("Last score: " + SharedControllerGame.Shared.LastScore.ToString()),
                      style);
        }
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    public void LoadGameScene()
    {
        Application.LoadLevel("Game");
    }
}
