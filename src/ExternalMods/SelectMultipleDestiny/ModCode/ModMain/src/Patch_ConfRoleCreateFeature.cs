using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;

namespace MOD_YzmLuck;

[HarmonyPatch(typeof(ConfRoleCreateFeature))]
internal class Patch_ConfRoleCreateFeature
{
    [HarmonyPrefix]
    [HarmonyPatch("RandomItem", new Type[]
    {
        typeof(int),
        typeof(int),
        typeof(ReturnAction<int, ConfRoleCreateFeatureItem>),
        typeof(Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem>)
    })]
    private static bool Prefix(ref Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem> __result)
    {
        if (UIBornLuckList.Patch_Customize(ref __result))
        {
            return false;
        }
        if (!IsApplyPatch())
        {
            return true;
        }
        System.Collections.Generic.List<ConfRoleCreateFeatureItem> list = new System.Collections.Generic.List<ConfRoleCreateFeatureItem>();
        Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem> _resultLucks = new Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem>();
        Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem> list2 = g.conf.roleCreateFeature.allLuckList[1];
        System.Collections.Generic.Dictionary<int, bool> luckFilter = UILuckFilter.LuckFilter;
        int[] tianming = new int[3] { 2804, 2805, 2806 };
        Il2CppSystem.Collections.Generic.List<ConfRoleCreateFeatureItem>.Enumerator enumerator = list2.GetEnumerator();
        while (enumerator.MoveNext())
        {
            ConfRoleCreateFeatureItem current = enumerator.Current;
            if (current != null && current.weight > 0)
            {
                if (UILuckFilter.AllLevel)
                {
                    list.Add(current);
                }
                else if (luckFilter.ContainsKey(current.level) && luckFilter[current.level])
                {
                    list.Add(current);
                }
            }
        }
        if (list.Count == 0)
        {
            Tool.Warning("随机气运可选的气运数量为0！", "temp");
            return true;
        }
        System.Collections.Generic.Dictionary<int, bool> lockLucks = Patch_UICreatePlayerProperty.lockLucks;
        if (lockLucks != null && lockLucks.Count != 0)
        {
            foreach (int key in lockLucks.Keys)
            {
                if (_resultLucks.Count >= 9)
                {
                    break;
                }
                ConfRoleCreateFeatureItem item = g.conf.roleCreateFeature.GetItem(key);
                if (item != null && !_resultLucks.Contains(item))
                {
                    _resultLucks.Add(item);
                }
            }
            list.RemoveAll((ConfRoleCreateFeatureItem item3) => _resultLucks.Contains(item3));
        }
        if (UILuckFilter.TianMing)
        {
            int[] array = tianming;
            foreach (int id in array)
            {
                if (_resultLucks.Count >= 9)
                {
                    break;
                }
                ConfRoleCreateFeatureItem item2 = g.conf.roleCreateFeature.GetItem(id);
                if (item2 != null && !_resultLucks.Contains(item2))
                {
                    _resultLucks.Add(item2);
                }
            }
            list.RemoveAll((ConfRoleCreateFeatureItem confRoleCreateFeatureItem) => tianming.Contains(confRoleCreateFeatureItem.id));
        }
        if (UILuckFilter.ModLuck && _resultLucks.Count < 9)
        {
            System.Collections.Generic.List<ConfRoleCreateFeatureItem> list3 = list.FindAll((ConfRoleCreateFeatureItem v) => v.weight >= 10000 && !_resultLucks.Contains(v));
            if (list3 != null && list3.Count >= 0)
            {
                int count = ((list3.Count >= 9) ? 9 : list3.Count);
                foreach (int item3 in RandomNotRepeat(0, list3.Count, count))
                {
                    if (_resultLucks.Count >= 9)
                    {
                        break;
                    }
                    _resultLucks.Add(list3[item3]);
                }
                list.RemoveAll((ConfRoleCreateFeatureItem item3) => _resultLucks.Contains(item3));
            }
        }
        if (_resultLucks.Count < 9)
        {
            int count2 = ((list.Count >= 9) ? 9 : list.Count);
            foreach (int item4 in RandomNotRepeat(0, list.Count, count2))
            {
                if (_resultLucks.Count >= 9)
                {
                    break;
                }
                _resultLucks.Add(list[item4]);
            }
        }
        __result = _resultLucks;
        return false;
    }

    private static bool IsApplyPatch()
    {
        bool flag = Patch_UICreatePlayerProperty.lockLucks != null && Patch_UICreatePlayerProperty.lockLucks.Count != 0;
        if (!UILuckFilter.IsActive)
        {
            return false;
        }
        if (UILuckFilter.LuckFilter == null || UILuckFilter.LuckFilter.Count == 0)
        {
            return false;
        }
        if (!UILuckFilter.TianMing && !UILuckFilter.ModLuck && UILuckFilter.AllLevel && !flag)
        {
            return false;
        }
        return true;
    }

    private static System.Collections.Generic.List<int> RandomNotRepeat(int min, int max, int count)
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        if (min >= max || count <= 0)
        {
            return list;
        }
        if (max - min < count)
        {
            return list;
        }
        while (list.Count < count)
        {
            int item = CommonTool.Random(min, max);
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
        return list;
    }
}
