using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderToText : MonoBehaviour
{
    public InputField input;
    Slider slider;
    public Button up;
    public Button down;
    public float buttonStep=0.1f;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();

        if (up)
            up.onClick.AddListener(()=>GoUp());

        if (down) {
            down.onClick.AddListener(() => GoDown());
        }      

        input.onEndEdit.AddListener((string v) => OnInputChanged(v));

        slider.onValueChanged.AddListener((float v) => OnSliderChange(v));

        OnSliderChange(0);
    }

    public void GoUp()
    {
        //Debug.Log("GoUp");
        slider.value += buttonStep;
    }

    public void GoDown()
    {
        //Debug.Log("GoDown");
        slider.value -= buttonStep;
    }

    void OnInputChanged(string v)
    {
        //slider.onValueChanged.RemoveListener((float val) => OnSliderChange(val));


        if (v != slider.value.ToString("0.0"))
        {
            float val = 0;
            float.TryParse(v, out val);
            val =((float)Mathf.Round(val * 100f) / 100f);

            //Debug.Log("OnInputChanged" + val);

            slider.value = val;
        }

        //slider.onValueChanged.AddListener((float val) => OnSliderChange(val));
    }

    void OnSliderChange(float v)
    {

        if (input.text != v.ToString("0.0"))
        {
            v = ((float)Mathf.Round(v * 100) / 100f);

            //Debug.Log("OnSliderChange " + v);

            //input.onEndEdit.RemoveListener((string s) => OnInputChanged(s));
            input.text = v.ToString("0.0");
            //input.onEndEdit.AddListener((string s) => OnInputChanged(s));

        }

    }
}
