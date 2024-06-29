using System.Collections.Generic;
using System.Linq;
using Drawing;
using UnityEngine;

public class Wall : BuildingUnit
{
    [field: Header("Wall Properties")]
    [SerializeField] Transform _wallSegmentPrefab;

    readonly List<Transform> _segments = new();

    protected override void Start()
    {
        base.Start();
    }

    public void Setup(HexCell myCell, List<HexCell> wallNeighbors)
    {
        foreach (var neighbor in wallNeighbors)
        {
            var rot = Quaternion.LookRotation(myCell.WorldPos - neighbor.WorldPos);

            var corners = HexUtils.Corners(HexGrid.Instance.HexSize, HexGrid.Instance.Orientation);

            var i = HexUtils.SideIndex(Mathf.Round(rot.eulerAngles.y)) + 1;
            var a = myCell.WorldPos + corners[i % 6];
            var b = myCell.WorldPos + corners[(i + 1) % 6];
            var edgeMidPoint = (a + b) / 2f;
            var midpoint = (edgeMidPoint + myCell.WorldPos) / 2f;

            // To fix model offset
            rot = Quaternion.Euler(rot.eulerAngles + new Vector3(-90f, 0f, 0f));

            CreateSegment(midpoint, rot);
        }
    }

    Transform CreateSegment(Vector3 position, Quaternion rotation)
    {
        var segment = Instantiate(_wallSegmentPrefab, position, rotation, transform);
        segment.localScale *= HexGrid.Instance.HexSize;
        segment.position = position;

        _segments.Add(segment);
        return segment;
    }

    void ClearSegments()
    {
        foreach (var seg in _segments)
        {
            Destroy(seg.gameObject);
        }

        _segments.Clear();
    }
}