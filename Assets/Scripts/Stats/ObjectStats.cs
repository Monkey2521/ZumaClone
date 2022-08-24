using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Object stats", fileName = "New object stats")]
public class ObjectStats : ScriptableObject
{
    [SerializeField] private int _healthPoints;
    //[SerializeField] private int _damage;

    public int MaxHP => _healthPoints;
    //public int Damage => _damage;
}
