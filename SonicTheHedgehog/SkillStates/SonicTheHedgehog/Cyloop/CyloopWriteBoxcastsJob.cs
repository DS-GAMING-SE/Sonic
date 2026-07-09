using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SonicTheHedgehog.SkillStates.Cyloop
{
    public struct CyloopWriteBoxcastsJob : IJob
    {
        public NativeArray<BoxcastCommand> output;
        public void Execute()
        {

        }
    }
}
