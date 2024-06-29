using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Systems.SceneManagement;

public class SelectedWayspotUi : MonoBehaviour
{
    [SerializeField] TMP_Text _wayspotName;
    [SerializeField] TMP_Text _ownerName;
    [SerializeField] TMP_Text _rangeText;
    [SerializeField] Button _battleButton;

    EventBinding<MapMarkerSelected> MapMarkerSelected;

    static readonly Color BARB_COLOR = new Color(0.70f, 0.09f, 0f);

    void OnEnable()
    {
        MapMarkerSelected = new EventBinding<MapMarkerSelected>(HandleMapMarkerSelected);
        Bus<MapMarkerSelected>.Register(MapMarkerSelected);
    }

    void OnDisable()
    {
        Bus<MapMarkerSelected>.Unregister(MapMarkerSelected);
    }

    void HandleMapMarkerSelected(MapMarkerSelected @event)
    {
        Blackboard.Instance.WayspotIdentifier = "";
        Blackboard.Instance.BaseConfig = null;

        if (@event.Selected == null) return;

        if (@event.Selected is OutpostMarker outpostMarker)
        {
            bool inRange = RangeCheck(outpostMarker.transform.position);

            if (inRange)
            {
                Blackboard.Instance.WayspotIdentifier = "Outpost: " + outpostMarker.Identifier.ToHexString();
                Blackboard.Instance.BaseConfig = outpostMarker.BaseConfig;
            }

            _wayspotName.text = "Outpost";
            _ownerName.text = "Barbarian Outpost";
            _ownerName.color = BARB_COLOR;
            _rangeText.text = inRange ? "" : "Walk closer to battle this outpost.";
            _rangeText.gameObject.SetActive(!inRange);
            _battleButton.gameObject.SetActive(inRange);
        }

        if (@event.Selected is VpsMarker vpsMarker)
        {
            _wayspotName.text = vpsMarker.VpsTarget.Name;

            bool inRange = RangeCheck(vpsMarker.transform.position);

            if (Player.Instance.DoesPlayerControlWayspot(vpsMarker.VpsTarget.Identifier))
            {
                _ownerName.color = Player.Instance.Color;
                _ownerName.text = "Castle under your control!";
                _rangeText.gameObject.SetActive(false);
                _battleButton.gameObject.SetActive(false);
            }
            else
            {
                if (inRange)
                {
                    Blackboard.Instance.WayspotIdentifier = vpsMarker.VpsTarget.Identifier;
                    Blackboard.Instance.BaseConfig = vpsMarker.BaseConfig;
                }

                _ownerName.color = BARB_COLOR;
                _ownerName.text = "Barbarian Encampment";
                _rangeText.text = inRange ? "" : "Walk closer to battle this castle.";
                _rangeText.gameObject.SetActive(!inRange);
                _battleButton.gameObject.SetActive(inRange);
            }
        }
    }

    bool RangeCheck(Vector3 markerPosition)
    {
        // Range Check
        var playerPos = PlayerLocationController.Instance.transform.position;
        var hitObject = markerPosition;
        hitObject.y = playerPos.y;

        // TODO: should calculate this based on GPS LatLng?
        return Vector3.Distance(playerPos, hitObject) <= PlayerLocationController.BATTLE_RANGE;
    }

    public void BattleButtonPressed()
    {
        if (!Player.Instance.HasAnyUnits)
        {
            ToastMessage.Instance.EnqueueMessage("You are out of units!");
            return;
        }

        SceneLoader.Instance.LoadSceneGroup(SceneGroupIds.BATTLE);
    }
}