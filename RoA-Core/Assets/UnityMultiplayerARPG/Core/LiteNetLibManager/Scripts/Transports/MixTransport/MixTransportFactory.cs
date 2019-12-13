﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiteNetLibManager
{
    public class MixTransportFactory : BaseTransportFactory
    {
        public override bool CanUseWithWebGL { get { return true; } }
        public string connectKey = "SampleConnectKey";
        public int webSocketPortOffset = 100;

        public override ITransport Build()
        {
            return new MixTransport(connectKey, webSocketPortOffset);
        }
    }
}
