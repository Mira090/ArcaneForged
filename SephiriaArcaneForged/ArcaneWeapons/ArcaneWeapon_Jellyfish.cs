using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_Jellyfish : ArcaneWeapon_Flags
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (!NetworkAvatar || !NetworkAvatar.Inventory)
                return;

            foreach (KeyValuePair<ItemPosition, Charm_Basic> charm in NetworkAvatar.Inventory.charms)
            {
                if ((bool)charm.Value && charm.Value is Charm_SummonUnit { isJellyfish: true } charm_SummonUnit)
                {
                    charm_SummonUnit.UpdateUnitDamageServer();
                }
            }
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (!NetworkAvatar || !NetworkAvatar.Inventory)
                return;

            foreach (KeyValuePair<ItemPosition, Charm_Basic> charm in NetworkAvatar.Inventory.charms)
            {
                if ((bool)charm.Value && charm.Value is Charm_SummonUnit { isJellyfish: true } charm_SummonUnit)
                {
                    charm_SummonUnit.UpdateUnitDamageServer();
                }
            }
        }
    }
}
