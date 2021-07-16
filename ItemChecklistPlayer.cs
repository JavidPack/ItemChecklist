using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ItemChecklist
{
	class ItemChecklistPlayer : ModPlayer
	{
		//	internal static ItemChecklistPlayer localInstance;

		// This is a list of items...Holds clean versions of unloaded mystery and loaded real items. 
		internal List<Item> foundItems;
		//
		internal bool[] foundItem;
		internal bool[] findableItems;
		//Skipping:
		// Main.itemName 0 is empty
		// Mysteryitem is to skip --> ItemID.Count
		// Deprecated  if (this.type > 0 && ItemID.Sets.Deprecated[this.type])

		internal int totalItemsToFind;
		internal int totalItemsFound;  // eh, property? dunno.

		// Because of save, these values inherit the last used setting while loading
		//internal SortModes sortModePreference = SortModes.TerrariaSort;
		internal bool announcePreference;
		internal bool findChestItemsPreference = true;
		internal int showCompletedPreference;

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (ItemChecklist.ToggleChecklistHotKey.JustPressed)
			{
				if (!ItemChecklistUI.Visible)
				{
					ItemChecklist.instance.ItemChecklistUI.UpdateNeeded();
				}
				ItemChecklistUI.Visible = !ItemChecklistUI.Visible;
				// Debug assistance, allows for reinitializing RecipeBrowserUI
				//if (!ItemChecklistUI.visible)
				//{
				//	ItemChecklistUI.instance.RemoveAllChildren();
				//	var isInitializedFieldInfo = typeof(Terraria.UI.UIElement).GetField("_isInitialized", BindingFlags.Instance | BindingFlags.NonPublic);
				//	isInitializedFieldInfo.SetValue(ItemChecklistUI.instance, false);
				//	ItemChecklistUI.instance.Activate();
				//}
			}
		}

		public override void OnEnterWorld(Player player)
		{
			var itemChecklistPlayer = Main.LocalPlayer.GetModPlayer<ItemChecklistPlayer>();
			ItemChecklistUI.Visible = false;
			ItemChecklistUI.announce = announcePreference;
			ItemChecklistUI.collectChestItems = findChestItemsPreference;
			//ItemChecklistUI.sortMode = sortModePreference;
			ItemChecklistUI.showCompleted = showCompletedPreference;
			ItemChecklist.instance.ItemChecklistUI.RefreshPreferences();
			ItemChecklist.instance.ItemChecklistUI.UpdateNeeded();
		}

		// Do I need to use Initialize? I think so because of cloning.
		public override void Initialize()
		{
			if (!Main.dedServ)
			{
				foundItems = new List<Item>();
				foundItem = new bool[ItemLoader.ItemCount];
				findableItems = new bool[ItemLoader.ItemCount];
				for (int i = 0; i < ItemLoader.ItemCount; i++)
				{
					if (i > 0 && !ItemID.Sets.Deprecated[i] && i != ItemID.Count && ItemChecklistUI.vanillaIDsInSortOrder != null && ItemChecklistUI.vanillaIDsInSortOrder[i] != -1) // TODO, is this guaranteed?
					{
						totalItemsToFind++;
						findableItems[i] = true;
					}
				}

				announcePreference = false;
				findChestItemsPreference = true;
				//sortModePreference = SortModes.TerrariaSort;
				showCompletedPreference = 0;
			}
		}

		public override void UpdateAutopause()
		{
			ChestCheck();
		}

		public override void PreUpdate()
		{
			ChestCheck();
		}

		private void ChestCheck()
		{
			if (!Main.dedServ && player.whoAmI == Main.myPlayer)
			{
				for (int i = 0; i < 59; i++)
				{
					if (!player.inventory[i].IsAir && !foundItem[player.inventory[i].type] && findableItems[player.inventory[i].type])
					{
						mod.GetGlobalItem<ItemChecklistGlobalItem>().ItemReceived(player.inventory[i]); // TODO: Analyze performance impact? do every 60 frames only?
					}
				}
				if (player.chest != -1 && (player.chest != player.lastChest || Main.autoPause && Main.gamePaused) && ItemChecklistUI.collectChestItems)
				{
					//Main.NewText(player.chest + " " + player.lastChest);
					Item[] items;
					if (player.chest == -2) 
						items = player.bank.item;
					else if (player.chest == -3)
						items = player.bank2.item;
					else if (player.chest == -4)
						items = player.bank3.item;
					else
						items = Main.chest[player.chest].item;
					for (int i = 0; i < 40; i++)
					{
						if (!items[i].IsAir && !foundItem[items[i].type] && findableItems[items[i].type])
						{
							mod.GetGlobalItem<ItemChecklistGlobalItem>().ItemReceived(items[i]);
						}
					}
				}
				if (ItemChecklistUI.collectChestItems)
				{
					if (MagicStorageIntegration.Enabled)
						MagicStorageIntegration.FindItemsInStorage();
					if (MagicStorageIntegration.EnabledExtra)
						MagicStorageIntegration.FindItemsInStorageExtra();
				}
			}
		}

		public override TagCompound Save()
		{
			// sanitize? should be possible to add item already seen.
			return new TagCompound
			{
				["FoundItems"] = foundItems.Select(ItemIO.Save).ToList(),
				//["SortMode"] = (int)ItemChecklistUI.sortMode,
				["Announce"] = ItemChecklistUI.announce, // Not saving default, saving last used....good thing?
				["CollectChestItems"] = ItemChecklistUI.collectChestItems,
				["ShowCompleted"] = ItemChecklistUI.showCompleted,
			};
		}

		public override void Load(TagCompound tag)
		{
			foundItems = tag.GetList<TagCompound>("FoundItems").Select(ItemIO.Load).ToList();
			//sortModePreference = (SortModes)tag.GetInt("SortMode");
			announcePreference = tag.GetBool("Announce");
			if (tag.ContainsKey("CollectChestItems")) // Missing tags get defaultvalue, which would be false, which isn't what we want.
				findChestItemsPreference = tag.GetBool("CollectChestItems");
			showCompletedPreference = tag.GetInt("ShowCompleted");

			foreach (var item in foundItems)
			{
				if (item.Name != "Unloaded Item")
				{
					foundItem[item.type] = true;
					totalItemsFound++;
				}
			}
		}
	}
}
