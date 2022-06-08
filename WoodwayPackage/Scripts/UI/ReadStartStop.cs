using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadStartStop : MonoBehaviour
{
    private GameObject TreadmillController;
    private GameObject ReadButton;

    /// <summary><c>hasClicked</c> is the state of this button</summary>
    private bool hasClicked = false;
    /// <summary><c>isEnabled</c> tells if treadmill is ready to be driven</summary>
    private bool isEnabled = false;
    /// <summary><c>readOn</c> treadmill read state</summary>
    public bool readOn;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
        TreadmillController = this.GetComponentInParent<TreadmillGUIConfig>().TreadmillController;

        ReadButton = this.transform.parent.gameObject;

        var colors = ReadButton.GetComponent<Button>().colors;
        colors.normalColor = Color.green;
        colors.selectedColor = Color.green;

        ReadButton.GetComponent<Button>().colors = colors;

        readOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        isEnabled = TreadmillController.GetComponent<SerialController>().isEnabled;

        //timer to ensure communication has had time to establish
        if (time < 1)
            ReadButton.GetComponent<Button>().interactable = false;
        else
            ReadButton.GetComponent<Button>().interactable = true;

        time += Time.deltaTime;
    }


    public void OnClick()
    {
        var colors = ReadButton.GetComponent<Button>().colors;

        if (!hasClicked && !isEnabled)
        {
            hasClicked = !hasClicked;
            TreadmillController.GetComponent<SerialController>().EnableTreadmill();

            TreadmillController.GetComponent<SpeedController>().ReadTreadmillSpeed = !(TreadmillController.GetComponent<SpeedController>().ReadTreadmillSpeed);
        
            TreadmillController.GetComponent<InclineController>().ReadTreadmillIncline = !(TreadmillController.GetComponent<InclineController>().ReadTreadmillIncline);

            colors.normalColor = Color.red;
            colors.selectedColor = Color.red;

            ReadButton.GetComponent<Button>().colors = colors;

            readOn = true;
        }
        //stop reading stops treadmill 
        else
        {
            hasClicked = !hasClicked;

            TreadmillController.GetComponent<SerialController>().StopTreadmill();

            TreadmillController.GetComponent<SpeedController>().ReadTreadmillSpeed = !(TreadmillController.GetComponent<SpeedController>().ReadTreadmillSpeed);

            TreadmillController.GetComponent<InclineController>().ReadTreadmillIncline = !(TreadmillController.GetComponent<InclineController>().ReadTreadmillIncline);

            //if treadmill is in read + write mode, stopping read stops both functions
            if (TreadmillController.GetComponent<SpeedController>().WriteTreadmillSpeed && TreadmillController.GetComponent<InclineController>().WriteTreadmillIncline)
            {
                
                this.transform.parent.gameObject.transform.parent.gameObject.transform.Find("WriteButton").GetComponent<ObjectButton>().OnClick();
                this.transform.parent.gameObject.transform.parent.gameObject.transform.Find("WriteButton").GetComponentInChildren<WriteStartStop>().OnClick();
            }

            colors.normalColor = Color.green;
            colors.selectedColor = Color.green;
            
            ReadButton.GetComponent<Button>().colors = colors;


            readOn = false;

            time = 0;
        }

        

    }
             
}
