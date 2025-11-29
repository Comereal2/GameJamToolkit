using System.Collections;
using PremadeComponents.ScriptableObjects;
using Types;
using UnityEngine;
using UnityEngine.Pool;

namespace PremadeComponents.Singleton
{
    [AddComponentMenu(Consts.PackageComponentCategory + Consts.SingletonSubCategory + "SingletonAudioManager"), Tooltip("Used as a manager for all AudioSources placed on scene")]
    public class SingletonAudioManager : MonoBehaviour
    {
        public static SingletonAudioManager Instance { get; private set; }
        
        [field: SerializeField] public AudioClipDatabase AudioClipDatabase { get; private set; }
        [SerializeField, Tooltip("This AudioSource should be directly on the camera as a non-directional audio output")] private AudioSource cameraAudioSource;

        [SerializeField, Tooltip("Maximum amount of AudioSources")] private int maxAudioSourceAmount = 10;

        private ObjectPool<AudioSource> audioSources;
        
        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("Secondary Singleton Audio Manager detected. This should not happen. Deleting original instance.");   
                Destroy(Instance);
            }
            Instance = this;

            audioSources = new(OnCreateAudioSource, OnTakeFromPool, OnReturnToPool, OnDestroyAudioSource, maxSize: maxAudioSourceAmount);
        }

        #region CameraAudioSourceFunctions

        public bool TryPlayOneShot(string key) => 
            TryPlayOneShot(key, cameraAudioSource);

        public bool TryPlayMusic(string key, bool isLooping = true, bool resumeFromCurrentTime = false) => 
            TryPlayMusic(key, isLooping, resumeFromCurrentTime ? cameraAudioSource.time : 0f);

        public bool TryPlayMusic(string key, bool isLooping = true, float songStartPos = 0f) => 
            TryPlayMusic(key, cameraAudioSource, isLooping, songStartPos);

        public bool TryPlayMusicWithDelay(string key, float delay, bool isLooping = true, bool resumeFromCurrentTime = false) => 
            TryPlayMusicWithDelay(key, delay, isLooping, resumeFromCurrentTime ? cameraAudioSource.time : 0f);

        public bool TryPlayMusicWithDelay(string key, float delay, bool isLooping = true, float songStartPos = 0f) =>
            TryPlayMusicWithDelay(key, cameraAudioSource, delay, isLooping, songStartPos);

        public void ModifyPitch(float pitch) => ModifyPitch(pitch, cameraAudioSource);

        #endregion

        #region AudioSourcePoolFunctions

        #region PoolBehavior
        
        private AudioSource OnCreateAudioSource()
        {
            AudioSource audioSource = new GameObject("Audio Source", typeof(AudioSource)).GetComponent<AudioSource>();
            audioSource.gameObject.SetActive(false);
            return audioSource;
        }

        private void OnTakeFromPool(AudioSource audioSource)
        {
            audioSource.gameObject.SetActive(true);
        }

        private void OnReturnToPool(AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.gameObject.SetActive(false);
        }

        private void OnDestroyAudioSource(AudioSource audioSource)
        {
            if(audioSource) Destroy(audioSource.gameObject);
        }
        
        #endregion
        
        public bool TryPlayOneShot(string key, Vector3 position, bool isPositionGlobal = true) => TryPlayOneShot(key, GetAndSetSource(position, isPositionGlobal), true);
        
        public AudioSource TryPlayMusic(string key, Vector3 position, bool isPositionGlobal = true, bool isLooping = true, float songStartPos = 0f)
        {
            var src = GetAndSetSource(position, isPositionGlobal);
            return TryPlayMusic(key, src, isLooping, songStartPos, true) ? src : null;
        }

        public AudioSource TryPlayMusicWithDelay(string key, float delay, Vector3 position, bool isPositionGlobal = true, bool isLooping = true, float songStartPos = 0f)
        {
            var src = GetAndSetSource(position, isPositionGlobal);
            return TryPlayMusicWithDelay(key, src, delay, isLooping, songStartPos, true) ? src : null;
        }

        private IEnumerator ReturnToPoolAfterTime(float time, AudioSource source)
        {
            yield return new WaitForSeconds(time);
            audioSources.Release(source);
        }

        private AudioSource GetAndSetSource(Vector3 pos, bool isPosGlobal = true)
        {
            var source = audioSources.Get();
            if (isPosGlobal) source.transform.position = pos;
            else source.transform.localPosition = pos;
            return source;
        }

        #endregion

        #region Functionality

        private bool TryPlayOneShot(string key, AudioSource source, bool returnToPool = false)
        {
            if (!AudioClipDatabase.TryGetAudioClip(key, out var clip)) return false;
            source.volume = AudioClipDatabase.MasterVolume * AudioClipDatabase.SfxVolume;
            source.PlayOneShot(clip);
            source.volume = AudioClipDatabase.MasterVolume * AudioClipDatabase.MusicVolume;
            if(returnToPool) StartCoroutine(ReturnToPoolAfterTime(clip.length, source));
            return true;
        }

        private bool TryPlayMusic(string key, AudioSource source, bool isLooping = true, float songStartPos = 0f, bool returnToPool = false)
        {
            if (!AudioClipDatabase.TryGetAudioClip(key, out var clip)) return false;
            source.Stop();
            source.clip = clip;
            source.loop = isLooping;
            source.time = songStartPos;
            source.Play();
            if(returnToPool && !isLooping) StartCoroutine(ReturnToPoolAfterTime(clip.length - songStartPos, source));
            return true;
        }

        private bool TryPlayMusicWithDelay(string key, AudioSource source, float delay, bool isLooping = true, float songStartPos = 0f, bool returnToPool = false)
        {
            if (!AudioClipDatabase.TryGetAudioClip(key, out var clip)) return false;
            source.Stop();
            source.clip = clip;
            source.loop = isLooping;
            source.time = songStartPos;
            source.PlayDelayed(delay);
            if(returnToPool && !isLooping) StartCoroutine(ReturnToPoolAfterTime(clip.length + delay - songStartPos, source));
            return true;
        }
        
        public void ModifyPitch(float pitch, AudioSource source) => source.pitch = Mathf.Clamp(pitch, -3, 3);

        #endregion
    }
}