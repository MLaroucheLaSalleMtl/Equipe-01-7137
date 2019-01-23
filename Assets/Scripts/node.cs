using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node : MonoBehaviour
{
    

    public enum NodeType
    {
        plain = 0 , 
        Rocky = 1, 
        montain = 2, 
        dry = 3, 
        water =4

    }
    public NodeType type;
    public float getSize
    {
        get { return size; }
    }
    float size = 1;
    Vector2 position;
    public float Value = 0;
    public float AverageHeight = 1;
    public float terrainhardness;
    public int RessourceAmount;
    public Resource resource;
    public Terrain terrain;
    public void SetSize(float x)
    {
        size = x;
        _collider.size = x * Vector3.one;
    }
    public BoxCollider collider
    {
        get
        {
            return _collider;
        }
    }
    private BoxCollider _collider;

 
    /// <summary>
    /// Get Value from position on the map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public float GetValue(int x, int y)
    {
        position = new Vector2(x, y);
        //Get the value of the lands, Usually we want the value to not be that different to the plain level + 5m
        var val = size * 100; 
        var avgh = 0f;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var h = 35 - Mathf.Abs(terrain.terrainData.GetHeight(x + i, y + j));
                avgh += terrain.terrainData.GetHeight(x + i, y + j);
               
                 
                if (type == NodeType.dry) h /= 2;
                Value += h;
                val += h;
            }
        }

        avgh /= (size * size);
        AverageHeight = avgh;
        return val;
    }
    
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

}
