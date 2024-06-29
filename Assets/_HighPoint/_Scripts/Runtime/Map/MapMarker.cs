using DG.Tweening;
using UnityEngine;

public class MapMarker : MonoBehaviour
{
    void OnEnable()
    {
        PlayerLocationController.OnNewGpsPosition += HandleNewGpsPosition;
    }

    void OnDisable()
    {
        PlayerLocationController.OnNewGpsPosition -= HandleNewGpsPosition;
    }

    void Start()
    {
        FacePlayer();
    }

    void HandleNewGpsPosition(Niantic.Lightship.Maps.Core.Coordinates.LatLng lng)
    {
        FacePlayer();
    }

    void FacePlayer()
    {
        var relativePos = PlayerLocationController.Instance.transform.position - transform.position;
        var lookAngle = Quaternion.LookRotation(relativePos);
        var newRotation = Quaternion.Euler(0f, lookAngle.eulerAngles.y, 0f);

        // Only rotate if the new rotation is quite a bit different
        if (Quaternion.Angle(newRotation, transform.rotation) >= 10f) 
        {
            // transform.rotation = newRotation;
            transform.DORotateQuaternion(newRotation, 0.5f).SetEase(Ease.OutExpo);
        }
    }
}