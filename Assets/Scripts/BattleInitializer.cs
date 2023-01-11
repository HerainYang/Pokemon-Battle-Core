using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables;
using UnityEngine;
using UnityEngine.Serialization;

public class BattleInitializer : MonoBehaviour
{
    public BasicPlayerInfo[] playerInfos;
    private void Start()
    {
        _ = UIWindowsManager.Instance.ShowUIWindowAsync("BattleStartPanel");
        BattleMgr.Instance.InitData(playerInfos, 1);
    }
}
