using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Show off all the Debug UI components.
public class DebugUI : MonoBehaviour
{
    public GameObject leftEye;
    public GameObject rightEye;
    public GameObject canvas;
    
    bool inMenu;

    private Text convergenceText;
    private Text IPDText;

    void Start ()
    {        
        convergenceText = DebugUIBuilder.instance.AddLabel("Convergence").GetComponent<Text>();
        IPDText = DebugUIBuilder.instance.AddLabel("IPD").GetComponent<Text>();


  //      DebugUIBuilder.instance.AddLabel("Right");
  //      DebugUIBuilder.instance.AddLabel("onvergence");
  //      DebugUIBuilder.instance.AddLabel("IPD");
  //      var sliderPrefab = DebugUIBuilder.instance.AddSlider("Slider", 1.0f, 10.0f, SliderPressed, true);
  //      var textElementsInSlider = sliderPrefab.GetComponentsInChildren<Text>();
  //      Assert.AreEqual(textElementsInSlider.Length, 2, "Slider prefab format requires 2 text components (label + value)");
  //      sliderText = textElementsInSlider[1];
  //      Assert.IsNotNull(sliderText, "No text component on slider prefab");
  //      sliderText.text = sliderPrefab.GetComponentInChildren<Slider>().value.ToString();
  //      DebugUIBuilder.instance.AddDivider();
  //      DebugUIBuilder.instance.AddToggle("Toggle", TogglePressed);
  //      DebugUIBuilder.instance.AddRadio("Radio1", "group", delegate(Toggle t) { RadioPressed("Radio1", "group", t); }) ;
  //      DebugUIBuilder.instance.AddRadio("Radio2", "group", delegate(Toggle t) { RadioPressed("Radio2", "group", t); }) ;
  //      DebugUIBuilder.instance.AddLabel("Secondary Tab", 1);
		//DebugUIBuilder.instance.AddDivider(1);
  //      DebugUIBuilder.instance.AddRadio("Side Radio 1", "group2", delegate(Toggle t) { RadioPressed("Side Radio 1", "group2", t); }, DebugUIBuilder.DEBUG_PANE_RIGHT);
  //      DebugUIBuilder.instance.AddRadio("Side Radio 2", "group2", delegate(Toggle t) { RadioPressed("Side Radio 2", "group2", t); }, DebugUIBuilder.DEBUG_PANE_RIGHT);

        //DebugUIBuilder.instance.Show();
        //inMenu = true;
	}

    //public void setLeftPosition(Vector3 pos)
    //{
    //    leftSphereText.text = "Left : " + pos.ToString();
    //}

    //public void setRightPosition(Vector3 pos)
    //{
    //    rightSphereText.text = "Right : " + pos.ToString();
    //}

    //public void setConvergencePosition(Vector3 pos)
    //{
    //    convergenceText.text = "Left : " + pos.ToString();
    //}

    //public void setIPD(Vector3 pos)
    //{
    //    convergenceText.text = "Left : " + pos.ToString();
    //}

    //public void setLeftPosition(Vector3 leftPos)
    //{
    //    leftSphereText.text = "Left : " + leftPos.ToString();
    //}

    //public void TogglePressed(Toggle t)
    //{
    //    Debug.Log("Toggle pressed. Is on? "+t.isOn);
    //}
    //public void RadioPressed(string radioLabel, string group, Toggle t)
    //{
    //    Debug.Log("Radio value changed: "+radioLabel+", from group "+group+". New value: "+t.isOn);
    //}

    //public void SliderPressed(float f)
    //{
    //    Debug.Log("Slider: " + f);
    //    sliderText.text = f.ToString();
    //}

    void Update()
    {
        var ipd = (rightEye.transform.position - leftEye.transform.position).magnitude;
        var leftVector = leftEye.transform.localRotation * Vector3.forward;
        var rightVector = rightEye.transform.localRotation * Vector3.forward;
        var convergenceDistance = ipd / ((leftVector.x / leftVector.z - rightVector.x / rightVector.z) + 1e-9f);
        var convergence = leftVector / leftVector.z * convergenceDistance;

        IPDText.text = ipd.ToString();
        convergenceText.text = convergence.ToString();

        //.transform.localPosition = new Vector3(0,0,4);
        canvas.GetComponent<RectTransform>().localPosition = convergence;

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }
    }

    void LogButtonPressed()
    {
        Debug.Log("Button pressed");
    }
}
