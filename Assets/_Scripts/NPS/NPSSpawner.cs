using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPSSpawner : MonoBehaviour
{
    public static NPSSpawner Instans; 

    [SerializeField] private NPSLogic _NPSLogic;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private Store _store;
    [SerializeField] private NPSAllTypes _NPSTypes;
    [SerializeField] private List<NPSLogic> _spawnNPS;
    [SerializeField] private int _spawnCount = 10;
    [SerializeField] private float _spawnTime = 10;
    [SerializeField] private bool _isSpawning = true;

    private float _spawnTimeLast;
    private int _spawnCountLast;

    private void Awake()
    {
        Instans = this;
    }
    private void Update()
    {
        if (_isSpawning)
        {
            if(_spawnTimeLast > 0)
            {
                _spawnTimeLast -= Time.deltaTime;
            } 
            else
            {
                if (_spawnCountLast < _spawnCount)
                {
                    SpawnNewNPS();
                }
                _spawnTimeLast = _spawnTime;
            }
        }
    }
    private void Start()
    {
        _spawnTimeLast = _spawnTime;
    }
    private void SpawnNewNPS()
    {
        NPSLogic newNPS;
        newNPS = Instantiate(_NPSLogic, GetRandomSpawnpoint());
        _spawnNPS.Add(newNPS);
        ChoiceTravel(newNPS);
    }
    private Transform GetRandomSpawnpoint()
    {
        int random = 0;
        random = Random.Range(0, _spawnPoints.Count);
        return _spawnPoints[random];
    }
    private void ChoiceTravel(NPSLogic nPS)
    {
        if (_store.NeedÑustomers())
        {
            _store.NewÑustomers(nPS);
            return;
        }
        NPSGoHome(nPS);
    }
    public void NPSGoHome(NPSLogic nPS)
    {
        nPS.GoHome(GetRandomSpawnpoint());
    }
    public void NPSGone (NPSLogic nPS)
    {
        _spawnNPS.Remove(nPS);
        Destroy(nPS.gameObject);
    }
}
