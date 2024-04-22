using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    int[,] map;
    GameObject[,] field;
    GameObject obj;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        map = new int[,] {
        {0,0,2,0,0 },
        {0,0,1,0,2 },
        {0,2,0,0,0 },
        };
        field = new GameObject[
            map.GetLength(0),
            map.GetLength(1)
            ];
        //string debugText = "";
        for(int y = 0; y < map.GetLength(0); y++)
        {
            for(int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y,x] == 1)
                {
                    GameObject player = Instantiate(playerPrefab,
                        new Vector3(x, map.GetLength(0) - y, 0),
                        Quaternion.identity);
                    field[y, x] = player;

                }
                if (map[y, x] == 2){
                    GameObject Box = Instantiate(boxPrefab,
                       new Vector3(x, map.GetLength(0) - y, 0),
                       Quaternion.identity);
                    field[y, x] = Box;
                }
               // debugText += map[y, x].ToString() + ",";
            }
            //debugText += "\n";
        }
        //Debug.Log(debugText);
    }

    // Update is called once per frame
    void Update()
    {
   
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector2Int playerIndex =GetPlayerIndex();
            MoveNumber(tag, playerIndex, playerIndex+new Vector2Int(1,0));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(tag, playerIndex, playerIndex+new Vector2Int(-1,0));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(tag, playerIndex, playerIndex + new Vector2Int(0, -1));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(tag, playerIndex, playerIndex + new Vector2Int(0, 1));
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
   bool MoveNumber(string tag,Vector2Int moveFrom,Vector2Int moveTo)
    {
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false;}
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }
        if (field[moveTo.y,moveTo.x] != null && field[moveTo.y,moveTo.x].tag=="Box")
        {
            Vector2Int velocity=moveTo-moveFrom;
            bool success = MoveNumber(tag, moveTo, moveTo + velocity);
            if (!success) { return false; }
        }

        // GameObject�̍��W�iposition)���ړ������Ă���C���f�b�N�X�̓���ւ�
        field[moveFrom.y, moveFrom.x].transform.position =
            new Vector3(moveTo.x, field.GetLength(0) - moveTo.y, 0);
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        field[moveFrom.y, moveFrom.x] = null;
        return true;
        
    }
        
}
