using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 切割者
/// </summary>
public class Cutter : MonoBehaviour
{
    /// <summary> 允许切割的层 </summary>
    [SerializeField]
    private LayerMask m_collisionMask = -1;
    /// <summary> 绳子和相机距离 </summary>
    [SerializeField]
    private float z = 0;

    private Collider2D m_Collider;
    private Camera m_CurCamera;
    private bool isCut;
    private Vector2 _oldPosition = Vector2.zero;

    [SerializeField]
    private GameObject m_CurCutObj;

    private int cutCount;
    public int maxCutCount;

    public GameObject CurCutObj
    {
        get
        {
            return m_CurCutObj;
        }
    }

    private void Start()
    {
        isCut = false;
        m_CurCamera = Camera.main;
        m_Collider = GetComponent<Collider2D>();
        m_Collider.enabled = false;
    }

    /// <summary>
    /// 切绳子
    /// </summary>
    /// <param name="_rope">Rope.</param>
    public void Cut(GameObject _rope)
    {
        cutCount++;
        if (cutCount > maxCutCount)
            return;
        
        cutNode.Add(_rope);
        RopeNode rn = _rope.GetComponent<RopeNode>();

        if (!rn.rope.canCut)
            return;

        rn.rope.canCut = false;
        m_CurCutObj = rn.rope.RopeObj;

        var rope = Instantiate(rn, _rope.transform.position, _rope.transform.rotation);
        rope.rope = rn.rope;
        rope.RopeRoot = rn.RopeRoot;
        rope.transform.parent = rope.RopeRoot;
        if (rope.rope != null)
            rope.rope.ConRootRopeNodes.Add(rope);
        cutNode.Add(rope.gameObject);
        rope.gameObject.SetActive(true);

        if (rn != null)
            RopeCut(rn);
    }

    public UnityAction<GameObject> cuted;
    private void RopeCut(RopeNode _rope)
    {
        _rope.CutConnect();
        if (cuted != null)
            cuted(_rope.gameObject);
    }

    private void RopeCut(GameObject _rope)
    {
        RopeNode rn = _rope.GetComponent<RopeNode>();
        if (rn != null)
        {
            if (rn.rope.canCut)
            {
                rn.rope.canCut = false;
                RopeCut(rn);
            }
        }
    }

    private void LateUpdate()
    {
        TryRayCastAll();
    }

    private List<GameObject> cutNode = new List<GameObject>();
    private void TryRayCastAll()
    {
        float v_distance = Vector2.Distance(_oldPosition, transform.position);
        if (v_distance >= 0 && isCut)
        {
            int v_rayCastlayerMask = m_collisionMask.value;

            RaycastHit2D[] v_hits = Physics2D.LinecastAll(_oldPosition, transform.position, v_rayCastlayerMask);
            int hitCount = v_hits.Length;

            for (int i = 0; i < hitCount; i++)
            {
                if (v_hits[i].collider != null && !cutNode.Contains(v_hits[i].transform.gameObject))
                {
                    Cut(v_hits[i].collider.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 切割挂在绳子上的物体
    /// </summary>
    private int TriggerCount = 0;
    private void OnTriggerEnter2D(Collider2D _collider)
    {
        if(_collider.gameObject.tag == "RopeObj")
        {
            cutCount++;
            if (cutCount > maxCutCount)
                return;

            TriggerCount++;
            if (TriggerCount > 1)
                return;
            RopeCut(_collider.gameObject);
        }
    }

    private Vector3 mousePos;
    private Vector3 mouseWorldPos;
    private int frameCount;
    private void Update()
    {
        mousePos = Input.mousePosition;
        mouseWorldPos = m_CurCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, z));

        if (Input.GetMouseButtonDown(0))
        {
            m_Collider.enabled = true;
            frameCount = 0;
            cutNode.Clear();
            isCut = true;
            _oldPosition = mouseWorldPos;
        }
        if (Input.GetMouseButton(0))
        {
            frameCount++;
            if (frameCount == 10)
            {
                _oldPosition = mouseWorldPos;
                frameCount = 0;
            }
            transform.position = mouseWorldPos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_Collider.enabled = false;
            isCut = false;
            TriggerCount = 0;
            cutCount = 0;
        }
    }
} 