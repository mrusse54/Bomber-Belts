using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIScript_MRussell : MonoBehaviour {

    public CharacterScript mainScript;

    public float[] bombSpeeds;
    public float[] buttonCooldowns;
    public float playerSpeed;
    public int[] beltDirections;
    public float[] buttonLocations;

	// Use this for initialization
	void Start () {
        mainScript = GetComponent<CharacterScript>();

        if (mainScript == null)
        {
            print("No CharacterScript found on " + gameObject.name);
            this.enabled = false;
        }

        buttonLocations = mainScript.getButtonLocations();

        playerSpeed = mainScript.getPlayerSpeed();
	}

    int targetBelt = 0; //you have to use this so the robot keeps going to the correct belt instead of updating belt everytime causing it to move incorrectly 
						// like it moving up then down or breaking when on the blue side
	int lifeCounter = 8;
	int bestBomb ;


	// Update is called once per frame
	void Update () {


		buttonCooldowns = mainScript.getButtonCooldowns();  // all button cooldown in an array

        beltDirections = mainScript.getBeltDirections(); // all the belt directions in an array

        bombSpeeds = mainScript.getBombSpeeds(); //bomb speeds can vary depending on the button being pressed more than once

        float[] bombDistance = mainScript.getBombDistances(); //array with all the distances of the bombs if less than 9.09 then bomb is coming at my side

        float playerLocation = mainScript.getCharacterLocation (); //player location that will be used to track distance and stuff

		int currentSlot = 0;//going to need this for targetBelt variable to assign values to it for the proper belt/bomb to track

		float distanceFromCurrent = Mathf.Infinity;//placeholder that will be replaced when looking at the belts/bombs

		float bombTime = 0; //time bomb will blow up

		float playerTime = 0; //player time to travel to 

		int bestH = 0; //best heuristic

		int currentH = 0; // current bomb heuristic

		for (int i = 0; i < 8; i++){//Anything with 8 elements works so 8 can work

		bool canMakeIt = (Mathf.Abs (playerLocation - buttonLocations [i]) / playerSpeed) + 0.35f < bombDistance[i] / bombSpeeds[i]; //last calculation to check to make sure the target is still

					currentH = 0; //current heuristic

			 playerTime = Mathf.Abs (playerLocation - buttonLocations [i]) / playerSpeed; //player time to reach button

				bombTime =  bombDistance [i] / bombSpeeds [i]; //bomb time to blow up

				distanceFromCurrent = Mathf.Abs (playerLocation - buttonLocations [i]); //distance from current button

			if(canMakeIt){ //if you can make it +10 making it higher heuristic
				currentH += 10;
			}

			if (beltDirections [i] == -1 || beltDirections [i] == 0) { //if the belt direction is coming towards you or sitting still +10 heuristic
				currentH += 10;
			}

			if (beltDirections [i] == -1) { //if the belt direction is coming towards you +10 heuristic
				currentH += 10;
			}

			if(playerTime < bombTime && bombTime > buttonCooldowns[i]){ //if you can reach the bomb with button not on cooldown +10 heuristic
				currentH += 10;
			}
			
			if (beltDirections [i] != -1 || beltDirections [i] != 0) { //if the belt is going towards the other player -10 heuristic
				currentH -= 10;
			}

			if(Mathf.Abs(bombDistance[i]) > 9.09){//if the bomb is over the middle so on the other side -10 heuristic
				currentH -= 10;
			}

			currentH += (int)(10 * distanceFromCurrent); //get the distance for heuristic value the farther the higher the number
			

		currentH += (int)(10 * bombTime); //the longer the bombtime the higher the heuristic value

				if(bestH < currentH){ //compare all 8 bombs
					targetBelt = i; //whatever bomb is the best store it
					bestH = currentH; //the best heuristic is updated
				}

		}

		if (buttonLocations[targetBelt] < mainScript.getCharacterLocation()) //moving your robot
		{
			mainScript.moveDown();
		}
		else if (buttonLocations[targetBelt] > mainScript.getCharacterLocation())
		{
			mainScript.moveUp();
		}
		
				mainScript.push(); //just push the button while moving to the bomb you want
		
		
		




	

	}
}
