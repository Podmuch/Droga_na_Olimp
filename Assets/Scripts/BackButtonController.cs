//Button allows back to menu before game end
using UnityEngine;

public class BackButtonController : MonoBehaviour {

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
