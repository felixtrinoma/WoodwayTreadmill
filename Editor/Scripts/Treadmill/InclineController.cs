using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScriptableObjectArchitecture.Variables;

public class InclineController : MonoBehaviour
{
    private SerialController serialController;
    [Space]
    public bool ReadTreadmillIncline = false;
    public bool WriteTreadmillIncline = false;
    [Space]
    [Header("Angle parameters")]
    [Range(0, 25)]
    [Tooltip("in %")]
    public float setAngle = 0;
    [Tooltip("in °")]
    public FloatVariable angle;


    [Space]
    [Header("Control parameters")]
    public Slider angleSlider;
    public bool stop = true;

    /// <summary><c>readTAngle</c> is the angle read from the treadmill in % </summary>
    private float readTAngle = 0;

    void Start()
    {
        angle.Value = 0;

        serialController = this.gameObject.GetComponent<SerialController>();

        angleSlider = this.gameObject.GetComponent<TreadmillGUILink>().TreadmillGUI.transform.Find("SliderAndInput Angle").gameObject.GetComponentInChildren<Slider>();

        if (angleSlider != null)
            angleSlider.onValueChanged.AddListener((float f) => OnAngleSliderValueChanged(f));
    }

    void OnAngleSliderValueChanged(float f)
    {
        setAngle = f;
    }

    /// <summary>convert angle value from degree to slope percentage </summary>
    /// <param><c>deg</c> angle in degree</param>
    /// <returns>slope in %</returns>
    public float DegToPerc(float deg)
    {
        float perc = Mathf.Tan(Mathf.Deg2Rad * deg) * 100f;
        return perc;
    }

    /// <summary>convert slope value from percentage to angle in degree </summary>
    /// <param><c>perc</c> slope in %</param>
    /// <returns>angle in degree</returns>
    public float PercToDeg(float perc)
    {
        float deg = Mathf.Atan2(perc, 100f)*Mathf.Rad2Deg;
        return deg;
    }

    /// <summary>convert slope value from percentage in byte to float percentage </summary>
    /// <param><c>bSlope</c> slope in % as a byte</param>
    /// <returns>slope in %</returns>
    float SlopeByteToPerc(byte[] bSlope)
    {
        int sign = bSlope[0];

        int tens = (Convert.ToInt32(bSlope[1]) - 48) * 10;
        int unts = (Convert.ToInt32(bSlope[2]) - 48);
        float tnths = (Convert.ToInt32(bSlope[3]) - 48) / 10f;

        float tot = tens + unts + tnths;

        if (sign == 45)
            tot = - tot;

        return tot;
    }

    /// <summary>convert slope value from float percentage to percentage in a byte</summary>
    /// <param><c>bSlope</c> slope in % as a float</param>
    /// <returns>slope in % as a byte</returns>
    byte[] SlopePerctoByte(float deg)
    {

        int tens = (int)(deg / 10);
        int unts = (int)(deg - tens * 10);
        int tnths = (int)((deg - tens * 10 - unts) * 10);

        byte[] bPerc = new byte[4];

        bPerc[1] = Convert.ToByte(tens + 48);
        bPerc[2] = Convert.ToByte(unts + 48);
        bPerc[3] = Convert.ToByte(tnths + 48);

        if(Mathf.Sign(deg) == -1)
            bPerc[0] = Convert.ToByte(45);
        else
            bPerc[0] = Convert.ToByte(43);

        return bPerc;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (ReadTreadmillIncline)
        {
            readTAngle = SlopeByteToPerc(serialController.slopeMsg);

            // if treadmill is in read only, slider takes value from treadmill reading
            if (!WriteTreadmillIncline)
                angleSlider.value = readTAngle;
            //if writing to treadmill is on and set value is not reached
            if (WriteTreadmillIncline && (setAngle < (readTAngle - 0.1f) | setAngle > (readTAngle + 0.1f)))
            {
                //if value is within treadmill limits
                if (setAngle >= 0 && setAngle <= 25)
                {
                    serialController.SendSlopeTreadmill(SlopePerctoByte(setAngle));
                }  
            }      
        } 

        // read slope value from Treadmill or slider and convert it in degrees
        if (ReadTreadmillIncline)
        {
            angle.Value = PercToDeg(readTAngle);
        }
        else
        {
            angle.Value = PercToDeg(setAngle);
        }

    }


    
}
