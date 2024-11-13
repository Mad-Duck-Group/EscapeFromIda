using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    
    public TMP_Text file1Time;

    public void Update()
    {
        float myTime = Time.realtimeSinceStartup;
        file1Time.SetText(myTime.ToString("Time : "+"00:00"));
    }
}
