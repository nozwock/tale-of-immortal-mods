Nested functions not preceded by a period `.` are static.

# World Map
- Hooking to a "month change" event.
Unfortunately there isn't really any such event exposed by the game, there's
`EMapType.ClickSkipMonth` but that by itself isn't really useful as it's the
CheckPopup's `onYesCall` (which spawns after `ClickSkipMonth`) that actually skips
the month.

Now, we could get out target `CheckPopup` using `EGameType.OnOpenUIEnd` and a
bool flag on `ClickSkipMonth` but even that isn't enough as there's another
popup that spawns the first time player runs out of days in a month, that isn't
covered by `ClickSkipMonth` which makes the whole thing useless.

What can be done however is hooking into `UILoadingBar`.
```csharp
private int newMonth;
private int CurrentMonth
{
    get
    {
        var date = GameTool.DayToDate(g.data.world.worldDay);
        return (date[0] - 1) * 12 + date[1];
    }
}

public void Init()
{
    g.events.On(EGameType.OpenUIEnd, (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd);
}

private void OnOpenUIEnd(ETypeData e)
{
    var edata = e.Cast<OpenUIEnd>();
    if (edata.uiType.uiName == UIType.LoadingBar.uiName) // The circular loading UI on month skip
    {
        if (newMonth != CurrentMonth)
        {
            OnSkipMonth();
            newMonth = CurrentMonth;
        }
    }
}

private void OnSkipMonth() { }
```

There were some methods I had tried hooking into with Harmony as well but no
luck, they didn't seem to get called as you'd expect:
`UIMapMainPlayerInfo.UpdateSkipMonth` and `UIMapMainPlayerInfo.OnSkipMonth`.

# World Unit/NPC/Player
```
DataMgr g.data
    DataWorld dataWorld
    DataWorld.World world
       int worldDay
    DataUnit dataUnit
        LuckData
        ...

GameTool
    int[3] DayToDate(int day)

WorldUnitBase g.world.playerUnit
    WorldUnitData data
        DataUnit.UnitInfoData unitData
            string unitID
            PropertyData propertyData
                gradeID // Not sure how this maps to cultivation realm. Have to rely on Condition() I suppose.
                age
                life
                inTrait
                outTrait1
                outTrait2
                ...
        WorldUnitDynData dynUnitData // Would recommend reading/writing from here instead of PropertyData
            DynInt age
                int baseValue
            life
            inTrait
            outTrait1
            ...

```
```csharp
static int? GetUnitGrade(WorldUnitBase unit)
{
    for (int i = 0; i < 99; i++)
    {
        if (UnitConditionTool.Condition("grade_0_{i}_{i}", new UnitConditionData(unit, null)))
        {
            return i;
        }
    }
    return null;
}
```

# Battle
```
EBattleType
    BattleStart
    BattleEnd
    BattleExit
    UnitDie
        ETypeData.UnitDie
            UnitCtrlBase unit
                UnitDataBase data
                    UnitType unitType
            MartialTool.HitData hitData

UnitType
    Monst
    Player
    ...

MonstType
    Common
    Elite
    BOSS
    NPC

EffectTool
    bool IsUnitMonstType(UnitCtrlBase unit, int type) // (int)MonstType.*

WorldBattleMgr g.world.battle
    isBattle
    WorldBattleData data
        isRealBattle
        int dungeonLevel
        ConfDungeonBaseItem dungeonBaseItem

UIBattleDamgeInfo
    int monstGrade

UIBattleInfo
    List<UnitCtrlBase> allMonst

UIBattleInfoInfo
    monstCount
    List<UnitCtrlMonst> _hideBossHpList

SceneType
    static SceneBattle battle
        BattleDataMgr battleData
            monstDieCount
        BattleMapMgr battleMap
            List<UnitCtrlMonst> bossUnitCtrl
            UnitCtrlPlayer playerUnitCtrl
            isStartBattle
            isActiveBattle
            monstCount
```

# Soul Reaver
```
SoulDevourSwordMgr g.world.soulDevourSword
    .GetLevelExp()
    .AddExp(int exp)
    .GetSoulSwordEffectProcess()
    .EffectProcess(int type, int addNum = 1)
    SoulDevourSwordMgr.Data data
        unitId  // The unit that currently has the soul sword, you'll probably
                // want to check if player has it before doing anything
                // g.world.playerUnit.data.unitData.unitID
        exp
        level
        atk
        lastDevourMonth // Used for determining remaining time till swordDevourInterval
        nextDevourMonth // State for devour cooldown
        ...

ConfMgr g.conf
    ConfSoulDevourSwordDevour soulDevourSwordDevour
    ConfSoulDevourSwordParamer : ConfSoulDevourSwordParamerBase soulDevourSwordParamer
        soulDevourSwordAtkLimit
        swordDevourInterval // Devour deadline

ConfSoulDevourSwordDevourItem // How much stats to give based on the realm (grade) of the devoured NPC
	npcGrade
	increaseExp
	atkUp
	devourCD
```

# Shrine
```
UIImmortalAncestralHall : UIImmortalAncestralHallBase
    Button btnChallenge // "Worhsip" button
    goBlur // ⌄
    goFill // Both these together form the worship progress bar
    MapBuild10008Data build10008Data
        ConfWorldBuilding10008Item building10008Item
            consecrateAmount // Worship amount needed this month
        MapBuild10008 build10008
            MapBuild10008.Data data
                giveValue   // Worship amount this month, gets reset to 0 on
                            // receiving blessing (filling up the bar once)
                giveTotalValue // Total worship overall
                lastGiveMonth // Can't worship if != 0
            MapBuild10008Data build10008Data
```
