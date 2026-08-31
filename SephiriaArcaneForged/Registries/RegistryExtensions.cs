using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.Registries
{
    public static class RegistryExtensions
    {
        public static T SetDamageId<T>(this T item) where T : ModArcaneWeapon
        {
            item.DamageId = ModDamageId.CreateArcaneWeapon(item.Name);
            return item;
        }
        public static T SetEffect<T>(this T item, string key) where T : ModArcaneWeapon
        {
            item.EffectString = new LocalizedString(key);
            return item;
        }
    }
}
