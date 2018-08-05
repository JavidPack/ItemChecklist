using System;
using MagicStorage;
using MagicStorage.Components;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ItemChecklist
{
	// autopause?
	static class MagicStorageIntegration
	{
		static Mod MagicStorage;
		public static bool Enabled => MagicStorage != null;
		static Point16 previousStorageAccess = new Point16(-1, -1);
		static StorageAccess tile;
		//static StorageAccess tile = null; //throws TypeInitializationException 

		public static void Load()
		{
			MagicStorage = ModLoader.GetMod("MagicStorage");
		}

		public static void Unload()
		{
			MagicStorage = null;
		}

		internal static void FindItemsInStorage()
		{
			var storagePlayer = Main.LocalPlayer.GetModPlayer<StoragePlayer>();
			Point16 storageAccess = storagePlayer.ViewingStorage();
			if (storageAccess == previousStorageAccess)
				return;
			previousStorageAccess = storageAccess;
			if (!Main.playerInventory || storageAccess.X < 0 || storageAccess.Y < 0)
				return;
			ModTile modTile = TileLoader.GetTile(Main.tile[storageAccess.X, storageAccess.Y].type);
			if (modTile == null || !(modTile is StorageAccess))
			{
				return;
			}
			TEStorageHeart heart = ((StorageAccess)modTile).GetHeart(storageAccess.X, storageAccess.Y);
			if (heart == null)
			{
				return;
			}
			var items = heart.GetStoredItems();
			// Will 1000 items crash the chat?
			foreach (var item in items)
			{
				ItemChecklist.instance.GetGlobalItem<ItemChecklistGlobalItem>().ItemReceived(item);
			}
		}
	}
}
