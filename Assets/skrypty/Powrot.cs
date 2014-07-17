//Przycisk pozwalający wrócić do menu przed zakończeniem rozgrywki
using UnityEngine;

public class Powrot : MonoBehaviour {

    void OnMouseEnter()
    {
        renderer.material.color = Color.black;
    }
    void OnMouseExit()
    {
        renderer.material.color = Color.red;
    }
    void OnMouseDown()
    {
        Application.LoadLevel(2);
    }
}
