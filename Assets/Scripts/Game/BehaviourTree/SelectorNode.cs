using System.Collections.Generic;

namespace Game.BehaviourTree
{
    class SelectorNode : CompositeNode
    {
        public SelectorNode(params INode[] nodes) : base(nodes)
        {

        }

        public override IEnumerator<NodeResult> Tick()
        {
            NodeResult returnNodeResult = NodeResult.Failure;

            foreach (INode node in _nodes)
            {
                IEnumerator<NodeResult> result = node.Tick();

                while (result.MoveNext() && result.Current == NodeResult.Running)
                {
                    yield return NodeResult.Running;
                }

                returnNodeResult = result.Current;

                if(returnNodeResult == NodeResult.Failure)
                    continue;

                if (returnNodeResult == NodeResult.Succes)
                    break;
            }
            yield return returnNodeResult;
        }
    }
}
