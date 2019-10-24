using UnityEngine;

/// <summary>
/// the output color tutorial asset.
/// </summary>
[CreateAssetMenu(fileName = "OutputColorTutorialAsset", menuName = "Rendering/OutputColorTutorialAsset")]
public class OutputColorTutorialAsset : RayTracingTutorialAsset
{
  /// <summary>
  /// create tutorial.
  /// </summary>
  /// <returns>the tutorial.</returns>
  public override RayTracingTutorial CreateTutorial()
  {
    return new OutputColorTutorial(this);
  }
}
