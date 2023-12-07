using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;

public class LineChart : MonoBehaviour {
    
    [System.Serializable]
    public class Line {
        public LineChart lineChart;
        public static Dictionary<Line,LineRenderer> lineRendererObjectMap = new Dictionary<Line,LineRenderer>();
        public List<Point> points=new List<Point>();
        public Color color=Color.white;

        [HideInInspector]
        public LineRenderer renderer;

        public Line(Transform parent, Material mat, LineChart _lineChart){
            lineChart = _lineChart;
            GameObject newLineRenderer = new GameObject();
            newLineRenderer.transform.parent=parent;
            newLineRenderer.transform.localPosition=Vector3.zero;
            newLineRenderer.transform.localRotation=Quaternion.Euler(0,0,0);
            newLineRenderer.transform.localScale = Vector3.one;
            renderer = newLineRenderer.AddComponent<LineRenderer>();
            renderer.sortingOrder=2;
            renderer.useWorldSpace=false;
            renderer.material = mat;
            lineRendererObjectMap.Add(this,renderer);
            renderer.startWidth=.0125f;
            renderer.endWidth=.0125f;
            renderer.alignment = LineAlignment.TransformZ;
        }

        ~Line(){
            if(renderer.gameObject!=null)
                Destroy(renderer.gameObject);

            if(lineRendererObjectMap.ContainsKey(this)) {
                lineRendererObjectMap.Remove(this);
            }
        }
    }

    [System.Serializable]
    public class Point {
        public TextMeshProUGUI labelTMP;
        public string label="";
        public Vector2 coordinate;
        Line line;
        LineChart lineChart;
        public Vector3 localPosition;

        public Point(Line _line, float x, float y,string _label="") {
            coordinate.x = x;
            coordinate.y = y;
            label= _label;
            line = _line;
            lineChart = line.lineChart;
        }

        public void SetLabel(string labelText) {
            if(labelText == "" ) {
                labelTMP?.gameObject.SetActive(false);
                labelTMP=null;
            }
            else {
                if(labelTMP==null)
                    labelTMP = lineChart.PoolLabel();
                labelTMP.text = labelText;
                labelTMP.transform.localPosition = localPosition;
            }
        }

        public void UpdateLocalPosition(Vector3 _pos) {
            localPosition = _pos;
            if(labelTMP!=null) {
                labelTMP.transform.localPosition = localPosition;
            }
        }
    }

    
    [SerializeField]
    GameObject labelParent;
    List<TextMeshProUGUI> labels = new List<TextMeshProUGUI>();
    public int maxLabels = 12;

    private void Awake() {
        GameObject labelTemplate = labelParent.transform.GetChild(0).gameObject;
        for(int i = 0; i < maxLabels - 1; i++) {
            GameObject obj = Instantiate(labelTemplate, labelParent.transform);
            labels.Add(obj.GetComponent<TextMeshProUGUI>());
            obj.SetActive(false);
        }
    }

    public TextMeshProUGUI PoolLabel() {
        foreach(TextMeshProUGUI tmp in labels) {
            if(!tmp.gameObject.activeSelf) {
                tmp.gameObject.SetActive(true);
                return tmp;
            } 
        }
        return null;
    }

    public Line CreateNewLine() {
        Line line = new Line(transform,defaultLineMat,this);
        lines.Add(line);
        return line;
    }

    public List<Line> lines = new List<Line>();
    public bool keepRangeX;
    public bool keepRangeY;
    [ShowIf("keepRangeX")]
    public int xRange;
    [ShowIf("keepRangeY")]
    public int yRange;
    public string xAxisLabel, yAxisLabel;
    public string xUnits="",yUnits="";
    
    public Material defaultLineMat;
    public Vector2 graphSize = new Vector2(1f,1f);
    public float padding=.01f;

    public int maxPoints=300;
    public enum MaxPointsBehavior {
        DoNothing,
        MergePoints,
        PruneOldest,
        DontAllowNewPoints,
    }

    public enum LowestValueBehavior {
        IsZero,
        IsLowest,
        Custom
    }

    public MaxPointsBehavior maxPointsBehavior;

    public bool useCustomHighestX;
    [ShowIf("useCustomHighestX")]
    public float customHighestXValue;

    public bool useCustomHighestY;
    [ShowIf("useCustomHighestY")]
    public float customHighestYValue;

    public float highestXValue;

    public float highestYValue;
    public LowestValueBehavior lowestXBehavior,lowestYBehavior;

    public bool bottomLeftValueIsZero=true;
    public int customBottomLeftValue;
    float lowestXValue,lowestYValue;

    public TextMeshProUGUI xAxisLabelTmp, yAxisLabelTmp,xAxisLeftValue, xAxisRightValue, yAxisTopValue, yAxisBottomValue;

    [Button]
    public void UpdateChart() {
        float maxX=0, maxY=0, minX=Mathf.Infinity,minY=Mathf.Infinity;

        foreach(Line line in lines) {
            if(line.points.Count==0) continue;
            
            if(line.points.Count>maxPoints) {
                MaxPointsReached();
            }

            if(line.renderer==null) continue;

            line.renderer.startColor = line.color;
            line.renderer.endColor = line.color;


            //if(dynamicXAxis)
            if(!useCustomHighestX)
                maxX=Mathf.Max(maxX,line.points.Select(el => el.coordinate.x).Max());
            //if(dynamicYAxis)
            if(!useCustomHighestY)
                maxY=Mathf.Max(maxY,line.points.Select(el => el.coordinate.y).Max());

            if(lowestXBehavior == LowestValueBehavior.IsLowest) {
                minX = Mathf.Min(minX,line.points.Select(el => el.coordinate.x).Min());
            }
            if(lowestYBehavior == LowestValueBehavior.IsLowest) {
                minY = Mathf.Min(minX,line.points.Select(el => el.coordinate.y).Min());
            }
        }

        switch(lowestXBehavior) {
            case LowestValueBehavior.IsZero: lowestXValue = 0;break;
            case LowestValueBehavior.IsLowest: lowestXValue = minX;break;
        }
        switch(lowestYBehavior) {
            case LowestValueBehavior.IsZero: lowestYValue = 0;break;
            case LowestValueBehavior.IsLowest: lowestYValue = minY;break;
        }

        if(useCustomHighestX) {
            highestXValue = keepRangeX ? Mathf.Max(customHighestXValue, xRange) : customHighestXValue;
        }
        else {
            highestXValue = maxX;
        }

        if(useCustomHighestY) {
            highestXValue = keepRangeY ? Mathf.Max(customHighestYValue, yRange) : customHighestYValue;
        }
        else {
            highestYValue = maxY;
        }

        foreach(Line line in lines) {
            if(line.renderer == null) continue;
            line.renderer.positionCount=line.points.Count;
            line.renderer.SetPositions(line.points.Select(el => new Vector3((el.coordinate.x-lowestXValue) * (graphSize.x/(highestXValue-lowestXValue)),(el.coordinate.y-lowestYValue)*(graphSize.y/(highestYValue-lowestYValue)),0)).ToArray());
        }

        xAxisLabelTmp.text = xAxisLabel + ((xUnits.Length>0) ? $"({xUnits})" : "");
        yAxisLabelTmp.text = yAxisLabel + ((yUnits.Length>0) ? $"({yUnits})" : "");

        xAxisLeftValue.text = lowestXValue.ToString();

        yAxisBottomValue.text = lowestYValue.ToString();
        xAxisRightValue.text = highestXValue.ToString();
        yAxisTopValue.text = highestYValue.ToString();

        xAxisLeftValue.transform.localPosition = new Vector3(0,-padding,0);
        yAxisBottomValue.transform.localPosition = new Vector3(-padding,0,0);
        xAxisRightValue.transform.localPosition = new Vector3(graphSize.x,-padding,0);
        yAxisTopValue.transform.localPosition = new Vector3(-padding,graphSize.y,0);
        xAxisLabelTmp.transform.localPosition = new Vector3(graphSize.x*.5f,-padding*2f,0);
        yAxisLabelTmp.transform.localPosition = new Vector3(-padding*2f,graphSize.y*.5f,0);
    }

    void MaxPointsReached() {
        switch(maxPointsBehavior) {
            case MaxPointsBehavior.DoNothing:
                break;
            case MaxPointsBehavior.MergePoints:
                MergePoints();
                break;
            case MaxPointsBehavior.PruneOldest:
                PruneOldest();
                break;
            case MaxPointsBehavior.DontAllowNewPoints:
                DontAllowNewPoints();
                break;
        }
    }

    void MergePoints() {
        foreach(Line line in lines) {
            List<Point> newPoints = new List<Point>();
            for(int i = 1; i < line.points.Count-1; i+=3) {
                Point point = new Point(
                    line,
                    line.points[i-1].coordinate.x,
                    (new List<float>{line.points[i].coordinate.y,line.points[i-1].coordinate.y, line.points[i+1].coordinate.y}).Average()
                );
                newPoints.Add(point);
            }
            line.points=newPoints;
            //xRange=newPoints.Count;
        }
    }

    void DontAllowNewPoints(){
        foreach(Line line in lines) {
            line.points = line.points.Take(maxPoints).ToList();
        }
    }

    void PruneOldest(){
        foreach(Line line in lines) {
            line.points.RemoveAt(0);
        }
    }
}