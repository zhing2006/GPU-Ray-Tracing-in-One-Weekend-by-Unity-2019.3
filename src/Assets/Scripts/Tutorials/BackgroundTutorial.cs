using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// the background tutorial.
/// </summary>
public class BackgroundTutorial : RayTracingTutorial
{
  /// <summary>
  /// constructor.
  /// </summary>
  /// <param name="asset">the tutorial asset.</param>
  public BackgroundTutorial(RayTracingTutorialAsset asset) : base(asset)
  {
  }

  /// <summary>
  /// render.
  /// </summary>
  /// <param name="context">the render context.</param>
  /// <param name="camera">the camera.</param>
  public override void Render(ScriptableRenderContext context, Camera camera)
  {
    base.Render(context, camera);
    var outputTarget = RequireOutputTarget(camera);

    var cmd = CommandBufferPool.Get(typeof(OutputColorTutorial).Name);
    try
    {
      using (new ProfilingSample(cmd, "RayTracing"))
      {
        cmd.SetRayTracingShaderPass(_shader, "RayTracing");
        cmd.SetRayTracingTextureParam(_shader, _outputTargetShaderId, outputTarget);
        cmd.DispatchRays(_shader, "BackgroundRayGenShader", (uint) outputTarget.rt.width, (uint) outputTarget.rt.height, 1, camera);
      }
      context.ExecuteCommandBuffer(cmd);

      using (new ProfilingSample(cmd, "FinalBlit"))
      {
        cmd.Blit(outputTarget, BuiltinRenderTextureType.CameraTarget, Vector2.one, Vector2.zero);
      }
      context.ExecuteCommandBuffer(cmd);
    }
    finally
    {
      CommandBufferPool.Release(cmd);
    }
  }
}
