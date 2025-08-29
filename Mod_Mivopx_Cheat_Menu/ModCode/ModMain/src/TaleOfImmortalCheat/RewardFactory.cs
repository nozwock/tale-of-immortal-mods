using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;

namespace TaleOfImmortalCheat;

public class RewardFactory
{
	private int BiggestRewardId;

	private int BiggestId;

	public RewardFactory()
	{
		(BiggestId, BiggestRewardId) = GetIds();
	}

	private static (int, int) GetIds()
	{
		int num = -1;
		int num2 = -1;
		Il2CppSystem.Collections.Generic.List<ConfGameItemRewardItem>.Enumerator enumerator = Game.ConfMgr.gameItemReward._allConfList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ConfGameItemRewardItem current = enumerator.Current;
			num = ((num > current.rewardID) ? num : current.rewardID);
			num2 = ((num2 > current.id) ? num2 : current.id);
		}
		return (num2, num);
	}

	private void CreateRewardForId(int rewardId, int item, int quantity)
	{
		ConfGameItemRewardItem confGameItemRewardItem = new ConfGameItemRewardItem();
		confGameItemRewardItem.rewardID = rewardId;
		confGameItemRewardItem.condition = "0";
		confGameItemRewardItem.itemType = 1;
		confGameItemRewardItem.item = item;
		confGameItemRewardItem.numMin = quantity;
		confGameItemRewardItem.numMax = quantity;
		confGameItemRewardItem.group = 1;
		confGameItemRewardItem.firstRewardGroup = 0;
		confGameItemRewardItem.layMax = -1;
		confGameItemRewardItem.noLuckAffect = 1;
		confGameItemRewardItem.weight = "20000";
		confGameItemRewardItem.id = BiggestId++;
		Game.ConfMgr.gameItemReward.AddItem(confGameItemRewardItem);
		Il2CppSystem.Collections.Generic.List<ConfGameItemRewardItem> list;
		if (!Game.ConfMgr.gameItemReward.rewardItems.ContainsKey(rewardId))
		{
			list = new Il2CppSystem.Collections.Generic.List<ConfGameItemRewardItem>();
			Game.ConfMgr.gameItemReward.rewardItems.Add(rewardId, list);
		}
		else
		{
			list = Game.ConfMgr.gameItemReward.rewardItems[rewardId];
		}
		confGameItemRewardItem.group = list.Count + 1;
		list.Add(confGameItemRewardItem);
	}

	public int CreateReward(Item item)
	{
		int num = ++BiggestRewardId;
		CreateRewardForId(num, item.Id, item.Quantity);
		return num;
	}

	public int CreateRewards(System.Collections.Generic.IEnumerable<Item> items)
	{
		int num = ++BiggestRewardId;
		foreach (Item item in items)
		{
			CreateRewardForId(num, item.Id, item.Quantity);
		}
		return num;
	}
}
