using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unsorted/Available colors", fileName = "New colors")]
public class AvailableColors : ScriptableObject
{
    [SerializeField] private List<Color> _colors;

    public List<Color> Colors => _colors;

    public Color GetRandomColor() => _colors[Random.Range(0, _colors.Count)];
}
