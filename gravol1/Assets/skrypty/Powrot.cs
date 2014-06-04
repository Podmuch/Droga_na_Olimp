using UnityEngine;
using System.Collections;

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
