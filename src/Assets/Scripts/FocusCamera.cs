using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// the focus camera.
/// </summary>
[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class FocusCamera : MonoBehaviour
{
  /// <summary>
  /// the focus distance.
  /// </summary>
  public float focusDistance = 10.0f;
  /// <summary>
  /// the len aperture.
  /// </summary>
  public float aperture = 1.0f;

  [System.NonSerialized] public Vector3 leftBottomCorner;
  [System.NonSerialized] public Vector3 rightTopCorner;
  [System.NonSerialized] public Vector2 size;

  private Camera thisCamera;

  /// <summary>
  /// Unity Update.
  /// </summary>
  public void Update()
  {
    if (thisCamera == null)
      thisCamera = GetComponent<Camera>();

    var theta = thisCamera.fieldOfView * Mathf.Deg2Rad;
    var halfHeight = math.tan(theta * 0.5f);
    var halfWidth = thisCamera.aspect * halfHeight;
    leftBottomCorner = transform.position + transform.forward * focusDistance -
                       transform.right * focusDistance * halfWidth -
                       transform.up * focusDistance * halfHeight;
    size = new Vector2(focusDistance * halfWidth * 2.0f, focusDistance * halfHeight * 2.0f);
    rightTopCorner = leftBottomCorner + transform.right * size.x + transform.up * size.y;
  }

  /// <summary>
  /// Unity OnDrawGizmosSelected
  /// </summary>
  public void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    var pt1 = leftBottomCorner;
    var pt2 = pt1 + transform.right * size.x;
    var pt3 = rightTopCorner;
    var pt4 = pt1 + transform.up * size.y;
    Gizmos.DrawLine(pt1, pt2);
    Gizmos.DrawLine(pt2, pt3);
    Gizmos.DrawLine(pt3, pt4);
    Gizmos.DrawLine(pt4, pt1);
    Gizmos.DrawLine(pt1, pt3);
    Gizmos.DrawLine(pt2, pt4);
    Gizmos.DrawWireSphere(transform.position, aperture * 0.5f);
    Gizmos.color = Color.white;
  }
}
