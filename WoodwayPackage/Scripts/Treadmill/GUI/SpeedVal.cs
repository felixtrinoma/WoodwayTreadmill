using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedVal : MonoBehaviour
{

    Text text;

    private GameObject TreadmillController;

    private float speedVal;


    // Start is called before the first frame update
    void Start()
    {
        TreadmillController = this.GetComponentInParent<TreadmillGUIConfig>().TreadmillController;

        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        speedVal = TreadmillController.GetComponent<SpeedController>().MStoKM(TreadmillController.GetComponent<SpeedController>().camSpeed.Value);
        text.text = speedVal.ToString("00.0");

    }
}
