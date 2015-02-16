using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GameFacilities;
using System.Threading;

public class CameraController : MonoBehaviour {

    private bool isMouseDown = false;
    private Vector3 lastTrailPos = new Vector3(0, 0, 0);
    private LineRenderer line;
    private int curId = 0;
    private Vector3 lastPos;

    private Gesture taskLine;
    private Gesture userInputLine;

    private bool firstDraw = true;
    private TrailRenderer trail;
    private Timer timer;
    private float timeLeft;
    private bool failed = false;

    private GameSessionInfo gameInfo;
    private GUIStyle menuStyle;


    public Transform userLineDrawer;
    public Transform userTrailDrawer;
    public GameObject light;

    void OnGUI()
    {
        GUI.Label(new Rect(90, 30, 160, 30), new GUIContent("Score: " + gameInfo.Score.ToString()), menuStyle);

        timeLeft = (float)Math.Round(timeLeft, 1);
        String timeValue = timeLeft.ToString();

        if (!timeValue.Contains("."))
            timeValue = timeValue + ".0";

        GUI.Label(new Rect(200, 30, 160, 30), new GUIContent("Time left: " + timeValue), menuStyle);
    }

    void TimerTick(System.Object state)
    {
        timeLeft -= 0.1f;

        if (timeLeft <= 0)
        {
            failed = true;
        }
    }

	// Use this for initialization
	void Start () 
    {
        menuStyle = new GUIStyle();
        menuStyle.fontSize = 20;
        menuStyle.fontStyle = FontStyle.Bold;
        menuStyle.normal.textColor = Color.white;

        timer = new Timer(TimerTick, null, 1000, 100);
	}


    void Awake()
    {
        gameInfo = new GameSessionInfo();

        Vector3 cameraPos = this.transform.position;

        GameSessionInfo.TaskInfo task = gameInfo.NextTask();
        timeLeft = task.TimeAllowed;

        taskLine = new Gesture(task.Task);
        taskLine.Parent = this.transform.parent;
        taskLine.Position = new Vector3(cameraPos.x - 7, cameraPos.y - 4, 0);

        userInputLine = new Gesture();
        userInputLine.Parent = this.transform.parent;
        userInputLine.Position = new Vector3(cameraPos.x, cameraPos.y, 0);
        userInputLine.Draw = false;

        lastPos = new Vector3();
    }

    void divideVectorToPoints(Vector3 prev, Vector3 cur)
    {
        float dist = Vector3.Distance(prev, cur);
        Vector3 vect = cur - prev;

        float parts = (dist <= 5) ? 1 : (float)Math.Truncate(dist / 2.0);
        for (int i = 1; i <= parts; ++i)
        {
            Vector3 pos = new Vector3(prev.x + i * vect.x / parts, prev.y + i * vect.y / parts, -9);
            line.SetVertexCount(++curId);
            line.SetPosition(curId - 1, pos);
            lastPos = pos;
            pos.z = 0;
            trail.transform.position = pos;

            userInputLine.AddPoint(new Vector2(pos.x - userInputLine.Position.x, pos.y - userInputLine.Position.y));
        }
    }

    void nextTask()
    {
        gameInfo.AddScorePoint();
        GameSessionInfo.TaskInfo task = gameInfo.NextTask();
        timeLeft = task.TimeAllowed;

        taskLine.Dispose();

        Vector3 cameraPos = this.transform.position;
        taskLine = new Gesture(task.Task);
        taskLine.Parent = this.transform.parent;
        taskLine.Position = new Vector3(cameraPos.x - 7, cameraPos.y - 4, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (failed)
            finishLevel();

        if (!isMouseDown)
        {
            isMouseDown = Input.GetMouseButtonDown(0);

            if (isMouseDown)
            {
                if (line != null)
                    Destroy(line.gameObject);
                if (trail != null)
                    Destroy(trail.gameObject);
                curId = 0;

                lastPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                trail = (Instantiate(userTrailDrawer, lastPos, new Quaternion()) as Transform).gameObject.GetComponent<TrailRenderer>();
                line = (Instantiate(userLineDrawer, lastPos, new Quaternion()) as Transform).gameObject.GetComponent<LineRenderer>();

                trail.GetComponent<TrailRenderer>().sortingOrder = 1;
                firstDraw = true;
                
            }
        }

        if (isMouseDown && Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            bool isFits = taskLine.Fits(userInputLine);

            if (isFits)
            {
                nextTask();
            }
            else
            {

            }
            userInputLine.Reset();

            Destroy(line.gameObject);

            line = (Instantiate(userLineDrawer, lastPos, new Quaternion()) as Transform).gameObject.GetComponent<LineRenderer>();
        }

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = -9;

        light.transform.position = new Vector3(pos.x, pos.y, 99);

        if (isMouseDown && Vector3.Distance(pos, lastPos) > 0.5)
        {
            if (firstDraw)
            {
                firstDraw = false;

                line.SetVertexCount(++curId);
                line.SetPosition(curId - 1, pos);
                lastPos = pos;
                pos.z = 0;
                trail.transform.position = pos;

                userInputLine.AddPoint(new Vector2(pos.x - userInputLine.Position.x, pos.y - userInputLine.Position.y));
            }
            else
                divideVectorToPoints(lastPos, pos);
        }
	}

    void OnMouseDrag()
    {
        print("Drag");
    }

    void finishLevel()
    {
        SharedControllerGame.Shared.LastScore = gameInfo.Score;
        Application.LoadLevel("MainMenu");
    }
}
