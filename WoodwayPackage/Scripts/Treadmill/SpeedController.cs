using UnityEngine;
using UnityEngine.UI;
using System;
using ScriptableObjectArchitecture.Variables;

public class SpeedController : MonoBehaviour
{
    private SerialController serialController;
    [Space]
    public bool ReadTreadmillSpeed = false;
    public bool WriteTreadmillSpeed = false;
    [Space]
    [Header("Speed parameters")]
    [Range(0, 20f)]
    [Tooltip("in km/h")]
    public float setSpeed = 0;
    [Tooltip("in m/s")]
    public FloatVariable camSpeed;


    [Space]
    [Header("Control parameters")]
    public bool stop = true;

    /// <summary><c>readTSpeed</c> is the speed read from the treadmill in m/s</summary>
    private float readTSpeed;

    /// <summary><c>ratio</c> is the ratio to convert between mm/s and km/h</summary>
    private float ratio = 10000/36;

    public Slider speedSlider;

    void Start()
    {
        camSpeed.Value = 0;

        serialController = this.gameObject.GetComponent<SerialController>();

        speedSlider = this.gameObject.GetComponent<TreadmillGUILink>().TreadmillGUI.transform.Find("SliderAndInput Vitesse").gameObject.GetComponentInChildren<Slider>();
        //spToggle = this.gameObject.GetComponent<TreadmillGUILink>().TreadmillGUI.transform.Find("ToggleSelfPaced").gameObject.GetComponentInChildren<Toggle>();


        if (speedSlider != null)
            speedSlider.onValueChanged.AddListener((float f) => OnSpeedSliderValueChanged(f));
        
    }

    void OnSpeedSliderValueChanged(float f)
    {
        setSpeed = f;
    }

    public float KMtoMS(float km)
    {
        float ms = km / 3.6f;
        return ms;
    }

    /// <summary>convert m/s to km/h</summary>
    /// <param><c>ms</c> speed in m/s</param>
    /// <returns>speed in km/h</returns>
    public float MStoKM(float ms)
    {
        float km = ms * 3.6f;
        return km;
    }

    /// <summary>convert speed in mm/s expressed in a byte to a float in km/h</summary>
    /// <param><c>bSpeed</c>speed in mm/s written in a byte</param>
    /// <returns> speed in km/h</returns>
    float SpeedMMStoKM(byte[] bSpeed)
    {
        int thds = (Convert.ToInt32(bSpeed[0]) - 48) * 1000;
        int hrds = (Convert.ToInt32(bSpeed[1]) - 48) * 100;
        int tens = (Convert.ToInt32(bSpeed[2]) - 48) * 10;
        int unts = (Convert.ToInt32(bSpeed[3]) - 48);

        int tot = thds + hrds + tens + unts;

        float tSpeed = tot / (ratio);

        return tSpeed;
    }

    /// <summary>convert speed in km/h expressed in a float to a byte expressing speed in mm/s</summary>
    /// <param><c>f</c>speed in km/h</param>
    /// <returns> speed byte in mm/s</returns>
    byte[] SpeedKMtoMMS(float f)
    {
        float fmms = f * (ratio);
        int sKm = System.Convert.ToInt32(fmms);

        int thds = sKm / 1000;
        int hrds = (sKm - thds*1000) / 100;
        int tens = (sKm - thds*1000 - hrds*100) / 10;
        int unts = (sKm - thds*1000 - hrds*100 - tens*10);

        byte[] bsKm = new byte[4];

        bsKm[0] = Convert.ToByte(thds + 48);
        bsKm[1] = Convert.ToByte(hrds + 48);
        bsKm[2] = Convert.ToByte(tens + 48);
        bsKm[3] = Convert.ToByte(unts + 48);

        return bsKm;
    }


    // Update is called once per frame
    void FixedUpdate()
    {


        if (ReadTreadmillSpeed)
        {

            readTSpeed = SpeedMMStoKM(serialController.speedMsg);


            if (!WriteTreadmillSpeed)
                speedSlider.value = readTSpeed;

            if (WriteTreadmillSpeed &&  (setSpeed < (readTSpeed - 0.1f) | setSpeed > (readTSpeed+0.1f)))
            {
                if (setSpeed <= 20f && setSpeed > 0)
                {
                    serialController.SendSpeedTreadmill(SpeedKMtoMMS(setSpeed));
                }
            }
        }

        

        if (ReadTreadmillSpeed)
        {
            camSpeed.Value = KMtoMS(readTSpeed);
        }else
        {
            camSpeed.Value = KMtoMS(setSpeed);
        }

    }
}
