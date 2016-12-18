using ItemChecklist.UI;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ItemChecklist
{
	class ItemChecklistGlobalItem : GlobalItem
	{
		// OnPIckup only called on LocalPlayer: I think
		public override void OnCraft(Item item, Recipe recipe)
		{
			ItemReceived(item);
		}

		// OnPIckup only called on LocalPlayer: i == Main.myPlayer
		public override bool OnPickup(Item item, Player player)
		{
			ItemReceived(item);
			return true;
		}

		// TODO, unloaded items, check against??
		internal void ItemReceived(Item item)
		{
			var itemChecklistPlayer = Main.LocalPlayer.GetModPlayer<ItemChecklistPlayer>(mod);
			if (!itemChecklistPlayer.foundItem[item.type] && itemChecklistPlayer.findableItems[item.type])
			{
				Item newItem = new Item();
				newItem.SetDefaults(item.type);
				itemChecklistPlayer.foundItems.Add(newItem);
				itemChecklistPlayer.totalItemsFound++;
				itemChecklistPlayer.foundItem[item.type] = true;
				//ItemChecklist.instance.ItemChecklistUI.UpdateCheckboxes();
				ItemChecklist.instance.ItemChecklistUI.UpdateNeeded();
				if (ItemChecklistUI.announce)
				{
					Main.NewText($"Congrats: You found your first {item.name}.       Total Progress: {itemChecklistPlayer.totalItemsFound}/{itemChecklistPlayer.totalItemsToFind}");
				}
			}
		}
	}
}
