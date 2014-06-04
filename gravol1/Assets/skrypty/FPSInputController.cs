//Klasa z internetu (wersja FPSInputController'a w jêzyku C#)
//z pewnymi modyfikacjami umo¿liwiaj¹cymi ruch graczem za pomoc¹ przycisków na planszy (dla urz¹dzeñ dotykowych)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]

public class FPSInputController : MonoBehaviour
{
    private CharacterMotor motor;

    // Use this for initialization
    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        //modyfikacja I - inicjalizacja zmiennych na podstawie statycznych w³aœciwoœci przycisków
        //je¿eli przycisków nie ma (np w menu) to wszystkie ustawiamy na false
		bool skok=false, lewo=false, prawo = false;
		if(FindObjectOfType<Przycisk>()!=null) 
		{
			skok = Przycisk.skok;
			lewo = Przycisk.lewo;
			prawo = Przycisk.prawo;
		}
        // Get the input vector from kayboard or analog stick
        Vector3 directionVector = new Vector3(0, 0, Input.GetAxis("Vertical"));

        if (directionVector != Vector3.zero)
        {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;

            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1.0f, directionLength);

            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;

            // Multiply the normalized direction vector by the modified length
            directionVector = directionVector * directionLength;
        }
        //modyfikacja II - je¿eli wektor ruchu jest zerowy (klawiatura nie u¿yta, sprawdzamy czy któryœ z przycisków jest aktywny) 
		else {
			directionVector=new Vector3(0,0,(lewo||prawo)?0.5f:0.0f);
			if (directionVector != Vector3.zero)
			{
				// Get the length of the directon vector and then normalize it
				// Dividing by the length is cheaper than normalizing when we already have the length anyway
				float directionLength = directionVector.magnitude;
				directionVector = directionVector / directionLength;
				
				// Make sure the length is no bigger than 1
				directionLength = Mathf.Min(1.0f, directionLength);
				
				// Make the input vector more sensitive towards the extremes and less sensitive in the middle
				// This makes it easier to control slow speeds when using analog sticks
				directionLength = directionLength * directionLength;
				
				// Multiply the normalized direction vector by the modified length

			}
		}
		// Apply the direction to the CharacterMotor
		motor.inputMoveDirection = transform.rotation * directionVector;
        //modyfikacja III - reakcja na przycisk do skoku
        motor.inputJump = Input.GetButton("Jump")||skok;
    }
}