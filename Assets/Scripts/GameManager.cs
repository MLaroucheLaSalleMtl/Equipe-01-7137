using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Terrain[] terrain;
    public node[] Nodes;
    public static Owner[] owners= new Owner[2] { new Owner() { Name = "Nana"}, new Owner() { Name = "David" } };
    public static float SecondPerGenerations = 60;
    
    [Header("Assets")]
    public GameObject node;
    public GameObject node_bound;
    [Header("Resource")]
    public Goods[] Resources;
    public GameObject[] Buildings;
    public GameObject _army;
    public static GameObject ArmyPrefab;
    Camera _main;
    [Header("Flair")]

    public GameObject[] Cursor3D;

    [Header("Camera")]
    public Vector3 CameraOffset;
    public Vector3 CameraPosition;
    public float Boundary = 40;
    public float cameraSmoothness = 6;
    public float EdgeScrollingSpeed = 5;
    Vector2 cursorinput;
    private void Awake()
    {
        instance = this;
        buildmode = -1;
        ArmyPrefab = _army;
        CancelSelection();
        _main = UnityEngine.Camera.main;

        owners[0].OnGain += OnOwnerGain;
    }

    public void OnOwnerGain(Goods g, Vector3 pos)
    {
        if (g.bit)
        {
            var e = Instantiate(g.bit, pos, Quaternion.identity);
            if (e)
                StartCoroutine(popup(e));
        }
        else
        {
            print(g.Name + " has no bits!");
        }
   
    }
    IEnumerator popup(GameObject c)
    {
        var t = 1.5f;
        while (t >0)
        {
            t -= Time.smoothDeltaTime;
            c.transform.position += Vector3.up * t * Time.smoothDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(.5f);
        Destroy(c.gameObject);
        yield break;
    }

    private void FixedUpdate()
    {
        
        foreach (var item in owners)
            item.Routine();

        MUI.ShowUI(owners[0],selection);
        BUI.CancelUI.SetActive(buildmode >= 0);
    }
    private void Update()
    {
        CameraFunction(_main.transform, CameraPosition);
        MouseInteraction();
    }
    RaycastHit lastresult;
    public LayerMask Interatable, BuildingMask;
    Vector3 MousePosition;
    [SerializeField]
    MainUI MUI;
    [SerializeField]
    BuildingUI BUI;
 
    void OnMouseClick(Vector3 pos )
    {
        print(lastresult.collider.gameObject);
        var tempsel = lastresult.collider.gameObject.GetComponent<entity>();
        if (buildmode >= 0)
        {
            
            if(BUI.CanBePlaceThere(pos,owners[0])) PlaceBuilding(buildmode, owners[0]);
        }
        if (!selection )
        {
            if (tempsel && !(tempsel is building))
            {
                selection = tempsel;
                UiSelection[0].SetActive(true);
                UiSelection[1].SetActive(true);
            }
          
        }
        else
        {
            switch (currentmode)
            {

                case 1:
                    (selection as unit).MoveTo(pos);
                    break;
                case 2: if(tempsel && tempsel != selection)
                    (selection as unit).Attack(tempsel  );
                    break;
                case 3:
                    if (tempsel && tempsel != selection && (selection is unit) && (selection.GetOwner == tempsel.GetOwner))
                    {
                        var x =  (selection as unit).Merge(tempsel as unit);
                        CancelSelection();
                        selection = x;
                    }                     
                    break;
                case 4:
                    (selection as unit).Chill();
                    break;
                default:
                    break;
            }

 

        }
    }
    public GameObject[] UiSelection;
    int currentmode = 0;
    entity target;
 
    public void SetUIMode(int x )
    {
        if (x == 0) return;
        foreach (var item in UiSelection)
        {
            item.SetActive(false);
        }

        foreach (var item in Cursor3D)
        {
            item.SetActive(false);
        }

       if(x > 0) Cursor3D[ Mathf.Clamp(x-1, 0, Cursor3D.Length-1)].SetActive(true);
        UiSelection[0].SetActive(true); // image nad name 
      UiSelection[2].SetActive(true);

        currentmode = x;
    }
    public void Chillout()
    {
        if (selection)
            (selection as unit).Chill();
    }
    public void CancelSelection()
    {
        foreach (var item in UiSelection)
        {
            item.SetActive(false);
        }
        foreach (var item in Cursor3D)
        {
            item.SetActive(false);
        }
        if(currentmode > 0)
        {
            currentmode = 0;
            UiSelection[0].SetActive(true); // image nad name 
            UiSelection[1].SetActive(true); // main icons
        }
        else
        {
            selection = null;
            buildmode = -1;
            ClearHighLight();
            _lastbuilding = null;
        }
    }
    void MouseInteraction()
    {

        BUI.Highlight.transform.position = MousePosition;


        var r = Camera.main.ScreenPointToRay(Input.mousePosition);
        var y = Physics.Raycast(r, out lastresult, Mathf.Infinity, Interatable);
        if (buildmode >= 0)
        {

            BUI.CanBePlaceThere(lastresult.point,owners[0]);
            if (owners[0].Settled)
                BUI.Highlight.transform.right = (owners[0].Cores[0].transform.position - BUI.Highlight.transform.position);
            y = Physics.Raycast(r, out lastresult, Mathf.Infinity, BuildingMask);
        }
        
        if (y)
        {

            if (EventSystem.current.IsPointerOverGameObject()) return;
            MousePosition = lastresult.point;
        
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseClick(lastresult.point);

            }


        }


    }
    [SerializeField]
    public static entity selection;

    //Relate to camera
    Vector3 EdgeScrolling
    {
        get
        {
            Vector3 t =Vector3.zero;
            var pos = Input.mousePosition;
      /*
            if (pos.x > Screen.width - Boundary) t += Vector3.right;
            if (pos.x < 0 + Boundary) t += -Vector3.right;
            if (pos.y > Screen.height - Boundary) t += Vector3.up;
            if (pos.y < 0 + Boundary) t += -Vector3.up;

    */


            return t;


        }


    }
    Vector2 cam;
    private float camzoom;
    [SerializeField]
    int buildmode = -1;

    building _lastbuilding;
    public void Build(int x)
    {
        if (!Buildings[x].GetComponent<building>().HasEnoughRessource(owners[0].Inventory, owners[0].Gold)  ) { print("Not enough ressource or Gold"); _lastbuilding = null; return; } 
        var g = Instantiate(Buildings[x].GetComponent<building>().graphics[1], BUI.Highlight.transform);
        building_highlight = g;
        BUI.Planing(g, Buildings[x].GetComponent<building>());
      
       
        var t = g.GetComponentsInChildren<Collider>();
        for (int i = 0; i < t.Length; i++)
        {
            Destroy(t[i]);
        }
        buildmode = x;
    }
 
   void PlaceBuilding(int j, Owner n)
    {
        var x = Instantiate(Buildings[j], MousePosition, Quaternion.identity).GetComponent<building>();
        x.transform.rotation = building_highlight.transform.rotation;
        x.TransferOwner(n);
        x.build(MousePosition, n);
        x.Tier = 0;
        owners[0].Pay(x.costs[0].materials);
        buildmode = -1;
        ClearHighLight();
        if (owners[0].Settled)
        {
 
            foreach (var item in BUI.Uis)
                item.SetActive(false);
            BUI.Uis[1].gameObject.SetActive(true);
        }
        else
        {
            foreach (var item in BUI.Uis)
                item.SetActive(false);
            BUI.Uis[0].gameObject.SetActive(true);
        }
        buildmode = -1;
        if(_lastbuilding  && _lastbuilding is Wall && x is Wall)
        {
            (_lastbuilding as Wall).boundTo = x as Wall;
            
        }
  

        _lastbuilding = x;
        if (_lastbuilding is Wall) Build(j);

    }

    GameObject building_highlight;
    void ClearHighLight()
    {
        if(building_highlight)Destroy(building_highlight.gameObject);
    }
    public void CameraFunction(Transform camera, Vector3 position)
    {

        Cursor.lockState = CursorLockMode.Confined;
     
        if(Input.GetKey(KeyCode.Mouse1))
        cursorinput += new Vector2(Input.GetAxis("Mouse X"),
                  Input.GetAxis("Mouse Y"));

        cursorinput.y = Mathf.Clamp(cursorinput.y, -70, -20);
        camzoom = Mathf.Clamp(camzoom + Input.GetAxis("Mouse ScrollWheel") * -350 * Time.smoothDeltaTime, 1, 350);
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
        BorderCalculation borderCalculation = new BorderCalculation();
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
                n.Initialize(x,y); 
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
            var q = new Goods() ;
           
            q.setRessource(Resources[0], 100 * Random.Range(1, 10));
          //  q.transform.parent = n.transform;
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
