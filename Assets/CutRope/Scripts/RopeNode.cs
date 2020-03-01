using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpringJoint2D))]
[RequireComponent(typeof(HingeJoint2D))]
[RequireComponent(typeof(LineRenderer))]
public class RopeNode : MonoBehaviour
{
    public Transform target;
    public bool isConnectRoot;
    public bool isRope = true;
    //public Rope rope;
    public Transform RopeRoot { get; set; }
    private Collider2D m_Collider;
    private SpringJoint2D m_SJ;
    private HingeJoint2D m_HJ;
    private LineRenderer m_Line;
    private Rigidbody2D m_JointConBody;
    private float m_RopeNodeDis;
    private float m_RopeWidth;
    public int Index { get; set; }

    private bool isCut;
    private Rope m_Rope;

    public Rope rope
    {
        get
        {
            return m_Rope;
        }
        set
        {
            m_Rope = value;
            if (gameObject.tag == "RopeObj")
                Debug.Log("!!!!" + value);
        }
    }

    private void Start()
    {
        GetRopeComponent();
        RopeRoot = transform.parent;
    }

    public void InitRope(Transform _target, int _index, float _dis, Material _lineMat, float _ropeWidth)
    {
        target = _target;
        m_JointConBody = target.GetComponent<Rigidbody2D>();
        Index = _index;
        m_RopeNodeDis = _dis;
        m_RopeWidth = _ropeWidth;

        if (isRope)
            InitCollider();
        GetRopeComponent();

        InitJoint();
        InitLineRenderer(_lineMat);
        RopeRoot = rope.ropeRoot;
    }

    private void GetRopeComponent()
    {
        m_Collider = GetComponent<Collider2D>();
        m_SJ = GetComponent<SpringJoint2D>();
        m_HJ = GetComponent<HingeJoint2D>();
        m_Line = GetComponent<LineRenderer>();
    }

    public void InitJoint()
    {
        m_SJ.connectedBody = m_JointConBody;
        m_SJ.distance = m_RopeNodeDis;
        m_SJ.dampingRatio = 0;
        m_SJ.frequency = 2;
        m_SJ.autoConfigureDistance = false;
        m_SJ.autoConfigureConnectedAnchor = false;

        m_HJ.connectedBody = m_JointConBody;
        m_HJ.autoConfigureConnectedAnchor = false;
        if (Index == 0)
            m_HJ.connectedAnchor = Vector2.zero;
        else
            m_HJ.connectedAnchor = new Vector2(0, -m_RopeNodeDis);
        if (!isRope)
        {
            m_HJ.anchor = new Vector2(0, 1f);
        }
    }

    public void InitLineRenderer(Material _lineMat)
    {
        m_Line.materials = new Material[] { _lineMat };
        m_Line.startWidth = m_Line.endWidth = m_RopeWidth;
        m_Line.numCapVertices = 3;
        m_Line.sortingOrder = 1;
    }

    public void InitCollider()
    {
        BoxCollider2D _Bc = gameObject.AddComponent<BoxCollider2D>();
        //_Bc.offset = new Vector2(0, -(m_RopeNodeDis / 2));
        //_Bc.size = new Vector2(m_RopeWidth, m_RopeNodeDis - 0.1f);
        _Bc.offset = Vector2.zero;
        _Bc.size = new Vector2(m_RopeWidth, m_RopeNodeDis / 10 * 9);
        _Bc.isTrigger = true;
    }

    void Update()
    {
        if (!isCut && target != null && m_Line != null)
        {
            m_Line.SetPosition(0, transform.position);
            m_Line.SetPosition(1, target.position);
        }
    }

    private List<RopeNode> curCutNode = new List<RopeNode>();
    public void CutConnect()
    {
        curCutNode.Clear();

        m_SJ.enabled = false;
        m_HJ.enabled = false;
        m_Line.enabled = false;
        isCut = true;

        if (rope == null)
            return;

        rope.RopeObj = null;
        int allNodeCount = rope.ConRootRopeNodes.Count;
        for (int i = 0; i < allNodeCount; i++)
        {
            if(rope.ConRootRopeNodes[i].Index > Index)
            {
                curCutNode.Add(rope.ConRootRopeNodes[i]);
            }
        }

        isConnectRoot = false;
        m_Collider.isTrigger = false;
        rope.NotRootRopeNodes.Add(this);
        rope.ConRootRopeNodes.Remove(this);
        int curCutCount = curCutNode.Count;
        for (int i = 0; i < curCutCount; i++)
        {
            curCutNode[i].isConnectRoot = false;
            curCutNode[i].mCollider.isTrigger = false;
            if (!rope.NotRootRopeNodes.Contains(curCutNode[i]))
                rope.NotRootRopeNodes.Add(curCutNode[i]);
            if (rope.ConRootRopeNodes.Contains(curCutNode[i]))
                rope.ConRootRopeNodes.Remove(curCutNode[i]);
        }
    }

    public Collider2D mCollider
    {
        get
        {
            return m_Collider;
        }
    }

    public SpringJoint2D SJ
    {
        get
        {
            return m_SJ;
        }
    }

    public HingeJoint2D HJ
    {
        get
        {
            return m_HJ;
        }
    }

    public LineRenderer Line
    {
        get 
        {
            return m_Line;
        }
        set
        {
            m_Line = value;
        }
    }

    public Rigidbody2D JointConBody
    {
        get
        {
            return m_JointConBody;
        }
        set
        {
            m_JointConBody = value;
        }
    }
}