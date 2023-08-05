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

		public override void OnEnterWorld()
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
					if (i > 0 && !ItemID.Sets.Deprecated[i] && ItemLoader.GetItem(i) is not Terraria.ModLoader.Default.UnloadedItem && ItemChecklistUI.vanillaIDsInSortOrder != null && ItemChecklistUI.vanillaIDsInSortOrder[i] != -1) // TODO, is this guaranteed?
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
			if (!Main.dedServ && Player.whoAmI == Main.myPlayer)
			{
				for (int i = 0; i < 59; i++)
				{
					if (!Player.inventory[i].IsAir && !foundItem[Player.inventory[i].type] && findableItems[Player.inventory[i].type])
					{
						// Looping because.. nervous that there might be more than one somehow?
						foreach (ItemChecklistGlobalItem item in Mod.GetContent<ItemChecklistGlobalItem>())
						{
							item.ItemReceived(Player.inventory[i]); // TODO: Analyze performance impact? do every 60 frames only?
						}
					}
				}
				if (Player.chest != -1 && (Player.chest != Player.lastChest || Main.autoPause && Main.gamePaused) && ItemChecklistUI.collectChestItems)
				{
					//Main.NewText(player.chest + " " + player.lastChest);
					Item[] items;
					if (Player.chest == -2) 
						items = Player.bank.item;
					else if (Player.chest == -3)
						items = Player.bank2.item;
					else if (Player.chest == -4)
						items = Player.bank3.item;
					else if (Player.chest == -5)
						items = Player.bank4.item;
					else
						items = Main.chest[Player.chest].item;
					for (int i = 0; i < 40; i++)
					{
						if (!items[i].IsAir && !foundItem[items[i].type] && findableItems[items[i].type])
						{
							foreach (ItemChecklistGlobalItem item in Mod.GetContent<ItemChecklistGlobalItem>())
							{
								item.ItemReceived(items[i]);
							}
						}
					}
				}
				if (ItemChecklistUI.collectChestItems && MagicStorageIntegration.Enabled)
					MagicStorageIntegration.FindItemsInStorage();
			}
		}

		public override void SaveData(TagCompound tag)
		{
			// sanitize? should be possible to add item already seen.
			tag["FoundItems"] = foundItems.Where(item => item.Name != "Unloaded Item").Select(ItemIO.Save).ToList();
			//tag["SortMode"] = (int)ItemChecklistUI.sortMode;
			tag["Announce"] = ItemChecklistUI.announce; // Not saving default, saving last used....good thing?
			tag["CollectChestItems"] = ItemChecklistUI.collectChestItems;
			tag["ShowCompleted"] = ItemChecklistUI.showCompleted;
		}

		public override void LoadData(TagCompound tag)
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
