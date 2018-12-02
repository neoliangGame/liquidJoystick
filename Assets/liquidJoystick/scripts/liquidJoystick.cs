using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class liquidJoystick : Graphic {

    public Mesh normalMesh = null;
    public Mesh changedMesh = null;

    double changeRate = 0f;

    [Range(0.01f, 1f)]
    public float sizeRate = 0.5f;
    float scaleSize = 40;

    protected override void Start()
    {
        float minSizeReferrence = (Screen.width < Screen.height) ? Screen.width : Screen.height;
        scaleSize = sizeRate * minSizeReferrence;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        List<Vector3> normalVectors = new List<Vector3>();
        normalMesh.GetVertices(normalVectors);
        List<Vector2> normalUVs = new List<Vector2>();
        normalMesh.GetUVs(0,normalUVs);

        List<Vector3> changedVectors = new List<Vector3>();
        changedMesh.GetVertices(changedVectors);

        Vector3 position = Vector3.zero;
        double x, y;
        for(int i = 0;i < normalVectors.Count; i++)
        {
            x = normalVectors[i].x * (1.0 - changeRate) + changedVectors[i].x * changeRate;
            y = normalVectors[i].z * (1.0 - changeRate) + changedVectors[i].z * changeRate;
            position = new Vector3( (float)x, (float)y ,0);
            vh.AddVert(position * scaleSize, color, normalUVs[i]);
        }
        
        for (int i = 2;i < normalMesh.triangles.Length; i += 3)
        {
            vh.AddTriangle(normalMesh.triangles[i-2], normalMesh.triangles[i - 1], normalMesh.triangles[i]);
        }
    }

    /// <summary>
    /// 查阅UGUI Image实现源码，发现添加mainTexture设置才能让自己添加的texture生效
    /// </summary>
    public override Texture mainTexture
    {
        get
        {
            if (material != null && material.mainTexture != null)
            {
                return material.mainTexture;
            }
            return s_WhiteTexture;
        }
    }

    /// <summary>
    /// 外部唯一需要调用的方法
    /// </summary>
    /// <param name="position">摇杆所在中心位置</param>
    /// <param name="angle">摇杆旋转角度</param>
    /// <param name="rate">摇杆变形率</param>
    public void SetLiquidJoystick(Vector2 position, float angle, float rate)
    {
        GetComponent<RectTransform>().position = position;
        transform.eulerAngles = new Vector3(0f,0f,angle);

        rate = (rate > 1f) ? 1f : rate;
        rate = (rate < 0f) ? 0f : rate;
        changeRate = rate;
        SetVerticesDirty();
    }
}
