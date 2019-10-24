using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

/// <summary>
/// the ray tracing render pipeline.
/// </summary>
public class RayTracingRenderPipeline : RenderPipeline
{
  /// <summary>
  /// the render pipeline asset.
  /// </summary>
  private RayTracingRenderPipelineAsset _asset;
  /// <summary>
  /// the ray tracing acceleration structure.
  /// </summary>
  private RayTracingAccelerationStructure _accelerationStructure;

  public readonly int accelerationStructureShaderId = Shader.PropertyToID("_AccelerationStructure");

  /// <summary>
  /// all prng states for camera.
  /// </summary>
  private readonly Dictionary<int, ComputeBuffer> _PRNGStates = new Dictionary<int, ComputeBuffer>();

  /// <summary>
  /// the tutorial.
  /// </summary>
  private RayTracingTutorial _tutorial;

  /// <summary>
  /// constructor.
  /// </summary>
  /// <param name="asset">the render pipeline asset.</param>
  public RayTracingRenderPipeline(RayTracingRenderPipelineAsset asset)
  {
    _asset = asset;
    _accelerationStructure = new RayTracingAccelerationStructure();

    _tutorial = _asset.tutorialAsset.CreateTutorial();
    if (_tutorial == null)
    {
      Debug.LogError("Can't create tutorial.");
      return;
    }

    if (_tutorial.Init(this) == false)
    {
      _tutorial = null;
      Debug.LogError("Initialize tutorial failed.");
      return;
    }
  }

  /// <summary>
  /// require the ray tracing acceleration structure.
  /// </summary>
  /// <returns>the ray tracing acceleration structure.</returns>
  public RayTracingAccelerationStructure RequestAccelerationStructure()
  {
    return _accelerationStructure;
  }

  /// <summary>
  /// require a PRNG compute buffer for camera.
  /// </summary>
  /// <param name="width">the buffer width.</param>
  /// <param name="height">the buffer height.</param>
  /// <returns></returns>
  public ComputeBuffer RequirePRNGStates(Camera camera)
  {
    var id = camera.GetInstanceID();
    if (_PRNGStates.TryGetValue(id, out var buffer))
      return buffer;

    buffer = new ComputeBuffer(camera.pixelWidth * camera.pixelHeight, 4 * 4, ComputeBufferType.Structured, ComputeBufferMode.Immutable);

    var _mt19937 = new MersenneTwister.MT.mt19937ar_cok_opt_t();
    _mt19937.init_genrand((uint)System.DateTime.Now.Ticks);

    var data = new uint[camera.pixelWidth * camera.pixelHeight * 4];
    for (var i = 0; i < camera.pixelWidth * camera.pixelHeight * 4; ++i)
      data[i] = _mt19937.genrand_int32();
    buffer.SetData(data);

    _PRNGStates.Add(id, buffer);
    return buffer;
  }

  /// <summary>
  /// render.
  /// </summary>
  /// <param name="context">the render context.</param>
  /// <param name="cameras">the all cameras.</param>
  protected override void Render(ScriptableRenderContext context, Camera[] cameras)
  {
    if (!SystemInfo.supportsRayTracing)
    {
      Debug.LogError("You system is not support ray tracing. Please check your graphic API is D3D12 and os is Windows 10.");
      return;
    }

    BeginFrameRendering(context, cameras);

    System.Array.Sort(cameras, (lhs, rhs) => (int)(lhs.depth - rhs.depth));

    BuildAccelerationStructure();

    foreach (var camera in cameras)
    {
      // Only render game and scene view camera.
      if (camera.cameraType != CameraType.Game && camera.cameraType != CameraType.SceneView)
        continue;

      BeginCameraRendering(context, camera);
      _tutorial?.Render(context, camera);
      context.Submit();
      EndCameraRendering(context, camera);
    }

    EndFrameRendering(context, cameras);
  }

  /// <summary>
  /// dispose.
  /// </summary>
  /// <param name="disposing">whether is disposing.</param>
  protected override void Dispose(bool disposing)
  {
    if (null != _tutorial)
    {
      _tutorial.Dispose(disposing);
      _tutorial = null;
    }

    foreach (var pair in _PRNGStates)
    {
      pair.Value.Release();
    }
    _PRNGStates.Clear();

    if (null != _accelerationStructure)
    {
      _accelerationStructure.Dispose();
      _accelerationStructure = null;
    }
  }

  /// <summary>
  /// build the ray tracing acceleration structure.
  /// </summary>
  private void BuildAccelerationStructure()
  {
    if (SceneManager.Instance == null || !SceneManager.Instance.isDirty) return;

    _accelerationStructure.Dispose();
    _accelerationStructure = new RayTracingAccelerationStructure();

    SceneManager.Instance.FillAccelerationStructure(ref _accelerationStructure);

    _accelerationStructure.Build();

    SceneManager.Instance.isDirty = false;
  }
}
