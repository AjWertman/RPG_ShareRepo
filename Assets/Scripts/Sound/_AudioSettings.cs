using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Sound
{
    /// <summary>
    /// Used to duplicate an AudioSource's settings to be edited in the inspector.
    /// </summary>
    [Serializable]
    public class _AudioSettings
    {
        [SerializeField] bool loop = false;

        [Range(0, 100)] [SerializeField] int volume = 100;

        [Tooltip("Sets the frequency of the sound. Use this to slow down or speed up the sound")]
        [Range(-3, 3)] [SerializeField] int pitch = 1;


        //Refactor 3D

        //Distance from audio source (for testing)
        ///Include a key to describe how far away a Unity unit is
        ///Maybe create a demonstration? or place an immovable player in the scene near an audio source
    }
}