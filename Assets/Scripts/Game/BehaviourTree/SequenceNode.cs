﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.BehaviourTree
{
    class SequenceNode : CompositeNode
    {
        public SequenceNode(params INode[] nodes) : base(nodes)
        {

        }
        public override IEnumerator<NodeResult> Tick()
        {
            NodeResult returnNodeResult = NodeResult.Succes;

            foreach (INode node in _nodes)
            {
                IEnumerator<NodeResult> result = node.Tick();

                while (result.MoveNext() && result.Current == NodeResult.Running)
                {
                    yield return NodeResult.Running;
                }

                returnNodeResult = result.Current;

                if (returnNodeResult == NodeResult.Succes)
                    continue;

                if (returnNodeResult == NodeResult.Failure)
                    break;
            }

            yield return returnNodeResult;
        }
    }
}
