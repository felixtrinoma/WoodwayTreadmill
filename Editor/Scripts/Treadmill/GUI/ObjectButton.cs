using UnityEngine;
using UnityEngine.UI;

public class ObjectButton : MonoBehaviour
{
    public GameObject LinkedObject;

    public string isOn = "", isOff = "";

    private void Awake()
    {
            GetComponentInChildren<Text>().text = isOff;
    }

    public void OnClick()
    {
        if(LinkedObject != null)
            LinkedObject.SetActive(!LinkedObject.activeSelf);

        if (GetComponentInChildren<Text>().text == isOff)
            GetComponentInChildren<Text>().text = isOn;
        else
            GetComponentInChildren<Text>().text = isOff;
    }
}
