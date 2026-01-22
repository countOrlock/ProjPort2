using UnityEngine;

public class AudioEnvironment : MonoBehaviour
{
    [SerializeField] AudioClip newTrack;

    void OnTriggerEnter()
    {
        MusicManager.instance.SwapTrack(newTrack);
    }

    void OnTriggerExit()
    {
        MusicManager.instance.ReturnToDefaultTrack();
    }
}
