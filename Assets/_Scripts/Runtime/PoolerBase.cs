using System;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// A simple base class to simplify object pooling in Unity 2021.
/// Derive from this class, call InitPool and you can Get and Release to your hearts content.
/// If you enjoyed the video or this script, make sure you give me a like on YT and let me know what you thought :)
/// </summary>
/// <typeparam name="T">A MonoBehaviour object you'd like to perform pooling on.</typeparam>
public abstract class PoolerBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private T _prefab;
    private ObjectPool<T> _pool;

    protected ObjectPool<T> Pool
    {
        get
        {
            if (_pool == null) throw new InvalidOperationException("You need to call InitPool before using it.");
            return _pool;
        }
        set => _pool = value;
    }

    protected void InitPool(T prefab, int initial = 10, int max = 20, bool collectionChecks = false)
    {
        _prefab = prefab;
        Pool = new ObjectPool<T>(
            CreatePooledItem,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionChecks,
            initial,
            max);
    }

    #region Overrides
    protected virtual T CreatePooledItem() => Instantiate(_prefab);
    protected virtual void OnTakeFromPool(T obj) => obj.gameObject.SetActive(true);
    protected virtual void OnReturnedToPool(T obj) => obj.gameObject.SetActive(false);
    protected virtual void OnDestroyPoolObject(T obj) => Destroy(obj.gameObject);
    #endregion

    #region Getters
    public T Get() => Pool.Get();
    public void Release(T obj) => Pool.Release(obj);
    #endregion
}