﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node : MonoBehaviour
{

   
 
    [SerializeField]
    protected Owner owner;

    MeshRenderer rendi;
    MeshFilter filty;
    public node Predecessor { get; set; }
    public node Successor { get; set; }
     List<Vector3> getBounds
    {
        get
        {
            var z = new List<Vector3>();
           z.Add(transform.position += new Vector3(-size / 2, 0, -size / 2));
            z.Add(transform.position += new Vector3(size / 2, 0, -size / 2));
            z.Add(transform.position += new Vector3(size / 2, 0, size / 2));
            z.Add(transform.position += new Vector3(-size / 2, 0, size / 2));
            return z;
        }


    }

    void generateMesh()
    {
      /*  collider.enabled = false;
        //Gonna try something later, for now just boring plane
        RaycastHit r;
        var v = Physics.Raycast(transform.position + Vector3.up * .25f, Vector3.down, out r);
        collider.enabled = true;

        var s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.transform.parent = transform;
        s.transform.position = transform.position + Vector3.up * .25f;
        s.transform.localScale = Vector3.one * size;
         s.transform.forward = -Vector3.up;*/
       /* if (v)
        {
            s.transform.forward = -r.normal;
        }*/
     
        /*
       OR WE CAN SIMPLY USE QUAD
  
        var m = new Mesh();
        m.name = position.ToString() + " " + size;
        m.vertices = getBounds.ToArray();

        var tri = new int[6];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;
        m.triangles = tri;
        var e = new Vector3[4];
        for (int i = 0; i < e.Length; i++)
            e[i] = Vector3.up;

        m.normals = e;
        var uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);


        m.uv = uv;


        m.RecalculateBounds();
        m.RecalculateNormals();
        filty.sharedMesh = m;*/
    }
    //We don't want to reset it, do we ?



    void OnTriggerEnter(Collider collision)
    {
        BorderCalculation borderCalculation = new BorderCalculation();
        if (collision.gameObject.GetComponent<unit>() != null)
        {
            unit unit = collision.gameObject.GetComponent<unit>();
            Owner ownerNode = this.GetOwner;
            node thisNode = this;
            if (unit.GetOwner != this.GetOwner)
            {
                if (this.GetOwner == GameManager.owners[0])
                {
                    GameManager.instance.musicLauncher.Losing(GameManager.owners[0]);
                }
                int indexConquered=0;
                int indexLost = 0;
                foreach (var item in unit.GetOwner.faction.NodeSquares)
                {
                    
                    if (item.Contains(this))
                    {
                        break;
                    }
                    indexConquered++;
                }
                
                foreach (var item in unit.GetOwner.faction.NodeSquares[indexConquered])
                {
                    item.SetOwner(unit.GetOwner);
                }
                
                if (ownerNode.Name != "Neutral")
                {
                    foreach (var item in unit.GetOwner.faction.NodeSquares)
                    {

                        if (item.Contains(thisNode))
                        {
                            break;
                        }
                        indexLost++;
                    }
                   
                    foreach (var item in ownerNode.faction.NodeSquares[indexLost])
                    {
                        item.SetOwner(unit.GetOwner);
                    }
                    
                    ownerNode.faction.NodesList = borderCalculation.CornerDraw(ownerNode.faction.NodeSquares, ownerNode);
                    unit.GetOwner.faction.NodesList = borderCalculation.CornerDraw(unit.GetOwner.faction.NodeSquares, unit.GetOwner);
                    foreach (var item in ownerNode.faction.NodesList)
                    {
                        Vector3 vector3Unit = new Vector3();
                        vector3Unit = item.transform.position;
                        vector3Unit.y += 5;
                    }
                    foreach (var item in unit.GetOwner.faction.NodesList)
                    {
                        Vector3 vector3Unit = new Vector3();
                        vector3Unit = item.transform.position;
                        vector3Unit.y += 5;
                    }
                    ownerNode.faction.GenFrontieres();
                    unit.GetOwner.faction.GenFrontieres();
                   
                }
                else
                {
                    unit.GetOwner.faction.NodesList = borderCalculation.CornerDraw(unit.GetOwner.faction.NodeSquares, unit.GetOwner);
                    foreach (var item in unit.GetOwner.faction.NodesList)
                    {
                        Vector3 vector3Unit = new Vector3();
                        vector3Unit = item.transform.position;
                        vector3Unit.y += 5;
                    }
                    unit.GetOwner.faction.GenFrontieres();
                   
                }
              
               

            }
        }
    }




    public Owner GetOwner
    {
        get { return owner; }
    } 
    public void SetOwner(Owner n)
    {
        owner = n;
        generateMesh();
    }
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
   
    

    public Goods resource;
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

 
    public void Initialize(int x, int y)
    {
        position = new Vector2(x, y);
    }
    public Vector2 GetPosition
    {
        get { return position; }
    }
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
