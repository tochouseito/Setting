using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject goalsPrefab;
    public GameObject clearText;
    public GameObject UIText;
    public GameObject StartText;
    //public GameObject TitleText;
    public GameObject Title;
    public GameObject particlePrefab;
    public GameObject wallPrefab;
    public GameObject player = null;
    public Quaternion targetRotation = Quaternion.identity;
    public Stack<GameAction> actionStack = new Stack<GameAction>();
    
    int stock = 0;
    //public Stack<GameAction> boxStack = new Stack<GameAction>();
    public enum ActionType
    {
        MovePlayer,
        MoveBox
    }
    public class GameAction
    {
        public ActionType actionType;
        public Vector2Int oldPosition;
        public Vector2Int newPosition;
        public string tags;
        public GameObject target;  // プレイヤーまたは箱
    }
    int[,] map;
    GameObject[,] field;
    GameObject obj;
    public float rotationSpeed = 180.0f;
    public Vector3 before ;
    string debugText = "";
    enum Scene
    {
        Title,
        Game,
        Clear
    }
    int scene = 0;
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        if (scene == 0)
        {
            Title.SetActive(true);
            StartText.SetActive(true);
        }
            
            map = new int[,] {
        {4,4,4,4,4,4,4,4,4,4},
        {4,3,0,0,0,0,0,0,0,4},
        {4,4,4,4,0,0,3,2,0,4},
        {4,0,0,0,0,0,4,4,4,4},
        {4,0,2,3,2,0,0,0,0,4},
        {4,0,2,0,1,0,0,0,0,4},
        {4,0,0,0,0,0,0,0,0,4},
        {4,0,0,4,0,0,0,0,0,4},
        {4,3,0,4,0,0,0,0,0,4},
        {4,4,4,4,4,4,4,4,4,4}
        };
            field = new GameObject[
                map.GetLength(0),
                map.GetLength(1)
                ];

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == 1)
                    {
                        player = Instantiate(playerPrefab,
                             new Vector3(x - (map.GetLength(1) / 2), (map.GetLength(0) / 2) - y, 0),
                             Quaternion.identity);
                        field[y, x] = player;

                    }
                    if (map[y, x] == 2)
                    {
                        GameObject Box = Instantiate(boxPrefab,
                           new Vector3(x - (map.GetLength(1) / 2), map.GetLength(0) / 2 - y, 0),
                           Quaternion.identity);
                        field[y, x] = Box;
                    }
                    if (map[y, x] == 3)
                    {
                        GameObject Goals = Instantiate(goalsPrefab,
                           new Vector3(x - (map.GetLength(1) / 2), map.GetLength(0) / 2 - y, -0.4f),
                           Quaternion.identity);
                        //field[y, x] = Goals;
                    }
                    if (map[y, x] == 4)
                    {
                        GameObject Walls = Instantiate(wallPrefab,
                            new Vector3(x - (map.GetLength(1) / 2), map.GetLength(0) / 2 - y, 0),
                            Quaternion.identity);
                        field[y, x] = Walls;
                    }
                    //GameObject ClearText = Instantiate(clearText,
                    //    new Vector3(x, map.GetLength(0) - y, 0),
                    //   Quaternion.identity);
                    // debugText += map[y, x].ToString() + ",";
                }
                //debugText += "\n";
            }
            //Debug.Log(debugText);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (scene == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space)){
                scene = 1;
                Reset();
            }
        }
        else
        {
            UIText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                stock = actionStack.Count;
                targetRotation = Quaternion.Euler(0, 0, 0);

                Vector2Int playerIndex = GetPlayerIndex();
                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(1, 0));
                for (int i = 0; i < 8; ++i)
                {
                    GameObject particle = Instantiate(particlePrefab,
                           new Vector3(playerIndex.x - (map.GetLength(1) / 2), (map.GetLength(0) / 2) - playerIndex.y, 0),
                           Quaternion.identity);
                }

            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                stock = actionStack.Count;
                targetRotation = Quaternion.Euler(0, 0, 180);
                Vector2Int playerIndex = GetPlayerIndex();
                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));
                for (int i = 0; i < 8; ++i)
                {
                    GameObject particle = Instantiate(particlePrefab,
                           new Vector3(playerIndex.x - (map.GetLength(1) / 2), (map.GetLength(0) / 2) - playerIndex.y, 0),
                           Quaternion.identity);
                }
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                stock = actionStack.Count;
                targetRotation = Quaternion.Euler(0, 0, 90);
                Vector2Int playerIndex = GetPlayerIndex();
                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, -1));
                for (int i = 0; i < 8; ++i)
                {
                    GameObject particle = Instantiate(particlePrefab,
                           new Vector3(playerIndex.x - (map.GetLength(1) / 2), (map.GetLength(0) / 2) - playerIndex.y, 0),
                           Quaternion.identity);
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                stock = actionStack.Count;
                targetRotation = Quaternion.Euler(0, 0, -90);
                Vector2Int playerIndex = GetPlayerIndex();
                MoveNumber("Player", playerIndex, playerIndex + new Vector2Int(0, 1));
                for (int i = 0; i < 8; ++i)
                {
                    GameObject particle = Instantiate(particlePrefab,
                           new Vector3(playerIndex.x - (map.GetLength(1) / 2), (map.GetLength(0) / 2) - playerIndex.y, 0),
                           Quaternion.identity);
                }
            }

            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            if (IsCleard())
            {

                clearText.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                scene = 0;
                Reset();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Undo();
            }
        }
    }
    
    Vector2Int GetPlayerIndex()
    {
       
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] == null)
                {
                    continue;
                }
                if (field[y,x].tag=="Player")
                {
                    return new Vector2Int(x,y);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }
    bool MoveNumber(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {
        
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Player")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber("Player", moveTo, moveTo + velocity);
            if (!success) { return false; }
        }
        if (field[moveTo.y,moveTo.x] != null && field[moveTo.y,moveTo.x].tag=="Box")
        {
            
            Vector2Int velocity=moveTo-moveFrom;
            bool success = MoveNumber("Box", moveTo, moveTo + velocity);
            if (!success) { return false; }
        }
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber("Wall", moveTo, moveTo + velocity);
            if (!success) { return false; }
            if (actionStack.Count == stock)
            {
                 
               
                    actionStack.Push(new GameAction
                    {
                        actionType = ActionType.MovePlayer,
                        oldPosition = moveFrom,
                        newPosition = moveTo,
                        tags = tag
                        //target = player
                    });
                
                
                
            }
            return false;
            //Vector2Int velocity = moveTo - moveFrom;
            //bool success = MoveNumber(tag, moveTo, moveTo + velocity);
            //if (!success) { return false; }
        }
        if(field[moveTo.y, moveTo.x] == null)
        {
            if (actionStack.Count == stock)
            {
                
                    actionStack.Push(new GameAction
                    {
                        actionType = ActionType.MovePlayer,
                        oldPosition = moveFrom,
                        newPosition = moveTo,
                        tags = tag
                        //target = player
                    });
                
                
            }
            //return false;
        }
        // GameObjectの座標（position)を移動させてからインデックスの入れ替え
        //field[moveFrom.y, moveFrom.x].transform.position =
        //new Vector3(moveTo.x, field.GetLength(0) - moveTo.y, 0);
        if (tag != "Wall")
        {
            Vector3 moveToPosition = new Vector3(
                moveTo.x - field.GetLength(1) / 2, field.GetLength(0) / 2 - moveTo.y, 0
                );
            field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);
            field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
            field[moveFrom.y, moveFrom.x] = null;
        }
        return true;
        
    }
    bool MoveUndo(string tag, Vector2Int moveFrom, Vector2Int moveTo)
    {

        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Player")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveUndo("Player", moveTo, moveTo + velocity);
            if (!success) { return false; }
        }
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {

            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveUndo("Box", moveTo, moveTo + velocity);
            if (!success) { return false; }
        }
        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Wall")
        {
            return false;
            //Vector2Int velocity = moveTo - moveFrom;
            //bool success = MoveNumber(tag, moveTo, moveTo + velocity);
            //if (!success) { return false; }
        }
        
        // GameObjectの座標（position)を移動させてからインデックスの入れ替え
        //field[moveFrom.y, moveFrom.x].transform.position =
        //new Vector3(moveTo.x, field.GetLength(0) - moveTo.y, 0);
        Vector3 moveToPosition = new Vector3(
            moveTo.x - field.GetLength(1) / 2, field.GetLength(0) / 2 - moveTo.y, 0
            );
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;
        return true;

    }
    bool IsCleard()
    {
        // Vector2Int型の可変長配列の作成
        List<Vector2Int>goals=new List<Vector2Int>();

        for(int y = 0; y < map.GetLength(0); y++)
        {
            for(int x = 0; x < map.GetLength(1); x++)
            {
                // 格納場所か否かを判断
                if (map[y, x] == 3)
                {
                    // 格納場所のインデックスを控えておく
                    goals.Add(new Vector2Int(x,y));
                }
            }
        }
        for(int i = 0; i < goals.Count; i++)
        {
            GameObject f= field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                // 一つでも箱がなかったら条件未達成
                return false;
            }
        }
        // 条件未達成でなければ条件達成
        
        debugText = "Clear!";
        Debug.Log(debugText);
        return true;
    }
   void Reset()
    {
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                Destroy(field[y, x]);
            }
        }
        actionStack.Clear();
        clearText.SetActive(false);
        StartText.SetActive(false);
        UIText.SetActive(false);
        Title.SetActive(false);
        Start();
    }
    void Undo()
    {
        if (actionStack.Count > 0)
        {
            GameAction lastAction = actionStack.Pop();
            Vector2Int oldPosition = lastAction.oldPosition;
            Vector2Int newPosition = lastAction.newPosition;
            
            // プレイヤーの位置を元に戻す
            MoveUndo(lastAction.tags, newPosition, oldPosition);
            //lastAction.target.transform.position = new Vector3(oldPosition.x, oldPosition.y, 0);

        }
        
        
    }
   
}
