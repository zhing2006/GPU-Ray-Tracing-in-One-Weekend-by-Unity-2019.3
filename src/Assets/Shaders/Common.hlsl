#include "UnityRaytracingMeshUtils.cginc"

#define CBUFFER_START(name) cbuffer name {
#define CBUFFER_END };

// Macro that interpolate any attribute using barycentric coordinates
#define INTERPOLATE_RAYTRACING_ATTRIBUTE(A0, A1, A2, BARYCENTRIC_COORDINATES) (A0 * BARYCENTRIC_COORDINATES.x + A1 * BARYCENTRIC_COORDINATES.y + A2 * BARYCENTRIC_COORDINATES.z)

CBUFFER_START(CameraBuffer)
float4x4 _InvCameraViewProj;
float3 _WorldSpaceCameraPos;
float _CameraFarDistance;
float3 _FocusCameraLeftBottomCorner;
float3 _FocusCameraRight;
float3 _FocusCameraUp;
float2 _FocusCameraSize;
float _FocusCameraHalfAperture;
CBUFFER_END

RaytracingAccelerationStructure _AccelerationStructure;

struct RayIntersection
{
  int remainingDepth;
  uint4 PRNGStates;
  float4 color;
};

struct AttributeData
{
  float2 barycentrics;
};

inline void GenerateCameraRay(out float3 origin, out float3 direction)
{
  float2 xy = DispatchRaysIndex().xy + 0.5f; // center in the middle of the pixel.
  float2 screenPos = xy / DispatchRaysDimensions().xy * 2.0f - 1.0f;

  // Un project the pixel coordinate into a ray.
  float4 world = mul(_InvCameraViewProj, float4(screenPos, 0, 1));

  world.xyz /= world.w;
  origin = _WorldSpaceCameraPos.xyz;
  direction = normalize(world.xyz - origin);
}

inline void GenerateCameraRayWithOffset(out float3 origin, out float3 direction, float2 offset)
{
  float2 xy = DispatchRaysIndex().xy + offset;
  float2 screenPos = xy / DispatchRaysDimensions().xy * 2.0f - 1.0f;

  // Un project the pixel coordinate into a ray.
  float4 world = mul(_InvCameraViewProj, float4(screenPos, 0, 1));

  world.xyz /= world.w;
  origin = _WorldSpaceCameraPos.xyz;
  direction = normalize(world.xyz - origin);
}

inline void GenerateFocusCameraRayWithOffset(out float3 origin, out float3 direction, float2 apertureOffset, float2 offset)
{
  float2 xy = DispatchRaysIndex().xy + offset;
  float2 uv = xy / DispatchRaysDimensions().xy;

  float3 world = _FocusCameraLeftBottomCorner + uv.x * _FocusCameraSize.x * _FocusCameraRight + uv.y * _FocusCameraSize.y * _FocusCameraUp;
  origin = _WorldSpaceCameraPos.xyz + _FocusCameraHalfAperture * apertureOffset.x * _FocusCameraRight + _FocusCameraHalfAperture * apertureOffset.y * _FocusCameraUp;
  direction = normalize(world.xyz - origin);
}
