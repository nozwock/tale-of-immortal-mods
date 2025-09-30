Nested functions not preceded by a period `.` are static.

# Modding
Your mod's entry point is the `ModMain.Init()` method under the namespace
specified in `.modNamespace` of `ModData.cache`, and the destructor method is
`ModMain.Destroy()`. Both of them alonside the class must be `public`.

`ModMain.Init()` function gets called in by the game on game start and
load/reload alongside with `ModMain.Destroy()` for the latter.

If you're initializing melon preferences in `Init()`, make sure it's run only
once using some checks as `CreateEntry()` and others are not supposed to be
called more than once. MelonLoader recommends doing initializing in
`OnInitializeMelon()` but it's not possible as it's not available due to the
game using an older version.

```
UIMgr g.ui
    T .GetUI<T>(UIType.UITypeBase uiType)

ConfMgr g.conf
    ConfLocalText localText
        Dictionary<string, ConfLocalTextItem> allText // Game text
    ConfRoleLogLocal roleLogLocal
        Dictionary<string, List<ConfRoleLogLocalItem>> allItemInKey // Other game text, mostly dialuoges

EGameType
    IntoWorld // On game load/reload
    (Open|Close)UIEnd // On late opening and closing of UIs
    // You'd use the ETypeData e like so:
    //  var edata = e.Cast<EGameTypeData.(Open|Close)UIEnd>();
    //  edata
    //      UIBase ui // Can use GetComponent etc here to get the UI object instead of using GetUI
    //          name // Use this against UIType.*.uiName to check which UI opened/closed
    SaveData

GameTool
    LS(string key) // Localization

UIType
    Loading // UILoading - the loading spinner during "Saving..." (game_baocunzhong)

UIArtifact
    UIArtifactSprite uiSprite
        int selSpriteSoleId
        List<DataUnit.ArtifactSpriteData.Sprite> spriteList
        UIArtifactSpriteTalentTree talentTree
            talentPosDic
        ArtifactSpriteModel spriteModel
            int id // Sprite Id - not reliable however as spriteModel can lag behind selected sprite and desync

DataMgr g.data
    DataObjectData obj
        .SetString([group], key, value) // Set persistent data to game save file
```

This is how you can spawn your own spinner while doing some heavy work in an another thread:
```cs
var ui = g.ui.OpenUI<UILoading>(UIType.Loading);
ui.InitData("Doing some work...");
ui.gameObject.SetActive(false);

// If creating a new UI object instead of using an existing one:
var ui = g.ui.CreateUI<UILoading>(UIType.Loading);
ui.InitData("Doing some work...");
UnityEngine.Object.Destroy(ui.gameObject);
```

## Events
There are three event groups available: `EGameType`, `EMapType`, and `EBattleType`.
```cs
static Il2CppSystem.Action<ETypeData> callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
Il2CppSystem.Action<ETypeData> callCloseUIEnd;

public void Init()
{
    callCloseUIEnd = (Il2CppSystem.Action<ETypeData>)OnCloseUIEnd;

    g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
    g.events.On(EGameType.CloseUIEnd, callCloseUIEnd);

    // NOTE: Don't do this to register/unregister, this won't unregister the callback:
    // g.events.On(EGameType.OpenUIEnd, (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd);
    // g.events.Off(EGameType.OpenUIEnd, (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd);
    // You need to instead assign the instance method or static method to a common field first, and only then pass that
    // on to the event callback registerer
}

public void Destroy()
{
    g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);
    g.events.Off(EGameType.CloseUIEnd, callCloseUIEnd);
}

static void OnOpenUIEnd(ETypeData e)
{
    var edata = e.Cast<EGameTypeData.OpenUIEnd>();
    if (edata.ui.name == UIType.Login.uiName) // Main menu
    {
        var ui = edata.ui.GetComponent<UILogin>();
        // ...
    }
}

void OnCloseUIEnd(ETypeData e)
{
    var edata = e.Cast<EGameTypeData.CloseUIEnd>();
    if (edata.uiType.uiName == UIType.Login.uiName) // Main menu
    {
        // ...
    }
}
```

# World Map
- Hooking to a "month change" event.
Unfortunately there isn't really any such event exposed by the game, there's
`EMapType.ClickSkipMonth` but that by itself isn't really useful as it's the
CheckPopup's `onYesCall` (which spawns after `ClickSkipMonth`) that actually skips
the month.

Now, we could get our target `CheckPopup` using `EGameType.OnOpenUIEnd` and a
bool flag on `ClickSkipMonth` but even that isn't enough as there's another
popup that spawns the first time player runs out of days in a month, that isn't
covered by `ClickSkipMonth` which makes the whole thing useless.

What can be done however is hooking into `UILoadingBar`.
```cs
Il2CppSystem.Action<ETypeData> callOpenUIEnd;

int newMonth;
int CurrentMonth
{
    get
    {
        var date = GameTool.DayToDate(g.data.world.worldDay);
        return (date[0] - 1) * 12 + date[1];
    }
}

public void Init()
{
    callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
    g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
}

void OnOpenUIEnd(ETypeData e)
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

void OnSkipMonth() { }
```

There were some methods I had tried hooking into with Harmony as well but no
luck, they didn't seem to get called as you'd expect:
`UIMapMainPlayerInfo.UpdateSkipMonth` and `UIMapMainPlayerInfo.OnSkipMonth`.

```
g.data
    buildSchool
        dragonDoor
            intimacy // Following Bug's affinity
```

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
        .RewardItem(id) // Add items with an optional popup window showing the items added
        .RewardPropItem(propData)
        // For creating copies of existing prop item
        // Don't use the count variant as it doesn't seem to create proper copies
        // Just run a for loop if you don't have showUI turned on
        .CostPropItem(...) // For removing items while showing notifications
        DataUnit.UnitInfoData unitData
            string unitID // Sole ID
            DataUnit.UnitDataProps : DataProps propData
                List<DataProps.PropsData> allProps // Inventory items, can contain null PropsData
            PropertyData propertyData
                gradeID // Not sure how this maps to cultivation realm. Have to rely on Condition() I suppose.
                age
                life
                inTrait
                outTrait1
                outTrait2
                ...
            ArtifactSpriteData artifactSpriteData // Artifact spirits
                List<Sprite> sprites
                    int soleID
                    int spriteID
                    List<Talent> talents // Unlocked talents
                    intimacy
                    stamina
        WorldUnitDynData dynUnitData // Would recommend reading/writing from here instead of PropertyData
            DynInt age
                int baseValue
            life
            inTrait
            outTrait1
            ...

DataProps
    List<PropsData> allProps
DataProps.PropsData
    string soleID
    int[] values // Item's quantity [2] and other attributes, for e.g. for artifacts [4] is durability
    ConfItemPropsItem propsItem
        int type // Compare with PropsType
        sale // Whether it can be sold
        isMultiDrop
        // Whether multiple of this can be dropped as loot? Because it sure it doesn't indicate items that can stack,
        // because I've seen items with it set to 0 that are still stackable

enum PropsType // Only contains 7 types (0-6)
// In ItemProps.json, there's actually 9 (0-8) types
// 7 is mood items, 8 is imp items

ConfMgr g.conf
    ConfArtifactSpriteTalent artifactSpriteTalent // Artifact spirits' talents and their requirements
    // activeCost is an array of [itemId, count], these are the items required to unlock the talent.
    // Unlock2Type 0 means no special unlock requirement but activeCost should be present, while 17 is truly the "No
    // Cost" ones.
    // Item ids of the kind 5311[0-9]201 are spirit dews.
    ConfArtifactShape artifactShape // Artifact items' details
    // You can filter out allProps for artifacts using the id field from here
```
```cs
// Or, ((unit.data.unitData.propertyData.gradeID - 1) / 5) + 1
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
