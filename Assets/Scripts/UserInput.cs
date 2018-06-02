using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		// for PC, uses mouse click but can be easily modified to tap for mobile
		if (Input.GetMouseButtonUp (0)) {
			//create a raycast from screen to mouse position on screen
			Ray raycast = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit raycastHit;
			if (Physics.Raycast (raycast, out raycastHit)) { // raycast hit an object
				CustomButton  customButton = raycastHit.collider.gameObject.GetComponent<CustomButton> ();
				if (customButton) { // hit object is of type CustomButton
					customButton.OnClicked();
				}
			}
		}
	}
}
