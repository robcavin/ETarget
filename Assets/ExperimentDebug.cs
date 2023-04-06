using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperimentDebug : MonoBehaviour
{
    public Vector2 ETTargetSize;
    public Vector2 ControllerTargetSize;

    public bool eyesRequired = true;
    public bool controllerRequired = true;

    private string typedString;

    private Text ETTargetSizeText;
    private Text ControllerTargetSizeText;
    private Text typedStringText;

    bool inMenu;


    public void eyeRequiredPressed(Toggle t)
    {
        eyesRequired = t.isOn;
    }

    public void controllerRequiredPressed(Toggle t)
    {
        controllerRequired = t.isOn;
    }

    // Start is called before the first frame update
    void Start()
    {
        DebugUIBuilder.instance.AddToggle("Eyes Requried", eyeRequiredPressed, eyesRequired);
        DebugUIBuilder.instance.AddToggle("Cntr Requried", controllerRequiredPressed, controllerRequired);
        ETTargetSizeText = DebugUIBuilder.instance.AddLabel("ET target size : XXXdeg").GetComponent<Text>();
        ControllerTargetSizeText = DebugUIBuilder.instance.AddLabel("Cntr target size : XXXdeg").GetComponent<Text>();
        typedStringText = DebugUIBuilder.instance.AddLabel("Typed : ").GetComponent<Text>();
        typedString = "";

        DebugUIBuilder.instance.Show();
        inMenu = true;
    }

    // Update is called once per frame
    void Update()
    {
        ETTargetSizeText.text = "ET target size : " + ETTargetSize.x.ToString("F2") + "deg";
        ControllerTargetSizeText.text = "Controller target size : " + ControllerTargetSize.x.ToString("F2") + "deg";
        typedStringText.text = "Typed : " + typedString;

        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }
    }

    public void updateTypedString(char a)
    {
        if (a == (char)KeyCode.Backspace)
            typedString = typedString[..^1];
        else
            typedString += a;

        while (typedString.Length > 30)
            typedString = typedString[1..];
    }
}
