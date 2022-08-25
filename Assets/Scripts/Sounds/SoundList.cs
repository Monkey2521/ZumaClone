using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundList
{
    [SerializeField] private List<Sound> _sounds = new List<Sound>();

    public Sound this[string name]
    {
        get
        {
            foreach(var sound in _sounds)
            {
                if (sound.Name == name)
                {
                    return sound;
                }
            }

            return null;
        }
    }
}
