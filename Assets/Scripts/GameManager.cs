using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static entity LastClick;
    public MeshRenderer Fog;
    public Terrain[] terrain;
    public GameObject radius;
    public node[] Nodes;
    public static Owner[] owners = new Owner[5]
    { new Owner() { Name = "Wessex", MainColor = Color.blue, vector3 = new Vector3(368, 0, 177) },
        new Owner() { Name = "Picts", MainColor = Color.green, vector3 = new Vector3(309, 0, 273) },
         new Owner() { Name = "Neutral", MainColor = Color.gray, vector3 = new Vector3(0, 0, 0) },
         new Owner() { Name = "Wels", MainColor = Color.yellow, vector3 = new Vector3( 259, 0, 200) },
          new Owner() { Name = "Dimitri", MainColor = Color.magenta, vector3 = new Vector3(200, 0, 193) },


    };




    //Should have use a dictionary, gonna change it later, for now let's use that
    public static Owner GetOwner(string a)
    {
        foreach (var item in owners)
        {
            if (item.Name == a) return item;
        }
        return null;
    }
    public static float SecondPerGenerations = 60;
    public static bool DEBUG_GODMODE = false;
    public GameObject Help;
    [Header("Assets")]
    public GameObject node;
    public GameObject node_bound;
    [Header("Resource")]
    public Goods[] Resources;
    public GameObject[] Buildings;
    public GameObject _army;
    public GameObject[] Missiles;
    public static GameObject ArmyPrefab;
    public GameObject NodeRendererPrefab;
    public GameObject healingPrefab,DivnityPrefab;

    Camera _main;

    //Todo Remove
    public void ShowBuildingDesc(int x)
    {
        var y = Buildings[x].GetComponent<building>();
        MUI.showDescription(y.name, y.GetSummary());
    }

    public void HelpM(bool t)
    {
        Help.gameObject.SetActive(t);
        AudioSource.PlayClipAtPoint(GameManager.instance.menuClick, Camera.main.transform.position);
    }
    [Header("Flair")]
    public AudioClip error;
    public AudioClip build, completeBuild, menuClick, GainItem, endaudio, GameOverMusic, War;
    public GameObject[] Cursor3D;

    [Header("Camera")]
    public Vector3 CameraOffset;
    public Vector3 CameraPosition;
    public float Boundary = 40;
    public float cameraSmoothness = 6;
    public float EdgeScrollingSpeed = 5;
    Vector2 cursorinput;

    public grumbleAMP grumbleAMP;
    private void Awake()
    {
        instance = this;
        buildmode = -1;
        ArmyPrefab = _army;
        CancelSelection();
        _main = UnityEngine.Camera.main;

        foreach (var item in owners)
        {
            if (item.Name == "Neutral") continue;
            item.OnGain += OnOwnerGain;
            item.OnRelationModification += OnPlayerRelationshipChanged;
        }

        for (int i = 1; i < owners.Length; i++)
        {
            if (owners[i].Name == "Neutral") continue;
            var t = gameObject.AddComponent<Owner_AI>();
            t.owner = owners[i];
            t.TBC += Random.Range(-3f, 6f);
        }
    }

    public static void ShowMessage(string f)
    {
        GameManager.instance._pup.SetText(f);
    }
    public void DeclareWar(Owner z)
    {
        if (AtWarWith.ContainsKey(z.Name))
            if (AtWarWith[z.Name]) return;


        _pup.SetText("You are now peacen't with " + z.Name + "!");
        AtWarWith.Add(z.Name, true);
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
    public void OnHeal(Vector3 pos)
    {
        var e = Instantiate(healingPrefab, pos, Quaternion.identity);
        if (e)
            StartCoroutine(popup(e));
    }
    public void OnBoost(Vector3 pos)
    {
        var e = Instantiate(DivnityPrefab, pos, Quaternion.identity);
        if (e)
            StartCoroutine(popup(e));
    }
    Dictionary<string, bool> AtWarWith = new Dictionary<string, bool>();
    public void OnPlayerRelationshipChanged(Owner p1, Owner p2, float val)
    {
         Owner own, at;
        if (!(p1 == owners[0] || p2 == owners[0])) return;
        if (p1 == owners[0]) { own = p1; at = p2; }
        else { own = p2;  at = p1; } 

 
        if(at.Relation.ContainsKey(own.Name) && at.Relation[own.Name] < -50)
            if (own.Relation.ContainsKey(at.Name) && own.Relation[at.Name] < -50)
                DeclareWar(at);


        
    }

    [SerializeField]
    Animator animBlack;
    [SerializeField]
    GameObject GameOverItem;

 
    bool Loser = false;
    public static void SetGameOver()
    {
        instance.Loser = true;
        instance.StartCoroutine(instance.GameOver());
        
    }
    IEnumerator GameOver()
    {
        foreach (var item in GetComponents<Owner_AI>())
            item.enabled = false;
        

        var audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
       
        AudioSource.PlayClipAtPoint(GameManager.instance.endaudio, Camera.main.gameObject.transform.position);
       
        yield return new WaitForSecondsRealtime(2f);
        audioSource.clip = GameOverMusic;
        audioSource.Play();
        animBlack.SetTrigger("fade");
        yield return new WaitForSecondsRealtime(3f);
        GameOverItem.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        GameOverItem.SetActive(false);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1;
        instance = null;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
      

        yield break;
    }


    IEnumerator popup(GameObject c)
    {
        AudioSource.PlayClipAtPoint(GameManager.instance.GainItem, c.transform.position);
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
        if (Loser) return;
        foreach (var item in owners)
            item.Routine();

        if (owners.Length > 0)
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
    public LayerMask Interatable, BuildingMask, Unit;
    Vector3 MousePosition;
    [SerializeField]
    MainUI MUI;
    [SerializeField]
    BuildingUI BUI;



    bool dragged = false;
    Vector3 MouseClickPos, MouseReleasePos;

    #region Mouse Interaction and the such
    void MouseInteraction()
    {
        if (Loser) return;
        var ctrl = Input.GetKey(KeyCode.LeftControl);


        var r = Camera.main.ScreenPointToRay(Input.mousePosition);
        var y = Physics.Raycast(r, out lastresult, Mathf.Infinity, Interatable);
        if (buildmode >= 0)
        {
         
            BUI.CanBePlaceThere(lastresult.point, owners[0]);
            /*  if (owners[0].Settled)
                   BUI.Highlight.transform.right = (owners[0].Cores[0].transform.position - BUI.Highlight.transform.position);
              */
            if (building_highlight)
            {
                rotationQE += Time.fixedDeltaTime;



                var lolrot = 0;
                if (rotationQE > .1f)
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


            if (EventSystem.current.IsPointerOverGameObject()) {OnMouseRelease(lastresult.point); return; } 
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

            foreach (var item in Cursor3D)
            {
                if (item) item.transform.position = lastresult.point + Vector3.up * .3f;
            }
        }

        if (!ctrl && BUI.Highlight) { BUI.Highlight.transform.position = MousePosition; }



    }

    void OnMouseClick(Vector3 pos)
    {

        MouseClickPos = pos;
        var tempsel = lastresult.collider.gameObject.GetComponent<entity>();
        if (buildmode >= 0)
        {

            if (BUI.CanBePlaceThere(pos, owners[0])) PlaceBuilding(buildmode, MousePosition, building_highlight.transform.rotation, owners[0]);
        }
      

        if (buildmode > 0) return;
        BUI.SetBList(false);
        if (!selection[0])
        {
            if (tempsel && !(tempsel is building) && tempsel.GetOwner == owners[0])
            {
                selection[0] = tempsel;
                selection[0].OnSelected();
                LastClick = selection[0];
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
                    if (selection.Length <= 1)
                    {
                        foreach (var item in selection)
                            if (item) (item as unit).MoveTo(pos);
                    }
                    else
                    {
                        print("Moving " + selection.Length + " peps to " + pos);
                        Formation(pos, _main.transform.forward, selection, .2f);
                    }
                    CancelSelection(1);

                    break;
                case 2:
                    var s = Physics.OverlapSphere(pos, 1, GameManager.instance.Unit, QueryTriggerInteraction.Collide);
                    Chillout();

                    foreach (var item in s)
                    {
                       var vs = item.GetComponent<entity>();
                        if (!vs || vs.GetOwner == owners[0] ) continue;
                        foreach (var itddem in selection)
                            if (item)
                                (itddem as unit).TargetToHunt.Enqueue(vs);


                    }

                     foreach (var item in selection)
                    {
                   
                       if((item as unit).TargetToHunt.Count > 0)
                            if (item)
                                (item as unit).OrderedAttack((item as unit).TargetToHunt.Dequeue());

                    }
                        


                    CancelSelection(0);
                    break;
                case 4:

   Chillout();
                    CancelSelection(1);
                    break;
                default:
                    break;
            }



        }
    }

    void OnMouseHold(Vector3 pos)
    {
        var h = Screen.height;
        var w = Screen.width;
        if (pos.x > w || pos.y < 0) return;
        if (pos.y> h || pos.y < 0) return;
        if (TimeWithMouse > .04f )
        {
            MouseReleasePos = pos;
            MUI.BoxSelection(MouseClickPos, MouseReleasePos);
            dragged = true;
                
        }

        TimeWithMouse += Time.fixedDeltaTime;
    }

    void OnMouseRelease(Vector3 pos)
    {
        LastClick = null;
        if (dragged)
        {

            var s = new List<unit>();
            if (FrustrumSelection())
            {
                //Costly but played once by frame so...

                foreach (var item in FindObjectsOfType<entity>())
                {
                    if (item.GetOwner == owners[0])
                        if (item is unit)
                        {
                            if (IsWithinPolygon(item.transform.position))
                            {

                                item.OnSelected();
                                s.Add(item as unit);
                            }
                        }

                }
            }
            if (s.Count > 0)
            {

                //  countsoldierspear.text = s.Count.ToString("D4"); this also 

                OnDragSelection(s.ToArray());
            }


        }


        MUI.BSelection.gameObject.SetActive(false);

        dragged = false;
        TimeWithMouse = 0;
    }
    public Unit_UI[] unit_UIs;

    /*int LengthOfArray(unit[][] b , int a)
    {
        var c = 0;
        foreach (var x in b)
        {

        }
  
    }*/
    public void OnDragSelection(unit[] e)
    {
        selection = e;
        var se = new  List<unit>[99];
        for (int i = 0; i < e.Length; i++)
        {
            if (se[e[i].ID] == null /*|| se[e[i].ID].Length <= 0*/) { se[e[i].ID] = new List<unit>();  }
            se[e[i].ID].Add(e[i]) ;
        }

        for (int i = 0; i < se.Length; i++)
            if (se[i] != null) unit_UIs[i].General(se[i].ToArray(), i);

        MUI.attack.SetActive(true);
        //countsoldierspear.text = e.ToString(); nevermind this 
        /*UiSelection[0].SetActive(true);
        UiSelection[1].SetActive(true);*/
        //MUI.Action_sticker.SetTrigger("open");
    }
    #endregion
    public static Vector3[] Formation(Vector3 pos, Vector3 dir, entity[] e, float dist = 2)
    {
        var t = new Vector3[e.Length];
        var q = (int)Mathf.Sqrt(t.Length) + 1;
        for (int x = 0; x < q; x++)
            for (int y = 0; y < q; y++)
            {
                if (y + x * q >= e.Length) break;
                t[y + x * q] = new Vector3(pos.x - q * dist + q * x * dist, pos.y, pos.z - q * dist + q * y * dist);
                if(e[y + x * q] && e[y + x * q] is unit ) (e[y + x * q] as unit).MoveTo(t[y + x * q]);
            }

        return t;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(TL, Vector3.one / 3f); Gizmos.DrawCube(TR, Vector3.one / 3f);
        Gizmos.DrawCube(BL, Vector3.one / 3f); Gizmos.DrawCube(BR, Vector3.one / 3f);
    }
    #region Frustrum Detection
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
        var mstart = _main.WorldToScreenPoint(MouseClickPos);
        var mend = _main.WorldToScreenPoint(MouseReleasePos);
        mstart.z = 0;
        var m = (mstart + mend) / 2f;

        var x = Mathf.Abs(mend.x - mstart.x);
        var y = Mathf.Abs(mend.y - mstart.y);
        RaycastHit hit;
        int i = 0;
        TL = new Vector3(m.x - x / 2f, m.y + y / 2f, 0f);
        TR = new Vector3(m.x + x / 2f, m.y + y / 2f, 0f);
        BL = new Vector3(m.x - x / 2f, m.y - y / 2f, 0f);
        BR = new Vector3(m.x + x / 2f, m.y - y / 2f, 0f);
        if (Physics.Raycast(_main.ScreenPointToRay(TL), out hit, 200f, BuildingMask))
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
    #endregion
    public GameObject[] UiSelection;
    int currentmode = 0;
    entity target;

   
    public void SetUIMode(int x)
    {
        if (Loser) return;
        if (x == 0) return;
        foreach (var item in UiSelection)
        {
            item.SetActive(false);
        }

        foreach (var item in Cursor3D)
        {
            item.SetActive(false);
        }
        //why bother doing a switch tbh
        if (x == 2) Cursor3D[1].gameObject.SetActive(true);
        if (x == 3) Cursor3D[2].gameObject.SetActive(true);
        // if (x > 0) Cursor3D[Mathf.Clamp(x - 1, 0, Cursor3D.Length - 1)].SetActive(true);
        //UiSelection[0].SetActive(true); // image nad name 
        //UiSelection[2].SetActive(true);
        MUI.Action_sticker.SetBool("SWBC", true);
        AudioSource.PlayClipAtPoint(GameManager.instance.menuClick, Camera.main.transform.position);
        currentmode = x;
    }
    public void Chillout()
    {
        /*  if (selection[0])
              (selection[0] as unit).Chill();*/
        foreach (var item in selection)
        {
            (item as unit).Chill();
        }
    }

    public void CancelSelection(int a = 0)
    {
        if (a > 0) CancelSelection(a - 1);
        foreach (var item in UiSelection)
        {
            item.SetActive(false);
        }
        foreach (var item in Cursor3D)
        {
            item.SetActive(false);
        }
        if (currentmode > 0)
        {
            currentmode = 0;
            //  UiSelection[0].SetActive(true); // image nad name 
            //    UiSelection[1].SetActive(true); // main icons
            //MUI.Action_sticker.SetBool("SWCB", false);

            //MUI.attack.SetActive(false);
        }
        else
        {
            if (selection.Length > 0)
                foreach (var item in selection)
                    if (item != null) item.OnDeselected();

            // MUI.Action_sticker.SetTrigger("close");
            MUI.attack.SetActive(false);
            selection = new entity[1];
            foreach (var item in unit_UIs)            
                item.Reset();
            MUI.EndDescription();
            BUI.SetBList(false);
        }
    }
    public void CancelBuilding()
    {
        if (!BUI.BuildingSticker.GetBool("SWCB")) BUI.BuildingSticker.SetTrigger("close");
        if (buildmode == -1)
            BUI.BuildingSticker.SetBool("SWCB", false);


        buildmode = -1;

        ClearHighLight();
        _lastbuilding = null;

    }



    float TimeWithMouse = 0, rotationQE;

    [SerializeField]
    public static entity[] selection = new entity[1];


    //Relate to camera
    Vector3 EdgeScrolling
    {
        get
        {
            Vector3 t = Vector3.zero;
            var pos = Input.mousePosition;
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");


            /*
                   if (pos.x > Screen.width - Boundary) t += Vector3.right;
                   if (pos.x < 0 + Boundary) t += -Vector3.right;
                   if (pos.y > Screen.height - Boundary) t += Vector3.up;
                   if (pos.y < 0 + Boundary) t += -Vector3.up;
 */

             t += Vector3.right * x;
             t +=  Vector3.up * y;


            return t;


        }


    }
    Vector2 cam;
    private float camzoom;
    public float GetZoomLevel
    {
        get { return camzoom; }
    }
    [SerializeField]
    int buildmode = -1;

    public PopMessage _pup, _buildpup;

    building _lastbuilding;
    public void Build(int x)
    {
        if (Loser) return;
        //BUI.SetBList(false);
        MUI.EndDescription();
        buildmode = -1;
        ClearHighLight();
        if (!Buildings[x].GetComponent<building>().HasEnoughRessource(owners[0].Inventory, owners[0].Gold, true))
        {
            print(owners[0] + " :Not enough ressource or Gold");
            _lastbuilding = null;
            return;
        }
        
        var g = Instantiate(Buildings[x].GetComponent<building>().graphics[1], BUI.Highlight.transform);
        building_highlight = g;
        var yr = Instantiate(radius, building_highlight.transform);
        yr.SetActive(true);
        yr.transform.position = g.transform.position + Vector3.up * .1f; ;
        yr.transform.localScale = Vector3.one * (Buildings[x].GetComponent<building>().RequiredCloseness);
        BUI.Planing(g, Buildings[x].GetComponent<building>());
        
      //  radius.gameObject.SetActive(true);
        //radius.transform.localScale = new Vector3(Buildings[x].)
        var t = g.GetComponentsInChildren<Collider>();
        for (int i = 0; i < t.Length; i++)
        {
            Destroy(t[i]);
        }
        buildmode = x;
        BUI.BuildingSticker.SetBool("SWCB", true);
        building_highlight.transform.localRotation = lastrotation;
        AudioSource.PlayClipAtPoint(GameManager.instance.menuClick, Camera.main.transform.position);
     
    }

    public void PlaceBuilding(int j, Owner n)
    {
        PlaceBuilding(j, MousePosition, building_highlight.transform.rotation, n);
    }


    public building PlaceBuilding(int j, Vector3 pos, Quaternion rot, Owner n)
    {
     
        var x = Instantiate(Buildings[j], pos, Quaternion.identity).GetComponent<building>();
        x.transform.rotation = rot; //building_highlight.transform.rotation;
        lastrotation = rot;//building_highlight.transform.rotation;
        x.TransferOwner(n);
   
        x.build(pos, n);
        x.Tier = 0;
        n.Pay(x.costs[0].materials);

   
 /*
        if (_lastbuilding && _lastbuilding is Wall && x is Wall)
        {
            (_lastbuilding as Wall).boundTo = x as Wall;

        }*/

        if (n == owners[0])
        {
            buildmode = -1;
            ClearHighLight();
            if (n.Settled)
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
            CancelSelection();
        }


        _lastbuilding = x;
        BUI.SetStartingPoint(x.transform.position);
        if (n == owners[0]) BUI.BuildingSticker.SetBool("SWCB", false);


        /*
       var e = Physics.OverlapSphere(x.transform.position, x.RequiredCloseness);
         * 
         * 
         * 
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

        AudioSource.PlayClipAtPoint(build, x.transform.position);
        return x;

    }
    Quaternion lastrotation;
    [HideInInspector]
    public GameObject building_highlight;
    void ClearHighLight()
    {
        if (building_highlight) Destroy(building_highlight.gameObject);

    }
    public void CameraFunction(Transform camera, Vector3 position)
    {
        if (Loser) return;
        Cursor.lockState = CursorLockMode.Confined;
        var ctrl = Input.GetKey(KeyCode.LeftControl);
        if (Input.GetKey(KeyCode.Mouse1) && !ctrl)
            cursorinput += new Vector2(Input.GetAxis("Mouse X"),
                      Input.GetAxis("Mouse Y"));

        cursorinput.y = Mathf.Clamp(cursorinput.y, -70, -20);
        var zoooooom = Input.GetAxis("Mouse ScrollWheel");
        if (EventSystem.current.IsPointerOverGameObject()) zoooooom = 0;
        camzoom = Mathf.Clamp(camzoom + zoooooom * -350 * Time.smoothDeltaTime, 1, 250);

        camera.transform.position = Vector3.Lerp(camera.transform.position, position + Vector3.forward * camzoom, 125 * Time.fixedDeltaTime);
        camera.transform.LookAt(position + CameraOffset + -Vector3.up * cameraSmoothness / 2 * Time.fixedDeltaTime);
        camera.transform.RotateAround(position, Vector3.up, cursorinput.x * cameraSmoothness * 8 * Time.fixedDeltaTime);
        camera.transform.RotateAround(position, camera.transform.right, (-cursorinput.y) * 8 * cameraSmoothness * Time.fixedDeltaTime);


        var es = _main.gameObject.transform.TransformDirection(-EdgeScrolling * (1 + Mathf.Abs(camzoom) / 20) * EdgeScrollingSpeed * cameraSmoothness * Time.smoothDeltaTime);



        if (Input.GetMouseButton(2)) es = _main.gameObject.transform.TransformDirection((new Vector2(Input.GetAxis("Mouse X"),
                    Input.GetAxis("Mouse Y"))) * (1 + Mathf.Abs(camzoom) / 20) * EdgeScrollingSpeed * cameraSmoothness * Time.smoothDeltaTime);


        //if(camera.transform.eulerAngles.x > (90 -30) && camera.transform.eulerAngles.x <(90 +1))

        es.y = 0;
        CameraPosition += es;
    }
    private void Start()
    {
 
        Nodes = CreateNodes(terrain[0]);

        foreach (var item in GameManager.instance.Nodes)
        {
            item.SetOwner(owners[2]);
        }
        foreach (var item in unit_UIs)
            item.Reset();

        foreach (var item in owners)
        {
            if (item.Name == "Neutral") continue;
            item.Start();
            for (int i = 0; i < owners.Length; i++)
            {
                if (i == 2) continue;
                item.modRelation(owners[i], Random.Range(-30, 30));
            }
               



        }




    }

    #region Nodes
    //Nodes related - need to yeet to somewhere else 

    node[] CreateNodes(Terrain a, int precision = 7)
    {
        var e = new List<node>();
        var t = a.terrainData;
        for (int x = 0; x < t.size.x; x += precision)
        {
            for (int y = 0; y < t.size.z; y += precision)
            {
                var pos = new Vector3(x, t.GetHeight(x/* + precision*/ , y /*+ precision*/), y);
                var n = Instantiate(node, a.transform.position + new Vector3(x + precision / 2, t.GetHeight(x + 5, y + 5), y + precision / 2), Quaternion.identity).GetComponent<node>();
                n.SetSize(precision);
                n.Initialize(x, y);
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
                if (n.AverageHeight > 43 && n.AverageHeight < 55 && Random.Range(0, 1f) > .3f)
                {

                    n.type = global::node.NodeType.plain;
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
                RaycastHit harambe;
                var extraPrecision = Physics.Raycast(pos + Vector3.up, Vector3.down, out harambe, 9);
                n.transform.position = morePrecision(n.transform.position);
                n.transform.parent = a.transform;
                SpawnRessource(n, x, y);
                e.Add(n);
            }
        }


        return e.ToArray();
    }
    //Costly
    Vector3 morePrecision(Vector3 pos)
    {
        RaycastHit harambe;
        var extraPrecision = Physics.Raycast(pos + Vector3.up, Vector3.down, out harambe, 9, Precison, QueryTriggerInteraction.Ignore);
        if (extraPrecision)
            pos = harambe.point;
        return pos;
    }
    [SerializeField]
    LayerMask Precison;
    void SpawnRessource(node n, int x, int y)
    {

        //tree for now
        var seed = Random.Range(0, 1f);
        //Sparse Ressources, so it is not easy
        if (n.AverageHeight > 49 && n.AverageHeight < 55 && n.type == global::node.NodeType.plain)
        {
            if (seed < .6f) return;
            //tree

            var q = new Goods();

            q.setRessource(Resources[0], 100 * Random.Range(1, 10));
            //  q.transform.parent = n.transform;
            n.resource = q;

            n.Value += n.resource.getAmount * n.Value;

            for (int i = 0; i < n.resource.getAmount / 50; i++)
            {
                if (Random.Range(0, 1f) > .3f) continue;
                var t = new Vector3(Random.Range(-n.getSize / 1.4f, n.getSize / 1.4f), .45f, Random.Range(-n.getSize / 1.4f, n.getSize / 1.4f));



                var e = Instantiate(n.resource.model, n.transform.position, Quaternion.identity).GetComponent<GetRessourceInfo>();
                e.SetNode(n);

                e.transform.position = n.transform.position + t;
                e.transform.parent = n.transform;
                e.transform.localScale *= 2;
                e.transform.position = morePrecision(e.transform.position) + Vector3.up * 1f;
            }

            return;
        }

        if (n.AverageHeight > 28 && n.AverageHeight < 35)
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
                var t = new Vector3(Random.Range(-n.getSize / 1.4f, n.getSize / 1.4f), -.1f, Random.Range(-n.getSize / 1.4f, n.getSize / 1.4f));

                var e = Instantiate(n.resource.model, n.transform.position, Quaternion.identity).GetComponent<GetRessourceInfo>();
                e.SetNode(n);
                e.transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 360);
                e.transform.position = n.transform.position + t;
                e.transform.parent = n.transform;
            }
        }
    }

    #endregion
}
