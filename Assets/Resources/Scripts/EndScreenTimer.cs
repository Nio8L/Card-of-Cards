using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EndScreenTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    void Update()
    {
        int hours   = Mathf.FloorToInt(Timer.timer.time / 3600);
        int minutes = Mathf.FloorToInt(Timer.timer.time % 3600/60  );
        int seconds = Mathf.FloorToInt(Timer.timer.time % 60  );

        string hoursZero   = "0";
        string minutesZero = "0";
        string secondsZero = "0";

        if (minutes > 9){
            minutesZero = "";
        }
        if (seconds > 9){
            secondsZero = "";
        }
        if (hours > 9){
            hoursZero = "";
        }


        timerText.text = hoursZero + hours  + ":" + minutesZero + minutes + ":" + secondsZero + seconds;
    }
}
