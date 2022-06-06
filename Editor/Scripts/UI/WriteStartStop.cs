using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WriteStartStop : MonoBehaviour
{
    private GameObject TreadmillController;
    private GameObject WriteButton;

    /// <summary><c>hasClicked</c> is the state of this button</summary>
    private bool hasClicked = false;
    /// <summary><c>isEnabled</c> tells if treadmill is ready to be driven</summary>
    private bool isEnabled = false;
    /// <summary><c>readOn</c> treadmill read state</summary>
    private bool readOn;


    // Start is called before the first frame update
    void Start()
    {

        TreadmillController = this.GetComponentInParent<TreadmillGUIConfig>().TreadmillController;

        WriteButton = this.transform.parent.gameObject;

        readOn = this.transform.parent.gameObject.transform.parent.gameObject.transform.Find("ReadButton").GetComponentInChildren<ReadStartStop>().readOn;
    }

    // Update is called once per frame
    void Update()
    {
        isEnabled = TreadmillController.GetComponent<SerialController>().isEnabled;

        readOn = this.transform.parent.gameObject.transform.parent.gameObject.transform.Find("ReadButton").GetComponentInChildren<ReadStartStop>().readOn;
        
        var colors = WriteButton.GetComponent<Button>().colors;

        if (readOn)
        {
            WriteButton.GetComponent<Button>().interactable = true;
            if (!hasClicked)
            {
                colors.normalColor = Color.green;
                colors.selectedColor = Color.green;
            }

        }
        // if read is OFF, write in deactivated
        else
        {
            WriteButton.GetComponent<Button>().interactable = false;
            colors.normalColor = Color.gray;
            colors.selectedColor = Color.gray;
        }
        WriteButton.GetComponent<Button>().colors = colors;

    }

    /// <summary>Defines the actions when the Write Start/Stop button is clicked </summary>
    public void OnClick()
    {
        var colors = WriteButton.GetComponent<Button>().colors;

        // in Treadmill is not in read, clicking on this button does nothing
        if (!readOn)
        {
            this.transform.parent.gameObject.GetComponent<ObjectButton>().OnClick();
            return;
        }
            
        // if button is not ON (hasClicked is False) and treadmill is enabled, writing bools are turned True and button turns red.
        if (!hasClicked && isEnabled)
        {
            hasClicked = !hasClicked;

            TreadmillController.GetComponent<SpeedController>().WriteTreadmillSpeed = !(TreadmillController.GetComponent<SpeedController>().WriteTreadmillSpeed);
            TreadmillController.GetComponent<InclineController>().WriteTreadmillIncline = !(TreadmillController.GetComponent<InclineController>().WriteTreadmillIncline);

            colors.normalColor = Color.red;
            colors.selectedColor = Color.red;

            WriteButton.GetComponent<Button>().colors = colors;

        }
        // else if button is clicked and hasClicked is True, writing bools are turned False and button turn green for next start. 
        else
        {
            hasClicked = !hasClicked;

            TreadmillController.GetComponent<SpeedController>().WriteTreadmillSpeed = !(TreadmillController.GetComponent<SpeedController>().WriteTreadmillSpeed);
            TreadmillController.GetComponent<InclineController>().WriteTreadmillIncline = !(TreadmillController.GetComponent<InclineController>().WriteTreadmillIncline);

            colors.normalColor = Color.green;
            colors.selectedColor = Color.green;

            WriteButton.GetComponent<Button>().colors = colors;

        }
    }
             
}
