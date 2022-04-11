using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public List<GameObject> PrefabObjects;
	public float userlevel;
	public SinkFundament fundament;
	public RadialProgressTimer radialTimer;
	public Tutorial tut;
	public UserInterface ui;
	//public HandleTangibleObj tanobj;
    public NewTutorial newTut;

	private int i;
	private int cubeRange;
	private int missingCubes;
	private GameObject actualCube;
	private bool gamerunning, tutrunning;
	private List<GameObject> goList;
	private bool nextcube;
	private Vector3 buildposition;
	private Vector3 spawnposition;

	void Start () {
		
		spawnposition = GameObject.Find("Spawnposition").transform.position;
		buildposition = fundament.transform.position;
		goList = new List<GameObject>();
		userlevel = 0; 
		i = -1;
		SelectDiffiulty();
		NextLevel();
		nextcube = false;
		//tanobj.setObjects(PrefabObjects);
		//fundament.settanobj(tanobj);
	}

	
	void Update () {
		// start tutorial when "T" is pressed on keyboard, TODO change start of tutorial to ingame content?
		if (Input.GetKeyDown(KeyCode.T)) {
            DeleteOldLevel();
            gamerunning = false;
            newTut.StartTutorial();
            tutrunning = true;
        }
        // Continue game with key "C" or after tutorial has finished
        // decrease user level, otherwise it will inncrease even if user doesnt succesfully completed
        if (Input.GetKeyDown(KeyCode.C) || (tutrunning == true && newTut.tutover == true)) {
        	tutrunning = false;
        	userlevel--;
            NextLevel();
        }
        // end game if timer run out of time
        if (radialTimer.timeover == true && tutrunning == false) {
        	gamerunning = false;
    		radialTimer.timer = 0;
    		GameLost();	
        }

        // spawn cube when fundament is on right position
    	if (nextcube == true && fundament.issinking == false) {
    		SpawnCube();
    		nextcube = false;
    	}

        // check cube position and rotation when game is active
    	if (gamerunning) {
			CheckCubePosition();
			CheckOldCubes();
    	}

	}


	IEnumerator waitfornextLevel() {
		yield return new WaitForSeconds(3);
		NextLevel();
	}


	// Load next level
	public void NextLevel() {
		radialTimer.timer = 0;
		i = -1;

        SelectDiffiulty();
        DeleteOldLevel();
		fundament.movetostart();
		ui.ChangeText(missingCubes.ToString());
		ui.ChangeGameStatusText("");
		gamerunning = true;
		radialTimer.timerrunning = true;

		SpawnCube();
		// increase userlevel when level successfully completed
		userlevel++; 
	}

    private void SpawnCube() {
		GameObject lastCube = null;			
		if (actualCube != null) {
			lastCube = actualCube;
		}
		i++;
		
		// spawn roof as last cube 
		//if (missingCubes == 1) {
			//actualCube = (GameObject)Instantiate(PrefabObjects[0], spawnposition, Quaternion.identity);
			//actualCube.transform.Rotate (270f, 0f, 0f);
		//} 
		// dont spawn bow if last cube was small tower, this isnt stackable
		//else
         if (lastCube != null && lastCube.name.Contains("Small")) {
			actualCube = (GameObject)Instantiate(PrefabObjects[Random.Range(1, 4)], spawnposition, Quaternion.identity);
			actualCube.transform.Rotate (270f, 0f, 0f);
		}
        // spawn random cube 
		else { 
			actualCube = (GameObject)Instantiate(PrefabObjects[Random.Range(1, cubeRange)], spawnposition, Quaternion.identity);
			actualCube.transform.Rotate (270f, 0f, 0f);
		}
		actualCube.name = actualCube.name.Replace("(Clone)", " " + i);
	} 


	private void CheckCubePosition() {
        // cube is in right position, rotation and isnt moving anymore
        if (fundament.issinking == false) {
			if(actualCube.GetComponent<Rigidbody>().useGravity == true && rightRotation() && rightPosition() 
				&& actualCube.GetComponent<Rigidbody>().velocity ==  Vector3.zero
				&& !Input.GetMouseButton(0)) {
				missingCubes--;
				if (missingCubes == 0) {
					gamerunning = false;
					radialTimer.timerrunning = false;
					ui.ChangeText(missingCubes.ToString());
					ui.ChangeGameStatusText("Gewonnen!");
					StartCoroutine(waitfornextLevel());	
				}
				else {
					goList.Add(actualCube);
					ui.ChangeText(missingCubes.ToString());
					fundament.sink(goList);
					nextcube = true;
				}
			}	
		}
	}


	// checks if actual object is stacked on top of tower 
	private bool rightPosition(){
		Vector3 dest = buildposition + new Vector3(0.0f, 0.14f, 0.0f);		
		
		Vector3 pos =  actualCube.transform.position;
		if(pos.x > dest.x - 0.07f  && pos.x < dest.x + 0.07f && 
			pos.z > dest.z - 0.07f && pos.z < dest.z + 0.07f &&			
			pos.y > (dest.y - 0.02f + i * 0.04f ) && pos.y < (dest.y + 0.02f + i * 0.04f))	{
				return true;
		}
		else {
			return false;
		}
	}


	// checks if actual object is stacked with its right rotation 
	private bool rightRotation() {
		if (actualCube.name.Contains("Tower")) {
			// tower has to sides to put it on
			if (actualCube.transform.rotation.eulerAngles.x < 290 && actualCube.transform.rotation.eulerAngles.x > 250
				|| actualCube.transform.rotation.eulerAngles.x < 110 && actualCube.transform.rotation.eulerAngles.x > 70) {
				return true;
			}
			else return false;
		}
		// roof and bow have one side to put it on
		else if (actualCube.name.Contains("Roof") || actualCube.name.Contains("Bow")) {
			if (actualCube.transform.rotation.eulerAngles.x < 290 && actualCube.transform.rotation.eulerAngles.x > 250) {
				return true;
			} else return false;
		}
		else return true;
	}


	private void CheckOldCubes() {
		Vector3 dest = buildposition + new Vector3(0.0f, 0.14f, 0.0f);
		if (fundament.issinking == false) {
			for (var obj = 0; obj < goList.Count; obj++) {
            	Vector3 pos =  goList[obj].transform.position;
            	if(pos.y > (dest.y - 0.02f + obj * 0.08f  - i*0.04f) && pos.y < (dest.y + 0.02f + obj * 0.08f - i*0.04f)) {

				} else {
					GameLost();
					//Debug.Log(obj);
					//Debug.Log(i);
				}
        	}
		}		
	}


	// end game if it is lost, show lost text and start level new
	private void GameLost() {
		userlevel--;
		gamerunning = false;
		radialTimer.timerrunning = false;
		ui.ChangeGameStatusText("Leider verloren");
		StartCoroutine(waitfornextLevel());
	}


	// set difficulty
	// playtime is time to build the tower
	// cube Range is allowed Prefab Objects to spawn in level
	// missing cubes are number of cubes to stack
	private void SelectDiffiulty() {
		if(userlevel == 0) {
			radialTimer.playtime = 60;
			missingCubes = 2;
			cubeRange = 3;
		}
		else if(userlevel == 1) {
			radialTimer.playtime = 70;
			missingCubes = 3;
			cubeRange = 4;
		}
		else if(userlevel == 2) {
			radialTimer.playtime = 80;
			missingCubes = 4;
			cubeRange = 5;
		}
		else if(userlevel >= 2) {
			radialTimer.playtime = 90;
			missingCubes = 5;
			cubeRange = 6;
		} 
	}


	// Delete old level in Scene
	private void DeleteOldLevel() {
		goList.Clear();
		GameObject[] cubes;
		cubes = GameObject.FindGameObjectsWithTag("tangible");
		foreach(GameObject go in cubes) {
        	Destroy(go);
    	}
	}
}
