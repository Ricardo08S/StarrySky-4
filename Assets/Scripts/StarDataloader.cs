using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

public class StarDataloader
{
  [System.Serializable]
  public class Star
  {
    public float catalog_number;
    public Vector3 position;
    public Color colour;
    public float size;
    public string RightAscension { get; private set; }
    public string Declination { get; private set; }


    private double right_ascension;
    private double declination;
    private float ra_proper_motion;
    private float dec_proper_motion;

    [JsonProperty("RAh")]
    public string RAh { get; set; }
    [JsonProperty("RAm")]
    public string RAm { get; set; }
    [JsonProperty("RAs")]
    public string RAs { get; set; }

    [JsonProperty("DE-")]
    public string DE_Sign { get; set; }
    [JsonProperty("DEd")]
    public string DEd { get; set; }
    [JsonProperty("DEm")]
    public string DEm { get; set; }
    [JsonProperty("DEs")]
    public string DEs { get; set; }
    [JsonProperty("Vmag")]
    public string Vmag { get; set; }
    [JsonProperty("SpType")]
    public string SpType { get; set; }

    [JsonProperty("pmRA")]
    public string pmRA { get; set; }
    [JsonProperty("pmDE")]
    public string pmDE { get; set; }
    [JsonProperty("HR")]
    public string HR { get; set; }
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Constellation")]
    public string Constellation { get; set; }
    [JsonProperty("K")]
    public string Temperature { get; set; }
    [JsonProperty("Common")]
    public string CommonName { get; set; }
    [JsonProperty("FlamsteedF")]
    public string FlamsteedF { get; set; }
    [JsonProperty("HD")]
    public string HD { get; set; }

    // Constructor
    public Star()
    {

    }
    public void Initialize()
    {
      double ra = (double.Parse(RAh) + double.Parse(RAm) / 60.0 + double.Parse(RAs) / 3600.0) * 15 * (Math.PI / 180);
      this.right_ascension = ra;

      double dec;
      if (string.IsNullOrEmpty(DE_Sign))
      {
        DE_Sign = "+";
      }

      if (DE_Sign == "-")
      {
        dec = -(double.Parse(DEd) + double.Parse(DEm) / 60.0 + double.Parse(DEs) / 3600.0) * (Math.PI / 180);
      }
      else
      {
        dec = (double.Parse(DEd) + double.Parse(DEm) / 60.0 + double.Parse(DEs) / 3600.0) * (Math.PI / 180);
      }
      this.declination = dec;

      if (string.IsNullOrEmpty(pmRA))
      {
        this.ra_proper_motion = 0.0f;
      }
      else
      {
        this.ra_proper_motion = float.Parse(pmRA);
      }
      if (string.IsNullOrEmpty(pmDE))
      {
        this.dec_proper_motion = 0.0f;
      }
      else
      {
        this.dec_proper_motion = float.Parse(pmDE);
      }

      this.catalog_number = float.Parse(HR);

      if (string.IsNullOrEmpty(FlamsteedF))
      {
        FlamsteedF = "";
      }
      if (string.IsNullOrEmpty(HD))
      {
        HD = "";
      }

      position = GetBasePosition();
      colour = SetColour(SpType);
      size = SetSize(Vmag);

      this.RightAscension = $"{double.Parse(RAh):00}h {double.Parse(RAm):00}m {double.Parse(RAs):00.00}s";
      this.Declination = $"{DE_Sign}{double.Parse(DEd):00}° {double.Parse(DEm):00}' {double.Parse(DEs):00.00}\"";
    }

    public Vector3 GetBasePosition()
    {
      double x = System.Math.Cos(right_ascension);
      double y = System.Math.Sin(declination);
      double z = System.Math.Sin(right_ascension);

      double y_cos = System.Math.Cos(declination);
      x *= y_cos;
      z *= y_cos;

      return new((float)x, (float)y, (float)z);
    }

    private Color SetColour(string spectralType)
    {
      Color IntColour(int r, int g, int b)
      {
        return new Color(r / 255f, g / 255f, b / 255f);
      }
      // OBAFGKM colours from: https://arxiv.org/pdf/2101.06254.pdf
      Color[] col = new Color[8];
      col[0] = IntColour(0x5c, 0x7c, 0xff); // O1
      col[1] = IntColour(0x5d, 0x7e, 0xff); // B0.5
      col[2] = IntColour(0x79, 0x96, 0xff); // A0
      col[3] = IntColour(0xb8, 0xc5, 0xff); // F0
      col[4] = IntColour(0xff, 0xef, 0xed); // G1
      col[5] = IntColour(0xff, 0xde, 0xc0); // K0
      col[6] = IntColour(0xff, 0xa2, 0x5a); // M0
      col[7] = IntColour(0xff, 0x7d, 0x24); // M9.5


      int col_idx = -1;
      if (string.IsNullOrEmpty(spectralType))
      {
        return Color.white;
      }


      if (spectralType.StartsWith("O"))
      {
        col_idx = 0;
      }
      else if (spectralType.StartsWith("B"))
      {
        col_idx = 1;
      }
      else if (spectralType.StartsWith("A"))
      {
        col_idx = 2;
      }
      else if (spectralType.StartsWith("F"))
      {
        col_idx = 3;
      }
      else if (spectralType.StartsWith("G"))
      {
        col_idx = 4;
      }
      else if (spectralType.StartsWith("K"))
      {
        col_idx = 5;
      }
      else if (spectralType.StartsWith("M"))
      {
        col_idx = 6;
      }

      if (col_idx == -1)
      {
        return Color.white;
      }


      float percent = 0;
      if (spectralType.Length > 1)
      {
        if (float.TryParse(spectralType.Substring(1), out float spectral_index))
        {
          percent = (spectral_index) / 10.0f;
        }
      }

      return Color.Lerp(col[col_idx], col[col_idx + 1], percent);
    }

    private float SetSize(string magnitude)
    {
      if (float.TryParse(magnitude, out float mag))
      {
        return 1 - Mathf.InverseLerp(-1.46f, 7.96f, mag);
      }
      else
      {
        return 0.1f;
      }
    }
  }


  public List<Star> LoadData()
  {
    List<Star> stars = new();
    const string filename = "bsc5-all";

    TextAsset textAsset = Resources.Load(filename) as TextAsset;

    if (textAsset == null)
    {
      Debug.LogError($"File {filename} not found in Resources folder.");
      return stars;
    }
    Debug.Log($"File {filename} Loaded Successfully, file size {textAsset.bytes.Length} bytes");

    try
    {
      stars = JsonConvert.DeserializeObject<List<Star>>(textAsset.text);
      if (stars != null)
      {
        foreach (var star in stars)
        {
          star.Initialize();
        }
      }
      else
      {
        Debug.LogError("Failed to deserialize data");
      }


    }
    catch (Exception e)
    {
      Debug.LogError("Error parsing JSON: " + e.Message);
      return new List<Star>();
    }

    return stars;
  }
}