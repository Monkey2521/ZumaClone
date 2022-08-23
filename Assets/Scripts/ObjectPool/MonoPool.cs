using System.Collections.Generic;
using UnityEngine;

public sealed class MonoPool<TObject> : ObjectPool<TObject> where TObject : MonoBehaviour
{
    private TObject _prefab;
    private Transform _objParent;

    public MonoPool(List<TObject> objects) : base(objects) { }

    public MonoPool(TObject[] objects) : base(objects) { }

    public MonoPool(TObject obj, int capacity, Transform parent)
    {
        _prefab = obj;
        _objParent = parent;

        _objects = new List<TObject>(capacity);

        for (int i = 0; i < capacity;  i++)
        {
            CreateObject();
        }
    }

    protected override void CreateObject()
    {
        if (_objects == null)
        {
            _objects = new List<TObject>();
        }

        _objects.Add(Object.Instantiate(_prefab, _objParent));
    }

    public override void Release(TObject obj)
    {
        obj.gameObject.SetActive(false);
        base.Release(obj);
    }
}
