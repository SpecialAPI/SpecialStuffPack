using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Placeables
{
    public class CCCChallenge : DungeonPlaceableBehaviour, IPlayerInteractable
    {
        public static void Init()
        {
            var thingy = AssetBundleManager.Load<GameObject>("cccplaceable");
            var sp = thingy.AddSprite("eye_icon_idle_001", "PlaceableCollection", "Brave/LitTk2dCustomFalloffTintableTilted");
            sp.HeightOffGround = 0;
            var chall = thingy.AddComponent<CCCChallenge>();
            chall.notifId = SpriteBuilder.AddSpriteToCollection("eye_icon_thin_idle_001", EasyCollectionSetup("PlaceableCollection"), "tk2d/CutoutVertexColor");
            BreachShrinePlacer.RegisterBreachShrine(thingy, new(19.55f, 39.6f));
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            Bounds bounds = sprite.GetBounds();
            bounds.SetMinMax(bounds.min + transform.position, bounds.max + transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2));
        }

        public float GetOverrideMaxDistance()
        {
            return -1;
        }

        public void Interact(PlayerController interactor)
        {
            transform.position.GetAbsoluteRoom()?.DeregisterInteractable(this);
            interactor.Ext().isCCCRun = true;
            interactor.Ext().wasLastInFoyer = true;
            interactor.Ext().CCCSequenceNextFloor();
            interactor.RemoveAllPassiveItems();
            interactor.RemoveAllActiveItems();
            interactor.inventory.DestroyAllGuns();
            interactor.AddOwnerlessModifier(PlayerStats.StatType.AmmoCapacityMultiplier, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            EncounterTrackable.SuppressNextNotification = true;
            LootEngine.GivePrefabToPlayer(RustySidearmObject.gameObject, interactor);
            EncounterTrackable.SuppressNextNotification = true;
            LootEngine.GivePrefabToPlayer(MachinePistolObject.gameObject, interactor);
            var sidearm = interactor.inventory.AllGuns.Find(x => x.InfiniteAmmo);
            if(sidearm != null)
            {
                sidearm.PreventStartingOwnerFromDropping = true;
            }
            interactor.startingGunIds = new() { RustySidearmId, MachinePistolId };
            Instantiate(VFXDatabase.MiniBlank, sprite.WorldCenter, Quaternion.identity);
            Exploder.DoDistortionWave(sprite.WorldCenter, 2f, 0.2f, 4f, 0.2f);
            var ss = new GameObject("sound");
            AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Capture_01", ss);
            ss.AddComponent<BadLuckAffectedChest>().StartCoroutine(HandleOminousMessage(ss, notifId, sprite.Collection));
            Destroy(gameObject);
        }

        public static IEnumerator HandleOminousMessage(GameObject d, int id, tk2dSpriteCollectionData coll)
        {
            yield return new WaitForSeconds(3f);
            AkSoundEngine.PostEvent("Play_WPN_kthulu_blast_01", d);
            GameUIRoot.Instance.notificationController.DoCustomNotification("????????? ????????", "???? ?????", coll, id, UINotificationController.NotificationColor.PURPLE, false, false);
            Destroy(d, 3f);
            yield break;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
        }

        public void OnExitRange(PlayerController interactor)
        {
        }

        public int notifId;
    }
}
