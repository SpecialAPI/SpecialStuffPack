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
            string name = "Orb of Knowledge";
            string shortdesc = "Secrets Long Forgotten";
            string longdesc = "Tells mysterious things that are either cryptic secrets or straigt up nonsense.\n\nAn orb as old as the Gungeon itself, it knows all about Gungeon.";
            OrbOfKnowledge item = ItemBuilder.EasyInit<OrbOfKnowledge>("items/knowledgeorb", "sprites/knowledge_orb_idle_001", name, shortdesc, longdesc, ItemQuality.D, SpecialStuffModule.globalPrefix, null, null);
            item.knowledge = new List<string>
            {
                "THE CREST AWAITS IN THE SEWERS",
                "2 ITEM 1 TOKEN OF MASTERY IS ALL IT TAKES",
                "FOLLOW THE RATS, THEY MAY LEAD YOU TO SECRETS",
                "THE FIREPLACE LOOKS SUSPICIOUS",
                "SAVE THE CAPED",
                "SURE, YOU CAN'T FIX THAT TV, BUT SOMEONE ELSE CAN",
                "INVISIBLE TRACKS IN THE MINES",
                "THE WATER WILL LEAD YOU",
                "INSIDE THE DRAGUN SKULL",
                "ALWAYS TOUCH MYSTERIOUS RIFTS IN SPACE",
                "GOLDEN KEYS LEAD TO BAD THINGS",
                "DOWNLOAD ENTER THE GUNGEON: HARDMODE",
                "THERE ARE NO ABSOLUTE TRUTH",
                "YET YOU STAND",
                "YOU ARE ME AND I AM YOU",
                "NOTHING IS LOST",
                "ARE YOU REALLY LISTENING TO AN ORB?",
                "YOU NEVER TRULY ESCAPE",
                "PRISMATISM IS BAD",
                "GUNAPI WILL NEVER RELEASE",
                "WHY IS MAKING PITS AT RUNTIME SO HARD WHY",
                "ENEMY BULLETS GREEN, AND ALLOWING THEM TO BE ABLE TO HEAL YOU. THIS IS ACTUALLY A REFERENCE TO A BOSS CALLED PROVIDENCE, THE PROFANED GODDESS FROM THE POPULAR TERRARIA CONTENT MOD THE CALAMITY MOD FOR THE POPULAR 2D SANDBOX RPG VIDEO " +
                "GAME TERRARIA. IN THE CALAMITY MOD, PROVIDENCE, THE PROFANED GODDESS WILL SOMETIMES HAVE AN ATTACK PHASE, SHOOTING OUT LOTS OF PROJECTILES AND TAKING VERY LITTLE DAMAGE. HOWEVER, DURING THIS ATTACK PHASE, PROVIDENCE, THE PROFANED " +
                "GODDESS WILL SOMETIMES FIRE OUT GREEN PROJECTILES, WHICH ON IMPACT, WILL HEAL THE PLAYER. THIS REFERENCE WAS CONFIRMED BY POPULAR INTERNET PERSONALITY BLEAKBUBBLES DURING A PRIVATE INTERVIEW. MOREOVER, THE PRISMATIC GUON STONE, " +
                "ANOTHER ITEM FROM THE POPULAR ENTER THE GUNGEON ITEM PACK THE BLEAKER ITEM PACK, IS, ALTHOUGH UNCONFIRMED, SPECULATED TO BE A REFERENCE TO AN ENDGAME WEAPON CALLED THE LAST PRISM FROM THE POPULAR GAME TERRARIA. THIS IS EVIDENT " +
                "BECAUSE OF THE UNCANNY VISUAL SIMILARITIES, AS WELL AS THE VERY SIMILAR BEHAVIOUR OF THESE TWO WEAPONS. THE FACT THAT THERE ARE SO MANY REFERENCES TO TERRARIA AND THE CALAMITY MOD FOR TERRARIA SHOWS HOW MUCH RESPECT CELEBRITY " +
                "BLEAKBUBBLES HAS FOR HIS/HER FELLOW DEVELOPERS AND MODDERS. THIS COULD ALSO HAVE THE CONNOTATION THAT BLEAKBUBBLES IS QUITE FOND OF THE POPULAR 2D SANDBOX GAME TERRARIA AND WISHES TO REFERENCE IT IN HIS WORKS.",
                "ENTER THE GUNGEON: HARDMODE IS THE BEST EXPANSION EVER MADE FOR ENTER THE GUNGEON. IT ADDS SOME EXTRA CHALLENGE TO THE GAME, TOTALLY FITS THE BASE ENTER THE GUNGEON AND IS VERY VERY VERY VERY VERY VERY BALANCED. IT THE PERFECT " +
                "EXPANSION. HERE ARE SOME OF IT'S RATINGS (5 PEOPLE/CREATURES/THINGS RATED IT IN TOTAL):\n" +
                " * A BLUE ARROW IN A BLUE CIRCLE: 5 STARS\n" +
                " * A SENTIENT GOLDEN LOCK: 5 STARS\n" +
                " * THE SAME GOLDEN LOCK BUT NOW COVERED IN DARK RED PAINT: 5 STARS\n" +
                " * LORD OF THE JAMMED(HE WAS CONTROLLED USING CROWN OF THE JAMMED): 5 STARS\n" +
                " * USER CH4MB3R(DEFINITELY NOT THE CREATOR OF ENTER THE GUNGEON: HARDMODE): 5 STARS\n" +
                "YOU DON'T HAVE IT? THEN YOU CAN GET IT ON https://hardmode.etg/download-hardmode FOR FREE!",
                "THE COOP VERSUS MOD IS THE MOST USELESS MOD I THINK I HAVE EVER SEEN. SERIOUSLY, HOW CAN YOU MAKE A MOD SO USELESS? IT'S SO USELESS, THAT EVEN THE CREATOR ADMITS ITS USELESSNESS AND HAS ALREADY ABANDONED IT.\n\n" +
                "ARE YOU A MAN OR A WOMAN? (SPOILER: IT DOESN'T MATTER.) DO YOU LIKE HUMAN OR LIZARD PEOPLE? DO YOU LIKE TO KILL THEM FOR MONEY? DO YOU ENJOY HATS? THEN WHAT ARE YOU DOING WITH A MOD THAT IS ABSOLUTELY USELESS?\n\n" +
                "WELL, WE ARE AN ANONYMOUS MEMBER OF THE EMPIRE MODDING COMMUNITY WHO HAS NOW OFFICIALLY TAKEN COOP VERSUS TO TASK.\n\n" +
                "\"COOP VERSUS IS BY FAR THE WORST COMMUNITY EFFORT I'VE SEEN IN AGES,\" HE WROTE IN AN IN-DEPTH",
                "\"STRING_NOT_FOUND\" - THAT ONE NPC FROM PSOG THAT ONLY APPEARS WHEN YOU SPEEDRUN THE GAME",
            };
            new Dictionary<SecretType, List<string>> 
            { 
                { SecretType.FIREPLACE_ROOM, new List<string> { "THE FIREPLACE LOOKS SUSPICIOUS", "BRING THE WATER" } }, 
                { SecretType.SEWER_ENTRANCE, new List<string> { "SOMETHING HIDDEN LIES BENEATH THIS TRAP DOOR" } },
                { SecretType.CREST_ROOM_SEWERS, new List<string> { "THE CATHEDRAL AWAITS" } },
                { SecretType.CREST_ROOM_GUNGEON, new List<string> { "PLACE THE CREST AND GO DOWN" } },
                { SecretType.SECRET_ROOM, new List<string> { "THE WALLS HERE LOOK THIN" } },
                { SecretType.HIGH_DRAGUNFIRE_SECRET_ROOM, new List<string> { "SOMETHING HIDDEN LIES IN THESE WALLS" } },
                { SecretType.HIGH_DRAGUNFIRE, new List<string> { "OPEN THE CHEST" } },
                { SecretType.BROTHER_ALBERN_SECRET_ROOM, new List<string> { "THE TRUTH LIES IN THESE WALLS" } },
                { SecretType.BROTHER_ALBERN, new List<string> { "TELL HIM THE TRUTH AND BE REWARDED" } },
                { SecretType.MINES_RAT_ROOM, new List<string> { "SWEEP THE FLOOR" } },
                { SecretType.MINES_RAT_TUNNEL_1, new List<string> { "BLANK" } },
                { SecretType.MINES_RAT_TUNNEL_2, new List<string> { "BLANK AGAIN" } },
                { SecretType.MINES_RAT_TUNNEL_3, new List<string> { "INSERT THE KEY" } },
                { SecretType.SELL_CREEP_HOLLOW, new List<string> { "2 ITEM 1 TOKEN OF MASTERY IS ALL IT TAKES" } },
                { SecretType.ARCANE_GUNPOWDER_MINES, new List<string> { "RIDE THE INVISIBLE TRACKS" } },
                { SecretType.PLANAR_LEAD_HOLLOW, new List<string> { "MAY THE WATER LEAD YOU" } },
                { SecretType.DRAGUN_SKULL, new List<string> { "INSIDE THE DRAGUN SKULL" } },
                { SecretType.BUSTED_TV, new List<string> { "SURE, YOU CAN'T FIX THAT TV, BUT SOMEONE ELSE CAN" } },
                { SecretType.CAPED_KIN, new List<string> { "SAVE THE CAPED" } },
                { SecretType.PARADOX_PORTAL, new List<string> { "ALWAYS TOUCH MYSTERIOUS RIFTS IN SPACE" } },
                { SecretType.ABBEY_SUPER_SECRET_1, new List<string> { "MISFORTUNE LIES IN THESE WALLS" } },
                { SecretType.ABBEY_SUPER_SECRET_2, new List<string> { "GOLDEN KEYS LEAD TO BAD THINGS" } }
            }.SplitDictionary(out item.secretKnowledgeKeys, out item.secretKnowledgeValues);
        }

        public SecretType GetRoomSecret(RoomHandler room)
        {
            if(room != null)
            {
                if (room.GetComponentsInRoom<FireplaceController>().Where((FireplaceController fireplace) => !fireplace.IsFlipped()).ToList().Count > 0)
                {
                    return SecretType.FIREPLACE_ROOM;
                }
                if(room.GetComponentsInRoom<CrestDoorController>().Count > 0)
                {
                    return SecretType.CREST_ROOM_GUNGEON;
                }
                if(room.GetComponentsInRoom<RewardPedestal>().Where((RewardPedestal pedestal) => pedestal.contents.PickupObjectId == GlobalItemIds.CathedralCrest).ToList().Count > 0)
                {
                    return SecretType.CREST_ROOM_SEWERS;
                }
                if (room.GetComponentsInRoom<RewardPedestal>().Where((RewardPedestal pedestal) => pedestal.contents.PickupObjectId == 349).ToList().Count > 0)
                {
                    return SecretType.PLANAR_LEAD_HOLLOW;
                }
                if (room.GetComponentsInRoom<RewardPedestal>().Where((RewardPedestal pedestal) => pedestal.contents.PickupObjectId == 351).ToList().Count > 0)
                {
                    return SecretType.ARCANE_GUNPOWDER_MINES;
                }
                if (room.GetComponentsInRoom<MajorBreakable>().Where((MajorBreakable breakable) => breakable.SpawnItemOnBreak && breakable.ItemIdToSpawnOnBreak == 350).ToList().Count > 0)
                {
                    return SecretType.DRAGUN_SKULL;
                }
                if(room.GetComponentsInRoom<SecretFloorInteractableController>().Where((SecretFloorInteractableController interactable) => interactable.targetLevelName == "tt_sewer").ToList().Count > 0)
                {
                    return SecretType.SEWER_ENTRANCE;
                }
                if (room.GetComponentsInRoom<SellCellController>().Count > 0 && GameManager.Instance != null && GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON)
                {
                    return SecretType.SELL_CREEP_HOLLOW;
                }
                if (room.connectedRooms != null && room.connectedRooms.Where((RoomHandler connectedroom) => connectedroom.area != null && connectedroom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET &&
                    (connectedroom.secretRoomManager == null || connectedroom.secretRoomManager.revealStyle == SecretRoomManager.SecretRoomRevealStyle.Simple)).ToList().Count > 0)
                {
                    return SecretType.SECRET_ROOM;
                }
            }
            return SecretType.NONE;
        }

        public List<string> knowledge;
        public List<SecretType> secretKnowledgeKeys;
        public List<List<string>> secretKnowledgeValues;
        
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
