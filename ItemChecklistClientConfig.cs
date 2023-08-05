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
		public bool ShowItemModSource { get; set; }

		[Header("AutomaticSettings")]
		// non-player specific stuff:

		[DefaultValue(typeof(Vector2), "475, 370")]
		[Range(0f, 1920f)]
		public Vector2 ItemChecklistSize { get; set; }

		[DefaultValue(typeof(Vector2), "400, 400")]
		[Range(0f, 1920f)]
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