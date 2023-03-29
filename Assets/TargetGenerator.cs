using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TargetGenerator : MonoBehaviour
{
    public int rows = 5;
    public int columns = 5;
    public float padding = 3.0f;
    public float size = 3.0f;
    public float distance = 100.0f;
    public GameObject target;
    public GameObject calibTarget;
    public GameObject leftTrackedEye;
    public GameObject rightTrackedEye;
    public bool depthTest = false;
    public float calibTargetNearDist = 0.25f;
    public float calibTargetFarDist = 10.0f;

    private List<Collider> targetColliders = new List<Collider>();
    private GameObject calibTargetInstance;
    private float calibStartTime;
    private bool calibRunning = false;
    private Vector2 calibPhiTheta;

    private StreamWriter writer;

    private string path;

    private System.Random rnd = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        path = Application.persistentDataPath;

        float width = columns * size + (columns - 1) * padding;
        float height = rows * size + (rows - 1) * padding;

        if (depthTest)
        {
            float theta = 0;
            float phi = 0;
            float x = Mathf.Sin(theta / 180 * Mathf.PI);
            float y = Mathf.Sin(phi / 180 * Mathf.PI);
            float z = Mathf.Sqrt(1 - (x * x + y * y));

            var position = new Vector3(x, y, z) * calibTargetFarDist;
            var rotation = new Vector3(x, y, z);
            calibTargetInstance = Instantiate(calibTarget, this.transform);
            var targetScale = 2 * Mathf.Atan((size / 2) / 180 * Mathf.PI) * calibTargetFarDist;

            calibTargetInstance.transform.localPosition = position;
            calibTargetInstance.transform.localRotation = Quaternion.LookRotation(rotation);
            calibTargetInstance.transform.localScale = new Vector3(targetScale, targetScale, 1);
            calibTargetInstance.SetActive(false);
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float theta = col * (size + padding) - (width / 2) + (size / 2);
                float phi = row * (size + padding) - (height / 2) + (size / 2);
                float x = Mathf.Sin(theta / 180 * Mathf.PI);
                float y = Mathf.Sin(phi / 180 * Mathf.PI);
                float z = Mathf.Sqrt(1 - (x * x + y * y));
                var position = new Vector3(x, y, z) * distance;
                var rotation = new Vector3(x, y, z);
                var targetScale = 2 * Mathf.Atan((size / 2) / 180 * Mathf.PI) * distance;
                var blob = Instantiate(target, this.transform);
                blob.transform.localPosition = position;
                blob.transform.localRotation = Quaternion.LookRotation(rotation);
                blob.transform.localScale = new Vector3(targetScale, targetScale, 1);

                targetColliders.Add(blob.GetComponent<Collider>());
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit info;
        if (Physics.Raycast(leftTrackedEye.transform.position, leftTrackedEye.transform.forward, out info))
        {
            //info.collider.gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }


        if (depthTest)
        {
            if (OVRInput.GetDown(OVRInput.Button.Two))

            {
                if (!calibRunning)
                {
                    calibRunning = true;
                    var datestring = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                    writer = new StreamWriter(path + "/" + datestring);
                    calibStartTime = Time.time;
                    calibTargetInstance.SetActive(true);
                    
                    calibPhiTheta = new Vector2((float) rnd.NextDouble() * 60 - 30, (float)rnd.NextDouble() * 60 - 30);
                }
                else
                {
                    calibRunning = false;
                    writer.Close();
                    calibTargetInstance.SetActive(false);
                }
            }

            if (calibRunning)
            {
                var delta = Time.time - calibStartTime;
                var percent = (Mathf.Cos(delta / 2) + 1) / 2;
                var nearDistDiopter = 1 / calibTargetNearDist;
                var farDistDiotper = 1 / calibTargetFarDist;
                var distanceDiopter = nearDistDiopter + percent * (farDistDiotper - nearDistDiopter);
                var distance = 1 / distanceDiopter;

                float theta = calibPhiTheta.x;
                float phi = calibPhiTheta.y;
                float x = Mathf.Sin(theta / 180 * Mathf.PI);
                float y = Mathf.Sin(phi / 180 * Mathf.PI);
                float z = Mathf.Sqrt(1 - (x * x + y * y));
                var position = new Vector3(x, y, z) * distance;
                var rotation = new Vector3(x, y, z);
                var targetScale = 2 * Mathf.Atan((size / 2) / 180 * Mathf.PI) * distance;

                calibTargetInstance.transform.localPosition = position;
                calibTargetInstance.transform.localRotation = Quaternion.LookRotation(rotation);
                calibTargetInstance.transform.localScale = new Vector3(targetScale, targetScale, 1);

                var ipd = (rightTrackedEye.transform.position - leftTrackedEye.transform.position).magnitude;
                var leftVector = leftTrackedEye.transform.localRotation * Vector3.forward;
                var rightVector = rightTrackedEye.transform.localRotation * Vector3.forward;
                var convergenceDistance = ipd / (leftVector.x / leftVector.z - rightVector.x / rightVector.z);
                var convergence = leftVector / leftVector.z * convergenceDistance;

                writer.WriteLine(position.x + "," + position.y + "," + position.z + "," + convergence.x + "," + convergence.y + "," + convergence.z);
            }
        }
    }
}
