using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Serializable]
    public class CollectibleSound
    {
        public string collectibleId;
        public AudioClip clip;
    }

    [SerializeField] private ImageTrackedCollectibles collectibles;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<CollectibleSound> sounds = new();

    private readonly Dictionary<string, AudioClip> soundsById = new();

    private void Awake()
    {
        foreach (var sound in sounds)
        {
            if (sound.clip != null && !string.IsNullOrWhiteSpace(sound.collectibleId))
            {
                soundsById[sound.collectibleId] = sound.clip;
            }
        }
    }

    private void OnEnable()
    {
        collectibles.CollectibleCollected += PlayCollectibleSound;
    }

    private void OnDisable()
    {
        collectibles.CollectibleCollected -= PlayCollectibleSound;
    }

    private void PlayCollectibleSound(string collectibleId)
    {
        if (soundsById.TryGetValue(collectibleId, out var clip))
        {
            audioSource.PlayOneShot(clip);
        }
    }
}