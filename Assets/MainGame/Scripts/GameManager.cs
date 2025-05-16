using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        if(!SaveModel.saveFileLoaded)
            SaveModel.LoadCurrentSave();
    }
}
