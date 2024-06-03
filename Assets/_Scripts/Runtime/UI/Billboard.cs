using Unity.VisualScripting;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform Target;

    float smoothTime = 0.3f;
    Vector3 velocity = Vector3.zero;

    void Update()
    {
        var target = Target.position;
        target.y = transform.position.y;
        var lookAt = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);

        transform.LookAt(lookAt);
    }
}