using System.Collections;
using UnityEngine;
using VXRLabs.Audio;
using NaughtyAttributes;

public class AudioSourcePoolClip : MonoBehaviour
{
    [SerializeField] bool playSoundAtPosition;

    [ShowIf("useTransformReferenceInsteadOfVector")]
    [SerializeField]
    Transform transformReferencePositionToPlaySoundAt;
    [ShowIf("playSoundAtPosition")]
    [SerializeField]
    bool useTransformReferenceInsteadOfVector;
    [ShowIf("!useTransformReferenceInsteadOfVector")]
    [SerializeField]
    Vector3 positionToPlaySound;
    

    [SerializeField] float volume=1f;

    [HideIf("useRandomPitch")]
    [SerializeField] float pitch = 1f;

    [SerializeField] float spatialBlend = 1f;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] bool useRandomPitch;
    [ShowIf("useRandomPitch")]
    [SerializeField] Vector2 randomPitchRange = new Vector2(.8f,1.2f);

    [SerializeField]  bool playRandomClip;
    [SerializeField]
    AudioClip[] clips;
    [HideInInspector]
    public AudioSourcePoolImproved.AudioData clipData;
    public void PlaySound()
    {
        clipData = new AudioSourcePoolImproved.AudioData();
        clipData.volume = volume;
        clipData.pitch = useRandomPitch ? Random.Range(randomPitchRange.x, randomPitchRange.y) : clipData.pitch = pitch;
        clipData.spatialBlend = spatialBlend;
        if (playSoundAtPosition) {
            if (useTransformReferenceInsteadOfVector)
                clipData.position = transformReferencePositionToPlaySoundAt.position;
            else
                clipData.position = positionToPlaySound;
        }

        int clipIndex=0;
        if (playRandomClip) clipIndex = Random.Range(0,clips.Length);

        clipData.clip = clips[clipIndex];
        clipData.maxDistance = maxDistance;

        AudioSourcePoolImproved.Instance.RequestPlaySound(clipData);
    }

    public void PlaySound(Transform transform)
    {
        transformReferencePositionToPlaySoundAt = transform;
        PlaySound();
    }

    public void PlaySoundWithPitch(float value) {
        pitch=value;
        PlaySound();
    }
}
