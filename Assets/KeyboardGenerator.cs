using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardGenerator : MonoBehaviour
{
    public Sprite a, b, c, d, e, f, g, h, i, j, k, l, m;
    public Sprite n, o, p, q, r, s, t, u, v, w, x, y, z;
    public Sprite space, a1, a2, a3, a4, a5, a6, a7, a8, a9, a0, backspace;
    public GameObject targetTemplate;
    public GameObject keyTemplate;
    public float keySize = 3;
    public float keySpacing = 3;
    public float keyDistance = 2;

    private List<char> allChars;
    private List<Sprite> allLetters;

    private List<Sprite> topNumbers;
    private List<Sprite> topLetters;
    private List<Sprite> middleLetters;
    private List<Sprite> lowerLetters;
    private List<Sprite> lowerControls;

    private GameObject[] topNumberKeys;
    private GameObject[] topKeys;
    private GameObject[] middleKeys;
    private GameObject[] lowerKeys;
    private GameObject[] lowerControlKeys;

    void layoutRow(GameObject[] objs, float size, float padding, int row, float totalHeight, float distance)
    {
        float width = objs.Length * size + (objs.Length - 1) * padding;
        for (int col = 0; col < objs.Length; col++)
        {
            float x = col * (size + padding) - (width / 2) + (size / 2);
            float y = row * (size + padding) - (totalHeight / 2) + (size / 2);

            objs[col].transform.position = new Vector3(x, y, distance);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        allChars = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', (char)KeyCode.Backspace, (char)KeyCode.Space };
        allLetters = new List<Sprite> { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z, a1, a2, a3, a4, a5, a6, a7, a8, a9, a0, backspace, space };

        topNumbers = new List<Sprite> { a1, a2, a3, a4, a5, a6, a7, a8, a9, a0, backspace };
        topLetters = new List<Sprite> { q, w, e, r, t, y, u, i ,o, p};
        middleLetters = new List<Sprite> { a, s, d, f, g, h, j, k, l};
        lowerLetters = new List<Sprite> { z, x, c, v, b, n, m };
        lowerControls = new List<Sprite> { space };

        topNumberKeys = new GameObject[topNumbers.Count];
        topKeys = new GameObject[topLetters.Count];
        middleKeys = new GameObject[middleLetters.Count];
        lowerKeys = new GameObject[lowerLetters.Count];
        lowerControlKeys = new GameObject[lowerControls.Count];

        var size = 2 * Mathf.Tan(keySize / 2 * Mathf.PI / 180) * keyDistance;

        for (int i = 0; i < allLetters.Count; i++)
        {
            var target = Instantiate(targetTemplate);
            var keySprite = Instantiate(keyTemplate, target.transform);
            keySprite.GetComponent<SpriteRenderer>().sprite = allLetters[i];
            keySprite.GetComponent<KeyState>().key = allChars[i];
            keySprite.transform.localScale = new Vector3(size / 20, size / 20, 1); // FIXME

            if (topNumbers.Contains(allLetters[i])) topNumberKeys[topNumbers.IndexOf(allLetters[i])] = target;
            else if (topLetters.Contains(allLetters[i])) topKeys[topLetters.IndexOf(allLetters[i])] = target;
            else if (middleLetters.Contains(allLetters[i])) middleKeys[middleLetters.IndexOf(allLetters[i])] = target;
            else if (lowerLetters.Contains(allLetters[i])) lowerKeys[lowerLetters.IndexOf(allLetters[i])] = target;
            else if (lowerControls.Contains(allLetters[i])) lowerControlKeys[lowerControls.IndexOf(allLetters[i])] = target;
        }

        var padding = 2 * Mathf.Tan(keySpacing / 2 * Mathf.PI / 180) * keyDistance;
        int rows = 5;
        float totalHeight = rows * size + (rows - 1) * padding;

        layoutRow(topNumberKeys, size, padding, 4, totalHeight, keyDistance);
        layoutRow(topKeys, size, padding, 3, totalHeight, keyDistance);
        layoutRow(middleKeys, size, padding, 2, totalHeight, keyDistance);
        layoutRow(lowerKeys, size, padding, 1, totalHeight, keyDistance);
        layoutRow(lowerControlKeys, size, padding, 0, totalHeight, keyDistance);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
