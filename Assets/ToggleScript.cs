using UnityEngine;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour
{
    private Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener( 
            delegate { ToggleValueChanged(toggle);
        });
    }

    void ToggleValueChanged(Toggle toggle)
    {
        Debug.Log("toggled " + toggle.name);
    }
}
