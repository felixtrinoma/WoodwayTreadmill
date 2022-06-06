using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlopeVal : MonoBehaviour
{

    Text text;

    private GameObject TreadmillController;

    private float slopeVal;


    // Start is called before the first frame update
    void Start()
    {
        TreadmillController = this.GetComponentInParent<TreadmillGUIConfig>().TreadmillController;

        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        slopeVal = TreadmillController.GetComponent<InclineController>().DegToPerc(TreadmillController.GetComponent<InclineController>().angle.Value);
        text.text = slopeVal.ToString("0.0");

    }
}
