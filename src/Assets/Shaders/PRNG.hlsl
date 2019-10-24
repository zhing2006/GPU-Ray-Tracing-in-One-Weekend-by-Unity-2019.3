#define M_PI (3.14159265358979323846264338327950288)

RWStructuredBuffer<uint4> _PRNGStates;

inline float cbrt(float d)
{
  return pow(d, 1.0f / 3.0f);
}

uint TausStep(inout uint z, int S1, int S2, int S3, uint M)
{
  uint b = (((z << S1) ^ z) >> S2);
  return z = (((z & M) << S3) ^ b);
}

uint LCGStep(inout uint z)
{
  return z = (1664525 * z + 1013904223);
}

float GetRandomValueTauswortheUniform(inout uint4 states)
{
  uint taus = TausStep(states.x, 13, 19, 12, 4294967294UL) ^ TausStep(states.y, 2, 25, 4, 4294967288UL) ^ TausStep(states.z, 3, 11, 17, 4294967280UL);
  uint lcg = LCGStep(states.w);

  return 2.3283064365387e-10f * (taus ^ lcg); // taus+
}

float GetRandomValue(inout uint4 states)
{
  float rand = GetRandomValueTauswortheUniform(states);
  return rand;
}

float3 GetRandomInUnitSphere(inout uint4 states)
{
  float u = GetRandomValue(states);
  float v = GetRandomValue(states);
  float theta = u * 2.f * (float)M_PI;
  float phi = acos(2.f * v - 1.f);
  float r = cbrt(GetRandomValue(states));
  float sinTheta = sin(theta);
  float cosTheta = cos(theta);
  float sinPhi = sin(phi);
  float cosPhi = cos(phi);
  float x = r * sinPhi * cosTheta;
  float y = r * sinPhi * sinTheta;
  float z = r * cosPhi;
  return float3(x, y, z);
}

float2 GetRandomInUnitDisk(inout uint4 states) {
  float a = GetRandomValue(states) * 2.0f * (float)M_PI;
  float r = sqrt(GetRandomValue(states));

  return float2(r * cos(a), r * sin(a));
}
