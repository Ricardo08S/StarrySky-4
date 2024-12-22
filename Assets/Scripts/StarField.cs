using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StarField : MonoBehaviour
{
  [Range(0, 100)]
  [SerializeField] private float starSizeMin = 0.5f;
  [Range(0, 100)]
  [SerializeField] private float starSizeMax = 7f;
  [Range(0.1f, 10f)]
  [SerializeField] private float cameraSensitivity = 5f;
  [SerializeField] private TextMeshProUGUI starInfoText;
  [SerializeField] private MenuScreenManager _menuScreenManager;
  private List<StarDataloader.Star> stars;
  private List<GameObject> starObjects;
  private Dictionary<int, GameObject> starMap = new();
  private Dictionary<int, GameObject> constellationVisible = new();

  private readonly int starFieldScale = 400;
  void Start()
  {
    if (_menuScreenManager == null)
    {
      Debug.LogError("HoverScreenManager not assigned. Please assign it in the Inspector.");
      return;
    }
    _menuScreenManager.SetStarFieldReference(this);
    starObjects = new();
    StarDataloader sdl = new();
    stars = sdl.LoadData();
    if (stars == null || stars.Count == 0)
    {
      Debug.LogError("Failed to load star data. Starfield creation aborted");
      return;
    }

    foreach (StarDataloader.Star star in stars)
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
      starMap.Add((int)star.catalog_number, stargo);
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
            StarDataloader.Star selectedStar = stars[starIndex];
            DisplayStarInfo(selectedStar);
          }
        }
      }
      else
      {
        starInfoText.text = "Click on a star to see its info";
      }
    }
  }

  private void DisplayStarInfo(StarDataloader.Star star)
  {
    if (starInfoText == null)
    {
      Debug.LogError("StarInfoText not assigned. Please assign TextMeshProUGUI in the Inspector");
      return;
    }

    string info = "";
    List<string> aliases = new();

    if (!string.IsNullOrEmpty(star.Name)) { aliases.Add($"{star.Name}"); }
    aliases.Add($"HR {star.catalog_number}");
    if (!string.IsNullOrEmpty(star.CommonName)) { aliases.Add($"{star.CommonName}"); }
    if (!string.IsNullOrEmpty(star.FlamsteedF)) { aliases.Add($"{star.FlamsteedF}"); }
    if (!string.IsNullOrEmpty(star.HD)) { aliases.Add($"HD {star.HD}"); }


    string aliasText = string.Join(", ", aliases);

    info = $"Aliases: {aliasText}\n" +
            $"Position: {star.position}\n" +
            $"Colour: {star.colour}\n" +
            $"Size: {star.size}";


    if (!string.IsNullOrEmpty(star.Constellation))
    {
      info += $"\nConstellation: {star.Constellation}";
    }

    if (!string.IsNullOrEmpty(star.SpType))
    {
      info += $"\nSpectral Type: {star.SpType}";
    }
    if (!string.IsNullOrEmpty(star.Temperature))
    {
      info += $"\nTemperature: {star.Temperature} K";
    }

    info += $"\nRight Ascension: {star.RightAscension}";
    info += $"\nDeclination: {star.Declination}";

    Debug.Log("Clicked Star Info: HR " + star.catalog_number);
    starInfoText.text = info;
  }

  private void OnValidate()
  {
    if (starObjects != null && stars != null)
    {
      for (int i = 0; i < starObjects.Count; i++)
      {
        if (starObjects[i] != null && stars[i] != null)
        {
          Material material = starObjects[i].GetComponent<MeshRenderer>().material;
          material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, stars[i].size));
        }
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
    (new int[] {3449, 3262, 3475, 3461, 3249, 3572},
     new int[]  {3449, 3262, 3449, 3475, 3449, 3461,
        3461, 3249, 3461, 3572}),
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
    // Taurus (8)
    (new int[] { 1457, 1412, 1409, 1910, 1497, 1791, 1373, 1165, 1346, 1239, 1030},
     new int[] { 1457, 1412, 1457, 1409, 1457, 1910, 1409, 1497, 1497, 1791,
        1409, 1373, 1373, 1165, 1373, 1346, 1346, 1412, 1346, 1239,
        1239, 1030 }),
    // Perseus (9)
    (new int[] {
        1017, 915, 1122, 936, 834, 921, 840, 1220, 1228, 1203, 1131
    },
    new int[] {
        1017, 915, 1017, 1122, 1017, 936, 915, 834, 936, 921,
        921, 840, 1122, 1220, 1220, 1228, 1228, 1203, 1203, 1131
    }),
    // Pisces (A)
    (new int[] {
        291, 383, 360, 437, 510, 596, 549, 489, 434, 294,
        221, 80, 9072, 8969, 8984, 8916, 8911, 8852
    },
    new int[] {
        291, 383, 291, 360, 360, 437, 437, 510, 510, 596,
        596, 549, 549, 489, 489, 434, 434, 294, 294, 221,
        221, 80, 80, 9072, 9072, 8969, 8969, 8984, 8969, 8916,
        8984, 8911, 8916, 8852, 8852, 8911, 360, 383
    }),
    // Camelopardalis (B)
    (new int[] {
        1148, 1686, 1035, 1542, 1204
    },
    new int[] {
        1148, 1686, 1148, 1035, 1148, 1542,
        1035, 1204, 1204, 1542
    }),

    // Scorpius (C)
    (new int[] {
        6134, 5953, 6165, 5944, 5984, 6241, 6247, 6271, 6380,
        6553, 6615, 6580, 6527
    },
    new int[] {
        6134, 5953, 6134, 6165, 6134, 5944, 6134, 5984,
        6165, 6241, 6241, 6247, 6247, 6271, 6271, 6380,
        6380, 6553, 6553, 6615, 6615, 6580, 6580, 6527
    }),
    // Libra (D)
    (new int[] {
        5603, 5531, 5787, 5685, 5908
    },
    new int[] {
        5603, 5531, 5603, 5787, 5531, 5685,
        5685, 5787, 5787, 5908
    }),
    // Virgo (E)
    (new int[] {
        5056, 5315, 5107, 4826, 5338, 5487,
        5264, 5511, 4910, 4932, 4681, 4517
    },
    new int[] {
        5056, 5315, 5056, 5107, 5056, 4826,
        5315, 5338, 5338, 5487, 5107, 5264,
        5264, 5511, 5107, 4910, 4910, 4932,
        4910, 4826, 4826, 4681, 4681, 4517
    }),
    // Sagittarius (F)
    (new int[] {
        6879, 6832, 6746, 6859, 7194, 6616, 6913, 7039, 6812, 7121,
        7234, 7150, 7217, 7304, 7340, 7440, 7650, 7623, 7581, 7343, 7348
    },
    new int[] {
        6879, 6832, 6879, 6746, 6879, 6859, 6879, 7194,
        6746, 6859, 6746, 6616, 6859, 6913, 6859, 7039,
        6913, 6812, 7039, 6913, 7039, 7121, 7039, 7194,
        7194, 7234, 7234, 7121, 7121, 7150, 7150, 7217,
        7217, 7304, 7304, 7340, 7234, 7440, 7440, 7650,
        7650, 7623, 7623, 7581, 7581, 7343, 7581, 7348
    }),
    // Lupus (G)
     (new int[] {
        5469, 5396, 5649, 5571, 5453, 5797, 5695, 5705, 5776, 5948, 5987, 5883
    },
    new int[] {
        5469, 5396, 5469, 5649, 5469, 5571, 5649, 5453, 5649, 5797,
        5571, 5695, 5695, 5705, 5695, 5776, 5797, 5776, 5776, 5948,
        5948, 5987, 5948, 5883
    }),
    // Centaurus (H)
    (new int[] {
        5459, 5267, 5132, 5231, 4819, 5249, 4743, 4621, 4390, 4467,
        5193, 5440, 5190, 5576, 5288, 5089, 5028
    },
    new int[] {
        5459, 5267, 5267, 5132, 5132, 5231, 5231, 4819, 5231, 5249,
        4819, 4743, 4743, 4621, 4621, 4390, 4390, 4467, 5249, 5193,
        5193, 5440, 5193, 5190, 5440, 5576, 5190, 5288, 5190, 5089,
        5089, 5028
    }),
    // Crux (I)
    (new int[] {
        4656, 4853, 4729, 4763
    },
    new int[] {
        4656, 4853, 4729, 4763
    }),
    // Capricornus (J)
    (new int[] {
        8322, 8278, 8167, 8204, 8075, 7980, 7776, 7747, 7936
    },
    new int[] {
        8322, 8278, 8278, 8167, 8167, 8204, 8167, 8075,
        8075, 8204, 8075, 7980, 8075, 7776, 7776, 7747,
        7776, 7936
    }),
    // Aquarius (K)
    (new int[] {
        7950, 8232, 8414, 8499, 8518, 8418, 8573, 8679, 8709, 8812,
        8559, 8597, 8698, 8841, 8892
    },
    new int[] {
        7950, 8232, 8232, 8414, 8414, 8499, 8414, 8518, 8499, 8418,
        8499, 8573, 8573, 8679, 8679, 8709, 8709, 8812, 8518, 8559,
        8559, 8597, 8597, 8698, 8698, 8841, 8841, 8892
    }),
     // Cetus (L)
    (new int[] {
        74, 188, 334, 509, 740, 811, 781, 402, 539, 708,
        681, 779, 804, 911, 754, 718, 896, 813, 649
    },
    new int[] {
        74, 188, 188, 334, 188, 509, 509, 740, 740, 811,
        811, 781, 334, 402, 402, 539, 539, 708, 708, 781,
        781, 681, 681, 779, 779, 804, 804, 911, 804, 754,
        754, 718, 911, 896, 896, 813, 813, 718, 718, 649
    }),
    // Eridanus (M)
    (new int[] {
        472, 566, 674, 721, 789, 794, 898, 1008, 1190, 1195,
        1347, 1393, 1464, 1173, 1088, 1003, 919, 818, 874, 984,
        1084, 1463, 1520, 1560, 1666, 1679, 1481
    },
    new int[] {
        472, 566, 566, 674, 674, 721, 721, 789, 789, 794,
        794, 898, 898, 1008, 1008, 1190, 1190, 1195, 1195, 1347,
        1347, 1393, 1393, 1464, 1464, 1173, 1173, 1088, 1088, 1003,
        1003, 919, 919, 818, 818, 874, 874, 984, 984, 1084,
        1084, 1463, 1463, 1520, 1520, 1560, 1560, 1666, 1666, 1679,
        1679, 1481
    }),

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

    for (int i = 0; i < 26; i++)
    {
      KeyCode key = (KeyCode)((int)KeyCode.A + i);
      if (Input.GetKeyDown(key))
      {
        bool isCapsLock = Input.GetKey(KeyCode.CapsLock);
        ToggleConstellation(10 + i, isCapsLock);
      }
    }
  }

  void ToggleConstellation(int index, bool isCapsLock = false)
  {
    if ((index < 0) || (index >= constellations.Count))
    {
      Debug.LogError($"Invalid constellation index: {index}");
      return;
    }

    // Jika menggunakan alfabet, sesuaikan indeks berdasarkan Caps Lock
    if (index >= 10)
    {
      Debug.Log($"Toggling constellation {(char)('A' + index - 10)} ({(isCapsLock ? "Caps Lock ON" : "Caps Lock OFF")})");
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
      if (starMap.ContainsKey(catalogNumber))
      {
        starMap[catalogNumber].GetComponent<MeshRenderer>().material.color = Color.white;
      }
      else
      {
        Debug.LogError($"Catalog Number {catalogNumber} not found in starMap");
      }
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

      Vector3 pos1 = Vector3.zero;
      Vector3 pos2 = Vector3.zero;

      if (starMap.ContainsKey(lines[i]))
      {
        pos1 = starMap[lines[i]].transform.position;
      }
      else
      {
        Debug.LogError($"Catalog Number {lines[i]} not found in starMap");
      }

      if (starMap.ContainsKey(lines[i + 1]))
      {
        pos2 = starMap[lines[i + 1]].transform.position;
      }
      else
      {
        Debug.LogError($"Catalog Number {lines[i + 1]} not found in starMap");
      }

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

  public void SetCameraSensitivity(float sensitivity)
  {
    cameraSensitivity = sensitivity;
  }

  public void SetStarSizeMin(float sizeMin)
  {
    starSizeMin = sizeMin;
    UpdateStarSizes();
  }

  public void SetStarSizeMax(float sizeMax)
  {
    starSizeMax = sizeMax;
    UpdateStarSizes();
  }

  public float GetCameraSensitivity()
  {
    return cameraSensitivity;
  }
  public float GetStarSizeMin()
  {
    return starSizeMin;
  }

  public float GetStarSizeMax()
  {
    return starSizeMax;
  }

  private void UpdateStarSizes()
  {
    if (starObjects != null && stars != null)
    {
      for (int i = 0; i < starObjects.Count; i++)
      {
        if (starObjects[i] != null && stars[i] != null)
        {
          Material material = starObjects[i].GetComponent<MeshRenderer>().material;
          material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, stars[i].size));
        }
      }
    }
  }
}