using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.BehaviourTree
{
    public abstract class DecoratorNode : INode
    {
        private INode _node;

        protected DecoratorNode(INode node)
        {
            _node = node;
        }

        public IEnumerator<NodeResult> Tick()
        {
            IEnumerator<NodeResult> result = _node.Tick();
            NodeResult decoratedState = NodeResult.Succes;

            while (result.MoveNext() && (decoratedState = Decorate(result.Current)) == NodeResult.Running)
            {
                yield return decoratedState;
                if (result.Current != NodeResult.Running)
                    result = _node.Tick();
            }
            yield return decoratedState;
        }

        protected abstract NodeResult Decorate(NodeResult state);
    }
}
