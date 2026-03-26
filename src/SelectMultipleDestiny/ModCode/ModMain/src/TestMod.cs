using System;
using UnityEngine;

namespace MOD_YzmLuck;

internal class TestMod
{
    private TimerCoroutine corUpdate;

    internal static bool IsTest => false;

    internal void OnModInit()
    {
        corUpdate = g.timer.Frame((Action)OnUpdate, 1, loop: true);
    }

    internal void OnModDestroy()
    {
        g.timer.Stop(corUpdate);
    }

    private void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            Test();
        }
        if (Input.GetKeyDown(KeyCode.End))
        {
            Test2();
        }
    }

    private void Test()
    {
    }

    private void Test2()
    {
    }

    private WorldUnitBase Player()
    {
        return g.world.playerUnit;
    }

    private WorldUnitData PlayerData()
    {
        return g.world.playerUnit?.data;
    }

    private DataUnit.UnitInfoData PlayerInfoData()
    {
        return g.world.playerUnit?.data?.unitData;
    }

    private DataUnit.PropertyData PlayerProperty()
    {
        return g.world.playerUnit?.data?.unitData.propertyData;
    }
}
