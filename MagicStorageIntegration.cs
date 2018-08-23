using System.Runtime.CompilerServices;
using MagicStorage;
using MagicStorage.Components;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ItemChecklist
{
	// This code can be used as an example of weak references.
	// The most important part of weak references is remembering to test your mod with the referenced .tmod file deleted and a fresh start of tModLoader. (To clear the loaded assemblies.)
	// If you do not, the mod might crash. Testing on non-windows machines is also important.
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
		//static StorageAccess tile; // Not assigning a field seems to work, but actually crashes on Mono (Mac/Linux). 
		// The solution that works is an inner class (Or non-inner, doesn't matter). Here we use a static class, but a non-static class could also be used if more convenient (using a constructor, unloading everything at once). 
		static class MagicStorageIntegrationMembers
		{
			internal static StorageAccess tile;
		}
		//static MagicStorageIntegrationMembers members;
		//class MagicStorageIntegrationMembers
		//{
		//	internal StorageAccess tile;
		//}

		public static void Load()
		{
			MagicStorage = ModLoader.GetMod("MagicStorage");
			if (Enabled)
				//MagicStorageIntegrationMembers.tile = null; // Will also crash. Here even though Enabled will be false, the Type of tile will still need to be resolved when this method runs.
				//members = new MagicStorageIntegrationMembers(); // Even thought the Type StorageAccess is hidden behind MagicStorageIntegrationMembers, this line will also cause MagicStorageIntegrationMembers and consequently StorageAccess to need to be resolved.
				Initialize(); // Move that logic into another method to prevent this.
		}

		// Be aware of inlining. Inlining can happen at the whim of the runtime. Without this Attribute, this mod happens to crash the 2nd time it is loaded on Linux/Mac. (The first call isn't inlined just by chance.) This can cause headaches. 
		// To avoid TypeInitializationException (or ReflectionTypeLoadException) problems, we need to specify NoInlining on methods like this to prevent inlining (methods containing or accessing Types in the Weakly referenced assembly). 
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void Initialize()
		{
			// This method will only be called when Enable is true, preventing TypeInitializationException
			MagicStorageIntegrationMembers.tile = new StorageAccess();
			//members = new MagicStorageIntegrationMembers();
			//members.tile = new StorageAccess();
		}

		public static void Unload()
		{
			if (Enabled) // Here we properly unload, making sure to check Enabled before setting MagicStorage to null.
				Unload_Inner(); // Once again we must separate out this logic.
			MagicStorage = null; // Make sure to null out any references to allow Garbage Collection to work.
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void Unload_Inner()
		{
			MagicStorageIntegrationMembers.tile = null;
			//members = null;
		}

		// In this example we used Initialize and Unload_Inner in conjunction with Load and Unload to avoid TypeInitializationException, but we could also just check Enabled before calling MagicStorageIntegration.Unload(); and assign MagicStorage and check Enabled before calling MagicStorageIntegration.Load(); 
		// I opted to keep the MagicStorage integration logic in the MagicStorageIntegration class to keep MagicStorage related code as sparse and unobtrusive as possible within the remaining codebase for this mod.
		// That said, in ItemChecklistPlayer, we see this:
		//	if (ItemChecklistUI.collectChestItems && MagicStorageIntegration.Enabled)
		//		MagicStorageIntegration.FindItemsInStorage();
		// I could have changed this to always call FindItemsInStorage and have FindItemsInStorage check Enabled, but that would require a FindItemsInStorage_Inner type class once again to prevent errors. 
		// Whatever approach you do, be aware of what you are doing and be careful.

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
