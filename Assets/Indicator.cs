using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    public RawImage indicator;

    public void ChangeIndicator(bool enable)
    {
        indicator.enabled = enable;
    }
}
