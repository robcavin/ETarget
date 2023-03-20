using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGenerator : MonoBehaviour
{
    public int rows = 5;
    public int columns = 5;
    public float padding = 3.0f;
    public float size = 3.0f;
    public float distance = 100.0f;
    public GameObject target;
    public GameObject leftTrackedEye;
    public GameObject rightTrackedEye;

    private List<Collider> targetColliders = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        float width = columns * size + (columns - 1) * padding;
        float height = rows * size + (rows - 1) * padding;
        
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
                Debug.Log(rotation);
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
            info.collider.gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }


    }
}
