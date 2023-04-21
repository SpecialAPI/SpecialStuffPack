using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Characters
{
    public static class NinjaRework
    {
        public static void Init()
        {
            var ninja = (GameObject)BraveResources.Load("PlayerNinja");
            SpecialAssets.assets.Add(ninja);
            var ninjaPlayer = ninja.GetComponent<PlayerController>();
            ninjaPlayer.BosscardSprites = new() { AssetBundleManager.Load<Texture2D>("ninja_bosscard") };
            ninjaPlayer.BosscardSpriteFPS = 1;
            var lib = ninja.transform.Find("PlayerSprite").GetComponent<tk2dSpriteAnimator>().Library;
            lib.clips = lib.clips.AddToArray(new(lib.GetClipByName("idle")) { name = "idle_twohands" });
            lib.clips = lib.clips.AddToArray(new(lib.GetClipByName("idle_bw")) { name = "idle_bw_twohands" });
            lib.clips = lib.clips.AddToArray(new(lib.GetClipByName("idle_forward")) { name = "idle_forward_twohands" });
            lib.clips = lib.clips.AddToArray(new(lib.GetClipByName("idle_backward")) { name = "idle_backward_twohands" });

            lib.clips = lib.clips.AddToArray(new(lib.GetClipByName("run_down")) { name = "run_down_twohands" });
            lib.clips = lib.clips.AddToArray(new(lib.GetClipByName("run_up")) { name = "run_up_twohands" });
            lib.clips = lib.clips.AddToArray(new(lib.GetClipByName("run_right")) { name = "run_right_twohands" });
            lib.clips = lib.clips.AddToArray(new(lib.GetClipByName("run_right_bw")) { name = "run_right_bw_twohands" });

            ninjaPlayer.AddToBreach("ninjabreachflag", new(12.5f, 22f), new(), new(8, 1, 10, 4), new(7, 1, 12, 19), new(-1, 0), "ninjaoverheadpanel", "The Ninja", "ninja", 5, "ninja_items");
        }
    }
}
