using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Displays live AR tracking info: number of feature points, number of planes,
/// and the position / orientation of the phone camera.
/// </summary>
public class ARDebugInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private ARPointCloudManager pointCloudManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private Camera arCamera;

    private readonly StringBuilder _sb = new();

    private void Awake()
    {
        // Fallbacks in case references were not assigned in the Inspector.
        if (infoText == null) infoText = GetComponent<TMP_Text>();
        if (pointCloudManager == null) pointCloudManager = FindAnyObjectByType<ARPointCloudManager>();
        if (planeManager == null) planeManager = FindAnyObjectByType<ARPlaneManager>();
        if (arCamera == null) arCamera = Camera.main;
    }

    private void Update()
    {
        if (infoText == null) return;

        // Feature points: sum of the points in every tracked point cloud.
        int featurePoints = 0;
        if (pointCloudManager != null)
        {
            foreach (var pointCloud in pointCloudManager.trackables)
            {
                if (pointCloud.positions.HasValue)
                    featurePoints += pointCloud.positions.Value.Length;
            }
        }

        // Planes: ignore planes that were merged into (subsumed by) another plane.
        int planes = 0;
        if (planeManager != null)
        {
            foreach (var plane in planeManager.trackables)
            {
                if (plane.subsumedBy == null)
                    planes++;
            }
        }

        _sb.Clear();
        _sb.Append("Feature points: ").Append(featurePoints).Append('\n');
        _sb.Append("Planes: ").Append(planes).Append('\n');

        if (arCamera != null)
        {
            // The TrackedPoseDriver writes the device pose into the camera's local transform,
            // so the local pose is the phone pose in AR session space (relative to where tracking started).
            Transform cam = arCamera.transform;
            Vector3 p = cam.localPosition;
            Vector3 r = cam.localEulerAngles;
            _sb.AppendFormat("Camera position (m): X {0:F2}  Y {1:F2}  Z {2:F2}\n", p.x, p.y, p.z);
            _sb.AppendFormat("Camera rotation (deg): X {0:F1}  Y {1:F1}  Z {2:F1}", r.x, r.y, r.z);
        }

        infoText.SetText(_sb);
    }
}
