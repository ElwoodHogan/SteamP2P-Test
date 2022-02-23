using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FrontMan;

public class MoveSphere : MonoBehaviour
{
    public float AccelForce;
    private void OnMouseDown()
    {
        print("doiwn");
    }
    private void OnMouseDrag()
    {
        Vector3 MouseYPos = FM.MainCam.ScreenToWorldPoint(Input.mousePosition);
        MouseYPos = Vector3.Normalize(new Vector3(MouseYPos.x, MouseYPos.y, 0));

        GetComponent<Rigidbody>().AddForce(MouseYPos * AccelForce, ForceMode.Acceleration);
    }
}
