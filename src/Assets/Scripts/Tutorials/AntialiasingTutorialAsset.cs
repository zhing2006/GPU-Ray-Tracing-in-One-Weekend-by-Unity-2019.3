using UnityEngine;

/// <summary>
/// the antialiasing tutorial asset.
/// </summary>
[CreateAssetMenu(fileName = "AntialiasingTutorialAsset", menuName = "Rendering/AntialiasingTutorialAsset")]
public class AntialiasingTutorialAsset : RayTracingTutorialAsset
{
  /// <summary>
  /// create tutorial.
  /// </summary>
  /// <returns>the tutorial.</returns>
  public override RayTracingTutorial CreateTutorial()
  {
    return new AntialiasingTutorial(this);
  }
}
