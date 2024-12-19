using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StarDataloader
{
    public class Star 
    {
    public float catalog_number;
    public Vector3 position;
    public Color colour;
    public float size;

    private readonly double right_ascension;
    private readonly double declination;
    private readonly float ra_proper_motion;
    private readonly float dec_proper_motion;


    // Constructor
    public Star(float catalog_number, double right_ascension, double declination, byte spectral_type,
                byte spectral_index, short magnitude, float ra_proper_motion, float dec_proper_motion) {
      this.catalog_number = catalog_number;
      this.right_ascension = right_ascension;
      this.declination = declination;
      this.ra_proper_motion = ra_proper_motion;
      this.dec_proper_motion = dec_proper_motion;
      position = GetBasePosition();
      colour = SetColour(spectral_type, spectral_index);
      size = SetSize(magnitude);
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

    private Color SetColour(byte spectral_type, byte spectral_index) 
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
      if (spectral_type == 'O') 
      {
        col_idx = 0;
      } 
      else if (spectral_type == 'B') 
      {
        col_idx = 1;
      } 
      else if (spectral_type == 'A') 
      {
        col_idx = 2;
      } 
      else if (spectral_type == 'F') 
      {
        col_idx = 3;
      } 
      else if (spectral_type == 'G') 
      {
        col_idx = 4;
      } 
      else if (spectral_type == 'K') 
      {
        col_idx = 5;
      } 
      else if (spectral_type == 'M') 
      {
        col_idx = 6;
      }

      if (col_idx == -1) 
      {
        return Color.white;
      }

      float percent = (spectral_index - 0x30) / 10.0f;
      return Color.Lerp(col[col_idx], col[col_idx + 1], percent);
    }

    private float SetSize(short magnitude) 
    {
      return 1 - Mathf.InverseLerp(-146, 796, magnitude);
    }
  }

  public List<Star> LoadData() 
  {
    List<Star> stars = new();
    const string filename = "BSC5";
    TextAsset textAsset = Resources.Load(filename) as TextAsset;
    if (textAsset == null) 
    {
        Debug.LogError($"File {filename} tidak ditemukan di folder Resources.");
    } 
    else 
    {
        Debug.Log($"File {filename} berhasil dimuat. Ukuran: {textAsset.bytes.Length} bytes.");
    }

    MemoryStream stream = new(textAsset.bytes);
    BinaryReader br = new(stream);
    
    int sequence_offset = br.ReadInt32();
    int start_index = br.ReadInt32();
    int num_stars = -br.ReadInt32();
    int star_number_settings = br.ReadInt32();
    int proper_motion_included = br.ReadInt32();
    int num_magnitudes = br.ReadInt32();
    int star_data_size = br.ReadInt32();

    for (int i = 0; i < num_stars; i++) 
    {
      float catalog_number = br.ReadSingle();
      double right_ascension = br.ReadDouble();
      double declination = br.ReadDouble();
      byte spectral_type = br.ReadByte();
      byte spectral_index = br.ReadByte();
      short magnitude = br.ReadInt16();
      float ra_proper_motion = br.ReadSingle();
      float dec_proper_motion = br.ReadSingle();
      Star star = new(catalog_number, right_ascension, declination, spectral_type, spectral_index, magnitude, ra_proper_motion, dec_proper_motion);
      stars.Add(star);
    }

    return stars;
  }
}