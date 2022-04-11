using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	// change text for missing cubes sign
	public void ChangeText(string str) {	
		TextMesh textObject = GameObject.Find("MissingCubesCount").GetComponent<TextMesh>();
     	textObject.text = str;
     	if (str == "1") {
     		TextMesh text2 = GameObject.Find("MissingCubes2").GetComponent<TextMesh>();
     		text2.text = "Klotz";
     	}
     	else {
     		TextMesh text2 = GameObject.Find("MissingCubes2").GetComponent<TextMesh>();
     		text2.text = "Klötze";
     	}
	}


	// show lost or win text in front of user
	public void ChangeGameStatusText(string str) {
		TextMesh textObject = GameObject.Find("GameStatus").GetComponent<TextMesh>();
     	textObject.text = str;
     	if (str == "Gewonnen!") {
     		textObject.color = Color.green;
     	}
     	else {
			textObject.color = Color.red;
     	}
	}
}
