using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgressTimer : MonoBehaviour {

	public Image LoadingBar;
	public float currenttime;
	public int playtime;

	public float timer = 0.0f;
	public bool timeover;
	public bool timerrunning;

	private Color progressBrownColor;

	// Use this for initialization
	void Start () {
		timeover = false;
		timerrunning = true;

		progressBrownColor = new Color();
    	ColorUtility.TryParseHtmlString ("#593304", out progressBrownColor);
	}
	
	// Update is called once per frame
	void Update () {
		// set actual time
		timer += Time.deltaTime;	
    	if (timer >= playtime) { 
    		timeover = true;
    		timer = 0;	
    	}
    	else {
    		timeover = false;
    	}
		// change time in view when game is running
    	if (timeover == false && timerrunning == true) {
			ChangeTime();

			// fill radial timer bar 
	 		if (currenttime != 0 ){
	 			float current = (playtime - currenttime) / playtime;
				LoadingBar.fillAmount = current;
				LoadingBar.color = progressBrownColor;
				if (currenttime <= 10) {
					LoadingBar.color = Color.red;
				} 
	 		}
	 		else {
	 			LoadingBar.fillAmount = 1;
	 		}
    	}
	}

	// change time text in view
	private void ChangeTime() {
		TextMesh textObject = GameObject.Find("Timer").GetComponent<TextMesh>();
    	int time = playtime - (int)timer; 
    	textObject.text = time.ToString();
    	if (time <= 10) {
    		textObject.color = Color.red;
    	}
    	else {
    		textObject.color = Color.black;
    	}
    	currenttime = time;
	}
}
