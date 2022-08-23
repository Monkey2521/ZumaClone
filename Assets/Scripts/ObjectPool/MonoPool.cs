using System.Collections.Generic;
using UnityEngine;

public sealed class MonoPool<TObject> : ObjectPool<TObject> where TObject : MonoBehaviour
{
    private TObject _prefab;
    private Transform _poolParent;

    public MonoPool(List<TObject> objects) : base(objects) { }

    public MonoPool(TObject[] objects) : base(objects) { }

    public MonoPool(TObject obj, int capacity, Transform poolParent)
    {
        _prefab = obj;
        _poolParent = poolParent;

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

        _objects.Add(Object.Instantiate(_prefab, _poolParent));
    }

    public override void ReleaseObject(TObject obj)
    {
        obj.gameObject.SetActive(false);
        base.ReleaseObject(obj);
    }

    public override void ReleaseObjects(List<TObject> objects)
    {
        foreach (var obj in objects)
        {
            obj.gameObject.SetActive(false);
        }

        base.ReleaseObjects(objects);
    }

    public override void ReleaseObjects(TObject[] objects)
    {
        foreach (var obj in objects)
        {
            obj.gameObject.SetActive(false);
        }

        base.ReleaseObjects(objects);
    }

    public override void ClearPool()
    {
        foreach(var obj in _objects)
        {
            Object.Destroy(obj.gameObject);
        }

        base.ClearPool();
    }
}
