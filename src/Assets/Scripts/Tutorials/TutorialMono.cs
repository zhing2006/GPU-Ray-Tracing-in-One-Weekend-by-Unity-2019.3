using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// the tutorial mono behaviour.
/// </summary>
public class TutorialMono : MonoBehaviour
{
  /// <summary>
  /// the new render pipeline asset.
  /// </summary>
  public RenderPipelineAsset renderPipelineAsset;

  /// <summary>
  /// the old render pipeline asset.
  /// </summary>
  private RenderPipelineAsset _oldRenderPipelineAsset;

  /// <summary>
  /// Unity Start.
  /// </summary>
  public IEnumerator Start()
  {
    yield return new WaitForEndOfFrame();

    _oldRenderPipelineAsset = GraphicsSettings.renderPipelineAsset;
    GraphicsSettings.renderPipelineAsset = renderPipelineAsset;
  }

  /// <summary>
  /// Unity OnDestroy.
  /// </summary>
  public void OnDestroy()
  {
    GraphicsSettings.renderPipelineAsset = _oldRenderPipelineAsset;
  }
}
