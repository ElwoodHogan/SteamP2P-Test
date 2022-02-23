using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontMan : MonoBehaviour
{
    public static FrontMan FM;
    public Camera MainCam;
    public CanvasAI Canvas;

    private void Awake()
    {
        FM = this;
    }
}
