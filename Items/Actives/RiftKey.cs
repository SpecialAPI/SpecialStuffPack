using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Actives
{
    public class RiftKey : PlayerItem
    {
        public static void Init()
        {
            string name = "Rift Key";
            string shortdesc = "";
            string longdesc = "";
            var item = ItemBuilder.EasyInit<RiftKey>("items/riftkey", "sprites/rift_key_idle_001", name, shortdesc, longdesc, ItemQuality.A, SpecialStuffModule.globalPrefix, null, null);
            item.MaxExitDistance = 2f;
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            var dungeon = DungeonDatabase.GetOrLoadByName(GameManager.Instance.GetLastLoadedLevelDefinition().dungeonPrefabPath);
            var compiledListOfStuff = dungeon?.PatternSettings?.flows.ConvertAll(x => x?.fallbackRoomTable?.GetCompiledList()).FindAll(x => x != null).SelectMany(x => x);
            user.TeleportToPoint(GameManager.Instance.Dungeon.AddRuntimeRoom(compiledListOfStuff.RandomElement().room).GetCenterCell().ToVector2(), true);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return (user?.CurrentRoom?.area?.prototypeRoom?.exitData?.exits?.Count).GetValueOrDefault() > 0 && 
                user.CurrentRoom.area.prototypeRoom.exitData.exits.Exists(x => x.containedCells.Exists(x => Vector2.Distance(x + user.CurrentRoom.area.basePosition.ToVector2(), user.CenterPosition) <= MaxExitDistance));
        }

        public float MaxExitDistance;
    }
}
