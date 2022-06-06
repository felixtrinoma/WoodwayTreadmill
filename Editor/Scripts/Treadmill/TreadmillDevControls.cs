using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScriptableObjectArchitecture.Variables;

public class TreadmillDevControls : MonoBehaviour
{

    public GameObject TreadmillController;

    private Slider speedSlider;
    private Slider angleSlider;

    public FloatVariable mainSpeed;
    public FloatVariable mainAngle;

    [Space]
    [Header("Speed in km/h")]
    public float readSpeed;
    [Range(0, 20f)]
    public float speedToWrite;
    [Space]
    [Header("Angle in %")]
    public float readAngle;
    [Range(0, 25)]
    public float angleToWrite;
    [Space]

    private float lastSpeed;
    private float lastAngle;

    // Update is called once per frame
    void Update()
    {
        readSpeed = mainSpeed.Value;
        readAngle = mainAngle.Value;

        if (speedToWrite != lastSpeed && TreadmillController.GetComponent<SpeedController>().WriteTreadmillSpeed)
        {
            WriteSpeed(speedToWrite);
        }
        else
            speedToWrite = lastSpeed;

        if (angleToWrite != lastAngle && TreadmillController.GetComponent<InclineController>().WriteTreadmillIncline)
        {
            WriteAngle(angleToWrite);
        }
        else
            angleToWrite = lastAngle;


        speedSlider = TreadmillController.GetComponent<SpeedController>().speedSlider;
        angleSlider = TreadmillController.GetComponent<InclineController>().angleSlider;
    }


    /// <summary>
    /// Write speed value to the speed Slider
    /// </summary>
    /// <param name="speedVal">speed value in km/h </param>
    void WriteSpeed(float speedVal)
    {
        speedSlider.value = speedVal;
        lastSpeed = speedVal;
    }

    /// <summary>
    /// Write inclination value to the angle Slider
    /// </summary>
    /// <param name="angleVal">inclination value in % </param>
    void WriteAngle(float angleVal)
    {
        angleSlider.value = angleVal;
        lastAngle = angleVal;
    }

}
