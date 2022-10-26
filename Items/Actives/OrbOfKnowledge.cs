using Dungeonator;
using SpecialStuffPack.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items
{
    public class OrbOfKnowledge : PlayerItem
    {
        public static void Init()
        {
            string name = "Magic 8 Ball";
            string shortdesc = "28 Souls Left";
            string longdesc = "A weird object, seemignly from a different world.";
            OrbOfKnowledge item = EasyItemInit<OrbOfKnowledge>("items/8ball", "sprites/8_ball_idle_001", name, shortdesc, longdesc, ItemQuality.D, null, null);
            item.SetCooldownType(CooldownType.Timed, 1.5f);
            item.knowledge = new List<string>
            {
                "Never forget.",
                "Time is an illusion.",
                "The Bell tolls.",
                "You have abandoned humanity and accepted immortality.",
                "Everything is meaningless in the end.",
                "Actions speak louder than words.",
                "No one can help you.",
                "You are being watched.",
                "There are no Absolute Truths.",
                "Yet you stand.",
                "Nothing is lost.",
                "You are me and I am you.",
                "Hard choices will be justified in the end.",
                "Forever cursed.",
                "The walls feel hollow here.",
                "Time is ticking.",
                "You are dead.",
                "Frogs."
            };
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            var text = knowledge.FindAll(x => x != lastString).RandomElement();
            lastString = text;
            var tileset = GameManager.Instance?.Dungeon?.tileIndices?.tilesetId;
            if (Random.Range(0, 50) == 0)
            {
                text = "Hardmode will never be released.";
            }
            if(tileset == GlobalDungeonData.ValidTilesets.GUNGEON && Random.Range(0, 200) == 0)
            {
                text = "They are sealed in the walls around, worshipping a chest.";
            }
            if(user?.CurrentRoom != null && !shownSecretRooms.Contains(user.CurrentRoom) && user.CurrentRoom.connectedRooms.Exists(x => x?.area != null && x.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET))
            {
                text = "The walls feel hollow here.";
                shownSecretRooms.Add(user.CurrentRoom);
            }
            TextBoxManager.ClearTextBox(user.transform);
            TextBoxManager.ShowStoneTablet(user.CenterPosition + Vector2.up, user.transform, 2f, text, true, false);
        }

        public List<string> knowledge;
        public List<SecretType> secretKnowledgeKeys;
        public List<List<string>> secretKnowledgeValues;
        private string lastString;
        //private bool shownOubGem;
        //private bool shownAbbeyGem;
        //private bool shownRatGem;
        //private bool shownRngGem;
        //private bool shownDiamond;
        private List<RoomHandler> shownSecretRooms = new();
        
        public enum SecretType
        {
            NONE,
            FIREPLACE_ROOM,
            SEWER_ENTRANCE,
            CREST_ROOM_SEWERS,
            CREST_ROOM_GUNGEON,
            SECRET_ROOM,
            HIGH_DRAGUNFIRE_SECRET_ROOM,
            HIGH_DRAGUNFIRE,
            BROTHER_ALBERN_SECRET_ROOM,
            BROTHER_ALBERN,
            MINES_RAT_ROOM,
            MINES_RAT_TUNNEL_1,
            MINES_RAT_TUNNEL_2,
            MINES_RAT_TUNNEL_3,
            SELL_CREEP_HOLLOW,
            ARCANE_GUNPOWDER_MINES,
            PLANAR_LEAD_HOLLOW,
            DRAGUN_SKULL,
            BUSTED_TV,
            CAPED_KIN,
            PARADOX_PORTAL,
            ABBEY_SUPER_SECRET_2,
            ABBEY_SUPER_SECRET_1
        }
    }
}
