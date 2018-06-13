﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MLAgents
{


    public class ScriptableBrain : Brain
    {

        /**< Reference to the Decision component used to decide the actions */
//        public Decision decision = new RandomDecision();
#if UNITY_EDITOR
        public MonoScript decision;
#endif
        [SerializeField]
        public string c_decision;
        public string asm_decision;

        public void OnValidate()
        {
            #if UNITY_EDITOR
            if (decision != null)
            {
                c_decision = decision.GetClass().Name;
                asm_decision = decision.GetClass().Assembly.FullName;
            }
            else
            {
                c_decision = asm_decision = "";
            }
            #endif
        }

        protected override void DecideAction()
        {

            //var d = Activator.CreateInstance() as TestDecision;
            //Debug.Log(asm_decision + " " + c_decision);
            if (asm_decision != "" && c_decision != "")
            {
                try
                {
                    var d =
                        Activator.CreateInstance(asm_decision, c_decision).Unwrap() as TestDecision;
                    d.DecisionMethod();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            
            if (brainBatcher != null)
            {
                brainBatcher.SendBrainInfo(this.name, agentInfo);
            }
            
            if (isExternal)
            {
                agentInfo.Clear();
                return;
            }

            foreach (Agent agent in agentInfo.Keys)
            {
                Decision decision = agent.gameObject.GetComponent<Decision>();
                agent.UpdateVectorAction(decision.Decide(
                    agentInfo[agent].stackedVectorObservation,
                    agentInfo[agent].visualObservations,
                    agentInfo[agent].reward,
                    agentInfo[agent].done,
                    agentInfo[agent].memories));
            }

            foreach (Agent agent in agentInfo.Keys)
            {
                Decision decision = agent.gameObject.GetComponent<Decision>();
                agent.UpdateMemoriesAction(decision.MakeMemory(
                    agentInfo[agent].stackedVectorObservation,
                    agentInfo[agent].visualObservations,
                    agentInfo[agent].reward,
                    agentInfo[agent].done,
                    agentInfo[agent].memories));
            }
            agentInfo.Clear();
        }
    }

}