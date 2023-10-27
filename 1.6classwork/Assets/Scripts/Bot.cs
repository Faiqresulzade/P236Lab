using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class Bot : MonoBehaviour
{
    [SerializeField] private LayerMask collectableLayer;
    [SerializeField] private int bagCapacity = 2;
    [SerializeField] private Transform bag;
    [SerializeField] private Vector3 offset;

    private List<GameObject> _woods;
    private List<GameObject> _collectedWoods;
    private NavMeshAgent _agent;
    private bool _isCrafting;
    private int _treeCount;
    private GameObject _woodBase;
    private float _xPos;

    void Start()
    {
        _woodBase = GameObject.FindGameObjectWithTag("WoodBase");
        _agent = GetComponent<NavMeshAgent>();
        _woods = GameObject.FindGameObjectsWithTag("Collectable").ToList();
    }

    private void Update()
    {
        if (!_isCrafting)
        {
            _agent.SetDestination(FindClosestWood().transform.position);
            _isCrafting = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectable"))
        {
            other.tag = "Collected";
            other.transform.parent = bag;

            other.transform.DOLocalJump(Vector3.zero + offset, 2, 1, .5f);
            offset += new Vector3(0, .6f, 0);

            _woods.Remove(other.gameObject);

            _treeCount++;
            _isCrafting = false;

        }

        if (other.CompareTag("WoodBase"))
        {
            _collectedWoods = GameObject.FindGameObjectsWithTag("Collected").ToList();
            foreach (var woods in _collectedWoods)
            {
                _xPos += 1f;
                woods.transform.parent = other.transform;
                woods.transform.DOJump(new Vector3(20f + _xPos, 1f, -21f), 2, 1, .5f);
            }
            _isCrafting = false;
            Debug.Log(_treeCount);
            offset = Vector3.zero;
            _treeCount = 0;

        }
    }

    private GameObject FindClosestWood()
    {
        if (_treeCount == bagCapacity || _woods.Count == 0)
        {
            return _woodBase;
        }
        GameObject closestWood = _woods[0];
        Debug.Log(_treeCount);

        foreach (var wood in _woods)
        {
            float distance = Vector3.Distance(transform.position, wood.transform.position);
            float closestWoodDistance = Vector3.Distance(transform.position, closestWood.transform.position);

            if (distance < closestWoodDistance)
            {
                closestWood = wood;
            }
        }

        return closestWood;
    }

}
