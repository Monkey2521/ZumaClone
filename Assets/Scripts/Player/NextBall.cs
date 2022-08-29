using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextBall : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Animator _animator;

    /// <summary>
    /// Initialize next ball preview
    /// </summary>
    /// <param name="color">Next ball color</param>
    public void Init(Color color)
    {
        _renderer.color = color;
    }

    /// <summary>
    /// Reload animation
    /// </summary>
    /// <param name="time">Reload time</param>
    public void PlayAnim(float time)
    {
        _animator.SetTrigger("Reload");

        // TODO update 
    }

}
