using System.Collections.Generic;
using UnityEngine;

public class StarField : MonoBehaviour {
  [Range(0, 100)]
  [SerializeField] private float starSizeMin = 0f;
  [Range(0, 100)]
  [SerializeField] private float starSizeMax = 5f;
  private List<StarDataloader.Star> stars;
  private List<GameObject> starObjects;
  private readonly int starFieldScale = 400;

  void Start()
  {
    
    StarDataloader sdl = new();
    stars = sdl.LoadData();
    starObjects = new();
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
      starObjects.Add(stargo);
    }
  }

  private void FixedUpdate() 
  {
    if (Input.GetKey(KeyCode.Mouse1)) 
    {
      Camera.main.transform.RotateAround(Camera.main.transform.position, Camera.main.transform.right, Input.GetAxis("Mouse Y"));
      Camera.main.transform.RotateAround(Camera.main.transform.position, Vector3.up, -Input.GetAxis("Mouse X"));
    }
    return;
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
}