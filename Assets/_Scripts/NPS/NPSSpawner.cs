using System.Collections.Generic;
using UnityEngine;

public class NPSSpawner : MonoBehaviour
{
    public static NPSSpawner Instans; 

    [SerializeField] private NPSLogic _NPSLogic;
    [SerializeField] private List<Transform> _spawnPointsLeft;
    [SerializeField] private List<Transform> _spawnPointsRight;
    [SerializeField] private List<Transform> _outPointsLeft;
    [SerializeField] private List<Transform> _outPointsRight;
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
        _spawnTimeLast = 0;
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
        bool randomSide = Random.Range(0, 2) == 1;
        List<Transform> temp = randomSide ? _spawnPointsLeft : _spawnPointsRight;
        int random = 0;
        random = Random.Range(0, temp.Count);
        return temp[random];
    }
    private Transform GetRandomOutPoint()
    {
        bool randomSide = Random.Range(0,2) == 1;
        List<Transform> temp = randomSide ? _outPointsLeft : _outPointsRight;
        int random = 0;
        random = Random.Range(0, temp.Count);
        return temp[random];
    }
    private void ChoiceTravel(NPSLogic nPS)
    {
        if (_store.Need—ustomers())
        {
            _store.New—ustomers(nPS);
            return;
        }
        nPS.StartSetting();
        NPSGoHome(nPS);
    }
    public void NPSGoHome(NPSLogic nPS)
    {
        nPS.GoHome(GetRandomOutPoint());
    }
    public void NPSGone (NPSLogic nPS)
    {
        _spawnNPS.Remove(nPS);
        Destroy(nPS.gameObject);
    }
}
