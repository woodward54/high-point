using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    private static readonly object _instanceLock = new object();
    private static bool _quitting = false;

    public static T Instance
    {
        get
        {
            lock (_instanceLock)
            {
                if (_instance == null && !_quitting)
                {

                    _instance = GameObject.FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).ToString());
                        _instance = go.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = gameObject.GetComponent<T>();
        }
        else if (_instance.GetInstanceID() != GetInstanceID())
        {
            Destroy(gameObject);
            throw new System.Exception(string.Format("Instance of {0} already exists, removing {1}", GetType().FullName, ToString()));
        }

        OnAwake();
    }

    protected virtual void OnApplicationQuit()
    {
        _quitting = true;
    }

    protected virtual void OnAwake() { }
}

public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    [Tooltip("if this is true, this singleton will auto detach if it finds itself parented on awake")]
    public bool UnparentOnAwake = true;

    sealed protected override void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (UnparentOnAwake)
        {
            transform.SetParent(null);
        }
        
        if (_instance == null)
        {
            _instance = gameObject.GetComponent<T>();
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance.GetInstanceID() != GetInstanceID())
        {
            Destroy(gameObject);
            throw new System.Exception(string.Format("Instance of {0} already exists, removing {1}", GetType().FullName, ToString()));
        }

        OnAwake();
    }
}