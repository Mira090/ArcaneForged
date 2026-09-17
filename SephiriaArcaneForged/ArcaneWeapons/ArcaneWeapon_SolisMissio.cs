using Mirror;
using Mirror.RemoteCalls;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_SolisMissio : ArcaneWeapon_StatusInstance
    {
        [Header("Flame Sword : Solis Missio")]
        public bool isSolisMissioEnabled = true;

        public int solisMissioType = 1;

        public float throwSwordInterval = 0.36f;

        private float throwSwordIntervalTimer;

        public GameObject[] solisMissioFxPrefabs => SephiriaPrefabs.SolisMissioFxPrefabs;

        private int solisMissioFxIndex;

        public Vector2 solisMissioFxOffset = new Vector2(0, -0.3438f);

        public string solisMissioHUD_ID = "SOLISMISSIO";

        [SyncVar(hook = "OnSolisMissioStackChanged")]
        public int solisMissioStack;

        private float solisMissioActiveTime;

        private float solisMissioActiveTimeMax;

        private bool canChangeSolisMissioActiveTimeCurrentFrame;

        private bool onAppliedSolisMissioGuardSpeed;

        public Action<int, int> _Mirror_SyncVarHookDelegate_solisMissioStack;

        public bool UsingSolisMissioStack
        {
            get => NetworkAvatar && !NetworkAvatar.IsDead;
        }

        public int NetworksolisMissioStack
        {
            get
            {
                return solisMissioStack;
            }
            [param: In]
            set
            {
                GeneratedSyncVarSetter(value, ref solisMissioStack, 4L, _Mirror_SyncVarHookDelegate_solisMissioStack);
            }
        }

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnGuardSucceeded += OnGuardSucceeded;
            NetworkAvatar.OnParry += OnParry;
            NetworkAvatar.OnCounter += OnCounter;

            WeaponController.OnBasicAttack += OnBasicAttack;
            WeaponController.OnSpecialAttack += OnSpecialAttack;
            WeaponController.OnBeginAttackAnimation += StartBasicAttackAnimation;
            WeaponController.OnBeginSpecialAttackAnimation += StartSpecialAttackAnimation;

            if (isSolisMissioEnabled && solisMissioType == 0)
            {
                NetworkAvatar.OnUniversalBattleCallback += HandleUniversalBattleCallback;
            }
            if (isSolisMissioEnabled && solisMissioType == 1)
            {
                NetworkAvatar.CreateEffectHUD(solisMissioHUD_ID, GetWeaponHUDID() + "_SOLISMISSIO");
            }
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnGuardSucceeded -= OnGuardSucceeded;
            NetworkAvatar.OnParry -= OnParry;
            NetworkAvatar.OnCounter -= OnCounter;

            WeaponController.OnBasicAttack -= OnBasicAttack;
            WeaponController.OnSpecialAttack -= OnSpecialAttack;
            WeaponController.OnBeginAttackAnimation -= StartBasicAttackAnimation;
            WeaponController.OnBeginSpecialAttackAnimation -= StartSpecialAttackAnimation;

            if (isSolisMissioEnabled && solisMissioType == 0)
            {
                NetworkAvatar.OnUniversalBattleCallback -= HandleUniversalBattleCallback;
            }
            if (isSolisMissioEnabled && solisMissioType == 1)
            {
                NetworkAvatar.DestroyEffectHUD(GetWeaponHUDID() + "_SOLISMISSIO");
            }
        }

        private void OnCounter(DamageInstance damage)
        {
            OnGuardSucceeded(damage, true);
        }

        private void OnParry(DamageInstance damage)
        {
            OnGuardSucceeded(damage, true);
        }

        private void OnGuardSucceeded(DamageInstance damage, bool perfectGuard)
        {
            if (perfectGuard)
            {
                if (isSolisMissioEnabled)
                {
                    if (solisMissioType == 0)
                    {
                        if (NetworkAvatar && NetworkAvatar.Inventory)
                        {
                            ComboEffectBase comboEffectBase = NetworkAvatar.Inventory.FindComboEffect(ItemCategories.FlameSword);
                            if (comboEffectBase != null && comboEffectBase.isEnabled && comboEffectBase is ComboEffect_FlameSword comboEffect_FlameSword)
                            {
                                int num = comboEffect_FlameSword.ReturnSwordServerside(0.5f);
                                if (num > 0)
                                {
                                    int constValue2 = KeywordDatabase.GetConstValue("solisMissioMaxStack");
                                    if (solisMissioStack + num > constValue2)
                                    {
                                        NetworksolisMissioStack = constValue2;
                                    }
                                    else
                                    {
                                        NetworksolisMissioStack = solisMissioStack + num;
                                    }
                                }
                            }
                        }
                    }
                    else if (solisMissioType == 1)
                    {
                        AddSolisMissioActiveTime(KeywordDatabase.GetConstValue("solisMissioPerfectGuardAddTime"));
                    }
                }
            }
        }

        private void OnSolisMissioStackChanged(int oldValue, int newValue)
        {
            if (0 < newValue)
            {
                if (oldValue < newValue)
                {
                    NetworkAvatar.SetEffectHUDFlash(GetWeaponHUDID() + "_SOLISMISSIO");
                }

                NetworkAvatar.CreateEffectHUD(solisMissioHUD_ID, GetWeaponHUDID() + "_SOLISMISSIO");
                NetworkAvatar.SetEffectHUDValue(GetWeaponHUDID() + "_SOLISMISSIO", newValue.ToString());
            }
            else
            {
                NetworkAvatar.DestroyEffectHUD(GetWeaponHUDID() + "_SOLISMISSIO");
            }
        }
        [ServerCallback]
        protected void Update()
        {
            if (!IsEffectEnabled)
                return;

            if (!NetworkServer.active)
            {
                return;
            }

            if (!isSolisMissioEnabled)
            {
                return;
            }

            /*
            if (NetworkAvatar)
            {
                float num3 = (float)KeywordDatabase.GetConstValue("solisMissioDecreaseMoveSpeedOnGuard") / 100f;
                if (isGuardAnimationTurnedOn)
                {
                    if (!onAppliedSolisMissioGuardSpeed)
                    {
                        onAppliedSolisMissioGuardSpeed = true;
                        NetworkAvatar.NetworkmoveSpeedMultiplier -= num3;
                    }
                }
                else if (onAppliedSolisMissioGuardSpeed)
                {
                    onAppliedSolisMissioGuardSpeed = false;
                    NetworkAvatar.NetworkmoveSpeedMultiplier += num3;
                }
            }*/

            bool flag = false;
            if (solisMissioType == 0)
            {
                flag = solisMissioStack > 0 && UsingSolisMissioStack;
            }
            else if (solisMissioType == 1)
            {
                flag = solisMissioActiveTime > 0f && UsingSolisMissioStack;
            }

            if (flag)
            {
                float num4 = 1f;
                if (solisMissioType == 0)
                {
                    num4 += (float)solisMissioStack * ((float)KeywordDatabase.GetConstValue("solisMissioThrowSwordIntervalBonusByStack") / 100f);
                }

                throwSwordIntervalTimer += Time.deltaTime * num4;
                if (throwSwordIntervalTimer >= throwSwordInterval)
                {
                    if (NetworkAvatar && NetworkAvatar.Inventory)
                    {
                        ComboEffectBase comboEffectBase = NetworkAvatar.Inventory.FindComboEffect(ItemCategories.FlameSword);
                        if (comboEffectBase != null && comboEffectBase.isEnabled && comboEffectBase is ComboEffect_FlameSword comboEffect_FlameSword)
                        {
                            Vector3 vector = NetworkAvatar.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 3f;
                            int constValue3 = KeywordDatabase.GetConstValue("solisMissioThrowSwordDistance");
                            UnitAvatar unitAvatar3 = PlayerInputController.SearchTargetNearestPoint(NetworkAvatar, NetworkAvatar.transform.position, constValue3 * constValue3);
                            if (unitAvatar3 != null)
                            {
                                vector = unitAvatar3.transform.position;
                            }

                            Vector3 motionFrom = vector + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * 2.5f;
                            if (comboEffect_FlameSword.FireSword(motionFrom, vector))
                            {
                                if (solisMissioType == 0)
                                {
                                    NetworksolisMissioStack = solisMissioStack - 1;
                                }

                                RpcCreateSolisMissioFx();
                            }
                        }
                    }

                    throwSwordIntervalTimer = 0f;
                }
            }

            if (solisMissioType != 1)
            {
                return;
            }

            if (solisMissioActiveTime > 0f)
            {
                if (flag)
                {
                    solisMissioActiveTime -= Time.deltaTime;
                    if (solisMissioActiveTime <= 0f)
                    {
                        solisMissioActiveTime = 0f;
                        solisMissioActiveTimeMax = 0f;
                    }
                }

                if (solisMissioActiveTimeMax > Mathf.Epsilon)
                {
                    NetworkAvatar.SetEffectHUDFillAmount(GetWeaponHUDID() + "_SOLISMISSIO", 1f - solisMissioActiveTime / solisMissioActiveTimeMax);
                    NetworkAvatar.SetEffectHUDValue(GetWeaponHUDID() + "_SOLISMISSIO", solisMissioActiveTime.ToString("F1"));
                }
                else
                {
                    NetworkAvatar.SetEffectHUDFillAmount(GetWeaponHUDID() + "_SOLISMISSIO", 1f);
                    NetworkAvatar.SetEffectHUDValue(GetWeaponHUDID() + "_SOLISMISSIO", "-");
                }
            }
            else
            {
                NetworkAvatar.SetEffectHUDFillAmount(GetWeaponHUDID() + "_SOLISMISSIO", 1f);
                NetworkAvatar.SetEffectHUDValue(GetWeaponHUDID() + "_SOLISMISSIO", "-");
            }
        }
        public void StartBasicAttackAnimation(int idx)
        {
            if (isSolisMissioEnabled && solisMissioType == 1)
            {
                canChangeSolisMissioActiveTimeCurrentFrame = true;
            }
        }

        public void StartSpecialAttackAnimation(int idx)
        {
            if (isSolisMissioEnabled && solisMissioType == 1)
            {
                canChangeSolisMissioActiveTimeCurrentFrame = true;
            }
        }
        private void HandleUniversalBattleCallback(string message, int param)
        {
            if (message == "ADDFLAMESWORD" && param > 0)
            {
                int constValue = KeywordDatabase.GetConstValue("solisMissioMaxStack");
                if (solisMissioStack + param > constValue)
                {
                    NetworksolisMissioStack = constValue;
                }
                else
                {
                    NetworksolisMissioStack = solisMissioStack + param;
                }
            }
        }

        [ClientRpc]
        private void RpcCreateSolisMissioFx()
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            var func = "System.Void ArcaneWeapon_SolisMissio::RpcCreateSolisMissioFx()";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }

        private void OnBasicAttack(CombatBehaviour victim, DamageInstance damage, ProjectileBase projectile)
        {
            if (isSolisMissioEnabled && solisMissioType == 1 && canChangeSolisMissioActiveTimeCurrentFrame)
            {
                AddSolisMissioActiveTime((float)KeywordDatabase.GetConstValue("solisMissioBasicAttackAddTime_div100") * 0.01f);
                canChangeSolisMissioActiveTimeCurrentFrame = false;
            }
        }

        private void OnSpecialAttack(CombatBehaviour victim, DamageInstance damage, ProjectileBase projectile)
        {
            if (isSolisMissioEnabled && solisMissioType == 1 && canChangeSolisMissioActiveTimeCurrentFrame)
            {
                AddSolisMissioActiveTime(KeywordDatabase.GetConstValue("solisMissioSpecialAttackAddTime"));
                canChangeSolisMissioActiveTimeCurrentFrame = false;
            }
        }

        private void AddSolisMissioActiveTime(float time)
        {
            solisMissioActiveTime += time;
            solisMissioActiveTime = Mathf.Min(solisMissioActiveTime, KeywordDatabase.GetConstValue("solisMissioTimeLimit"));
            solisMissioActiveTimeMax = Mathf.Max(solisMissioActiveTime, solisMissioActiveTimeMax);
            NetworkAvatar.SetEffectHUDFlash(GetWeaponHUDID() + "_SOLISMISSIO");
            if (solisMissioActiveTimeMax > Mathf.Epsilon)
            {
                NetworkAvatar.SetEffectHUDFillAmount(GetWeaponHUDID() + "_SOLISMISSIO", 1f - solisMissioActiveTime / solisMissioActiveTimeMax);
            }
            else
            {
                NetworkAvatar.SetEffectHUDFillAmount(GetWeaponHUDID() + "_SOLISMISSIO", 1f);
            }
        }
        public ArcaneWeapon_SolisMissio()
        {
            _Mirror_SyncVarHookDelegate_solisMissioStack = OnSolisMissioStackChanged;
        }

        static ArcaneWeapon_SolisMissio()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(ArcaneWeapon_SolisMissio), "System.Void ArcaneWeapon_SolisMissio::RpcCreateSolisMissioFx()", InvokeUserCode_RpcCreateSolisMissioFx);
        }
        protected void UserCode_RpcCreateSolisMissioFx()
        {
            if (solisMissioFxPrefabs != null && solisMissioFxPrefabs.Length != 0 && WeaponController.currentWeapon.subWeapon)
            {
                GameObject key = solisMissioFxPrefabs.SafeRandomAccess(solisMissioFxIndex);
                solisMissioFxIndex++;
                if (solisMissioFxIndex >= solisMissioFxPrefabs.Length)
                {
                    solisMissioFxIndex = 0;
                }

                SpriteFx.Pool.Spawn(key, WeaponController.currentWeapon.subWeapon.position + (Vector3)solisMissioFxOffset).SetParent(WeaponController.currentWeapon.subWeapon);
            }
        }

        protected static void InvokeUserCode_RpcCreateSolisMissioFx(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcCreateSolisMissioFx called on server.");
            }
            else
            {
                ((ArcaneWeapon_SolisMissio)obj).UserCode_RpcCreateSolisMissioFx();
            }
        }
        protected override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
        {
            base.SerializeSyncVars(writer, forceAll);
            if (forceAll)
            {
                writer.WriteVarInt(solisMissioStack);
                return;
            }
            writer.WriteVarULong(syncVarDirtyBits);

            if ((syncVarDirtyBits & 0x4L) != 0L)
            {
                writer.WriteVarInt(solisMissioStack);
            }
        }
        protected override void DeserializeSyncVars(NetworkReader reader, bool initialState)
        {
            base.DeserializeSyncVars(reader, initialState);
            if (initialState)
            {
                GeneratedSyncVarDeserialize(ref solisMissioStack, _Mirror_SyncVarHookDelegate_solisMissioStack, reader.ReadVarInt());
                return;
            }
            long num = (long)reader.ReadVarULong();

            if ((num & 0x4L) != 0L)
            {
                GeneratedSyncVarDeserialize(ref solisMissioStack, _Mirror_SyncVarHookDelegate_solisMissioStack, reader.ReadVarInt());
            }
        }
    }
}
