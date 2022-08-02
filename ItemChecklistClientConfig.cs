using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ItemChecklist
{
	class ItemChecklistClientConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[DefaultValue(true)]
		[Label("Show Item Mod Source")]
		[Tooltip("Show which mod adds which item in the recipe catalog. Disable for immersion.")]
		public bool ShowItemModSource { get; set; }

		[Header("Automatic Settings")]
		// non-player specific stuff:

		[DefaultValue(typeof(Vector2), "475, 350")]
		[Range(0f, 1920f)]
		[Label("Item Checklist Size")]
		[Tooltip("Size of the Item Checklist UI. This will automatically save, no need to adjust")]
		public Vector2 ItemChecklistSize { get; set; }

		[DefaultValue(typeof(Vector2), "400, 400")]
		[Range(0f, 1920f)]
		[Label("Item Checklist Poisition")]
		[Tooltip("Position of the Item Checklist UI. This will automatically save, no need to adjust")]
		public Vector2 ItemChecklistPosition { get; set; }

		internal static void SaveConfig() {
			// in-game ModConfig saving from mod code is not supported yet in tmodloader, and subject to change, so we need to be extra careful.
			// This code only supports client configs, and doesn't call onchanged. It also doesn't support ReloadRequired or anything else.
			MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
			if (saveMethodInfo != null)
				saveMethodInfo.Invoke(null, new object[] { ModContent.GetInstance<ItemChecklistClientConfig>() });
			else
				ItemChecklist.instance.Logger.Warn("In-game SaveConfig failed, code update required");
		}
	}
}