using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlaceObjectsInGrid : MonoBehaviour
{
    public enum Axis {
        XY,
        XZ,
        ZY
    }
    [SerializeField]
    Vector2Int gridSize = new Vector2Int(0,0);
    [SerializeField]
    float distanceBetweenObjects = .2f;
    [SerializeField]
    Axis axis;
    [SerializeField]
    GameObject holder;

    void Awake()
    {
        holder = gameObject;
        PlaceInGrid();
    }

    public void SetHolder(GameObject obj) {
        holder = obj;
    }

    [Button]
    public void PlaceInGrid()
    {

        Vector3 axisVector = new Vector3();


        Vector3 initPos = holder.transform.GetChild(0).position;
        int x = 0, y = 0;
        for (int i = 0; i < holder.transform.childCount; i++)
        {
            switch(axis) {
                case Axis.XY:
                    axisVector=new Vector3(x, y , 0);
                    break;
                case Axis.XZ:
                    axisVector=new Vector3(x, 0 , y);
                    break;
                case Axis.ZY:
                    axisVector=new Vector3(0, x, y);
                    break;

            }
            holder.transform.GetChild(i).transform.position = initPos + (axisVector * distanceBetweenObjects);
                        
            if ((x +1) % gridSize.x == 0)
            {
                x = 0;
                y++;
            } else {
                x++;
            }
        }
    }
}
