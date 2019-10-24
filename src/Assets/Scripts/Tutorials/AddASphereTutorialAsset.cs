using UnityEngine;

/// <summary>
/// add a sphere tutorial asset.
/// </summary>
[CreateAssetMenu(fileName = "AddASphereTutorialAsset", menuName = "Rendering/AddASphereTutorialAsset")]
public class AddASphereTutorialAsset : RayTracingTutorialAsset
{
  /// <summary>
  /// create tutorial.
  /// </summary>
  /// <returns>the tutorial.</returns>
  public override RayTracingTutorial CreateTutorial()
  {
    return new AddASphereTutorial(this);
  }
}
