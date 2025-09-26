using UnityEngine;

namespace SimplyPairs
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource Source;

        [Header("Clips")]
        public AudioClip flipClip;
        public AudioClip placeClip;
        public AudioClip matchClip;
        public AudioClip mismatchClip;
        public AudioClip gameOverClip;

        private void Awake()
        {
            instance = this;
        }

        public void PlayPlace()
        {
            PlayClip(placeClip);
        }

        public void PlayFlip()
        {
            PlayClip(flipClip);
        }

        public void PlayMatch()
        {
            PlayClip(matchClip);
        }

        public void PlayMismatch()
        {
            PlayClip(mismatchClip);
        }

        public void PlayGameOver()
        {
            PlayClip(gameOverClip);
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip != null && Source != null)
                Source.PlayOneShot(clip);
        }
    }
}
