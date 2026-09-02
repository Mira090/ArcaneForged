using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace SephiriaArcaneForged.Registries
{
    public static class ArcaneWeaponDatabase
    {
        private static Dictionary<int, ArcaneWeaponEntity> weaponDictionary;

        public static void Initialize()
        {
            weaponDictionary = new Dictionary<int, ArcaneWeaponEntity>();
            Data.RegisterArcaneWeapons();
            foreach(var weapon in weaponDictionary.Values)
            {
                weapon.weapon = WeaponDatabase.FindWeaponById(weapon.id);
            }
        }

        public static void Destroy()
        {
            weaponDictionary = null;
        }

        public static ArcaneWeaponEntity FindWeaponById(WeaponEntity entity)
        {
            if (weaponDictionary.ContainsKey(entity.id))
            {
                return weaponDictionary[entity.id];
            }

            return null;
        }
        public static ArcaneWeaponEntity FindWeaponById(int id)
        {
            if (weaponDictionary.ContainsKey(id))
            {
                return weaponDictionary[id];
            }

            return null;
        }

        public static List<ArcaneWeaponEntity> GetAll()
        {
            return weaponDictionary.Values.Where(x => x.weapon != null).ToList();
        }
        public static void Register(ArcaneWeaponEntity weapon)
        {
            if (weapon == null)
            {
                Debug.LogWarning($"[{Core.ModName}] Arcane Weapon Register: weapon is null");
                return;
            }

            if (weaponDictionary.ContainsKey(weapon.id))
            {
                Debug.LogWarning(string.Format("[{0}] Arcane Weapon Register: id {1} already exists, use Modify()", Core.ModName, weapon.id));
                return;
            }

            weaponDictionary[weapon.id] = weapon;
            Debug.Log(string.Format("[{0}] Arcane Weapon registered: {1}", Core.ModName, weapon.id));
        }

        public static void Modify(int id, Action<ArcaneWeaponEntity> modifier)
        {
            if (weaponDictionary.TryGetValue(id, out var value))
            {
                try
                {
                    modifier(value);
                    return;
                }
                catch (Exception arg)
                {
                    Debug.LogError(string.Format("[{0}] Arcane Weapon Modify {1} failed: {2}", Core.ModName, id, arg));
                    return;
                }
            }

            Debug.LogWarning(string.Format("[{0}] Arcane Weapon Modify: id {1} not found", Core.ModName, id));
        }
        public static EnhancementMetadata GetRandomEnhancement(Random random, WeaponEntity current, WeaponEntity[] alreadyList)
        {
            var weapons = GetAll().Where(x => alreadyList == null || alreadyList.Length == 0 || !alreadyList.Contains(x.weapon)).Where(x => current == null || x.id != current.id).ToList();
            if (weapons.Count == 0)
                return new EnhancementMetadata();
            var index = random.Next(0, weapons.Count);
            return new EnhancementMetadata()
            {
                enhanced = weapons[index].weapon
            };
        }
    }
}
