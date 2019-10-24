using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BuildFinalScene
{
  [MenuItem("Tutorial/Generate Final Scene")]
  public static void GenerateScene()
  {
    var _mt19937 = new MersenneTwister.MT.mt19937ar_cok_opt_t();
    _mt19937.init_genrand(95273);

    var templateGo = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Sphere.prefab");
    var templateLambertian = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/RandomLambertian.mat");
    var templateMetal = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/RandomMetal.mat");
    var templateDielectric = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/RandomDielectric.mat");
    var renderers = new List<Renderer>();
    GameObject go;
    Renderer renderer;
    Material material;
    for (var a = -11; a < 11; ++a)
    {
      for (var b = -11; b < 11; ++b)
      {
        var chooseMat = _mt19937.genrand_real1();
        var center = new Vector3(a + 0.9f * (float)_mt19937.genrand_real1(),0.2f,b + 0.9f * (float)_mt19937.genrand_real1());
        if ((center - new Vector3(4.0f, 0.2f, 0.0f)).magnitude > 0.9f)
        {
          if (chooseMat < 0.8)
          {
            go = GameObject.Instantiate(templateGo);
            go.name = $"Sphere_L_{a}_{b}";
            go.transform.localPosition = center;
            go.transform.localScale = Vector3.one * 0.4f;
            renderer = go.GetComponent<Renderer>();
            material = new Material(templateLambertian);
            material.SetColor(
              "_Color",
              new Color(
                (float)(_mt19937.genrand_real1() * _mt19937.genrand_real1()),
                (float)(_mt19937.genrand_real1() * _mt19937.genrand_real1()),
                (float)(_mt19937.genrand_real1() * _mt19937.genrand_real1())));
            renderer.material = material;
            renderers.Add(renderer);
          }
          else if (chooseMat < 0.95)
          {
            go = GameObject.Instantiate(templateGo);
            go.name = $"Sphere_M_{a}_{b}";
            go.transform.localPosition = center;
            go.transform.localScale = Vector3.one * 0.4f;
            renderer = go.GetComponent<Renderer>();
            material = new Material(templateMetal);
            material.SetColor(
              "_Color",
              new Color(
                (float)(0.5 * (1.0 + _mt19937.genrand_real1())),
                (float)(0.5 * (1.0 + _mt19937.genrand_real1())),
                (float)(0.5 * (1.0 + _mt19937.genrand_real1()))));
            material.SetFloat("_Fuzz", (float)(0.5 * _mt19937.genrand_real1()));
            renderer.material = material;
            renderers.Add(renderer);
          }
          else
          {
            go = GameObject.Instantiate(templateGo);
            go.name = $"Sphere_D_{a}_{b}";
            go.transform.localPosition = center;
            go.transform.localScale = Vector3.one * 0.4f;
            renderer = go.GetComponent<Renderer>();
            material = new Material(templateDielectric);
            material.SetColor("_Color", Color.white);
            material.SetFloat("_IOR", 1.5f);
            renderer.material = material;
            renderers.Add(renderer);
          }
        }
      }
    }

    go = GameObject.Instantiate(templateGo);
    go.name = "Sphere_D";
    go.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
    go.transform.localScale = Vector3.one * 2.0f;
    renderer = go.GetComponent<Renderer>();
    material = new Material(templateDielectric);
    material.SetColor("_Color", Color.white);
    material.SetFloat("_IOR", 1.5f);
    renderer.material = material;
    renderers.Add(renderer);

    go = GameObject.Instantiate(templateGo);
    go.name = "Sphere_L";
    go.transform.localPosition = new Vector3(-4.0f, 1.0f, 0.0f);
    go.transform.localScale = Vector3.one * 2.0f;
    renderer = go.GetComponent<Renderer>();
    material = new Material(templateLambertian);
    material.SetColor("_Color", new Color(0.4f, 0.2f, 0.1f));
    renderer.material = material;
    renderers.Add(renderer);

    go = GameObject.Instantiate(templateGo);
    go.name = "Sphere_M";
    go.transform.localPosition = new Vector3(4.0f, 1.0f, 0.0f);
    go.transform.localScale = Vector3.one * 2.0f;
    renderer = go.GetComponent<Renderer>();
    material = new Material(templateMetal);
    material.SetColor("_Color", new Color(0.7f, 0.6f, 0.5f));
    material.SetFloat("_Fuzz", 0.0f);
    renderer.material = material;
    renderers.Add(renderer);

    SceneManager.Instance.renderers = renderers.ToArray();
  }
}
