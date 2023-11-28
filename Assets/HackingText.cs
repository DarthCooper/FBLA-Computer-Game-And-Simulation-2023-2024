using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HackingText : Button
{
    public string allText;
    public string word;

    public TMP_Text text;

    public string textColor = "\"green\"";

    protected override void Awake()
    {
        text = targetGraphic.GetComponent<TMP_Text>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        SetText();
    }

    public void SetText()
    {
        if (allText == null || allText != text.text)
        {
            allText = text.text;
        }
        string completePhrase = "";
        if (allText.Contains("<u>") && allText.Contains("</u>"))
        {
            string firstPart = allText.Substring(0, allText.IndexOf("<u>"));
            string coloredWord = "<color=" + textColor + "><u>" + word + "</color>";
            string endPart = allText.Substring(allText.IndexOf("</u>"));
            completePhrase = firstPart + coloredWord + endPart;
        }
        text.text = completePhrase;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        ResetText();
    }

    public void ResetText()
    {
        if (allText == null || allText != text.text)
        {
            allText = text.text;
        }
        string completePhrase = "";
        if (allText.Contains("<color=" + textColor + "><u>") && allText.Contains("</u>"))
        {
            string firstPart = allText.Substring(0, allText.IndexOf("<color=" + textColor + ">"));
            string coloredWord = "<u>" + word + "</u>";
            string endPart = allText.Substring(allText.IndexOf("</u>"));
            completePhrase = firstPart + coloredWord + endPart;
        }
        text.text = completePhrase;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        ComputerPuzzle.Instance.SubmitResponse(word, this);
    }
}
