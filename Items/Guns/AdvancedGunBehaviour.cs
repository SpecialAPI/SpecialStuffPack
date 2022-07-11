using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Items.Guns
{
    public abstract class AdvancedGunBehaviour : BraveBehaviour, ILevelLoadedListener, IGunInheritable
    {
		public void Awake()
		{
			Gun = GetComponent<Gun>();
			if(Gun != null)
			{
				OnCreation(Gun);
				Gun.OnInitializedWithOwner += OnInitializedWithOwner;
				Gun.OnInitializedWithOwner += InternalOnInitializedWithOwner;
				Gun.PostProcessProjectile += PostProcessProjectile;
				Gun.PostProcessVolley += PostProcessVolley;
				Gun.OnDropped += OnDropped;
				Gun.OnDropped += InternalOnDropped;
				Gun.OnAutoReload += OnAutoReload;
				Gun.OnReloadPressed += OnReloadPressed;
				Gun.OnFinishAttack += OnFinishAttack;
				Gun.OnPostFired += OnPostFired;
				Gun.OnAmmoChanged += OnAmmoChanged;
				Gun.OnBurstContinued += OnBurstContinued;
				Gun.OnPreFireProjectileModifier += OnPreFireProjectileModifier;
				Gun.OnReflectedBulletDamageModifier += OnReflectedBulletDamageModifier;
				Gun.OnReflectedBulletScaleModifier += OnReflectedBulletScaleModifier;
				Gun.ModifyActiveCooldownDamage += ModifyActiveCooldownDamage;
				if (Gun.CurrentOwner != null)
                {
					OnInitializedWithOwner(Gun.CurrentOwner);
					InternalOnInitializedWithOwner(Gun.CurrentOwner);
				}
			}
		}

		private void InternalOnInitializedWithOwner(GameActor owner)
        {
			if(owner == null)
            {
				return;
            }
			if(owner is PlayerController player)
            {
				OnPlayerPickup(player);
            }
			else if(owner is AIActor enemy)
            {
				OnEnemyPickup(enemy);
            }
        }

		private void InternalOnDropped()
        {
			if(Gun?.CurrentOwner != null)
            {
				if (Gun.CurrentOwner is PlayerController player)
                {
					OnDroppedByPlayer(player);
                }
            }
        }

		public virtual void OnDroppedByPlayer(PlayerController player)
        {

        }

		public virtual void OnEnemyPickup(AIActor enemyOwner)
        {
        }

		public virtual void OnPlayerPickup(PlayerController playerOwner)
        {
        }

		public virtual void Start()
        {
        }

		public virtual void OnCreation(Gun gun)
        {
        }

		public virtual void Update()
        {
        }

		public virtual float OnReflectedBulletDamageModifier(float originalDamage)
        {
			return originalDamage;
        }

		public virtual float OnReflectedBulletScaleModifier(float originalScale)
        {
			return originalScale;
        }

		public virtual float ModifyActiveCooldownDamage(float originalDamage)
        {
			return originalDamage;
        }

		public virtual void OnInitializedWithOwner(GameActor actor)
		{
		}

		public virtual void OnBurstContinued(PlayerController player, Gun gun)
        {
        }

		public virtual void PostProcessProjectile(Projectile projectile)
		{
		}

		public virtual void PostProcessVolley(ProjectileVolleyData volley)
        {
        }

		public virtual void OnDropped()
		{
		}

		public virtual void OnAutoReload(PlayerController player, Gun gun)
		{
		}

		public virtual void OnReloadPressed(PlayerController player, Gun gun, bool manual)
		{
		}

		public virtual void OnFinishAttack(PlayerController player, Gun gun)
		{
		}

		public virtual void OnPostFired(PlayerController player, Gun gun)
		{
		}

		public virtual void OnAmmoChanged(PlayerController player, Gun gun)
		{
		}

		public virtual Projectile OnPreFireProjectileModifier(Gun gun, Projectile projectile, ProjectileModule module)
		{
			return projectile;
		}

		static AdvancedGunBehaviour()
        {
			new Hook(typeof(GameManager).GetConstructor(new Type[0]), typeof(AdvancedGunBehaviour).GetMethod(nameof(AdvancedGunBehaviour.AddLevelLoadListener), BindingFlags.NonPublic | BindingFlags.Static));
        }

		private static void AddLevelLoadListener(Action<GameManager> orig, GameManager self)
        {
			orig(self);
			(self.GetType().GetField("BraveLevelLoadedListeners", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self) as List<Type>).Add(typeof(AdvancedGunBehaviour));
        }

        public void BraveOnLevelWasLoaded()
        {
			OnLevelLoadPreGeneration();
			if(Gun?.CurrentOwner != null && Gun.CurrentOwner is PlayerController player)
            {
				OnPlayerLevelLoadPreGeneration(player);
            }
			StartCoroutine(DelayedLoad());
        }

		private IEnumerator DelayedLoad()
        {
            while (Dungeon.IsGenerating)
            {
				yield return null;
            }
			OnLevelLoadPostGeneration();
			if (Gun?.CurrentOwner != null && Gun.CurrentOwner is PlayerController player)
			{
				OnPlayerLevelLoadPostGeneration(player);
			}
			yield break;
        }

		public virtual void OnLevelLoadPreGeneration()
        {
        }

		public virtual void OnPlayerLevelLoadPreGeneration(PlayerController player)
		{
		}

		public virtual void OnLevelLoadPostGeneration()
		{
		}

		public virtual void OnPlayerLevelLoadPostGeneration(PlayerController player)
		{
		}

        public virtual void InheritData(Gun sourceGun)
        {
        }

        public virtual void MidGameSerialize(List<object> data, int dataIndex)
        {
        }

		public T DeserializeObject<T>(List<object> data, ref int dataIndex)
        {
			T result = default;
			if(data[dataIndex] is T t)
            {
				result = t;
			}
			dataIndex++;
			return result;
        }

        public virtual void MidGameDeserialize(List<object> data, ref int dataIndex)
        {
        }

		[NonSerialized]
		public Gun Gun;
		public GameActor GenericOwner => Gun?.CurrentOwner;
		public PlayerController PlayerOwner => GenericOwner as PlayerController;
		public AIActor EnemyOwner => GenericOwner as AIActor;
		public bool EverPickedUp => (Gun?.HasBeenPickedUp).GetValueOrDefault();
	}
}
