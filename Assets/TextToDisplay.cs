using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextToDisplay : MonoBehaviour
{
    TMP_Text text;

    [TextAreaAttribute] public string words;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = words;
    }
}
