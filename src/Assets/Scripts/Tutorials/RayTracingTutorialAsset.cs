using UnityEngine;
using UnityEngine.Experimental.Rendering;

/// <summary>
/// the ray tracing tutorial asset.
/// </summary>
public abstract class RayTracingTutorialAsset : ScriptableObject
{
  /// <summary>
  /// the ray tracing shader.
  /// </summary>
  public RayTracingShader shader;

  /// <summary>
  /// create tutorial.
  /// </summary>
  /// <returns>the tutorial.</returns>
  public abstract RayTracingTutorial CreateTutorial();
}
