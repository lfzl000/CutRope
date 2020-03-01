using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 创建绳子
/// </summary>
public class Rope : MonoBehaviour
{
    [SerializeField]
    private bool isAutoCreateRope;
    
    /// <summary> 绳子的锚点(绳子挂在哪) </summary>
    [SerializeField]
    private GameObject m_RopeAnchor = null;

    /// <summary> 挂在绳子上的物体 </summary>
    [SerializeField]
    private GameObject m_RopeObj = null;

    /// <summary> 绳子节点个数 </summary>
    [SerializeField]
    private int m_RopeNodeCount = 0;

    /// <summary> 绳子宽度 </summary>
    [SerializeField]
    private float m_RopeWidth = 0.15f;

    /// <summary> 绳子单个节点间距 </summary>
    [SerializeField]
    private float m_RopeNodeDis = 1;

    /// <summary> 绳子材质 </summary>
    [SerializeField]
    private Material m_RopeNodeMaterial = null;

    private Rigidbody2D m_ropeAnchorRigid;
    private Rigidbody2D m_ropeObjRigid;

    public Transform ropeRoot { get; set; }

    public bool canCut = true;

    private List<RopeNode> m_ConRootRopeNodes = new List<RopeNode>();
    private List<RopeNode> m_NotRootRopeNodes = new List<RopeNode>();

    public GameObject RopeObj
    {
        get
        {
            return m_RopeObj;
        }
        set
        {
            m_RopeObj = value;
        }
    }

    public List<RopeNode> ConRootRopeNodes
    {
        get
        {
            return m_ConRootRopeNodes;
        }
    }

    public List<RopeNode> NotRootRopeNodes
    {
        get
        {
            return m_NotRootRopeNodes;
        }
    }

    private void Start()
    {
        Renderer ropeAnchorRender = m_RopeAnchor.GetComponent<Renderer>();
        if(ropeAnchorRender!=null)
        {
            ropeAnchorRender.sortingOrder = 2;
        }
        m_RopeAnchor.transform.parent = transform;
        m_ropeAnchorRigid = m_RopeAnchor.GetComponent<Rigidbody2D>();
        if (m_ropeAnchorRigid == null)
        {
            m_ropeAnchorRigid = m_RopeAnchor.AddComponent<Rigidbody2D>();
            m_ropeAnchorRigid.bodyType = RigidbodyType2D.Kinematic;
        }

        Renderer ropeObjRender = m_RopeObj.GetComponent<Renderer>();
        if (ropeObjRender != null)
        {
            ropeObjRender.sortingOrder = 3;
        }
        m_RopeObj.transform.parent = transform;
        m_RopeObj.tag = "RopeObj";
        m_ropeObjRigid = m_RopeObj.GetComponent<Rigidbody2D>();
        if (m_ropeObjRigid == null)
        {
            m_ropeObjRigid = m_RopeObj.AddComponent<Rigidbody2D>();
            m_ropeObjRigid.mass = 10;
        }

        if (isAutoCreateRope)
            CreateRopeNode();
    }

    /// <summary>
    /// 创建绳子
    /// </summary>
    private void CreateRopeNode()
    {
        var ropeRootGo = new GameObject("ropeRoot");
        ropeRoot = ropeRootGo.transform;
        ropeRootGo.transform.parent = transform;

        Transform preNode = m_ropeAnchorRigid.transform;

        for (int i = 0; i < m_RopeNodeCount; i++)
        {
            var ropeNode = new GameObject("ropeNode_" + i);
            ropeNode.layer = 15;
            ropeNode.transform.parent = ropeRootGo.transform;
            ropeNode.transform.position = m_RopeAnchor.transform.position - new Vector3(0, i * 2 * m_RopeNodeDis, 0);

            RopeNode rn = ropeNode.AddComponent<RopeNode>();
            rn.rope = this;
            rn.InitRope(preNode, i, m_RopeNodeDis, m_RopeNodeMaterial, m_RopeWidth);
            m_ConRootRopeNodes.Add(rn);
            preNode = ropeNode.transform;
        }

        RopeNode objNode = m_RopeObj.AddComponent<RopeNode>();
        objNode.rope = this;
        objNode.isRope = false;
        objNode.InitRope(preNode, -1, m_RopeNodeDis, m_RopeNodeMaterial, m_RopeWidth);
    }
}