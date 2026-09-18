using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_Nanmu : ArcaneWeapon_Basic
    {

        [Header("Speed Buff (Nanmu)")]
        public bool useNanmu = true;

        public Timer nanmuEndTimer = new Timer(8f);

        private bool isNanmuActive;

        public GameObject nanmuStartFxPrefab => SephiriaPrefabs.NanmuStartFxPrefab;

        public StudioEventEmitter nanmuStartSoundEvent;

        public NewWeaponFireData[] nanmuExtraSwingFireDatas => SephiriaPrefabs.NanmuExtraSwingFireDatas;

        public float nanmuExtraSwingDuration = 0.2f;

        private int nanmuExtraSwingCount;

        private List<float> nanmuExtraSwingTimers = new List<float>();

        [Header("Katana Gauge")]
        public float currentKatanaGauge = 0f;

        public bool isFilledGaugeCurrentFrame;

        public float attackToFillGaugeValue = 0.1f;

        public Timer katanaGaugeDisappearDelayTimer = new Timer(2f, resetOnTime: false);

        public float katanaGaugeDisappearSpeed = 0.1f;

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnBaisAttackSwing += OnBaisAttackSwing;
            WeaponController.OnBasicAttack += OnBasicAttack;
            //WeaponController.OnDashAttack += OnDashAttack;
            NetworkAvatar.CreateEffectHUD("KATANAGAUGE", "KatanaGaugeNanmu");
            NetworkAvatar.SetEffectHUDFillAmount("KatanaGaugeNanmu", 1 - currentKatanaGauge);
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnBaisAttackSwing -= OnBaisAttackSwing;
            WeaponController.OnBasicAttack -= OnBasicAttack;
            //WeaponController.OnDashAttack -= OnDashAttack;
            NetworkAvatar.DestroyEffectHUD("KatanaNanmu");
            NetworkAvatar.DestroyEffectHUD("KatanaGaugeNanmu");
        }

        private void Update()
        {
            if (!base.isServer || !IsEffectEnabled)
                return;

            if (currentKatanaGauge > 0f && katanaGaugeDisappearDelayTimer.Update(Time.deltaTime))
            {
                currentKatanaGauge -= katanaGaugeDisappearSpeed * Time.deltaTime;
                if (currentKatanaGauge < 0f)
                {
                    currentKatanaGauge = 0f;
                }
                NetworkAvatar.SetEffectHUDFillAmount("KatanaGaugeNanmu", 1 - currentKatanaGauge);
            }


            if (useNanmu && isNanmuActive)
            {
                NetworkAvatar.SetEffectHUDFillAmount("KatanaNanmu", nanmuEndTimer.Ratio);
                float deltaTime = Time.deltaTime;
                float num2 = 1f + (float)NetworkAvatar.GetCustomStat(ECustomStat.BuffDuration) / 100f;
                if (nanmuEndTimer.Update(deltaTime / num2))
                {
                    isNanmuActive = false;
                    //finalComboIdx = 2;
                    //attackMoveSet = 0;
                    //SetAttackMoveSet(0);
                    NetworkAvatar.DestroyEffectHUD("KatanaNanmu");
                }
            }

            if (nanmuExtraSwingTimers.Count > 0)
            {
                int customStatUnsafe = NetworkAvatar.GetCustomStatUnsafe("ATTACKSPEED");
                float num5 = 1f + (float)customStatUnsafe / 100f;
                for (int i = 0; i < nanmuExtraSwingTimers.Count; i++)
                {
                    nanmuExtraSwingTimers[i] -= Time.deltaTime * num5;
                    if (nanmuExtraSwingTimers[i] <= 0f)
                    {
                        Vector3 attackDirection = WeaponController.attackDirection;
                        int num6 = nanmuExtraSwingCount % nanmuExtraSwingFireDatas.Length;
                        if (WeaponController.currentWeapon)
                        {
                            WeaponController.currentWeapon.CreateBasicAttackProjectileFullyManual(nanmuExtraSwingFireDatas[num6], 0, WeaponController.shoulder.swingPoint.position, attackDirection, null, 1f, 0);
                        }
                        RpcCreateNanmuExtraSwingFx(num6);
                        nanmuExtraSwingCount++;
                        nanmuExtraSwingTimers.RemoveAt(i--);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            isFilledGaugeCurrentFrame = false;
        }

        private void OnBasicAttack(CombatBehaviour victim, DamageInstance damage, ProjectileBase projectile)
        {
            if (isFilledGaugeCurrentFrame || isNanmuActive)
            {
                return;
            }

            isFilledGaugeCurrentFrame = true;
            currentKatanaGauge += attackToFillGaugeValue;
            if (currentKatanaGauge > 1f)
            {
                currentKatanaGauge = 1f;
                if (useNanmu && !isNanmuActive)
                {
                    ActivateNanmu();
                }
            }
            NetworkAvatar.SetEffectHUDFillAmount("KatanaGaugeNanmu", 1 - currentKatanaGauge);

            katanaGaugeDisappearDelayTimer.Ratio = 0f;
        }

        private void OnDashAttack(CombatBehaviour victim, DamageInstance damage, ProjectileBase projectile)
        {
            if (isFilledGaugeCurrentFrame || isNanmuActive)
            {
                return;
            }

            isFilledGaugeCurrentFrame = true;
            currentKatanaGauge += attackToFillGaugeValue;
            if (currentKatanaGauge > 1f)
            {
                currentKatanaGauge = 1f;
                if (useNanmu && !isNanmuActive)
                {
                    ActivateNanmu();
                }
            }
            NetworkAvatar.SetEffectHUDFillAmount("KatanaGaugeNanmu", 1 - currentKatanaGauge);

            katanaGaugeDisappearDelayTimer.Ratio = 0f;
        }

        private void OnBaisAttackSwing(int idx)
        {
            if (base.isServer && useNanmu && isNanmuActive)
            {
                nanmuExtraSwingTimers.Add(nanmuExtraSwingDuration);
            }
        }
        private void ActivateNanmu()
        {
            isNanmuActive = true;
            NetworkAvatar.CreateEffectHUD("KATANANANMU", "KatanaNanmu");
            //finalComboIdx = 3;
            //attackMoveSet = 2;
            //SetAttackMoveSet(2);
            nanmuEndTimer.SetTimer(0f);
            RpcNanmuStartFx();
            currentKatanaGauge = 0f;
        }

        [ClientRpc]
        private void RpcNanmuStartFx()
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            var func = "System.Void ArcaneWeapon_Nanmu::RpcNanmuStartFx()";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }

        [ClientRpc]
        private void RpcCreateNanmuExtraSwingFx(int idx)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteVarInt(idx);
            var func = "System.Void ArcaneWeapon_Nanmu::RpcCreateNanmuExtraSwingFx(System.Int32)";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }
        protected void UserCode_RpcNanmuStartFx()
        {
            if ((bool)nanmuStartFxPrefab)
            {
                SpriteFx.Pool.Spawn(nanmuStartFxPrefab, NetworkAvatar.transform.position);
            }

            if ((bool)nanmuStartSoundEvent)
            {
                nanmuStartSoundEvent.Play();
            }
        }

        protected static void InvokeUserCode_RpcNanmuStartFx(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcNanmuStartFx called on server.");
            }
            else
            {
                ((ArcaneWeapon_Nanmu)obj).UserCode_RpcNanmuStartFx();
            }
        }

        protected void UserCode_RpcCreateNanmuExtraSwingFx__Int32(int idx)
        {
            if(NetworkAvatar.gameObject.TryGetComponent<WeaponControllerSimple>(out var controller) && controller.currentWeapon)
            {
                controller.currentWeapon.CreateBasicAttackSwingFxFullyManual(nanmuExtraSwingFireDatas[idx]);
            }
        }

        protected static void InvokeUserCode_RpcCreateNanmuExtraSwingFx__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcCreateNanmuExtraSwingFx called on server.");
            }
            else
            {
                ((ArcaneWeapon_Nanmu)obj).UserCode_RpcCreateNanmuExtraSwingFx__Int32(reader.ReadVarInt());
            }
        }

        static ArcaneWeapon_Nanmu()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(ArcaneWeapon_Nanmu), "System.Void ArcaneWeapon_Nanmu::RpcNanmuStartFx()", InvokeUserCode_RpcNanmuStartFx);
            RemoteProcedureCalls.RegisterRpc(typeof(ArcaneWeapon_Nanmu), "System.Void ArcaneWeapon_Nanmu::RpcCreateNanmuExtraSwingFx(System.Int32)", InvokeUserCode_RpcCreateNanmuExtraSwingFx__Int32);
        }
    }
}
