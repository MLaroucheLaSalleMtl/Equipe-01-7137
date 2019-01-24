using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Terrain[] terrain;
    public node[] Nodes;
    public Owner[] owners= new Owner[2] { new Owner() { Name = "Nana"}, new Owner() { Name = "David" } };

    [Header("Assets")]
    public GameObject node;
    public GameObject node_bound;
    [Header("Resource")]
    public Resource[] Resources;
    public GameObject[] Buildings;

    Camera _main;
    [Header("Flair")]
    public GameObject Highlight;

    [Header("Camera")]
    public Vector3 CameraOffset;
    public Vector3 CameraPosition;
    public float Boundary = 40;
    public float cameraSmoothness = 6;
    public float EdgeScrollingSpeed = 5;
    Vector2 cursorinput;
    private void Awake()
    {
        _main = UnityEngine.Camera.main;   
    }


    private void FixedUpdate()
    {
        
        foreach (var item in owners)
            item.Routine();
    }
    private void Update()
    {
        CameraFunction(_main.transform, CameraPosition);
        MouseInteraction();
    }
    RaycastHit lastresult;
    public LayerMask Interatable;
    Vector3 MousePosition;
    void OnMouseClick(Vector3 pos )
    {
        print(lastresult.collider.gameObject);

        if(buildmode >= 0)
        {
            PlaceBuilding(Buildings[buildmode], owners[0]);
        }
        if (!selection)
        {
            if (lastresult.collider.gameObject.GetComponent<entity>())
                selection = lastresult.collider.gameObject.GetComponent<entity>();
        }
        else
        {
            if (selection is unit)
            {
                (selection as unit).MoveTo(pos);
            }
        }
    }
    void MouseInteraction()
    {

        Highlight.transform.position = MousePosition;


        var r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out lastresult,Mathf.Infinity,Interatable))
            {

            MousePosition = lastresult.point;
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseClick(lastresult.point);
             
            }


            }
         

    }
    [SerializeField]
    entity selection;

    //Relate to camera
    Vector3 EdgeScrolling
    {
        get
        {
            Vector3 t =Vector3.zero;
            var pos = Input.mousePosition;
      
            if (pos.x > Screen.width - Boundary) t += Vector3.right;
            if (pos.x < 0 + Boundary) t += -Vector3.right;
            if (pos.y > Screen.height - Boundary) t += Vector3.up;
            if (pos.y < 0 + Boundary) t += -Vector3.up;
            return t;


        }


    }
    Vector2 cam;
    private float camzoom;
    [SerializeField]
    int buildmode = -1;
    public void Build(int x)
    {
        if (!Buildings[x].GetComponent<building>().HasEnoughRessource(owners[0].Inventory.ToArray())) return;
        var g = Instantiate(Buildings[x].GetComponent<building>().graphics[1], Highlight.transform);
        var t = g.GetComponentsInChildren<Collider>();
        for (int i = 0; i < t.Length; i++)
        {
            Destroy(t[i]);
        }
        buildmode = x;
    }
   void PlaceBuilding(GameObject building, Owner n)
    {
        var x = Instantiate(building, MousePosition, Quaternion.identity).GetComponent<building>();
        x.TransferOwner(n);
        x.build(MousePosition, n);
        buildmode = -1;
        ClearHighLight();
    }
    void ClearHighLight()
    {
        for (int i = 0; i < Highlight.transform.childCount; i++)
        {
            Destroy(Highlight.transform.GetChild(i).gameObject);
        }
    }
    public void CameraFunction(Transform camera, Vector3 position)
    {

        Cursor.lockState = CursorLockMode.Confined;
     
        if(Input.GetKey(KeyCode.Mouse1))
        cursorinput += new Vector2(Input.GetAxis("Mouse X"),
                  Input.GetAxis("Mouse Y"));

        cursorinput.y = Mathf.Clamp(cursorinput.y, -90, -50);
        camzoom = Mathf.Clamp(camzoom + Input.GetAxis("Mouse ScrollWheel") * -350 * Time.smoothDeltaTime, 1f, 350);
        camera.transform.position = Vector3.Lerp(camera.transform.position, position + Vector3.forward * camzoom, 125 * Time.fixedDeltaTime);
        camera.transform.LookAt(position + CameraOffset + -Vector3.up * cameraSmoothness / 2 * Time.fixedDeltaTime);
        camera.transform.RotateAround(position, Vector3.up, cursorinput.x * cameraSmoothness *8* Time.fixedDeltaTime);
        camera.transform.RotateAround(position, camera.transform.right, (-cursorinput.y) * 8 * cameraSmoothness * Time.fixedDeltaTime);


        var es = _main.gameObject.transform.TransformDirection(-EdgeScrolling *(1 + Mathf.Abs(camzoom)/20) * EdgeScrollingSpeed * cameraSmoothness * Time.smoothDeltaTime);
     
      

        if(Input.GetMouseButton(2))es = _main.gameObject.transform.TransformDirection((new Vector2(Input.GetAxis("Mouse X"),
                  Input.GetAxis("Mouse Y"))) * (1 + Mathf.Abs(camzoom) / 20) * EdgeScrollingSpeed * cameraSmoothness * Time.smoothDeltaTime);


        //if(camera.transform.eulerAngles.x > (90 -30) && camera.transform.eulerAngles.x <(90 +1))

        es.y = 0;
        CameraPosition += es;
    }
    private void Start()
    {
        Nodes = CreateNodes(terrain[0]);
    }





    //Nodes related - need to yeet to somewhere else 

    node[] CreateNodes(Terrain a, int precision = 5)
    {
        var e = new List<node>();
        var t = a.terrainData;
        for (int x = 0; x < t.size.x; x+=precision)
        {
            for (int y = 0; y < t.size.z; y += precision)
            {
                var pos = new Vector3(x, t.GetHeight(x + 5, y + 5), y);
                var n = Instantiate(node, a.transform.position + new Vector3(x + precision / 2, t.GetHeight(x + 5, y + 5), y + precision / 2), Quaternion.identity).GetComponent<node>();
                n.SetSize(precision);
                
                n.terrain = a;
                n.Value = n.GetValue(x, y);

                if (x > t.size.x / 2) n.SetOwner(owners[1]);
                else n.SetOwner(owners[0]);
                if (n.transform.position.y < 10) n.GetComponent<MeshRenderer>().material.color = Color.green;
                else if (n.transform.position.y < 20) n.GetComponent<MeshRenderer>().material.color = Color.yellow;
                else if (n.transform.position.y < 30)
                {
                    n.GetComponent<MeshRenderer>().material.color = Color.yellow + Color.red;
                    n.type = global::node.NodeType.Rocky;
                }
                else
                {
                    n.GetComponent<MeshRenderer>().material.color = Color.red;
                    n.type = global::node.NodeType.montain;

                } 

                if (n.transform.position.y < 0) //It's Water or there is a small hole
                {
                    if (n.transform.position.y < -.05f)
                    {
                        n.type = global::node.NodeType.water;
                        n.GetComponent<MeshRenderer>().material.color = Color.blue;

                    }

                    var b = n.transform.position;
                    b.y = 0;
                    n.transform.position = b;
                  
                }
             
                n.transform.parent = a.transform;
                SpawnRessource(n, x, y);
                e.Add(n);
            }
        }
        

        return e.ToArray();
    }
    void SpawnRessource(node n,int x, int y)
    {
        //tree for now
        var seed = Random.Range(0, 1f);
        if (seed < .6f) return; //Sparse Ressources, so it is not easy
        if (n.AverageHeight > 26 && n.AverageHeight < 30 && n.type == global::node.NodeType.plain)
        {
            //tree
            var q= Instantiate(Resources[0], n.transform.position,Quaternion.identity);
            q.setRessource(Resources[0], 100 * Random.Range(1, 10));
            q.transform.parent = n.transform;
            n.resource = q;
           
            n.Value += n.resource.getAmount* n.Value;
           
            for (int i = 0; i <n.resource.getAmount/50; i++)
            {
                if (Random.Range(0, 1f) > .3f) continue;
                var t = new Vector3(Random.Range(-n.getSize / 1.4f, n.getSize/1.4f) - 2, .5f, Random.Range(-n.getSize / 1.4f, n.getSize / 1.4f)-2);
               
                var e = Instantiate(n.resource.model, n.transform.position,Quaternion.identity);

                e.transform.position = n.transform.position + t;
                e.transform.parent = n.transform;
            }
        }
    }
}
