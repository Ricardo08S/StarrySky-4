using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LegacyStarField : MonoBehaviour
{
  [Range(0, 100)]
  [SerializeField] private float starSizeMin = 0.5f;
  [Range(0, 100)]
  [SerializeField] private float starSizeMax = 7f;
  [Range(0.1f, 10f)]
  [SerializeField] private float cameraSensitivity = 2f;
  [SerializeField] private TextMeshProUGUI starInfoText;
  private List<LegacyStarDataloader.LegacyStar> stars;
  private List<GameObject> starObjects;
  private Dictionary<int, GameObject> constellationVisible = new();

  private readonly int starFieldScale = 400;

  void Start()
  {
    starObjects = new();
    LegacyStarDataloader sdl = new();
    stars = sdl.LoadData();
    foreach (LegacyStarDataloader.LegacyStar star in stars)
    {

      GameObject stargo = GameObject.CreatePrimitive(PrimitiveType.Quad);
      stargo.transform.parent = transform;
      stargo.name = $"HR {star.catalog_number}";
      stargo.transform.localPosition = star.position * starFieldScale;

      stargo.transform.LookAt(transform.position);
      stargo.transform.Rotate(0, 180, 0);
      Material material = stargo.GetComponent<MeshRenderer>().material;
      material.shader = Shader.Find("Unlit/StarShader");
      material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, star.size));
      material.color = star.colour;

      SphereCollider collider = stargo.AddComponent<SphereCollider>();
      collider.radius = Mathf.Lerp(starSizeMin, starSizeMax, star.size) * 0.3f;

      starObjects.Add(stargo);
    }
  }

  private void FixedUpdate()
  {
    if (Input.GetKey(KeyCode.Mouse1))
    {
      Camera.main.transform.RotateAround(Camera.main.transform.position, Camera.main.transform.right, Input.GetAxis("Mouse Y") * cameraSensitivity);
      Camera.main.transform.RotateAround(Camera.main.transform.position, Vector3.up, -Input.GetAxis("Mouse X") * cameraSensitivity);
    }

    float scroll = Input.GetAxis("Mouse ScrollWheel");
    if (scroll != 0.0f)
    {
      Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - scroll * cameraSensitivity * 10, 15f, 90f);
    }

    HandleStarClicks();
  }

  private void HandleStarClicks()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit))
      {
        if (hit.collider.gameObject.transform.parent == transform)
        {
          GameObject selectedStarGO = hit.collider.gameObject;
          int starIndex = starObjects.IndexOf(selectedStarGO);
          if (starIndex != -1)
          {
            LegacyStarDataloader.LegacyStar selectedStar = stars[starIndex];
            DisplayStarInfo(selectedStar);
          }
        }
      }
      else{
        starInfoText.text = "Click on a star to see its info";
      }
    }
  }

  private void DisplayStarInfo(LegacyStarDataloader.LegacyStar star)
  {
    if (starInfoText == null)
      {
        Debug.LogError("StarInfoText not assigned. Please assign TextMeshProUGUI in the Inspector");
        return;
      }
    string info =
      $"Star Name: HR {star.catalog_number}\n" +
      $"Position: {star.position}\n" +
      $"Colour: {star.colour}\n" +
      $"Size: {star.size}";
      Debug.Log("Clicked Star Info:\n" + info);
    starInfoText.text = info;
  }

  private void OnValidate()
  {
    if (starObjects != null)
    {
      for (int i = 0; i < starObjects.Count; i++)
      {
        Material material = starObjects[i].GetComponent<MeshRenderer>().material;
        material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, stars[i].size));
      }
    }
  }

  private readonly List<(int[], int[])> constellations = new() {
    // Orion (0)
    (new int[] { 1948, 1903, 1852, 2004, 1713, 2061, 1790, 1907, 2124,
                 2199, 2135, 2047, 2159, 1543, 1544, 1570, 1552, 1567 },
     new int[] { 1713, 2004, 1713, 1852, 1852, 1790, 1852, 1903, 1903, 1948,
                 1948, 2061, 1948, 2004, 1790, 1907, 1907, 2061, 2061, 2124,
                 2124, 2199, 2199, 2135, 2199, 2159, 2159, 2047, 1790, 1543,
                 1543, 1544, 1544, 1570, 1543, 1552, 1552, 1567, 2135, 2047 }),
    // Monceros (1)
    (new int[] { 2970, 3188, 2714, 2356, 2227, 2506, 2298, 2385, 2456, 2479 },
     new int[] { 2970, 3188, 3188, 2714, 2714, 2356, 2356, 2227, 2714, 2506,
                 2506, 2298, 2298, 2385, 2385, 2456, 2479, 2506, 2479, 2385 }),
    // Gemini (2)
    (new int[] { 2890, 2891, 2990, 2421, 2777, 2473, 2650, 2216, 2895,
                 2343, 2484, 2286, 2134, 2763, 2697, 2540, 2821, 2905, 2985},
     new int[] { 2890, 2697, 2990, 2905, 2697, 2473, 2905, 2777, 2777, 2650,
                 2650, 2421, 2473, 2286, 2286, 2216, 2473, 2343, 2216, 2134,
                 2763, 2484, 2763, 2777, 2697, 2540, 2697, 2821, 2821, 2905, 2905, 2985 }),
    // Cancer (3)
    (new int[] {3475, 3449, 3461, 3572, 3249},
     new int[] {3475, 3449, 3449, 3461, 3461, 3572, 3461, 3249}),
    // Leo (4)
    (new int[] { 3982, 4534, 4057, 4357, 3873, 4031, 4359, 3975, 4399, 4386, 3905, 3773, 3731 },
     new int[] { 4534, 4357, 4534, 4359, 4357, 4359, 4357, 4057, 4057, 4031,
                 4057, 3975, 3975, 3982, 3975, 4359, 4359, 4399, 4399, 4386,
                 4031, 3905, 3905, 3873, 3873, 3975, 3873, 3773, 3773, 3731, 3731, 3905 }),
    // Leo Minor (5)
    (new int[] { 3800, 3974, 4100, 4247, 4090 },
     new int[] { 3800, 3974, 3974, 4100, 4100, 4247, 4247, 4090, 4090, 3974 }),
    // Lynx (6)
    (new int[] { 3705, 3690, 3612, 3579, 3275, 2818, 2560, 2238 },
     new int[] { 3705, 3690, 3690, 3612, 3612, 3579, 3579, 3275, 3275, 2818,
                 2818, 2560, 2560, 2238 }),
    // Ursa Major (7)
    (new int[] { 3569, 3594, 3775, 3888, 3323, 3757, 4301, 4295, 4554, 4660,
                 4905, 5054, 5191, 4518, 4335, 4069, 4033, 4377, 4375 },
     new int[] { 3569, 3594, 3594, 3775, 3775, 3888, 3888, 3323, 3323, 3757,
                 3757, 3888, 3757, 4301, 4301, 4295, 4295, 3888, 4295, 4554,
                 4554, 4660, 4660, 4301, 4660, 4905, 4905, 5054, 5054, 5191,
                 4554, 4518, 4518, 4335, 4335, 4069, 4069, 4033, 4518, 4377, 4377, 4375 }),
  };

  private void Update()
  {
    for (int i = 0; i < 10; i++)
    {
      if (Input.GetKeyDown(KeyCode.Alpha0 + i))
      {
        ToggleConstellation(i);
      }
    }
  }

  void ToggleConstellation(int index)
  {
    if ((index < 0) || (index >= constellations.Count))
    {
      return;
    }

    if (constellationVisible.ContainsKey(index))
    {
      RemoveConstellation(index);
    }
    else
    {
      CreateConstellation(index);
    }
  }

  void CreateConstellation(int index)
  {
    int[] constellation = constellations[index].Item1;
    int[] lines = constellations[index].Item2;

    foreach (int catalogNumber in constellation)
    {
      starObjects[catalogNumber - 1].GetComponent<MeshRenderer>().material.color = Color.white;
    }

    GameObject constellationHolder = new($"Constellation {index}");
    constellationHolder.transform.parent = transform;
    constellationVisible[index] = constellationHolder;

    for (int i = 0; i < lines.Length; i += 2)
    {
      GameObject line = new("Line");
      line.transform.parent = constellationHolder.transform;
      LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
      lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
      lineRenderer.useWorldSpace = false;
      Vector3 pos1 = starObjects[lines[i] - 1].transform.position;
      Vector3 pos2 = starObjects[lines[i + 1] - 1].transform.position;
      Vector3 dir = (pos2 - pos1).normalized * 3;
      lineRenderer.positionCount = 2;
      lineRenderer.SetPosition(0, pos1 + dir);
      lineRenderer.SetPosition(1, pos2 - dir);
    }
  }

  void RemoveConstellation(int index)
  {
    int[] constallation = constellations[index].Item1;

    foreach (int catalogNumber in constallation)
    {
      starObjects[catalogNumber - 1].GetComponent<MeshRenderer>().material.color = stars[catalogNumber - 1].colour;
    }
    Destroy(constellationVisible[index]);
    constellationVisible.Remove(index);
  }
}