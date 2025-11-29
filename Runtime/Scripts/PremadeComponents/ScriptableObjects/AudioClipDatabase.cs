using System;
using System.Collections.Generic;
using System.Linq;
using Types;
using UnityEngine;

namespace PremadeComponents.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Audio Clip Database", menuName = Consts.PackageCreateAssetMenuCategory + "AudioClipDatabase")]
    public class AudioClipDatabase : ScriptableObject
    {
        [SerializeField] private List<Structs.KeyToAudioClip> _audioClips = new();
        private Dictionary<string, AudioClip> audioClips = new();

        [field: SerializeField, Range(0, 1)] public float MasterVolume { get; private set; } = 1f;
        [field: SerializeField, Range(0, 1)] public float MusicVolume { get; private set; } = 0.5f;
        [field: SerializeField, Range(0, 1)] public float SfxVolume { get; private set; } = 0.5f;
        
        public void RefreshAudioClips()
        {
            audioClips.Clear();
            foreach (var clip in _audioClips.Where(clip => !audioClips.TryAdd(clip.Key, clip.Clip)))
            {
                Debug.LogWarning("Duplicate clip key found in Audio Clip List. Removing");
                _audioClips.Remove(clip);
            }
        }
        
        public AudioClip TryGetAudioClip(string key) => audioClips.GetValueOrDefault(key);
        
        public bool TryGetAudioClip(string key, out AudioClip clip) => audioClips.TryGetValue(key, out clip);
        
        public List<string> GetKeys() => audioClips.Keys.ToList();

        public List<string> GetKeys(Func<string, bool> predicate) => audioClips.Keys.Where(predicate.Invoke).ToList();
        
#if UNITY_EDITOR
        public void Editor_Add(AudioClip clip)
        {
            if (_audioClips.Any(c => c.Clip == clip)) return;

            _audioClips.Add(new(clip.name, clip));
        }
        
        public void Editor_Add(string key, AudioClip clip)
        {
            if (_audioClips.Any(c => c.Clip == clip && c.Key == key)) return;
            
            _audioClips.Add(new(key, clip));
        }

        public void Editor_Remove(string key) => _audioClips.Remove(_audioClips.First(c => c.Key == key));
        
        public void Editor_Remove(AudioClip clip) => _audioClips.Remove(_audioClips.First(c => c.Clip == clip));
        
        public void Editor_Remove(string key, AudioClip clip) => _audioClips.Remove(_audioClips.First(c => c.Key == key && c.Clip == clip));
#endif

        private void OnValidate()
        {
            foreach (var clip in _audioClips.Where(clip => clip.Key == string.Empty && clip.Clip))
            {
                clip.ReplaceKey(clip.Clip.name);
            }
            RefreshAudioClips();
        }
    }
}