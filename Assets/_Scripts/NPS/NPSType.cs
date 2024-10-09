using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WitchScripts/NPS/Type")]
public class NPSType : ScriptableObject
{
    /*
     ��� ? ��� ��� ? ��� �� ��� �������� ����� ����
    ���� - ����
    ����� ������� �� ����� ��������
    �������� ������ ������� �� ������� �� ����� ������� ���������
    ��������� ������, ����� �� ������ ������ ������ �� �������.
    ����� ��������� �� ��, ����� ������ � ����� �� ����� �������.

     */
    [SerializeField] private string _name;
    [SerializeField] private GameObject _skin;
    [SerializeField] private GameObject _product;
    [SerializeField] private List<GameObject> _products;
    [SerializeField] private int _difficulty;
    
    private void CheckPlayerProgress()
    {

    }

}
