using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class NewTutorial : MonoBehaviour {

    private StreamReader reader;
    private bool record;
    private bool recordstarted;
    private bool play;
    private bool started;
    private int initialcubes;

    private int cubesToStack;
    private string path;
    private int partOfTut;
    private bool stopreplay;

    public GameObject TutorialCube;
    public GameObject TutorialCube2;
    public GameObject TutorialTower;
    public GameObject TutorialRoof;
    public GameObject Fundament;

    public RadialProgressTimer radialTimer;
    public UserInterface ui;
    //public ReplayHandgesture replayHands;
    public bool tutover;
    private bool firstline;

    
    private Vector3 fundamentold;
    private Vector3 fundamentnew;
    private Quaternion fundamentRotOld;
    private Quaternion fundamentRotNew;

    private Vector3 delta;
    private Quaternion deltarot;


    void Start() {
        firstline = true;
        play = false;
        started = false;
        tutover = false;
        partOfTut = 0;
    }

    // Update is called once per frame
    void Update() {
        // cancel tutorial
        if (Input.GetKeyDown(KeyCode.C)) {
            EndTutorial();
        } /*
        else if (Input.GetKeyDown(KeyCode.P)) {
            play = true;
        } */


        if (play && !started) {
            
            //StartReplayWin();
            //StartReplayCollapse();
            //StartReplayTimer();
            //StartTutPart("Assets/RecordedData/Win.txt", 0, 3);
            ReadString();
            GetFloorOffset();
            //replayHands.SetDelta(delta, deltarot, fundamentnew);
            started = true;
        }
        else if (play && started && !tutover) {
            ReadString();
            checkCubes();
            checkTimer();
        }
    }


    void GetFloorOffset() {
        GameObject go = GameObject.Find("floor");

        fundamentnew = go.transform.position;
        delta = fundamentnew - fundamentold;

        fundamentRotNew = go.transform.rotation;
        deltarot = fundamentRotNew * Quaternion.Inverse(fundamentRotOld);
    }

    private void EndTutorial() {
        play = false;
        started = false;
        tutover = true;
        firstline = true;
        SetTutorialCubes();
        reader.Close();
        partOfTut = 0;
    }

    public void StartTutorial() {
        tutover = false;
        play = true;
        // start Hand gestures for tutorial too 
        StartTutPart("Assets/RecordedData/Collapse.txt", 0, 4);
        //replayHands.startHands(partOfTut);
    }

    void StartTutPart(string tutlocation, int timer, int cubes) {
        DeleteOldLevel();
        path = tutlocation;
        reader = new StreamReader(path);
        //started = true;
        play = true;

        // update info layout for tutorial level
        radialTimer.playtime = 60;
        radialTimer.timer = timer;
        radialTimer.timerrunning = true;
        stopreplay = false;
        cubesToStack = cubes;
        initialcubes = cubes;
        ui.ChangeText(cubesToStack.ToString());
    }

    void checkTimer() {
        if (radialTimer.timeover == true) {
            LevelLost(0.0f);
        }
    }

    void checkCubes() {
        Vector3[] pos = {TutorialCube.transform.position, TutorialTower.transform.position,TutorialRoof.transform.position };
        Vector3 cubedest = Fundament.transform.position + new Vector3(0.0f, 0.14f, 0.0f);

        if (cubesToStack != 0 && stopreplay == false) {
            if (pos[initialcubes - cubesToStack].x > cubedest.x - 0.07f && pos[initialcubes - cubesToStack].x < cubedest.x + 0.07f &&
                pos[initialcubes - cubesToStack].z > cubedest.z - 0.07f && pos[initialcubes - cubesToStack].z < cubedest.z + 0.07f &&
                pos[initialcubes - cubesToStack].y > (cubedest.y - 0.02f + (4 - cubesToStack) * 0.04f) && pos[initialcubes - cubesToStack].y < (cubedest.y + 0.02f + 4 - cubesToStack * 0.04f))
            {
                cubesToStack--;
                ui.ChangeText(cubesToStack.ToString());
                if (cubesToStack == 0) {
                    LevelWon();
                }
            }
        }
        if (cubesToStack == 2 && path.Equals("Assets/RecordedData/Collapse.txt") && stopreplay == false) {
            if (pos[1].y < (cubedest.y + 0.02f + 0 * 0.04f)) {
                LevelLost(1.5f);
                stopreplay = true;
            }
        }
    }

    IEnumerator WaitSeconds(string str, float time) {
        yield return new WaitForSeconds(time);
        ui.ChangeText("0");
        ui.ChangeGameStatusText(str);
        radialTimer.timerrunning = false;
        yield return new WaitForSeconds(1.0f);
        ui.ChangeGameStatusText("");

        SetTutorialCubes();

        /*
        if (partOfTut == 1) {
            StartTutPart("Assets/RecordedData/Timer.txt", 49, 3);
            replayHands.startHands(partOfTut);
        }
        else if (partOfTut == 2) {
            StartTutPart("Assets/RecordedData/Win.txt", 0, 3);
            replayHands.startHands(partOfTut);
        }
        else {
            EndTutorial();
        } */
    }

 
    void LevelWon() {
        partOfTut++;
        StartCoroutine(WaitSeconds("Gewonnen!", 1.5f));
    }

    void LevelLost(float time) {
        partOfTut++;
        StartCoroutine(WaitSeconds("Leider verloren", time));
    }

    void ReadString() {
        string line = reader.ReadLine();       
        if (string.IsNullOrWhiteSpace(line)) { 
            record = false;
            play = false;
            started = false;
            reader.Close();
        }

        if (firstline) {
            GetRecorderPosition(line);
            firstline = false;
        }
        else {
            string[] splitArray = line.Split(';');
            Vector3 tmp = new Vector3(Fundament.transform.position.x, float.Parse(splitArray[0]) + delta.y, Fundament.transform.position.z);
            Fundament.transform.position = tmp;

            if (splitArray.Length > 3) {
                SetTransform(splitArray, 1, TutorialCube);
            }
            if (splitArray.Length > 9) {
                SetTransform(splitArray, 8, TutorialTower);
            }
            if (splitArray.Length > 16 && path.Equals("Assets/RecordedData/Collapse.txt")) {
                SetTransform(splitArray, 15, TutorialCube2);
            }
            if (splitArray.Length > 16 && !path.Equals("Assets/RecordedData/Collapse.txt")) {
                SetTransform(splitArray, 15, TutorialRoof);
            }
           /* if (splitArray.Length > 23)
            {
                SetTransform(splitArray, 22, TutorialCube2) ;
            } */
        } 
    }

    private void GetRecorderPosition(string line) {
        string[] splitArray = line.Split(';');

        fundamentold = new Vector3(float.Parse(splitArray[0]), float.Parse(splitArray[1]), float.Parse(splitArray[2]));
        fundamentRotOld = new Quaternion(float.Parse(splitArray[3]), float.Parse(splitArray[4]), float.Parse(splitArray[5]), float.Parse(splitArray[6]));
    }

    private void SetTransform(string[] splitArray, int i, GameObject go) {
        Vector3 pos = new Vector3(float.Parse(splitArray[i]), float.Parse(splitArray[i + 1]), float.Parse(splitArray[i + 2]));
        Quaternion Rot = new Quaternion(float.Parse(splitArray[i + 3]), float.Parse(splitArray[i + 4]), float.Parse(splitArray[i + 5]), float.Parse(splitArray[i + 6]));

        go.transform.position = deltarot * ( pos -fundamentnew) + fundamentnew + deltarot * delta;
        go.transform.rotation = deltarot * Rot;
    }


    private void DeleteOldLevel() {
        GameObject[] cubes;
        cubes = GameObject.FindGameObjectsWithTag("tangible");
        foreach (GameObject go in cubes) {
            Destroy(go);
        }
    }

    void SetTutorialCubes() {
        GameObject[] obj = {TutorialCube, TutorialCube2, TutorialTower, TutorialRoof};
        foreach(GameObject ob in obj) {
            ob.transform.position = new Vector3(0.0f,-1.0f,0.0f);
        }
    }
}
