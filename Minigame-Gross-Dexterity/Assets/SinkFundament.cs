using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkFundament : MonoBehaviour {

	public float speed;
	public bool issinking;
	

	//private HandleTangibleObj tanobj;
	private Vector3 destination, startposition;
	private List<GameObject> sinkingGO;

	// Use this for initialization
	void Start () {
		startposition = transform.position;
		speed = 0.05f;
		destination = transform.position;
		issinking = false;
	}
	
	// Update is called once per frame
	// move fundament with speed variable towards next position every frame
	// if new position is reached, it isnt skining anymore anf cube positions will get checked in levelmanager
	void Update () {
		if (transform.position != destination) {
				float delta = speed * Time.deltaTime;
         		Vector3 currentPosition = transform.position;
         		Vector3 nextPosition = Vector3.MoveTowards(currentPosition, destination, delta);
         		// Move the object to the next position
         		transform.position = nextPosition;
		}
		else {
			if(sinkingGO != null && sinkingGO.Count > 0) {
				setphysictoobjects();
        		sinkingGO.Clear();
			}
			issinking = false;			
		}
	}

	// acitvate Rigidbody and physiks after sinking, disable parenting
	private void setphysictoobjects() {
		for (var obj = 0; obj < sinkingGO.Count; obj++) {
					sinkingGO[obj].transform.parent = null;
           			sinkingGO[obj].AddComponent<Rigidbody>();

           			//tanobj.setMass(sinkingGO[obj]);
        		}
	}

	private void setmass() {
	}


	// fundament sink 4cmm, set issinking so cube position will not be checked while sinking
	public void sink(List<GameObject> goList) {

		sinkingGO = new List<GameObject>(goList);
		//sinkingGO = goList;
		//parent sinking objects to fundament, need to destroy rigidbody otherwise sinking buggs 
		for (var obj = 0; obj < sinkingGO.Count; obj++) {
           	sinkingGO[obj].transform.parent = transform;
           	Destroy (sinkingGO[obj].GetComponent<Rigidbody>());
        }

		destination = transform.position - new Vector3(0,0.04f,0);
		issinking = true;
	}

	public void setfundament(float delta) {
		transform.position = transform.position - new Vector3(0,delta,0);
		destination = transform.position;
	}

	// move fundament back to start position
	public void movetostart() {
		transform.position = startposition;
		destination = startposition; 
	}
}
