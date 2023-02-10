using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleShape
{
    [CreateAssetMenu]
    public class SO_settings : ScriptableObject
    {
        public Sprite chosenSprite;
        public bool soundOn;

        [Header("Audio Packs")]
        public AudioPack buttonClick;
        public AudioPack hoverPicture;
        public AudioPack choosePicture;
        public AudioPack puzzlePickup;
        public AudioPack puzzleSet;
        public AudioPack puzzleReturnToOutside;
        public AudioPack win;
    }
    [System.Serializable]
    public struct AudioPack
    {
        public AudioClip klip;
        [Range(0f, 1f)] public float volume;
        [Range(-3f, 3f)] public float pitch;
    }
}
