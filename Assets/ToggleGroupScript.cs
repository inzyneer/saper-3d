using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UI.Toggle;

public class ToggleGroupScript : MonoBehaviour
{
    private ToggleGroup toggleGroup;
    private Toggle[] toggles;

    private void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();
        toggles = GetComponentsInChildren<Toggle>();

        switch (BackgroundLogic.level)
        {
            case 0:
                toggles[0].isOn = true;
                break;
            case 1:
                toggles[1].isOn = true;
                break;
            case 2:
                toggles[2].isOn = true;
                break;
            case 3:
                toggles[3].isOn = true;
                break;
        }
    }

    //public void SetToggle(string name)
    //{
    //    foreach (Toggle t in toggles)
    //    {
    //        if (t.name == name)
    //        {
    //            t.isOn = true;
    //        }
    //    }
    //}

    public void GetSelected()
    {
        foreach (Toggle t in toggles)
        {
            if (t.isOn)
            {
                switch (t.name)
                {
                    case "Easy": BackgroundLogic.level = 0; break;
                    case "Normal": BackgroundLogic.level = 1; break;
                    case "Hard": BackgroundLogic.level = 2; break;
                    case "Custom": BackgroundLogic.level = 3; break;
                }
            }
        }
    }
}
