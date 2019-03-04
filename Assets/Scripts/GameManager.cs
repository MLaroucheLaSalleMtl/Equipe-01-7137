using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public MeshRenderer Fog;
    public Terrain[] terrain;
    public node[] Nodes;
    public static Owner[] owners = new Owner[2] { new Owner() { Name = "Nana", MainColor = Color.blue }, new Owner() { Name = "David", MainColor = Color.green } };
    public static float SecondPerGenerations = 60;
    public static bool DEBUG_GODMODE = true;
    [Header("Assets")]
    public GameObject node;
    public GameObject node_bound;
    [Header("Resource")]
    public Goods[] Resources;
    public GameObject[] Buildings;
    public GameObject _army;
    public GameObject[] Missiles;
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
        owners[1].Gold += 100;
        for (int i = 1; i < owners.Length; i++)
        {
          var t =  gameObject.AddComponent<Owner_AI>();
            t.owner = owners[i];
        }
    }

    //Depreciated, was using shader before but wasn't optimized as using a for loop not clean 
   /* public void SeeFogofWar()
    {
        var mat = Fog.material;
        List<Vector4> lol= new List<Vector4>();
        for (int i = 0; i < owners[0].Units.Count; i++)  
        {
            var item = owners[0].Units[i];
            var e = new Vector4(item.transform.position.z, item.DetectionRange, item.transform.position.z, 0);
            //   lol.Add(e);
            mat.SetVector("_Holes" + i,e);
        }
       
        mat.SetInt("arr", 0);
      //  mat.SetVectorArray("_Holes",lol);
        mat.SetInt("arr", lol.Count);
    }*/
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
        while (t > 0)
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

        MUI.ShowUI(owners[0], selection[0]);
        //Useless SeeFogofWar();
      //  BUI.CancelUI.SetActive(buildmode >= 0);
    }


    private void Update()
    {
        CameraFunction(_main.transform, CameraPosition);
        MouseInteraction();
    }
    RaycastHit lastresult;
    public LayerMask Interatable, BuildingMask, Unit    ;
    Vector3 MousePosition;
    [SerializeField]
    MainUI MUI;
    [SerializeField]
    BuildingUI BUI;

    Vector3 MouseClickPos, MouseReleasePos;
    void OnMouseClick(Vector3 pos)
    {

        MouseClickPos = pos;
        var tempsel = lastresult.collider.gameObject.GetComponent<entity>();
        if (buildmode >= 0)
        {

            if (BUI.CanBePlaceThere(pos, owners[0])) PlaceBuilding(buildmode, MousePosition,building_highlight.transform.rotation,owners[0]);
        }

        if (buildmode > 0) return;
        if (!selection[0])
        {
            if (tempsel && !(tempsel is building) && tempsel.GetOwner == owners[0])
            {
                selection[0] = tempsel;
                selection[0].OnSelected();
                //UiSelection[0].SetActive(true);
                // UiSelection[1].SetActive(true);
                MUI.Action_sticker.SetTrigger("open");
            }

        }
        else if (currentmode <= 0)
        {
            CancelSelection();
        }
        else
        {

            switch (currentmode)
            {

                case 1: 
                    if(selection.Length <= 1)
                    {
                        foreach (var item in selection)
                            if (item) (item as unit).MoveTo(pos);
                    }
                    else
                    {
                        Formation(pos, _main.transform.forward, selection, .2f);
                    }
                 
               
                    break;
                case 2: if (tempsel && tempsel.GetOwner != owners[0])
                        foreach (var item in selection)
                            if (item) (item as unit).Attack(tempsel);
                    //Need to be clean ahah
                    CancelSelection();
                    CancelSelection();
                    break;
                case 3:
                    if (tempsel && tempsel != selection[0] && (selection[0] is unit) && (selection[0].GetOwner == tempsel.GetOwner))
                    {
                        var x = (selection[0] as unit).Merge(tempsel as unit);
                        CancelSelection();
                        CancelSelection();
                        selection[0] = x;
                        selection[0].OnSelected();
                    }
                    break;
                case 4:
                    (selection[0] as unit).Chill();
                    CancelSelection();
                    CancelSelection();
                    break;
                default:
                    break;
            }



        }
    }

    public Vector3[] Formation(Vector3 pos, Vector3 dir, entity[] e,float dist =2)
    {
        var t = new Vector3[e.Length];
        var q = (int)Mathf.Sqrt(t.Length) + 1;
        for (int x = 0; x <q; x++)
            for (int y = 0; y < q; y++)
            {
                if (y + x * q >= e.Length) break;
                t[y + x * q] = new Vector3(pos.x - q * dist + q * x * dist, pos.y, pos.z - q * dist + q * y * dist);
                (e[y + x * q] as unit).MoveTo(t[y + x * q]);
            }
              
        return t;
    }
    
    void OnMouseHold(Vector3 pos)
    {
        if (TimeWithMouse > .1f)
        {
            MouseReleasePos = pos;
            MUI.BoxSelection(MouseClickPos,MouseReleasePos);
            dragged = true;
        }
      
        TimeWithMouse += Time.fixedDeltaTime;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(TL, Vector3.one / 3f);        Gizmos.DrawCube(TR, Vector3.one / 3f);
        Gizmos.DrawCube(BL, Vector3.one / 3f);        Gizmos.DrawCube(BR, Vector3.one / 3f);
    }
    bool dragged = false;


    //From https://www.habrador.com/tutorials/select-units-within-rectangle/
    //Thing is ,a square on the UI is a parallelograme in 3D space ,because camre have frustrum

    bool IsWithinPolygon(Vector3 unitPos)
    {

        if (IsWithinTriangle(unitPos, TL, BL, TR))
            return true;
        if (IsWithinTriangle(unitPos, TR, BL, BR))
            return true;
        return false;
    }
    Vector3 TL, TR, BL, BR;
    bool FrustrumSelection()
    {
        var mstart  = _main.WorldToScreenPoint(MouseClickPos);
        var mend = _main.WorldToScreenPoint(MouseReleasePos);
        mstart.z = 0;
       var m = (mstart + mend )/ 2f ;

        var x = Mathf.Abs(mend.x - mstart.x);
        var y = Mathf.Abs(mend.y - mstart.y);
        RaycastHit hit;
        int i = 0;
        TL = new Vector3(m.x - x / 2f, m.y + y / 2f, 0f);
        TR = new Vector3(m.x + x/ 2f, m.y +y / 2f, 0f);
        BL = new Vector3(m.x - x / 2f, m.y - y / 2f, 0f);
        BR = new Vector3(m.x + x / 2f,m.y - y / 2f, 0f);
        if (Physics.Raycast(_main.ScreenPointToRay(TL), out hit, 200f,BuildingMask))
        {
            TL = hit.point;
            i++;
        }
        if (Physics.Raycast(_main.ScreenPointToRay(TR), out hit, 200f, BuildingMask))
        {
            TR = hit.point;
            i++;
        }
        if (Physics.Raycast(_main.ScreenPointToRay(BL), out hit, 200f, BuildingMask))
        {
            BL = hit.point;
            i++;
        }
        if (Physics.Raycast(_main.ScreenPointToRay(BR), out hit, 200f, BuildingMask))
        {
            BR = hit.point;
            i++;
        }
        return i == 4;
    }
    bool IsWithinTriangle(Vector3 p, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        bool isWithinTriangle = false;

        //Need to set z -> y because of other coordinate system
        float denominator = ((p2.z - p3.z) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.z - p3.z));

        float a = ((p2.z - p3.z) * (p.x - p3.x) + (p3.x - p2.x) * (p.z - p3.z)) / denominator;
        float b = ((p3.z - p1.z) * (p.x - p3.x) + (p1.x - p3.x) * (p.z - p3.z)) / denominator;
        float c = 1 - a - b;

        //The point is within the triangle if 0 <= a <= 1 and 0 <= b <= 1 and 0 <= c <= 1
        if (a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f)
        {
            isWithinTriangle = true;
        }

        return isWithinTriangle;
    }
    void OnMouseRelease(Vector3 pos)
    {

        if (dragged)
        {

            var s = new List<unit>();
            if (FrustrumSelection())
            {
                //Costly but played once by frame so...

                foreach (var item in FindObjectsOfType<entity>())
                {
                    if(item.GetOwner == owners[0])
                    if(item is unit)
                    {
                        if (IsWithinPolygon(item.transform.position))
                        {

                            item.OnSelected();
                            s.Add(item as unit);
                        }
                    }
               
                }
            }
            if (s.Count > 0) OnDragSelection(s.ToArray());


        }
     

        MUI.BSelection.gameObject.SetActive(false);

        dragged = false;
        TimeWithMouse = 0;
    } 

    public void OnDragSelection(unit[] e)
    {
        selection = e;
        UiSelection[0].SetActive(true);
        UiSelection[1].SetActive(true);
        MUI.Action_sticker.SetTrigger("open");
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
        //UiSelection[0].SetActive(true); // image nad name 
                                        //UiSelection[2].SetActive(true);
        MUI.Action_sticker.SetBool("SWBC",true);

        currentmode = x;
    }
    public void Chillout()
    {
        if (selection[0])
            (selection[0] as unit).Chill();
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
            //  UiSelection[0].SetActive(true); // image nad name 
            //    UiSelection[1].SetActive(true); // main icons
            MUI.Action_sticker.SetBool("SWCB", false);
        }
        else
        {
            if (selection.Length > 0)
                foreach (var item in selection)
                    if(item!=null)item.OnDeselected();
          
            MUI.Action_sticker.SetTrigger("close");
         
            selection = new entity[1];

        }
    }
    public void CancelBuilding()
    {
        if(!BUI.BuildingSticker.GetBool("SWCB")) BUI.BuildingSticker.SetTrigger("close");
        if (buildmode == -1)
            BUI.BuildingSticker.SetBool("SWCB", false);


        buildmode = -1;
       
        ClearHighLight();
        _lastbuilding = null;

    }


    
    float TimeWithMouse = 0, rotationQE;
    void MouseInteraction()
    {
        var ctrl = Input.GetKey(KeyCode.LeftControl);


        var r = Camera.main.ScreenPointToRay(Input.mousePosition);
        var y = Physics.Raycast(r, out lastresult, Mathf.Infinity, Interatable);
        if (buildmode >= 0)
        {
         
            BUI.CanBePlaceThere(lastresult.point,owners[0]);
            /*  if (owners[0].Settled)
                   BUI.Highlight.transform.right = (owners[0].Cores[0].transform.position - BUI.Highlight.transform.position);
              */
            if (building_highlight)
            {
                rotationQE += Time.fixedDeltaTime;
               


                    var lolrot = 0;
                    if(rotationQE > .1f)
                    {
                        if (Input.GetKey(KeyCode.E)) { lolrot = -30; rotationQE = 0; }
                        else if (Input.GetKey(KeyCode.Q)) { lolrot = 30; rotationQE = 0; }

                    }
                   

                    building_highlight.transform.rotation =
                        Quaternion.Euler(building_highlight.transform.eulerAngles +
                        Vector3.up * lolrot);
                 
             
            }
            y = Physics.Raycast(r, out lastresult, Mathf.Infinity, BuildingMask);
        }
        
        if (y)
        {

            if (EventSystem.current.IsPointerOverGameObject()) return;
            MousePosition = lastresult.point;

            if (Input.GetMouseButton(0))
            {
                OnMouseHold(lastresult.point);
            }
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseClick(lastresult.point);
               
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnMouseRelease(lastresult.point);
            }


        }

        if (!ctrl) { BUI.Highlight.transform.position = MousePosition; }
       


    }
    [SerializeField]
    public static entity[] selection = new entity[1];

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
        buildmode = -1;
        ClearHighLight();
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
        BUI.BuildingSticker.SetBool("SWCB", true);
        building_highlight.transform.localRotation = lastrotation;
    }
 
    public void PlaceBuilding(int j , Owner n)
    {
        PlaceBuilding(j, MousePosition, building_highlight.transform.rotation, n);
    }

    
   public building PlaceBuilding(int j,Vector3 pos,Quaternion rot,Owner n)
    {
        var x = Instantiate(Buildings[j],pos, Quaternion.identity).GetComponent<building>();
        x.transform.rotation = rot; //building_highlight.transform.rotation;
        lastrotation = rot;//building_highlight.transform.rotation;
        x.TransferOwner(n);
        x.build(pos, n);
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

        CancelSelection();

        _lastbuilding = x;
        BUI.SetStartingPoint(x.transform.position);
        if (_lastbuilding is Wall)Build(j);
        else BUI.BuildingSticker.SetBool("SWCB", false);
        var e = Physics.OverlapSphere(x.transform.position, x.RequiredCloseness );

        /*
         *    if(x.BuildRoad)
         * foreach (var item in e)
        {        
            if (!item.transform.IsChildOf(x.transform) && item.GetComponent<building>() )
            {
              //  if (Vector3.Distance(item.transform.position, x.transform.position) <= (x.RequiredCloseness/1.3f)) break;

                    BUI.CreateRoad(item.transform.position, x.gameObject);
                   break;
                          
            }
        }-*/
        return x;

    }
    Quaternion lastrotation;
    [HideInInspector]
    public GameObject building_highlight;
    void ClearHighLight()
    {
        if(building_highlight)Destroy(building_highlight.gameObject);
 
    }
    public void CameraFunction(Transform camera, Vector3 position)
    {

        Cursor.lockState = CursorLockMode.Confined;
        var ctrl = Input.GetKey(KeyCode.LeftControl);
        if (Input.GetKey(KeyCode.Mouse1) && !ctrl)
        cursorinput += new Vector2(Input.GetAxis("Mouse X"),
                  Input.GetAxis("Mouse Y")); 

        cursorinput.y = Mathf.Clamp(cursorinput.y, -70, -20);
        var zoooooom = Input.GetAxis("Mouse ScrollWheel");
        if (EventSystem.current.IsPointerOverGameObject()) zoooooom = 0 ;
        camzoom = Mathf.Clamp(camzoom + zoooooom * -350 * Time.smoothDeltaTime, 1, 350);

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
        owners[0].GenFactions();

    }





    //Nodes related - need to yeet to somewhere else 

    node[] CreateNodes(Terrain a, int precision = 6)
    {
        var e = new List<node>();
        var t = a.terrainData;
        for (int x = 0; x < t.size.x; x+=precision)
        {
            for (int y = 0; y < t.size.z; y += precision)
            {
                var pos = new Vector3(x, t.GetHeight(x/* + precision*/ , y /*+ precision*/), y);
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
   //Sparse Ressources, so it is not easy
        if (n.AverageHeight > 28.5f && n.AverageHeight < 32 && n.type == global::node.NodeType.plain)
        {
            if (seed < .6f) return;
            //tree
            var q = new Goods() ;
           
            q.setRessource(Resources[0], 100 * Random.Range(1, 10));
          //  q.transform.parent = n.transform;
            n.resource = q;
           
            n.Value += n.resource.getAmount* n.Value;
           
            for (int i = 0; i <n.resource.getAmount/50; i++)
            {
                if (Random.Range(0, 1f) > .3f) continue;
                var t = new Vector3(Random.Range(-n.getSize / 1.4f, n.getSize/1.4f) , .45f, Random.Range(-n.getSize / 1.4f, n.getSize / 1.4f));
               
                var e = Instantiate(n.resource.model, n.transform.position,Quaternion.identity).GetComponent<GetRessourceInfo>();
                e.SetNode(n);

                e.transform.position = n.transform.position + t;
                e.transform.parent = n.transform;
            }

            return;
        }

        if (  n.AverageHeight > 28 && n.AverageHeight < 35)
        {

            if (seed < .93f) return;
            var q = new Goods();

            q.setRessource(Resources[1], 75 * Random.Range(3, 20));
            //  q.transform.parent = n.transform;
            n.resource = q;

            n.Value += n.resource.getAmount * n.Value;

            for (int i = 0; i < n.resource.getAmount / 80; i++)
            {   
                if (Random.Range(0, 1f) > .3f) continue;
                var t = new Vector3(Random.Range(-n.getSize / 1.4f, n.getSize / 1.4f)  ,-.1f, Random.Range(-n.getSize / 1.4f, n.getSize / 1.4f)  );

                var e = Instantiate(n.resource.model, n.transform.position, Quaternion.identity).GetComponent<GetRessourceInfo>();
                e.SetNode(n);
                e.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360);
                e.transform.position = n.transform.position + t;
                e.transform.parent = n.transform;
            }
        }
    }
}
