using UnityEngine;

/// <summary>
/// the camera tutorial asset.
/// </summary>
[CreateAssetMenu(fileName = "CameraTutorialAsset", menuName = "Rendering/CameraTutorialAsset")]
public class CameraTutorialAsset : RayTracingTutorialAsset
{
  /// <summary>
  /// create tutorial.
  /// </summary>
  /// <returns>the tutorial.</returns>
  public override RayTracingTutorial CreateTutorial()
  {
    return new CameraTutorial(this);
  }
}
