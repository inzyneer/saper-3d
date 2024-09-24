using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class BackgroundLogic : MonoBehaviour
{
    #region Inspector
    [Header("Objects")]
    public GameObject prefab;
    public GameObject bomb;
    public GameObject flag;
    public GameObject red_x_symbol;
    public GameObject FlagNumber;
    [Header("UI")]
    public GameObject toggleHolder;
    public GameObject customX;
    public GameObject customY;
    public GameObject customMines;
    public EventSystem eventSystem;

    
    [Header("Vars")]
    public int gridWidth = 16;
    public int gridHeight = 16;
    public int mines = 20;
    public int mineChance = 50;
    public float pushForce = 500f;
    public float tileMass = 20f;
    public int le = 0;
    #endregion

    public Renderer rd;
    public LayerMask hitMask;

    public static int level;

    private static int customxx;
    private static int customyy;
    private static int custommm;

    int flagCount = 0;
    private Vector3 originPoint = new Vector3(0f,-13f,9f);

    bool mouseclicked0 = false;
    bool mouseclicked1 = false;
    bool generated = false;
    int gamestate = 3;

    GameObject cam_obj;
    GameObject[,] Tiles = null;
    Dictionary<int, GameObject> Bombs = new Dictionary<int, GameObject>();
    List<int> mineCoords = new List<int>();

    Rigidbody temp_rb;
    TMP_Text temp_txt;
    Transform temp_transform;

    //Material mat_green;
    //Material mat_def;

    // Start is called before the first frame update
    void Start()
    {
        //mat_green = Resources.Load("flag_mat_green", typeof(Material)) as Material;
        //mat_def = Resources.Load("flag_mat_red", typeof(Material)) as Material;
        //Application.isMobilePlatform()
        GameStart(level);
    }

    private void GameStart(int level)
    {
        cam_obj = GameObject.Find("MainCam");
        Camera cam = cam_obj.GetComponent<Camera>();
        DragCamera drag = cam_obj.GetComponent<DragCamera>();
        Debug.Log(level);

        customX.GetComponent<Slider>().SetValueWithoutNotify(customxx);
        customX.GetComponentInChildren<TMP_Text>().text = customxx.ToString();

        customY.GetComponent<Slider>().SetValueWithoutNotify(customyy);
        customY.GetComponentInChildren<TMP_Text>().text = customyy.ToString();

        customMines.GetComponent<Slider>().SetValueWithoutNotify(custommm);
        customMines.GetComponentInChildren<TMP_Text>().text = custommm.ToString();

        switch (level)
        {
            case 0:
                {
                    gridWidth = 14;
                    gridHeight = 14;
                    mines = 20;

                    cam.orthographicSize = 20;
                    originPoint -= new Vector3(gridWidth,0,0);
                    break;
                }
            case 1:
                {
                    gridWidth = 20;
                    gridHeight = 14;
                    mines = 40;
                    
                    cam.orthographicSize = 20;
                    originPoint -= new Vector3(gridWidth, 0, 0);
                    break;
                }
            case 2:
                {
                    gridWidth = 40;
                    gridHeight = 14;
                    mines = 90;

                    cam_obj.transform.position += new Vector3(0,5f,0);
                    cam.orthographicSize = 24;
                    originPoint -= new Vector3(gridWidth, 0, 0);
                    break;
                }
            case 3:
                {
                    gridWidth = customxx;
                    gridHeight = customyy;

                    if (customxx == 0)
                        gridWidth = 10;
                    if (customyy == 0)
                        gridHeight = 10;
                    if (custommm == 0)
                        mines = 1;

                    if (custommm >= gridWidth*gridHeight)
                        mines = gridWidth * gridHeight - 10;
                    else
                        mines = custommm;

                    if (gridWidth > 14 || gridHeight > 20)
                    {
                        if (gridHeight >= gridWidth)
                            cam.orthographicSize = (float)(gridHeight / 2) + gridHeight * 0.7f;
                        else
                            cam.orthographicSize = (float)(gridWidth / 2) + gridWidth * 0.3f;
                        cam_obj.transform.position = new Vector3(0, (float)(gridHeight / 2 + gridHeight * 0.3f), -100);
                    }

                    
                    drag.cameraBoundsLeft = -gridWidth;
                    drag.cameraBoundsRight = gridWidth;
                    drag.cameraBoundsUp = gridHeight*2 - 13f;
                    drag.cameraBoundsDown = -13f;

                    originPoint -= new Vector3(gridWidth, 0, 0);


                    break;
                }
            default:
                {
                    break;
                }
        }
        
        Tiles = new GameObject[gridWidth, gridHeight];
        temp_txt = FlagNumber.GetComponent<TMP_Text>();

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {

                Tiles[i, j] = GameObject.Instantiate(prefab) as GameObject;

                Tiles[i, j].transform.position = new Vector3(
                    i * 2f + 0.01f,
                    j * 2f + 0.01f,
                    0) + originPoint;

                Rigidbody rb = Tiles[i, j].GetComponentInChildren<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseclicked0 = true;
        }
        if (Input.GetMouseButtonDown(1))
        {
            mouseclicked1 = true;
        }

        temp_txt.text = "flags:" + (mines - flagCount);
        int counter = 0;
        for (int i = 0; i <= mineCoords.Count - 2; i += 2)
        {
            temp_transform = Tiles[mineCoords[i], mineCoords[i + 1]].transform.Find("button");
            if (temp_transform.Find("flag(Clone)") != null)
                counter++;
        }
        if (counter >= mines)
        {
            GameStatusUpdate(1);
        }
    }
    void FixedUpdate()
    {
        if (mouseclicked0)
        {
            TileHitLogic(0);
            mouseclicked0 = false;
        }
        if (mouseclicked1)
        {
            TileHitLogic(1);
            mouseclicked1 = false;
        }
    }

    private void GameStatusUpdate(int status)
    {
        GameObject text = GameObject.Find("MainText");

        if (status==1)
        {
            gamestate = 1;
            text.GetComponent<TMP_Text>().text = "win";
        } else if (status==0)
        {
            gamestate = 0;
            ShowAllBombs();
            text.GetComponent<TMP_Text>().text = "fail";
        }
    }

    private void TileHitLogic(int button)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // raycast from mouse position

        if (Physics.Raycast(ray, out RaycastHit hitData, 1000.0f, hitMask))
        {
            GameObject gm = hitData.collider.gameObject;

            Rigidbody _rb;
            Transform txt_tm;
            TMP_Text txt;
            GameObject _flag;
            switch (gm.name)
            {
            case "restart":
                {
                    toggleHolder.GetComponent<ToggleGroupScript>().GetSelected();
                    customxx = (int)customX.GetComponent<SliderScript>().value;
                    customyy = (int)customY.GetComponent<SliderScript>().value;
                    custommm = (int)customMines.GetComponent<SliderScript>().value;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    return;
                }
            case "button":
                {
                    if (gamestate == 0)
                    {
                        return;
                    }
                    if (generated)
                    {
                        _rb = hitData.collider.attachedRigidbody;
                        txt_tm = hitData.collider.transform.parent.Find("fieldNumber");
                        txt = txt_tm.GetComponent<TMP_Text>();


                        if (button == 0 && _rb.transform.tag != "fallen")
                        {
                            int[] arr = GetIndexOf(hitData.transform.parent.gameObject);

                            if (gm.transform.Find("flag(Clone)") == null)
                            {

                                if (CheckIfBombExists(arr[0], arr[1]))
                                {
                                    GameObject _iks = GameObject.Instantiate(red_x_symbol) as GameObject;
                                    _iks.transform.position = gm.transform.position + new Vector3(-0.92f, 1.58f, 0);
                                    GameStatusUpdate(0);
                                    return;
                                }


                                if (txt.text == "")
                                {
                                    PushObject(_rb);
                                    CalculateEmpty(arr[0], arr[1]);
                                }
                                else
                                {
                                    PushObject(_rb);
                                }

                            }
                                                        
                        }


                        if (button == 1)
                        {
                            if (gm.transform.Find("flag(Clone)") == null && _rb.transform.tag != "fallen" && flagCount < mines)
                            {
                                flagCount++;
                                _flag = GameObject.Instantiate(flag) as GameObject;
                                _flag.transform.SetParent(gm.transform);
                                _flag.transform.position = gm.transform.position + new Vector3(0, 0, -1f);
                            }
                            else if (gm.transform.Find("flag(Clone)") != null && _rb.transform.tag != "fallen" && flagCount <= mines)
                            {
                                DestroyFlag(gm);
                            }

                        }
                        return;
                    }
                    else
                    {
                        if (hitData.transform.gameObject.name == "button" && button == 0 && gm.transform.Find("flag(Clone)") == null)
                        {
                            _rb = hitData.collider.attachedRigidbody;
                            txt_tm = hitData.collider.transform.parent.Find("fieldNumber");
                            txt = txt_tm.GetComponent<TMP_Text>();

                            int[] arr = GetIndexOf(hitData.transform.parent.gameObject);
                            GenerateBombs(arr[0], arr[1]);
                            CalculateNeighbors();

                            if (CheckIfBombExists(arr[0], arr[1]))
                            {
                                GameObject _iks = GameObject.Instantiate(red_x_symbol) as GameObject;
                                _iks.transform.position = gm.transform.position + new Vector3(-0.92f, 1.58f, 0);
                                GameStatusUpdate(0);
                                return;
                            }
                            else if (txt.text == "")
                            {
                                PushObject(_rb);
                                CalculateEmpty(arr[0], arr[1]);
                            }
                            else
                            {
                                PushObject(_rb);
                            }
                            
                            generated = true;
                        }
                        return;
                    }

                }

            }

        }

    }

    private void PushObject(Rigidbody _rb)
    {
        _rb.transform.tag = "fallen";
        _rb.constraints = RigidbodyConstraints.None;
        _rb.AddForce(Vector3.back * pushForce * Time.fixedDeltaTime);
        _rb.AddTorque(UnityEngine.Random.Range(100, 300), UnityEngine.Random.Range(100, 300), UnityEngine.Random.Range(100, 300));
        _rb.useGravity = true;
        _rb.mass = tileMass;
    }

    IEnumerator PushDelay()
    {
        PushObject(temp_rb);
        temp_rb = null;
        yield return new WaitForSeconds(0.5f);
    }

    private void CalculateNeighbors()
    {
        float count = 0;
        Transform tm;
        TMP_Text text;

        for (int i = 0; i < gridWidth; i++) 
        {
            for (int j = 0; j < gridHeight; j++) 
            {
                if (!CheckIfBombExists(i, j))
                {
                    tm = Tiles[i,j].transform.Find("fieldNumber");
                    text = tm.GetComponent<TMP_Text>();
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            int nRow = i + x;
                            int nCol = j + y;

                            if (nRow >= 0 && nRow < gridWidth && nCol >= 0 && nCol < gridHeight)
                            {
                                if(CheckIfBombExists(nRow, nCol))
                                    count++;
                            }
                        }
                    }
                    if (count != 0)
                    {
                        text.SetText(count.ToString());
                    }
                    count = 0;
                }
            }
        }
    }

    private void CalculateEmpty(int row, int col)
    {
        Transform tm_to_tag = Tiles[row,col].transform;
        tm_to_tag.tag = "empty";

        for (int i=-1; i<=1; i++)
        {
            for (int j=-1; j<=1; j++)
            {
                int nRow = row + i;
                int nCol = col + j;

                if (nRow >= 0 && nRow < gridWidth && nCol >= 0 && nCol < gridHeight)
                {
                    Transform currentTransform = Tiles[nRow, nCol].transform;
                    TMP_Text text_obj = currentTransform.Find("fieldNumber").GetComponent<TMP_Text>();

                    temp_rb = currentTransform.GetComponentInChildren<Rigidbody>();


                    if (currentTransform.tag != "empty" && text_obj.text == "") 
                    {
                        StartCoroutine(PushDelay());

                        if (currentTransform.Find("button").Find("flag(Clone)") != null)
                        {
                            DestroyFlag(currentTransform.Find("button").Find("flag(Clone)").gameObject);
                        }
                        CalculateEmpty(nRow, nCol);
                    }


                    if (currentTransform.tag != "empty" && (text_obj.text == "1" || text_obj.text == "2" || text_obj.text == "3" || text_obj.text == "4" || text_obj.text == "5"))
                    {
                        StartCoroutine(PushDelay());
                        if (currentTransform.Find("button").Find("flag(Clone)") != null)
                        {
                            DestroyFlag(currentTransform.Find("button").Find("flag(Clone)").gameObject);
                        }
                    }

                }

            }
        }

    }

    private void GenerateBombs(int clicked_x, int clicked_y)
    {
        System.Random rnd = new System.Random();
        for (int i = 0; i < mines; i++)
        {
            int row, col;
            do
            {
                row = rnd.Next(gridWidth);
                col = rnd.Next(gridHeight);

            } while (CheckIfBombExists(row, col) || (row == clicked_x && col == clicked_y));

            Bombs.Add(i, GameObject.Instantiate(bomb) as GameObject);
            Vector3 pos = new Vector3(0, 1f, 1f);
            Bombs[i].transform.SetParent(Tiles[row, col].transform);
            Bombs[i].transform.position = Tiles[row, col].transform.position + pos;
            mineCoords.Add(row);
            mineCoords.Add(col);

            Tiles[row, col].tag = "empty";
        }
    }

    private void ShowAllBombs()
    {
        for (int x = 0; x < gridWidth; x++) 
        {
            for (int y = 0; y < gridHeight; y++) 
            {

                if (Tiles[x,y].transform.Find("bomba(Clone)")!=null)
                {
                    Rigidbody rb = Tiles[x, y].GetComponentInChildren<Rigidbody>();
                    PushObject(rb);
                }
            }
        }
    }

    private int[] GetIndexOf(GameObject gm)
    {
        int[] arr = new int[2];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (Tiles[x,y]==gm)
                {
                    arr[0] = x;
                    arr[1] = y;
                    return arr;
                }
            }
        }
        Debug.Log("no index found?");
        return null;
    }

    private bool CheckIfBombExists(int row, int col)
    {
        GameObject _tile = Tiles[row, col];
        if (_tile.transform.Find("bomba(Clone)") == null)
            return false;
        else
            return true;
    }

    private void DestroyFlag(GameObject gm)
    {
        if (flagCount>=0)
        {
            flagCount--;
            Transform transform = gm.transform.Find("flag(Clone)");
            if (transform != null)
                Destroy(transform.gameObject);
            else
                Debug.Log("missing flag !");
        }
    }
}
