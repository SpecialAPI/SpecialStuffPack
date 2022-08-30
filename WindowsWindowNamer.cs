using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SpecialStuffPack
{
    public static class WindowsWindowNamer
    {
		public static readonly string[] Splashes = new string[]
		{
			"Enter the Gungeon 2",
			"Enter the Gungeon: now with more frogs!",
			"Enter the Gungeon: award winnder for worst code!",
			"Enter the Gungeon: sponsored by Blowfish Chocolates™",
			"The Binding of Isaac Rebirth",
			"Enter the Gungeon: the return of magnificus",
			"theGungeon.Enter(you);",
			"Enter the Gungeon by Dodge Roll",
			"Enter the Gungeon by Dog Roll",
			"Enter the Gungeon: now with 100% less MtG!",
			"Enter the Gun Dungeon",
			"Enter the Gungeon: made with Unity!",
			"Enter the Gungeon: dodge the bullets!",
			"Enter the Gungeon: Sunlight Javelin is underrated!",
			"Enter the Gungeon: beware of golden keys!",
			"Enter the Gungeon: singleplayer Cultist edition",
			"Enter the Gungeon: Gunslinger is overpowered",
			"Enter the Gungeon: now with 10% less armor drops!",
			"Enter the Gungeon: now with guns!",
			"Enter the Gungeon: %ITEMS new items!",
			"Enter the Gungoen: totally not hardmode!",
			"Enter the Gungeon Gungeon Gungeon Gungeon",
			"Cult of the Bullet",
			"Enter the Gungeon: sus edition",
			"Enter the Gungeon: Enter the Gungeon: Enter the Gungeon",
			"Enter the Gungeon: download actual beastmode!",
			"Enter the Gungeon: the wiki lies!",
			"Enter the Gungeon: supply drop update",
			"Enter the Gungeon",
		};

		public static void RenameWindow()
        {
			if(Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsEditor)
			{
                try
				{
					var wind = GetActiveWindow();
					var splh = BraveUtility.RandomElement(Splashes.ToList());
					SetWindowText(wind, $"{splh.Replace("%ITEMS", ItemIds.Count.ToString())} (SpecialAPI's Stuff loaded)");
				}
                catch { }
			}
        }

		[DllImport("user32.dll")]
		private static extern int GetActiveWindow();
		[DllImport("user32.dll")]
		public static extern bool SetWindowText(int hwnd, string text);
	}
}
