using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolItem
{
    public void OnSpawn();
    public void OnRestore();
    public void OnDestroy();
}

public class GenericPool<T> where T : IPoolItem
{
    private Queue<T> pool;
    private Func<T> createInstance;

    public GenericPool(Func<T> createInstance)
    {
        this.createInstance = createInstance;
        pool = new Queue<T>();
    }

    public T Spawn()
    {
        T instance;
        if (pool.Count > 0)
        {
            instance = pool.Dequeue();
        }
        else
        {
            instance = createInstance();
        }

        instance.OnSpawn();
        return instance;
    }

    public void Restore(T instance)
    {
        instance.OnRestore();
        pool.Enqueue(instance);
    }

    public void Clear()
    {
        while (pool.Count > 0)
        {
            var instance = pool.Dequeue();
            instance.OnDestroy();
        }
    }
}