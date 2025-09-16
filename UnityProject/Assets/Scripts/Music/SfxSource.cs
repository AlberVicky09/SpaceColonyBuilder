using System;
using UnityEngine;

public class SfxSource : MonoBehaviour {

    public AudioSource source;
    public SfxTrackNamesEnum audioClip;

    public void Start() {
        //On start, attach to publisher
        AudioManager.Instance.AddNewSfxSource(this);
    }

    public void PlaySfx() {
        var s = Array.Find(AudioManager.Instance.sfxClips, x => x.sfxTrackName.Equals(audioClip));
        if (s != null) {
            source.PlayOneShot(s.clip);
        } else {
            Debug.Log("No sound found with name " + audioClip);
        }
    }
}
