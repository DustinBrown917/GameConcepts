using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour {

    private static BattleManager instance_;
    public static BattleManager Instance { get { return instance_; } }

    [SerializeField] private Camera cam;
    [SerializeField] private Vector3 camStartPosition;
    [SerializeField] private Vector3 camFinalPosition;
    [SerializeField] private float cameraSlideMaxSpeed = 1.0f;
    [SerializeField] private float smoothTime = 1.0f;
    private Vector3 vel;

    public UnityEvent OnBattleClosed;

    private void Awake()
    {
        instance_ = this;
    }

    public void IntroduceBattle()
    {
        GameStateHandler.ChangeState(GameStateHandler.States.PRE_BATTLE);
        cam.transform.position = camStartPosition;
        StartCoroutine(OpeningBattleSequence());
    }

    public void BeginBattle()
    {
        GameStateHandler.ChangeState(GameStateHandler.States.BATTLE);
    }

    public void EndBattle()
    {
        GameStateHandler.ChangeState(GameStateHandler.States.POST_BATTLE);
        StartCoroutine(ClosingBattleSequence());
    }

    public void CloseBattle()
    {
        GameStateHandler.ChangeState(GameStateHandler.States.BATTLE_SUMMARY);
        OnBattleClosed.Invoke();
    }

    private IEnumerator OpeningBattleSequence()
    {
        while (Vector3.Distance(cam.transform.position, camFinalPosition) > 0.001f)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, camFinalPosition, ref vel, smoothTime, cameraSlideMaxSpeed);
            yield return null;
        }
        cam.transform.position = camFinalPosition;

        BeginBattle();
    }

    private IEnumerator ClosingBattleSequence()
    {
        yield return new WaitForSeconds(2.0f);

        while (Vector3.Distance(cam.transform.position, camStartPosition) > 0.001f)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, camStartPosition, ref vel, smoothTime, cameraSlideMaxSpeed);
            yield return null;
        }
        cam.transform.position = camStartPosition;

        CloseBattle();
    }


}
