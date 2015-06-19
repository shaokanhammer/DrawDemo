using UnityEngine;
using System.Collections;

public class MouseDebuger : MonoBehaviour
{

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), 
            " X=" + Input.mousePosition.x + 
          "\n Y=" + (Screen.height - Input.mousePosition.y) + 
          "\n Z=" + Input.mousePosition.z);
    }
}
