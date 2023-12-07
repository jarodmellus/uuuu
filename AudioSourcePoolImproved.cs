
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;

namespace VXRLabs.Audio {
    public class AudioSourcePoolImproved : MonoBehaviour
    {
        
        [SerializeField]
        int defaultPoolSize = 12;
        [SerializeField]
        int maxPoolOverflow;
        [ShowNonSerializedField]
        int currentPoolOverflow;
        List<AudioSource> sourceQueue = new List<AudioSource>();

        public static AudioSourcePoolImproved Instance;
        
        void Awake()
        {
            Instance = this;

            InitializePool();
        }

        List<AudioSource> audioSources = new List<AudioSource>();

        void InitializePool()
        {
            for(int i = 0; i<defaultPoolSize; i++) {
                GameObject obj = new GameObject();
                obj.transform.SetParent(transform);
                obj.name = "Audio Source " + i.ToString();
                obj.SetActive(false);
                AudioSource src = obj.AddComponent<AudioSource>();
                audioSources.Add(src);
            }
        }
        public void RequestPlaySound(AudioSourcePoolClip clip)
        {
            RequestPlaySound(clip.clipData);
        }
        public void RequestPlaySound(AudioData data) {
            AudioSource source;
            for (int i = 0; i < audioSources.Count; i++)
            {
                source = audioSources[i];
                if(!source.gameObject.activeSelf) {
                    source.gameObject.SetActive(true);
                    PlaySound(audioSources[i], data);
                    return;
                }       
            }

            //No available source
            source = sourceQueue[sourceQueue.Count-1];
            sourceQueue.Remove(source);
            source.gameObject.SetActive(true);
            source.Stop();
            PlaySound(audioSources[0], data);
        }

        [System.Serializable]
        public struct AudioData {
            public AudioClip clip;
            public float pitch;
            public float volume;
            public float spatialBlend;
            public Vector3 position;
            public float maxDistance;
        }

        void PlaySound(AudioSource source, AudioData data) {
            source.clip = data.clip;
            source.pitch = data.pitch;
            source.volume = data.volume;
            source.transform.position = data.position;

            //Todo: Add following to audioData struct
            source.spatialBlend = data.spatialBlend;
            source.maxDistance = data.maxDistance;
            source.minDistance = .01f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.spread = 1f;

            source.time = 0f;
            source.Play();
            sourceQueue.Insert(0,source);
            StartCoroutine(SourceIsPlaying(source));
        }

        IEnumerator SourceIsPlaying(AudioSource source) {
            
            yield return new WaitUntil(()=>!source.isPlaying);
            source.gameObject.SetActive(false);
            sourceQueue.Remove(source);
        }
    }
}
