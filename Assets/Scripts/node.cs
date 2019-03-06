using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node : MonoBehaviour
{

   
 
    [SerializeField]
    protected Owner owner;
    protected float AngleToCenter;

    MeshRenderer rendi;
    MeshFilter filty;
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
            if (unit.GetOwner != this.GetOwner)
            {
                this.SetOwner(unit.GetOwner);
              //  borderCalculation.UpdateDraw(unit.GetOwner.faction.NodesList, this.GetOwner, this);
                borderCalculation.RemoveDraw(unit.GetOwner.faction.NodesList, unit.GetOwner, this);
                UnityEngine.Debug.Log("hahahah " + this.transform.position);
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
