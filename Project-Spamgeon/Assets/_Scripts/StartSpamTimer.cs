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
    [SerializeField] private ButtonGraphic buttonGraphic;
    private AudioSource audioSource;

    public UnityEvent TimerDone;

    private Coroutine cr_RunTimer;
    private Coroutine cr_NumberSwell;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        timerCountdownText.text = "Prepare yourself, time is of the essence...";
        timerCountdownText.enabled = true;
        SetButtonActive(false);
        CoroutineManager.BeginCoroutine(RunTimer(), ref cr_RunTimer, this);
    }

    private IEnumerator RunTimer()
    {
        yield return new WaitForSeconds(initialDelay);

        float countDownTime = timerCountdownTime;

        audioSource.pitch = 1.0f;

        while(countDownTime > 0)
        {
            timerCountdownText.text = countDownTime.ToString();
            countDownTime -= 1.0f;
            audioSource.Play();
            CoroutineManager.BeginCoroutine(CoroutineManager.ShrinkScaleFrom(timerCountdownText.transform, swellSize, Vector3.one, shrinkTime), ref cr_NumberSwell, this);
            yield return new WaitForSeconds(1.0f);
        }

        audioSource.pitch = 1.5f;
        audioSource.Play();
        timerCountdownText.enabled = false;
        SetButtonActive(true);
        CoroutineManager.BeginCoroutine(CoroutineManager.ShrinkScaleFrom(timerCountdownText.transform, swellSize, Vector3.one, shrinkTime), ref cr_NumberSwell, this);


        TimerDone.Invoke();
    }

    public void SetButtonActive(bool active)
    {
        buttonGraphic.gameObject.SetActive(active);
    }


    
}
