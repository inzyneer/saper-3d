using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private TextMeshProUGUI text;
    public float value;

    void OnEnable()
    {
        //Subscribe to the Slider Click event
        slider.onValueChanged.AddListener(delegate { sliderCallBack(slider.value); });
    }

    //Will be called when Slider changes
    void sliderCallBack(float val)
    {
        value = val;
        text.text = val.ToString();
    }

    void OnDisable()
    {
        //Un-Subscribe To Slider Event
        slider.onValueChanged.RemoveListener(delegate { sliderCallBack(slider.value); });
    }
}
