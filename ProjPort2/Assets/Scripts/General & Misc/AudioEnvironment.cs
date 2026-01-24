using UnityEngine;

public class AudioEnvironment : MonoBehaviour
{
    [SerializeField] AudioClip newTrack;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.instance.SwapTrack(newTrack);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.instance.ReturnToDefaultTrack();
        }
    }
}
