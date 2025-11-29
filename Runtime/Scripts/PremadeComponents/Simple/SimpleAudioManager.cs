using PremadeComponents.ScriptableObjects;
using PremadeComponents.Singleton;
using Types;
using UnityEngine;

namespace PremadeComponents.Simple
{
    [AddComponentMenu(Consts.PackageComponentCategory + Consts.SimpleSubCategory + "SimpleAudioManager"), Tooltip("Used as an extension for a singular AudioSource")]
    public class SimpleAudioManager : MonoBehaviour
    {
        [field: SerializeField] public AudioClipDatabase AudioClipDatabase { get; private set; }
        
        [field: SerializeField] public AudioSource AudioSource { get; private set; }

        private void Start()
        {
            if(SingletonAudioManager.Instance) Debug.Log("Singleton Audio Manager Detected. I highly discourage using both Simple and Singleton managers at the same time, as it can lead to confusion.");
        }

        public bool TryPlayOneShot(string key)
        {
            if (!AudioClipDatabase.TryGetAudioClip(key, out var clip)) return false;
            AudioSource.volume = AudioClipDatabase.MasterVolume * AudioClipDatabase.SfxVolume;
            AudioSource.PlayOneShot(clip);
            AudioSource.volume = AudioClipDatabase.MasterVolume * AudioClipDatabase.MusicVolume;
            return true;
        }
        
        public bool TryPlayMusic(string key, bool isLooping = true, bool resumeFromCurrentTime = false)
        {
            if (!AudioClipDatabase.TryGetAudioClip(key, out var clip)) return false;
            float currentTime = AudioSource.time;
            AudioSource.Stop();
            AudioSource.clip = clip;
            AudioSource.loop = isLooping;
            if (resumeFromCurrentTime) AudioSource.time = currentTime;
            AudioSource.Play();
            return true;
        }
        
        public bool TryPlayMusic(string key, bool isLooping = true, float songStartPos = 0f)
        {
            if (!AudioClipDatabase.TryGetAudioClip(key, out var clip)) return false;
            AudioSource.Stop();
            AudioSource.clip = clip;
            AudioSource.loop = isLooping;
            AudioSource.time = songStartPos;
            AudioSource.Play();
            return true;
        }
        
        public bool TryPlayMusicWithDelay(string key, bool isLooping, bool resumeFromCurrentTime, double delay)
        {
            if (!AudioClipDatabase.TryGetAudioClip(key, out var clip)) return false;
            float currentTime = AudioSource.time;
            AudioSource.Stop();
            AudioSource.clip = clip;
            AudioSource.loop = isLooping;
            if (resumeFromCurrentTime) AudioSource.time = currentTime;
            AudioSource.PlayScheduled(delay);
            return true;
        }

        public void ModifyPitch(float pitch) => AudioSource.pitch = Mathf.Clamp(pitch, -3, 3);
    }
}