using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontMan : MonoBehaviour
{
    public static FrontMan FM;
    public Camera MainCam;

    private void Awake()
    {
        FM = this;
    }
}
