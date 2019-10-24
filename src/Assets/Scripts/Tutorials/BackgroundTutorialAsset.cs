using UnityEngine;

/// <summary>
/// the background tutorial asset.
/// </summary>
[CreateAssetMenu(fileName = "BackgroundTutorialAsset", menuName = "Rendering/BackgroundTutorialAsset")]
public class BackgroundTutorialAsset : RayTracingTutorialAsset
{
  /// <summary>
  /// create tutorial.
  /// </summary>
  /// <returns>the tutorial.</returns>
  public override RayTracingTutorial CreateTutorial()
  {
    return new BackgroundTutorial(this);
  }
}