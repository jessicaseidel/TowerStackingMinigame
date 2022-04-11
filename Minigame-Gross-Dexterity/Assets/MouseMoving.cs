using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMoving: MonoBehaviour {
 
     public float speed = 10f;
     public Vector3 targetPos;
     public bool isMoving;
     const int MOUSE = 0;

     public bool useGravity = true;
     private Rigidbody rb;
     private GameObject ob;


     // Use this for initialization
     void Start () {
        targetPos = transform.position;
        isMoving = false;

		rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
     }
     
     // Update is called once per frame
     void Update () {		
		 
     	if(Input.GetMouseButtonDown(MOUSE)) {
	
            RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, 100.0f)) {
				if(hit.transform != null) {
					ob = hit.transform.gameObject;
     				//Debug.Log(ob);		
     			}
     		}	
        }

        if(Input.GetMouseButton(MOUSE) && ob && ob.tag == "tangible") {
            SetTarggetPosition();
        }
        if(isMoving) {
            MoveObject();
            ob.GetComponent<Rigidbody>().useGravity = false;
        }
        else if (!isMoving && ob && ob.tag == "tangible") {
            if (ob.GetComponent<Rigidbody>() != null) {
                ob.GetComponent<Rigidbody>().useGravity = true;
            }
        }
        // else if(!Input.GetMouseButton(MOUSE) && ob.tag == "tangible") {
        //    ob.GetComponent<Rigidbody>().useGravity = true;
        //}
     }

    void SetTarggetPosition() {
     	if (ob) {
     		Plane plane = new Plane(new Vector3(1,0,0), new Vector3(0.47f, 0.0f, 0.0f));
            //Plane plane = new Plane(0.47f, Vector3.forward, 0.1f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         	float point = 0f;

         	if(plane.Raycast(ray, out point)) {
         		targetPos = ray.GetPoint(point);
               // targetPos = new Vector3(0.47f, 0.899f, 0.283f);
         	}
         	isMoving = true;
     	}
    }


    void MoveObject() {
     	if (ob) {
        	ob.transform.LookAt(targetPos);
        	ob.transform.position = targetPos; //Vector3.MoveTowards(ob.transform.position, targetPos, speed * Time.deltaTime);
 
        	if (ob.transform.position == targetPos) {
            	isMoving = false;
        	}

     	} 
    }
 }

