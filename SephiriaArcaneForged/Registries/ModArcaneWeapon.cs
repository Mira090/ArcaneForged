using SephiriaArcaneForged.ArcaneWeapons;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.Registries
{
    public class ModArcaneWeapon
    {
        public static ModArcaneWeapon Create<T>(string name, int weaponId) where T : ArcaneWeapon_Basic
        {
            return new ModArcaneWeapon()
            {
                Name = name,
                ResourcePrefabName = $"ArcaneWeapon-{name}",
                AffixString = new LocalizedString($"ArcaneWeapon_{name}_Affix"),
                EffectString = new LocalizedString($"ArcaneWeapon_{name}_Effect"),
                Id = weaponId,
                ArcaneWeaponType = typeof(T)
            };
        }
        public static ModArcaneWeapon Create(string name, int weaponId)
            => Create<ArcaneWeapon_Basic>(name, weaponId);
        public static ModArcaneWeapon CreateFlag<T>(string name, int weaponId, string stat, int value = 1) where T : ArcaneWeapon_Flag
        {
            return new ModArcaneWeapon()
            {
                Name = name,
                ResourcePrefabName = $"ArcaneWeapon-{name}",
                AffixString = new LocalizedString($"ArcaneWeapon_{name}_Affix"),
                EffectString = new LocalizedString($"ArcaneWeapon_{name}_Effect"),
                Id = weaponId,
                Stat = stat,
                Value = value,
                ArcaneWeaponType = typeof(T)
            };
        }
        public static ModArcaneWeapon CreateFlag(string name, int weaponId, string stat, int value = 1)
            => CreateFlag<ArcaneWeapon_Flag>(name, weaponId, stat, value);
        public static ModArcaneWeapon CreateDebuff<T>(string name, int weaponId, string debuff, int percent = 100, params string[] stats) where T : ArcaneWeapon_ApplyDebuff
        {
            return new ModArcaneWeapon()
            {
                Name = name,
                ResourcePrefabName = $"ArcaneWeapon-{name}",
                AffixString = new LocalizedString($"ArcaneWeapon_{name}_Affix"),
                EffectString = new LocalizedString($"ArcaneWeapon_{name}_Effect"),
                Id = weaponId,
                Debuff = debuff,
                Percent = percent,
                Stats = stats,
                ArcaneWeaponType = typeof(T)
            };
        }
        public static ModArcaneWeapon CreateDebuff(string name, int weaponId, string debuff, int percent = 100, params string[] stats)
            => CreateDebuff<ArcaneWeapon_ApplyDebuff>(name, weaponId, debuff, percent, stats);
        public static ModArcaneWeapon CreateDebuffFlag<T>(string name, int weaponId, string stat, int value, string debuff, int percent = 100, params string[] stats) where T : ArcaneWeapon_ApplyDebuff
        {
            return new ModArcaneWeapon()
            {
                Name = name,
                ResourcePrefabName = $"ArcaneWeapon-{name}",
                AffixString = new LocalizedString($"ArcaneWeapon_{name}_Affix"),
                EffectString = new LocalizedString($"ArcaneWeapon_{name}_Effect"),
                Id = weaponId,
                Stat = stat,
                Value = value,
                Debuff = debuff,
                Percent = percent,
                Stats = stats,
                ArcaneWeaponType = typeof(T)
            };
        }
        public static ModArcaneWeapon CreateDebuffFlag(string name, int weaponId, string stat, int value, string debuff, int percent = 100, params string[] stats)
            => CreateDebuffFlag<ArcaneWeapon_ApplyDebuff>(name, weaponId, stat, value, debuff, percent, stats);
        public static ModArcaneWeapon CreateStatsFlag(string name, int weaponId, string stat, int value, params string[] stats)
            => CreateDebuffFlag(name, weaponId, stat, value, string.Empty, 0, stats);
        public static ModArcaneWeapon CreateStats<T>(string name, int weaponId, params string[] stats) where T : ArcaneWeapon_StatusInstance
        {
            return new ModArcaneWeapon()
            {
                Name = name,
                ResourcePrefabName = $"ArcaneWeapon-{name}",
                AffixString = new LocalizedString($"ArcaneWeapon_{name}_Affix"),
                EffectString = new LocalizedString($"ArcaneWeapon_{name}_Effect"),
                Id = weaponId,
                Stats = stats,
                ArcaneWeaponType = typeof(T)
            };
        }
        public static ModArcaneWeapon CreateStats(string name, int weaponId, params string[] stats)
            => CreateStats<ArcaneWeapon_StatusInstance>(name, weaponId, stats);

        public ArcaneWeaponEntity ArcaneWeaponEntity { get; internal set; }
        public string Name { get; internal set; }
        public string ResourcePrefabName { get; internal set; }
        public LocalizedString AffixString { get; internal set; }
        public LocalizedString EffectString { get; internal set; }
        public Type ArcaneWeaponType { get; internal set; }
        public int Id { get; internal set; }
        public uint AssetId { get; internal set; }
        public string[] Stats { get; internal set; }
        public string Stat { get; internal set; }
        public int Value { get; internal set; }
        public string Debuff { get; internal set; }
        public int Percent { get; internal set; }
        public GameObject ResourcePrefab => _resourcePrefab;
        protected GameObject _resourcePrefab;
        public virtual GameObject CreateResourcePrefab()
        {
            var o = new GameObject(ResourcePrefabName);
            var arcane = o.AddComponent(ArcaneWeaponType) as ArcaneWeapon_Basic;
            //Core.Logger($"CreateArcaneWeapon");
            o.hideFlags = HideFlags.HideAndDontSave;
            o.SetAssetId(AssetId);
            arcane.effectsString = EffectString;
            arcane.entityID = Id;
            if(arcane is ArcaneWeapon_StatusInstance status)
            {
                status.stats = Stats;
            }
            if(arcane is ArcaneWeapon_Flag flag)
            {
                flag.stat = Stat;
                flag.value = Value;
            }
            else if (arcane is ArcaneWeapon_ApplyDebuff debuff)
            {
                debuff.stat = Stat;
                debuff.value = Value;
                debuff.debuff = Debuff;
                debuff.percent = Percent;
            }
            arcane.enabled = false;
            return o;
        }
        public virtual void Init(uint assetId)
        {
            AssetId = assetId;
            _resourcePrefab = CreateResourcePrefab();
            ArcaneWeaponEntity = CreateEntity();
        }
        protected virtual ArcaneWeaponEntity CreateInstance()
        {
            return ScriptableObject.CreateInstance<ArcaneWeaponEntity>();
        }
        public ArcaneWeaponEntity CreateEntity()
        {
            var entity = CreateInstance();
            entity.name = Id + "_Arcane_" + Name;
            entity.id = Id;
            entity.resourcePrefab = ResourcePrefab;
            entity.affix = AffixString;
            return entity;
        }
        public virtual void Dispose()
        {
            if (ArcaneWeaponEntity != null)
                ScriptableObject.Destroy(ArcaneWeaponEntity);
            if (_resourcePrefab != null)
                GameObject.Destroy(_resourcePrefab);
        }
    }
}
