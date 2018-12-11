using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartSpamTimer : MonoBehaviour {

    [SerializeField] private Text timerCountdownText;

    [SerializeField] private float initialDelay;
    [SerializeField] private float timerCountdownTime;
    [SerializeField] private Vector3 swellSize;
    [SerializeField] private float shrinkTime;

    public UnityEvent TimerDone;

    private Coroutine cr_RunTimer;
    private Coroutine cr_NumberSwell;

    private void OnEnable() {
        timerCountdownText.text = "Prepare yourself, time is of the essence...";
        CoroutineManager.BeginCoroutine(RunTimer(), ref cr_RunTimer, this);
    }

    private IEnumerator RunTimer()
    {
        yield return new WaitForSeconds(initialDelay);

        float countDownTime = timerCountdownTime;

        while(countDownTime > 0)
        {
            timerCountdownText.text = countDownTime.ToString();
            countDownTime -= 1.0f;
            CoroutineManager.BeginCoroutine(CoroutineManager.ShrinkScaleFrom(timerCountdownText.transform, swellSize, Vector3.one, shrinkTime), ref cr_NumberSwell, this);
            yield return new WaitForSeconds(1.0f);
        }

        timerCountdownText.text = "Spam!";
        CoroutineManager.BeginCoroutine(CoroutineManager.ShrinkScaleFrom(timerCountdownText.transform, swellSize, Vector3.one, shrinkTime), ref cr_NumberSwell, this);


        TimerDone.Invoke();
    }


    
}
