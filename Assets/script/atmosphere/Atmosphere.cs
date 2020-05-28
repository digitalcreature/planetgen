using UnityEngine;

public class Atmosphere : MonoBehaviour {

  public float maxAltitude = 200;
  [MinMax(0, 1)]
  public FloatRange surfaceFadeRange = new FloatRange(0.125f, 0.85f);

  public AtmosphereTextureGenerator colors;

  float radius => gfield.radius + maxAltitude;

  public GSphereField gfield { get; private set; }

  public MeshRenderer render { get; private set; }
  public Light sun { get; private set; }

  int SURFACE_FADE;
  int RADIUS;
  int SUN_DIRECTION;
  int MAINTEX;
  int MAINTEX_SURFACE;

  MaterialPropertyBlock block;

  void Awake() {
    render = GetComponent<MeshRenderer>();
    block = new MaterialPropertyBlock();
    SURFACE_FADE = Shader.PropertyToID("_SurfaceFade");
    RADIUS = Shader.PropertyToID("_Radius");
    SUN_DIRECTION = Shader.PropertyToID("_SunDirection");
    MAINTEX = Shader.PropertyToID("_MainTex");
    MAINTEX_SURFACE = Shader.PropertyToID("_MainTex_Surface");
    gfield = GetComponentInParent<GSphereField>();
    sun = GameObject.FindWithTag("Sun").GetComponent<Light>();
  }

  void Start() {
    transform.localScale = radius * Vector3.one;
    block.SetFloat(RADIUS, radius);
    render.SetPropertyBlock(block);
    UpdateColorTextures();
  }

  void Update() {
    Camera camera = CameraRig.instance.cam;
    float altitude = (camera.transform.position - transform.position).magnitude - gfield.radius;
    float factor = surfaceFadeRange.InverseLerp(altitude / maxAltitude);
    block.SetFloat(SURFACE_FADE, 1 - factor);
    block.SetVector(SUN_DIRECTION, sun.transform.forward);
    render.SetPropertyBlock(block);
  }

  void UpdateColorTextures() {
    colors.Generate();
    block.SetTexture(MAINTEX_SURFACE, colors.surfaceMap);
    block.SetTexture(MAINTEX, colors.spaceMap);
    render.SetPropertyBlock(block);
  }

  void OnValidate() {
    if (render != null) {
      UpdateColorTextures();
    }
  }

}

[System.Serializable]
public class AtmosphereTextureGenerator {

  public int width = 64;
  public int height = 32;
  public Gradient surfaceDepthColor;
  public Gradient spaceDepthColor;
  public Gradient lightColor;

  public Texture2D surfaceMap { get; private set; }
  public Texture2D spaceMap { get; private set; }


  public void Generate() {
    surfaceMap = GenerateSurfaceMap();
    spaceMap = GenerateSpaceMap();
  }

  public Texture2D GenerateSurfaceMap() {
    return GenerateTexture(lightColor, surfaceDepthColor);
  }

  public Texture2D GenerateSpaceMap() {
    return GenerateTexture(lightColor, spaceDepthColor);
  }

  Texture2D GenerateTexture(Gradient xColor, Gradient yColor) {
    var tex = new Texture2D(width, height);
    tex.wrapMode = TextureWrapMode.Clamp;
    var pixels = new Color[width * height];
    for (int i = 0; i < width * height; i ++) {
      int x = i % width;
      int y = i / width;
      pixels[i] = xColor.Evaluate(x / (float) width) * yColor.Evaluate(y / (float) height);
    }
    tex.SetPixels(pixels);
    tex.Apply();
    return tex;
  }

}
