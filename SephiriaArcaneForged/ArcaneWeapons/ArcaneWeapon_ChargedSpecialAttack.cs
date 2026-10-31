using Mirror;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_ChargedSpecialAttack : ArcaneWeapon_StatusInstance
    {
        public int stat = 0;

        [SyncVar(hook = "HookChargedSweep_New_Stack")]
        public int chargedSweep_New_Stack;

        private bool chargedSweep_New_Used;

        private float chargedSweep_New_Timer;

        public string chargedSweep_New_HUD_ID = "CHARGEDSWEEP";

        public int chargedSweepTime = 5;


        public GameObject chargeFxPrefab => SephiriaPrefabs.ChargeFxPrefab;

        private SpriteFx currentChargingFx;

        public Action<int, int> _Mirror_SyncVarHookDelegate_chargedSweep_New_Stack;

        public bool HasChargedSweep => chargedSweep_New_Stack > 0;

        public int NetworkchargedSweep_New_Stack
        {
            get
            {
                return chargedSweep_New_Stack;
            }
            [param: In]
            set
            {
                GeneratedSyncVarSetter(value, ref chargedSweep_New_Stack, 4L, _Mirror_SyncVarHookDelegate_chargedSweep_New_Stack);
            }
        }

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.CreateEffectHUD(chargedSweep_New_HUD_ID, GetWeaponHUDID() + "_CHARGEDSWEEP");
            WeaponController.OnEndAttackAnimation += EndAttackAnimation;
            WeaponController.OnSwingCreated_Special += OnSpecialAttackCreated;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.DestroyEffectHUD(GetWeaponHUDID() + "_CHARGEDSWEEP");
            WeaponController.OnEndAttackAnimation -= EndAttackAnimation;
            WeaponController.OnSwingCreated_Special -= OnSpecialAttackCreated;

            if (currentChargingFx)
            {
                currentChargingFx.Despawn();
                currentChargingFx = null;
            }
        }
        [ServerCallback]
        protected void Update()
        {
            if (!NetworkServer.active)
                return;
            if (!IsEffectEnabled)
                return;

            if (WeaponController && NetworkAvatar && !chargedSweep_New_Used)
            {
                int constValue = KeywordDatabase.GetConstValue("chargedSweepMaxStack");
                if (chargedSweep_New_Stack < constValue)
                {
                    int constValue2 = chargedSweepTime;//KeywordDatabase.GetConstValue("chargedSweepTime");
                    float num = 1f;
                    int customStatUnsafe = NetworkAvatar.GetCustomStatUnsafe("ATTACKSPEED");
                    if (customStatUnsafe > 0)
                    {
                        float num2 = (float)KeywordDatabase.GetConstValue("chargedSweepAttackSpeedEfficiency") / 100f;
                        num += (float)customStatUnsafe / 100f * num2;
                    }

                    chargedSweep_New_Timer += Time.deltaTime * num;
                    if (chargedSweep_New_Timer >= (float)constValue2)
                    {
                        NetworkchargedSweep_New_Stack = chargedSweep_New_Stack + 1;
                        chargedSweep_New_Timer = 0f;
                        NetworkAvatar.SetEffectHUDFlash(GetWeaponHUDID() + "_CHARGEDSWEEP");
                        NetworkAvatar.SetEffectHUDValue(GetWeaponHUDID() + "_CHARGEDSWEEP", chargedSweep_New_Stack.ToString());

                        if(NetworkchargedSweep_New_Stack > 0 && stat == 0)
                        {
                            stat = 100;
                            NetworkAvatar.AddCustomStat(ECustomStat.SpecialAttackRange, stat);
                        }
                    }

                    if (chargedSweep_New_Stack == constValue)
                    {
                        NetworkAvatar.SetEffectHUDFillAmount(GetWeaponHUDID() + "_CHARGEDSWEEP", 0f);
                    }
                    else
                    {
                        NetworkAvatar.SetEffectHUDFillAmount(GetWeaponHUDID() + "_CHARGEDSWEEP", 1f - chargedSweep_New_Timer / (float)constValue2);
                    }
                }
            }
        }


        public void EndAttackAnimation()
        {
            if (chargedSweep_New_Used)
            {
                ClearStat();
                NetworkchargedSweep_New_Stack = 0;
            }

            chargedSweep_New_Used = false;
        }

        public void CancelWeaponAction()
        {
            if (chargedSweep_New_Used)
            {
                ClearStat();
                NetworkchargedSweep_New_Stack = 0;
            }

            chargedSweep_New_Used = false;
        }

        private void ClearStat()
        {
            NetworkAvatar.AddCustomStat(ECustomStat.SpecialAttackRange, -stat);
            stat = 0;
        }


        private void HookChargedSweep_New_Stack(int oldValue, int newValue)
        {
            if (newValue > 0)
            {
                if (currentChargingFx == null)
                {
                    currentChargingFx = SpriteFx.Pool.Spawn(chargeFxPrefab, WeaponController.currentWeapon.mainWeaponBody.hiltArea.transform.position);
                    currentChargingFx.SetParent(WeaponController.currentWeapon.mainWeaponBody.hiltArea.transform);
                }
            }
            else if ((bool)currentChargingFx)
            {
                currentChargingFx.Despawn();
                currentChargingFx = null;
            }
        }
        protected void OnSpecialAttackCreated(ProjectileBase projectile)
        {
            if (HasChargedSweep)
            {
                projectile.additionalDamagePercent += KeywordDatabase.GetConstValue("chargedSweepDamageBonusPercent") * chargedSweep_New_Stack;
                chargedSweep_New_Used = true;
                chargedSweep_New_Timer = 0f;
                NetworkAvatar.SetEffectHUDFillAmount(GetWeaponHUDID() + "_CHARGEDSWEEP", 1f);
                NetworkAvatar.SetEffectHUDValue(GetWeaponHUDID() + "_CHARGEDSWEEP", "0");
            }
        }
        public ArcaneWeapon_ChargedSpecialAttack()
        {
            _Mirror_SyncVarHookDelegate_chargedSweep_New_Stack = HookChargedSweep_New_Stack;
        }
        protected override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
        {
            base.SerializeSyncVars(writer, forceAll);
            if (forceAll)
            {
                writer.WriteVarInt(chargedSweep_New_Stack);
                return;
            }
            writer.WriteVarULong(syncVarDirtyBits);

            if ((syncVarDirtyBits & 0x4L) != 0L)
            {
                writer.WriteVarInt(chargedSweep_New_Stack);
            }
        }
        protected override void DeserializeSyncVars(NetworkReader reader, bool initialState)
        {
            base.DeserializeSyncVars(reader, initialState);
            if (initialState)
            {
                GeneratedSyncVarDeserialize(ref chargedSweep_New_Stack, _Mirror_SyncVarHookDelegate_chargedSweep_New_Stack, reader.ReadVarInt());
                return;
            }
            long num = (long)reader.ReadVarULong();

            if ((num & 0x4L) != 0L)
            {
                GeneratedSyncVarDeserialize(ref chargedSweep_New_Stack, _Mirror_SyncVarHookDelegate_chargedSweep_New_Stack, reader.ReadVarInt());
            }
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = base.BuildKeywords().ToList();
            list.Add(new Loc.KeywordValue("COOLDOWN", chargedSweepTime.ToString()));
            return list.ToArray();
        }
    }
}
