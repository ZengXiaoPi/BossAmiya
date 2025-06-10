using System;
using System.Collections.Generic;
using UnityEngine;
using WorkerSprite;

namespace BossAmiya
{
    public class BossAmiya_weapon : EquipmentScriptBase, IObserver
    {
        public override WeaponDamageInfo OnAttackStart(UnitModel actor, UnitModel target)
        {
            List<DamageInfo> list = [];
            string animationName = string.Empty;
            AgentModel agent = this.model.owner as AgentModel;
            list.Clear();
            list.Add(base.model.metaInfo.damageInfos[0].Copy());
            animationName = base.model.metaInfo.animationNames[0];
            WeaponDamageInfo result;
            if (agent.GetUnitBufByName("Amiya_MentalCrash") != null)
            {
                DamageInfo damageInfo = base.model.metaInfo.damageInfos[1].Copy();
                damageInfo.param = "MentalCrash";
                list.Clear();
                list.Add(damageInfo);
                animationName = base.model.metaInfo.animationNames[1];
                result = new WeaponDamageInfo(animationName, list.ToArray());
            }
            else if (!upgradeAttack)
            {
                DamageInfo damageInfo = base.model.metaInfo.damageInfos[0].Copy();
                damageInfo.param = "MentalCrash";
                list.Clear();
                list.Add(damageInfo);
                animationName = base.model.metaInfo.animationNames[0];
                result = new WeaponDamageInfo(animationName, list.ToArray());
            }
            else
            {
                DamageInfo damageInfo = base.model.metaInfo.damageInfos[0].Copy();
                damageInfo.param = "UpgradeAttack";
                list.Clear();
                list.Add(damageInfo);
                animationName = base.model.metaInfo.animationNames[0];
                result = new WeaponDamageInfo(animationName, list.ToArray());
            }
            return result;
        }
        public override void OnGiveDamageAfter(UnitModel actor, UnitModel target, DamageInfo dmg)
        {
            base.OnGiveDamageAfter(actor, target, dmg);
            if (dmg.param == "MentalCrash")
            {
                this.model.owner.mental -= 2;
                if (this.model.owner.mental <= 2)
                {
                    Extension.RemoveBuff(this.model.owner, typeof(Amiya_MentalCrash));
                    isUsedMentalCrash = true;
                }
                return;
            }
            if (dmg.param == "UpgradeAttack")
            {
                target.TakeDamage(actor, new DamageInfo(RwbpType.N, this.model.owner.maxMental * 0.1f));
                this.model.owner.hp -= this.model.owner.maxHp * 0.02f;
            }
            if (this.model.owner.mental > this.model.owner.maxMental * 0.5f && UnityEngine.Random.Range(0, 100) < 5 && !isUsedMentalCrash)
            {
                AgentModel agent = this.model.owner as AgentModel;
                agent.GetWorkerUnit().animChanger.state.SetAnimation(0, "MentalCrash_Start", false);
                agent.AddUnitBuf(new Amiya_MentalCrash());
            }
            this.model.owner.mental -= this.model.owner.maxMental * 0.01f;
        }
        public override void OnStageRelease()
        {
            SetObserver(false);
            AgentModel agentModel = base.model.owner as AgentModel;
            agentModel.GetUnit().animEventHandler.SetAnimEvent(null);
            agentModel.GetUnit().animChanger.GetAnimator().UseState = false;
            agentModel.GetUnit().animChanger.ChangeAnimator();
            agentModel.GetUnit().spriteSetter.SymbolRenderer.enabled = true;
            foreach (EGOgiftModel egogiftModel in agentModel.GetAllGifts())
            {
                if (egogiftModel != null)
                {
                    if (egogiftModel.metaInfo.attachType != EGOgiftAttachType.REPLACE)
                    {
                        EGOGiftRenderData egogiftRenderData = null;
                        if (agentModel.GetUnit().spriteSetter.TryGetGift(egogiftModel, out egogiftRenderData))
                        {
                            if (egogiftRenderData != null)
                            {
                                egogiftRenderData.renderer.enabled = true;
                            }
                        }
                    }
                }
            }
            agentModel.GetUnit().spriteSetter.SymbolRenderer.enabled = true;
            agentModel.GetUnit().spriteSetter.EyeRenderer.enabled = true;
            agentModel.GetUnit().spriteSetter.MouthRenderer.enabled = true;
            agentModel.GetUnit().spriteSetter.NoteRenderer.enabled = true;
            agentModel.GetUnit().spriteSetter.EyebrowRenderer.enabled = true;
            agentModel.GetUnit().spriteSetter.MouthReplaceGiftRender.enabled = true;
            agentModel.GetUnit().spriteSetter.panicRenderer.enabled = true;
            this.state = agentModel.GetUnit().animChanger.state;
        }
        public override void OnStageStart()
        {
            base.OnStageStart();
            SetObserver(true);
            diedModel.Clear();
            upgradeAttack = false;
            isUsedMentalCrash = false;
            try
            {
                AgentModel agentModel = base.model.owner as AgentModel;
                agentModel.GetUnit().animChanger.SetState(true);
                agentModel.GetUnit().animChanger.ChangeAnimator("BossBirdWeapon_BossAmiya");
                agentModel.GetUnit().spriteSetter.SymbolRenderer.enabled = false;
                foreach (EGOgiftModel egogiftModel in agentModel.GetAllGifts())
                {
                    if (egogiftModel != null)
                    {
                        if (egogiftModel.metaInfo.attachType != EGOgiftAttachType.REPLACE)
                        {
                            EGOGiftRenderData egogiftRenderData = null;
                            if (agentModel.GetUnit().spriteSetter.TryGetGift(egogiftModel, out egogiftRenderData))
                            {
                                if (egogiftRenderData != null)
                                {
                                    egogiftRenderData.renderer.enabled = false;
                                }
                            }
                        }
                    }
                }
                agentModel.GetUnit().spriteSetter.SymbolRenderer.enabled = false;
                agentModel.GetUnit().spriteSetter.EyeRenderer.enabled = false;
                agentModel.GetUnit().spriteSetter.MouthRenderer.enabled = false;
                agentModel.GetUnit().spriteSetter.NoteRenderer.enabled = false;
                agentModel.GetUnit().spriteSetter.EyebrowRenderer.enabled = false;
                agentModel.GetUnit().spriteSetter.MouthReplaceGiftRender.enabled = false;
                agentModel.GetUnit().spriteSetter.panicRenderer.enabled = false;
                this.state = agentModel.GetUnit().animChanger.state;
                base.model.metaInfo.specialWeaponAnim = "BossBirdWeapon_BossAmiya";
                base.model.metaInfo.range = 5f;
                return;
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public void OnNotice(string notice, params object[] param)
        {
            if (notice == NoticeName.OnAgentDead)
            {
                AgentModel agentModel = param[0] as AgentModel;
                if (agentModel == null)
                {
                    return;
                }
                if (this.diedModel.Contains(agentModel))
                {
                    return;
                }
                this.diedModel.Add(agentModel);
                if (diedModel.Count >= 12)
                {
                    upgradeAttack = true;
                }
            }
        }
        private void SetObserver(bool active)
        {
            if (active)
            {
                global::Notice.instance.Observe(global::NoticeName.OnAgentDead, this);
            }
            else
            {
                global::Notice.instance.Remove(global::NoticeName.OnAgentDead, this);
            }
        }
        public void OnPanic()
        {
            isRecoverying = true;
            AgentModel agentModel = base.model.owner as AgentModel;
            agentModel.GetUnit().animChanger.ChangeAnimatorWithUniqueFace("BossBirdWeapon_BossAmiya", false);
            agentModel.GetUnit().animEventHandler.SetAnimEvent(null);
            agentModel.GetUnit().animChanger.SetState(true);
            agentModel.GetMovableNode().StopMoving();
            agentModel.StopStun();
            agentModel.StopAction();
            agentModel.mental = 1;
            SefiraManager.instance.GetSefira(agentModel.currentSefira).OnAgentCannotControll(agentModel);
            agentModel.GetWorkerUnit().animChanger.SetState(true);
            agentModel.AddUnitBuf(new Amiya_RecoverMP(this));
            agentModel.GetWorkerUnit().animChanger.state.AddAnimation(0, "InRecover_Start", false, 0);
            agentModel.GetWorkerUnit().animChanger.state.AddAnimation(0, "InRecover_Loop", false, 0);
        }
        public override void OnFixedUpdate()
        {
            try
            {
                AgentModel agentModel = base.model.owner as AgentModel;
                if (agentModel.GetUnit().animChanger.state != this.state && !agentModel.IsAttackState() && !agentModel.IsDead() && agentModel.hp > 0f)
                {
                    agentModel.GetUnit().animChanger.SetState(true);
                    agentModel.GetUnit().animChanger.ChangeAnimator("BossBirdWeapon_BossAmiya");
                    agentModel.GetUnit().spriteSetter.SymbolRenderer.enabled = false;
                    foreach (EGOgiftModel egogiftModel in agentModel.GetAllGifts())
                    {
                        if (egogiftModel != null)
                        {
                            if (egogiftModel.metaInfo.attachType != EGOgiftAttachType.REPLACE)
                            {
                                EGOGiftRenderData egogiftRenderData = null;
                                if (agentModel.GetUnit().spriteSetter.TryGetGift(egogiftModel, out egogiftRenderData))
                                {
                                    if (egogiftRenderData != null)
                                    {
                                        egogiftRenderData.renderer.enabled = false;
                                    }
                                }
                            }
                        }
                    }
                    agentModel.GetUnit().spriteSetter.EyeRenderer.enabled = false;
                    agentModel.GetUnit().spriteSetter.MouthRenderer.enabled = false;
                    agentModel.GetUnit().spriteSetter.NoteRenderer.enabled = false;
                    agentModel.GetUnit().spriteSetter.EyebrowRenderer.enabled = false;
                    agentModel.GetUnit().spriteSetter.MouthReplaceGiftRender.enabled = false;
                    agentModel.GetUnit().spriteSetter.panicRenderer.enabled = false;
                    this.state = agentModel.GetUnit().animChanger.state;
                }
                base.OnFixedUpdate();
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private List<UnitModel> diedModel = [];
        private bool upgradeAttack = false;
        private bool isUsedMentalCrash = false;
        public Spine.AnimationState state;
        public bool isRecoverying = false;
    }
}
