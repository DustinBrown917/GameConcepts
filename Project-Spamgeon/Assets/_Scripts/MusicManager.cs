using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private static MusicManager instance_ = null;
    public static MusicManager Instance { get { return instance_; } }

    [SerializeField] private AudioClip[] songs;
    private AudioSource audioSource;

    private Coroutine cr_playSting;
    private Songs currentSong = Songs.INVALID;
    public Songs CurrentSong { get { return currentSong; } }
    private bool isPlayingSting = false;

    

    private void Awake()
    {
        if(instance_ == null)
        {
            instance_ = this;
        } else if(instance_ != this)
        {
            Destroy(this);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        if(instance_ == this)
        {
            instance_ = null;
        }
    }

    private IEnumerator EPlaySting(Songs song)
    {
        isPlayingSting = true;
        audioSource.loop = false;
        audioSource.clip = songs[(int)song];
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);

        audioSource.clip = songs[(int)currentSong];
        audioSource.loop = true;
        audioSource.Play();

        isPlayingSting = false;
    }

    public void SetCurrentSong(Songs song)
    {
        if(song == CurrentSong) { return; }

        currentSong = song;
        if (!isPlayingSting)
        {
            audioSource.clip = songs[(int)currentSong];
            audioSource.Play();
        }
    }

    public void PlaySting(Songs song)
    {
        CoroutineManager.BeginCoroutine(EPlaySting(song), ref cr_playSting, this);
    }


    public enum Songs
    {
        INVALID = -1,
        MENU = 0,
        BATTLE,
        LOOT_SPAM,
        VICTORY,
        DEFEAT
    }
}
