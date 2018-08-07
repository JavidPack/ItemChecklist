using MagicStorage;
using MagicStorage.Components;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ItemChecklist
{
	// This code can be used as an example of weak references.
	// The most important part of weak references is remembering to test your mod with the referenced .tmod file deleted and a fresh start of tModLoader.
	// If you do not, the mod might crash.
	// Here we show a few pitfalls that might cause a TypeInitializationException to be thrown.
	// Remember, you have to gate all access to Types and Methods defined in the weakly referenced mod or it will crash.
	// All calls to methods in this class by this mod besides Load and Unload are gated with a check to Enabled.
	static class MagicStorageIntegration
	{
		// Here we store a reference to the MagicStorage Mod instance. We can use it for many things. 
		// You can call all the Mod methods on it just like we do with our own Mod instance: MagicStorage.ItemType("ShadowDiamond")
		static Mod MagicStorage;

		// Here we define a bool property to quickly check if MagicStorage is loaded. 
		public static bool Enabled => MagicStorage != null;
		// Below is an alternate approach to the Enabled property seen above. 
		// To make sure an up-to-date version of the referenced mod is being used, usually we write "MagicStorage@0.4.3.1" in build.txt.
		// This, however, would throw an error if the 0.4.1 were loaded. 
		// This approach, with the @0.4.3.1 removed from the weakReferences in build.txt and the Version check added to Enabled below,
		// allows older versions of MagicStorage to load alongside this mod even though the integration won't work.
		// Usually letting tModLoader handle the version requirement and forcing your users to update their mods is easier and better.
		//public static bool Enabled => MagicStorage != null && MagicStorage.Version >= new Version(0, 4, 3, 1);

		static Point16 previousStorageAccess = new Point16(-1, -1);

		// Here is an example of using Types in MagicStorage. Note the special approaches that must be made with weak referenced Mods.
		//static StorageAccess tile = null; // Assigning a class/static field, even to null, will throw TypeInitializationException
		static StorageAccess tile; // Not assigning a field seems to work

		public static void Load()
		{
			MagicStorage = ModLoader.GetMod("MagicStorage");
			if (Enabled)
				// tile = null; // Will also crash. Here even though Enabled will be false, the Type of tile will still need to be resolved when this method runs.
				Initialize(); // Move that logic into another method to prevent this.
		}

		private static void Initialize()
		{
			tile = new StorageAccess(); // This method will only be called when Enable is true, preventing TypeInitializationException
		}

		public static void Unload()
		{
			MagicStorage = null; // Make sure to null out any references to allow Garbage Collection to work.
		}

		// StoragePlayer and TEStorageHeart are classes in MagicStorage. 
		// Make sure to extract the .dll from the .tmod and then add them to your .csproj as references.
		// As a convention, I rename the .dll file ModName_v1.2.3.4.dll and place them in Mod Sources/Mods/lib. 
		// I do this for organization and so the .csproj loads properly for others using the GitHub repository. 
		// Remind contributors to download the referenced mod itself if they wish to build the mod.
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
