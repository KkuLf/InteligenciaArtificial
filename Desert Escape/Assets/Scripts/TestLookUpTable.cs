using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLookUpTable : MonoBehaviour
{

    LookUpTable<string, string> _lookUpTable;
    void Start()
    {
        _lookUpTable = new LookUpTable<string, string>(TestOperation);
        print("1: " + _lookUpTable.Run("Hola"));
        print("2: " + _lookUpTable.Run("Hola"));
        print("3: " + _lookUpTable.Run("Hola"));
        print("4: " + _lookUpTable.Run("Chau"));
        print("5: " + _lookUpTable.Run("Hola"));
    }


    string TestOperation(string key)
    {
        key += " Season" + gameObject.name + " " + GetComponent<Collider>().ToString();
        return key;
    }
}
