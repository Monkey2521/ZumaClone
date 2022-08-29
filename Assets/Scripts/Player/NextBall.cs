using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextBall : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Animator _animator;

    public void Init(Color color)
    {
        _renderer.color = color;
    }

    public void PlayAnim(float time)
    {
        _animator.SetTrigger("Reload");
    }

}
