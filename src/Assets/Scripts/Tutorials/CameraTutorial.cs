using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

/// <summary>
/// the camera tutorial.
/// </summary>
public class CameraTutorial : RayTracingTutorial
{
  /// <summary>
  /// the focus camera shader parameters
  /// </summary>
  private static class FocusCameraShaderParams
  {
    public static readonly int _FocusCameraLeftBottomCorner = Shader.PropertyToID("_FocusCameraLeftBottomCorner");
    public static readonly int _FocusCameraRight = Shader.PropertyToID("_FocusCameraRight");
    public static readonly int _FocusCameraUp = Shader.PropertyToID("_FocusCameraUp");
    public static readonly int _FocusCameraSize = Shader.PropertyToID("_FocusCameraSize");
    public static readonly int _FocusCameraHalfAperture = Shader.PropertyToID("_FocusCameraHalfAperture");
  }

  private readonly int _PRNGStatesShaderId = Shader.PropertyToID("_PRNGStates");

  /// <summary>
  /// the frame index.
  /// </summary>
  private int _frameIndex = 0;

  private readonly int _frameIndexShaderId = Shader.PropertyToID("_FrameIndex");

  /// <summary>
  /// constructor.
  /// </summary>
  /// <param name="asset">the tutorial asset.</param>
  public CameraTutorial(RayTracingTutorialAsset asset) : base(asset)
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
    var focusCamera = camera.GetComponent<FocusCamera>();
    if (null == focusCamera)
      return;

    var outputTarget = RequireOutputTarget(camera);
    var outputTargetSize = RequireOutputTargetSize(camera);

    var accelerationStructure = _pipeline.RequestAccelerationStructure();
    var PRNGStates = _pipeline.RequirePRNGStates(camera);

    var cmd = CommandBufferPool.Get(typeof(OutputColorTutorial).Name);
    try
    {
      if (_frameIndex < 1000)
      {
        using (new ProfilingSample(cmd, "RayTracing"))
        {
          cmd.SetRayTracingVectorParam(_shader, FocusCameraShaderParams._FocusCameraLeftBottomCorner, focusCamera.leftBottomCorner);
          cmd.SetRayTracingVectorParam(_shader, FocusCameraShaderParams._FocusCameraRight, focusCamera.transform.right);
          cmd.SetRayTracingVectorParam(_shader, FocusCameraShaderParams._FocusCameraUp, focusCamera.transform.up);
          cmd.SetRayTracingVectorParam(_shader, FocusCameraShaderParams._FocusCameraSize, focusCamera.size);
          cmd.SetRayTracingFloatParam(_shader, FocusCameraShaderParams._FocusCameraHalfAperture, focusCamera.aperture * 0.5f);

          cmd.SetRayTracingShaderPass(_shader, "RayTracing");
          cmd.SetRayTracingAccelerationStructure(_shader, _pipeline.accelerationStructureShaderId,
            accelerationStructure);
          cmd.SetRayTracingIntParam(_shader, _frameIndexShaderId, _frameIndex);
          cmd.SetRayTracingBufferParam(_shader, _PRNGStatesShaderId, PRNGStates);
          cmd.SetRayTracingTextureParam(_shader, _outputTargetShaderId, outputTarget);
          cmd.SetRayTracingVectorParam(_shader, _outputTargetSizeShaderId, outputTargetSize);
          cmd.DispatchRays(_shader, "CameraRayGenShader", (uint) outputTarget.rt.width,
            (uint) outputTarget.rt.height, 1, camera);
        }

        context.ExecuteCommandBuffer(cmd);
        if (camera.cameraType == CameraType.Game)
          _frameIndex++;
      }

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
