using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableControl : MonoBehaviour
{

    Toggle controlToggle;

    public GameObject TreadmillController;
    public GameObject TreadmillGUI;

    // Start is called before the first frame update
    void Start()
    {
        controlToggle = GetComponent<Toggle>();
        controlToggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(controlToggle);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ToggleValueChanged(Toggle change)
    {
        TreadmillController.GetComponent<SpeedController>().ReadTreadmillSpeed = !(TreadmillController.GetComponent<SpeedController>().ReadTreadmillSpeed);
        TreadmillController.GetComponent<SpeedController>().WriteTreadmillSpeed = !(TreadmillController.GetComponent<SpeedController>().WriteTreadmillSpeed);
        TreadmillController.GetComponent<InclineController>().ReadTreadmillIncline = !(TreadmillController.GetComponent<InclineController>().ReadTreadmillIncline);
        TreadmillController.GetComponent<InclineController>().WriteTreadmillIncline = !(TreadmillController.GetComponent<InclineController>().WriteTreadmillIncline);

        foreach (Transform child in TreadmillGUI.transform)
        {
            //Component[] buttons;
            //Component[] sliders;
            //Component[] inputFields;
            //buttons = child.gameObject.GetComponentsInChildren<Button>();
            //sliders = child.gameObject.GetComponentsInChildren<Slider>();
            //inputFields = child.gameObject.GetComponentsInChildren<InputField>();
            //foreach (Button button in buttons)
            //    button.interactable = !button.interactable;
            //foreach (Slider slider in sliders)
            //    slider.interactable = !slider.interactable;
            //foreach (InputField inputField in inputFields)
            //    inputField.interactable = !inputField.interactable;
        }
    }

    public void EnableDisableControl()
    {
        //TreadmillController.GetComponent<SpeedController>().DisableEnableControl();
    }

}
