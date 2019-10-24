using UnityEngine;
using UnityEngine.Experimental.Rendering;

/// <summary>
/// the scene manager.
/// </summary>
public class SceneManager : MonoBehaviour
{
  const int maxNumSubMeshes = 32;
  private bool[] subMeshFlagArray = new bool[maxNumSubMeshes];
  private bool[] subMeshCutoffArray = new bool[maxNumSubMeshes];

  /// <summary>
  /// the instance.
  /// </summary>
  private static SceneManager s_Instance;

  /// <summary>
  /// get instance.
  /// </summary>
  public static SceneManager Instance
  {
    get
    {
      if (s_Instance != null) return s_Instance;

      s_Instance = GameObject.FindObjectOfType<SceneManager>();
      s_Instance?.Init();
      return s_Instance;
    }
  }

  /// <summary>
  /// all renderers.
  /// </summary>
  public Renderer[] renderers;

  /// <summary>
  /// whether need to rebuild acceleration structure.
  /// </summary>
  [System.NonSerialized]
  public bool isDirty = true;

  /// <summary>
  /// Unity Awake.
  /// </summary>
  public void Awake()
  {
    if (Application.isPlaying)
      DontDestroyOnLoad(this);

    isDirty = true;
  }

  /// <summary>
  /// fill ray tracing acceleration structure.
  /// </summary>
  /// <param name="accelerationStructure">the acceleration structure.</param>
  public void FillAccelerationStructure(ref RayTracingAccelerationStructure accelerationStructure)
  {
    foreach (var r in renderers)
    {
      if (r)
        accelerationStructure.AddInstance(r, subMeshFlagArray, subMeshCutoffArray);
    }
  }

  /// <summary>
  /// initialize
  /// </summary>
  private void Init()
  {
    for (var i = 0; i < maxNumSubMeshes; ++i)
    {
      subMeshFlagArray[i] = true;
      subMeshCutoffArray[i] = false;
    }
  }
}
