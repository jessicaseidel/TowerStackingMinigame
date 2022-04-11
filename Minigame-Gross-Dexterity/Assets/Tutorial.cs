using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

	private Vector3 destination, start, rotdest;
	private GameObject dexmoHand, cube1, cube2, graspCube;
	private float rotdestx, startrotdestx;
	
	public SinkFundament fundament;
	public List<GameObject> PrefabObjects;
	public RadialProgressTimer radialTimer;
	public bool tutover;
	public UserInterface ui;

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.Find("tutorial_hand");
		if (go != null) {
			Transform[] trans = GameObject.Find("tutorial_hand").GetComponentsInChildren<Transform>(true);
			foreach (Transform t in trans) {
				if (t.gameObject.name == "female_hand_left") {
					dexmoHand = t.gameObject;
				}
			}
			start = dexmoHand.transform.position; 
			destination = dexmoHand.transform.position; 
			rotdest =  dexmoHand.transform.eulerAngles;
			rotdestx = dexmoHand.transform.rotation.x;
			startrotdestx = dexmoHand.transform.rotation.x;

			tutover = false;
		}	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 deltaMove = new Vector3(0, 0, 0);

        // move hand to the next position
		if (dexmoHand != null) {
			if (dexmoHand.transform.position != destination) {
				float delta = 0.35f * Time.deltaTime;
				Vector3 currentPosition = dexmoHand.transform.position;
				Vector3 nextPosition = Vector3.MoveTowards(currentPosition, destination, delta);
				deltaMove = dexmoHand.transform.position - nextPosition;
				dexmoHand.transform.position = nextPosition;
			}

			// grasp cube follows hand position
			if (graspCube != null && graspCube.GetComponent<Rigidbody>().useGravity == false) {
				graspCube.transform.position -= deltaMove; 
			}
				
			// rotate Hand
			if (dexmoHand.transform.rotation.x != rotdestx) {
				Vector3 vec = new Vector3(1, 0, 0);
				dexmoHand.transform.rotation = Quaternion.Lerp(dexmoHand.transform.rotation, Quaternion.Euler(rotdestx, 0, 0), 1.5f * Time.deltaTime);
			} 

			if (radialTimer.timeover == true) {
				ui.ChangeGameStatusText("Leider verloren");
				radialTimer.timerrunning = false;	
				radialTimer.timer = 0;
			}
		}		
	}


	IEnumerator RunTutorial() {
		// show shadow hand in game
		if (dexmoHand != null) {
			dexmoHand.SetActive(true);
		}

//____________________________ Example LVL WIN_______________________________________________________________//
		radialTimer.playtime = 60;
		radialTimer.timer = 0;
		ui.ChangeText("2");

		// spawn first cube
		cube1 = (GameObject)Instantiate(PrefabObjects[0], new Vector3(0.75f, 0.589f, -1.561f), Quaternion.identity);
		cube1.transform.Rotate (270f, 0f, 0f);
		graspCube = cube1;

		// grasp first cube
		destination =  new Vector3(0.869f, 0.5144f, -1.5858f);
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));
		graspCube.GetComponent<Rigidbody>().useGravity = false;

		// bring first cube to fundament
		destination =  new Vector3(0.9536f, 0.6f, -1.6272f);
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));
		destination =  new Vector3(1.051f, 0.535f, -1.607f);
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));

		// stack first cube
		yield return new WaitForSeconds(0.5f);
		graspCube.GetComponent<Rigidbody>().useGravity = true;
		ui.ChangeText("1");

		// hand  away from cube an back to start
		destination =  new Vector3(1.08f, 0.6f, -1.607f);
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));
		destination = start;

		// spawn second cube
		cube2 = (GameObject)Instantiate(PrefabObjects[1], new Vector3(0.75f, 0.589f, -1.561f), Quaternion.identity);
		cube2.transform.Rotate (270f, 90f, 0f);
		graspCube = cube2;

		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));

		// grasp second cube
		destination =  new Vector3(0.869f, 0.5144f, -1.5858f);
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));
		graspCube.GetComponent<Rigidbody>().useGravity = false;

		// bring second cube to fundament
		destination =  new Vector3(0.9536f, 0.6f, -1.6272f);
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));
		destination =  new Vector3(1.035f, 0.65f, -1.607f);
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));

		// stack second cube
		yield return new WaitForSeconds(0.5f);
		graspCube.GetComponent<Rigidbody>().useGravity = true;
		ui.ChangeText("0");
		ui.ChangeGameStatusText("Gewonnen!");
		radialTimer.timerrunning = false;

		// hand  away from cube an back to start
		destination =  new Vector3(1.08f, 0.6f, -1.607f);
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));
		destination = start;
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));

		yield return new WaitForSeconds(1); 

//____________________________ Example LVL Lost, Tower collapsed ____________________________________________//

		// prepare lvl for collapse, set tower, set time etc
		DeleteCubes();
		ui.ChangeGameStatusText("");
		radialTimer.playtime = 50;
		radialTimer.timer = 0;
		radialTimer.timerrunning = true;
		ui.ChangeText("2");
		fundament.setfundament(0.12f);

		GameObject a = (GameObject)Instantiate(PrefabObjects[0], new Vector3(0.95f, 0.43f, -1.607f), Quaternion.identity);
		a.transform.Rotate (270f, 0f, 0f);
		GameObject b = (GameObject)Instantiate(PrefabObjects[2], new Vector3(0.973f, 0.51f, -1.607f), Quaternion.identity);
		b.transform.Rotate (270f, 0f, 0f);
		GameObject c = (GameObject)Instantiate(PrefabObjects[0], new Vector3(0.955f, 0.6f, -1.607f), Quaternion.identity);
		c.transform.Rotate (270f, 0f, 0f);

		// spawn first cube
		cube1 = (GameObject)Instantiate(PrefabObjects[0], new Vector3(0.75f, 0.589f, -1.561f), Quaternion.identity);
		cube1.transform.Rotate (270f, 0f, 0f);
		graspCube = cube1;

		// grasp first cube
		destination =  new Vector3(0.869f, 0.5144f, -1.5858f);
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));
		graspCube.GetComponent<Rigidbody>().useGravity = false;

		// bring first cube to fundament
		destination =  new Vector3(1.151f, 0.535f, -1.607f);
		yield return new WaitUntil(() => (dexmoHand.transform.position == destination));
		graspCube.GetComponent<Rigidbody>().useGravity = true;

		ui.ChangeGameStatusText("Leider verloren");
		radialTimer.timerrunning = false;

		yield return new WaitForSeconds(1); 

//____________________________ Example LVL Lost, run out of time ____________________________________________//

		// prepare lvl for collapse, set tower, set time etc
		DeleteCubes();

		ui.ChangeGameStatusText("");
		radialTimer.playtime = 60;
		radialTimer.timer = 47;
		radialTimer.timerrunning = true;
		ui.ChangeText("2");
		fundament.setfundament(-0.04f);

		a = (GameObject)Instantiate(PrefabObjects[0], new Vector3(0.95f, 0.51f, -1.607f), Quaternion.identity);
		a.transform.Rotate (270f, 0f, 0f);
		b = (GameObject)Instantiate(PrefabObjects[2], new Vector3(0.95f, 0.6f, -1.607f), Quaternion.identity);
		b.transform.Rotate (270f, 0f, 0f);

		// spawn first cube
		cube1 = (GameObject)Instantiate(PrefabObjects[0], new Vector3(0.75f, 0.589f, -1.561f), Quaternion.identity);
		cube1.transform.Rotate (270f, 0f, 0f);
		graspCube = cube1;
		
		yield return new WaitForSeconds(15); 
		tutover = true;
	}


	public void StartTutorial() {
		dexmoHand.transform.position = start;
		StartCoroutine(RunTutorial());
	 	RunTutorial();		
	}

	public void DeleteCubes() {
		GameObject[] cubes = GameObject.FindGameObjectsWithTag("tangible");
		foreach(GameObject go in cubes) {
        	Destroy(go);
    	}
	}


	// Delete old level prefab  in scene
	public void DeleteTutorial() {
    	dexmoHand.SetActive(false);
	}
}
