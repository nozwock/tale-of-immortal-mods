- Code blocks containing stuff in a hierarchy are class/field/function that were interesting enough for me to note here.
- Nested functions not preceded by a period `.` are static.

# Modding
## Tooling
- [Unity Explorer](https://github.com/sinai-dev/UnityExplorer)
- [Il2Cpp Dumper](https://github.com/Perfare/Il2CppDumper) \
    Used for building dummy mono dlls from the Il2Cpp one, and symbol metadata for Ghidra and other decompilers.

    The dummy dll can be disassembled using [ILSpy] to get what is essentially game headers (as `.cs` files).

    Use the [remove_delegate_classes.py] script to clean up compiler-generated delegate classes from the `.cs` files.
- [Melonloader Preferences Manager](https://github.com/nozwock/MelonPreferencesManager) \
    Originally made by sinai-dev but the repo seems to be down so the above is a backup.
- Other's of note, haven't explored these yet:
    - [Il2CppInterop](https://github.com/BepInEx/Il2CppInterop)
    - [Cpp2IL](https://github.com/SamboyCoding/Cpp2IL)

## IL2Cpp Caveats
- Game's `private` fields can be accessed normally without needing to reach out for Harmony `AccessTools`, etc. This is
  due to `Unhollower*` I believe, although I've not really looked into it.
- On how to inject your classes into the game (Il2Cpp's domain): [Class Injection]
- You obviously can't make Harmony Transpiler patches since there's no IL to patch, Pre/Postfix patches are fine though.

## General
There's an `Example.cs` in `<Game Folder>/Mod/modFQA/代码编写教程/ModMain/Example/` that goes over some of the game's
API.

Your mod's entry point is the `ModMain.Init()` method under the namespace
specified in `.modNamespace` of `ModData.cache`, and the destructor method is
`ModMain.Destroy()`. Both of them alonside the class must be `public`.

`ModMain.Init()` function gets called in by the game on game start and
load/reload alongside with `ModMain.Destroy()` for the latter.

If you're initializing melon preferences in `Init()`, make sure it's run only once using some checks as `CreateEntry()`
and others are not supposed to be called more than once. MelonLoader recommends doing initializing in
`OnInitializeMelon()` but it's not possible as it's not available due to the game using an older version and also that
the mod is not loaded by the MelonLoader itself but by the game instead, that means those methods like
`MelonMod.OnApplicateLateStart` will not work whatsoever while the mod is being loaded by the game.

```
UIMgr g.ui
    T .GetUI<T>(UIType.UITypeBase uiType) // Reuse existing alive UI object
    .CreateUI(...)
    .CloseUI(...)

ConfMgr g.conf
    ConfLocalText localText
        Dictionary<string, ConfLocalTextItem> allText // Game text
    ConfRoleLogLocal roleLogLocal
        Dictionary<string, List<ConfRoleLogLocalItem>> allItemInKey // Other game text, mostly dialuoges

EGameType
    IntoWorld
    /* On game load/reload but after your ModMain.Init(), when most of the loading is done and the player is about to
    enter the game. */
    OpenUIEnd|CloseUIEnd // On late opening and closing of UIs
    // You'd use the ETypeData e like so:
    //  var edata = e.Cast<EGameTypeData.(Open|Close)UIEnd>();
    //  edata
    //      UIBase ui // Can use GetComponent etc here to get the UI object instead of using GetUI
    //          name // Use this against UIType.*.uiName to check which UI opened/closed
    SaveData
    /* Just before a game save is made, you can set your DataObjectData data here if you'd like. I'm not sure if this
    event is always emitted before the game spins up the thread that starts saving game data, if it does, you can save
    your data reliably in a blocking manner assuming the mod is on the same thread as the subroutine that issued that
    event, otherwise it'd be race condition and you can't realiably use this event to save your state. */

GameTool
    LS(string key) // Localization
    /* This calls to ConfLocalTextEx.text(this ConfLocalText) which further calls GameTool.LSTextEx(), which finally
    seems responsible for giving out the text based on the selected language, which is done based on
    SceneLogin.languageType */
ConfLocalText
    GetText(key) // Similar to GameTool.LS()
SceneLogin
    LanguageType languageType

UIType
    Loading // UILoading - the loading spinner during "Saving..." (game_baocunzhong)
    LoadingBar // The circular pie-chart like progress bar on month skips
    CheckPopup // Yes/No (type=2) & Ok (type=1) prompts

// Popup notifications
UITipItem
    AddTip(string, float duration)

UICostItemTool
    AddTipText() // Bottom left item cost notifications
    ...

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
        .ContainsKey(...)
        .GetString(...)

DataMgr
    .SaveData(Action<bool>) // Save game function
```

This is how you can spawn your own spinner while doing some heavy work in an another thread:
```cs
var ui = g.ui.OpenUI<UILoading>(UIType.Loading);
ui.InitData("Doing some work...");
g.ui.CloseUI(ui);

// If creating a new UI object instead of using an existing one:
var ui = g.ui.CreateUI<UILoading>(UIType.Loading);
ui.InitData("Doing some work...");
g.ui.CloseUI(ui);
// Or
UnityEngine.Object.Destroy(ui.gameObject);
```

## Events
There are three event groups available: `EGameType`, `EMapType`, and `EBattleType`.
```cs
static Il2CppSystem.Action<ETypeData> callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
Il2CppSystem.Action<ETypeData> callCloseUIEnd;
// NOTE: Don't try to use System.Action instead, even though it compiles, in my testing personally, Off() didn't
// unregister the callback

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

## Localizing Mod
This is about localization using game's own mechanism and tooling (`GameTool.LS`).

Create a `LocalText.json` file under `ModExcel/`. There you can specify keyed-texts. One annyoing thing is that not only
the `key` needs to be unique, but the conf item `id` too.
```json
[
  {
    "key": "my_unique_key",
    "ch": "Hello",
    "tc": "Hello",
    "en": "Hello",
    "kr": "Hello",
    "id": 1396374552
  }
]
```
Once that's done however, in your code, you can just call `GameTool.LS("my_unique_key")` to get the text based on the
language set.

There's a script [toi_random_id.py] that can be used to generate random ids for conf items/entries' `id` field if it's
not present or is `0`, it's done using game's own logic in `ModTool.RandomID()`. JSONC style comments won't be preserved
however and will be removed.

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
    newMonth = -1; // Reset state on game load/reload

    callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
    g.events.On(EGameType.OpenUIEnd, callOpenUIEnd);
}

void OnOpenUIEnd(ETypeData e)
{
    var edata = e.Cast<EGameTypeData.OpenUIEnd>();
    if (edata.uiType.uiName == UIType.LoadingBar.uiName) // The circular loading UI on month skip
    {
        if (newMonth != CurrentMonth)
        {
            OnSkipMonth(); // Called once
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

UIType
    DragonDoorUpgrade // Greenscale Spring UI
    /* The delegate UIDragonDoorUpgrade.<>c__DisplayClass5_1$$<UpgradeEffect>b__2 builds up the DragonDoorSelectEffect
    UI on clicking on "Upgrade", using names & desc via ConfSchoolDragonDoorBuff.GetEffect()->ConfBattleEffect.GetItem()
    + BattleSkillValueData.GetDescRichText(desc, valueData, 2)
    Buff ids for a level can be retrieved from ConfSchoolDragonDoor. */
    DragonDoorSelectEffect // UI for selecting Greenscale upgrades
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


[ILSpy]: https://github.com/icsharpcode/ILSpy
[Class Injection]: https://github.com/BepInEx/Il2CppInterop/blob/master/Documentation/Class-Injection.md
[remove_delegate_classes.py]: ../scripts/remove_delegate_classes.py
[toi_random_id.py]: ../scripts/toi_random_id.py
