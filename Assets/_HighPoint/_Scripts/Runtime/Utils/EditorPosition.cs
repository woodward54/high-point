using UnityEngine;

public class EditorPosition : MonoBehaviour
{
    [SerializeField] Vector3 _position;

    void Start()
    {
#if UNITY_EDITOR
        transform.position = _position;
#endif
    }
}