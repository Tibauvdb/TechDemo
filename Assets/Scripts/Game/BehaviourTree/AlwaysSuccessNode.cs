using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.BehaviourTree
{
    public class AlwaysSuccessNode : DecoratorNode
    {
        private readonly bool _continueWhenRunning;

        public AlwaysSuccessNode(INode node, bool continueWhenRunning = true) : base(node)
        {
            _continueWhenRunning = continueWhenRunning;
        }

        protected override NodeResult Decorate(NodeResult result)
        {
            if (result == NodeResult.Running && _continueWhenRunning)
                return NodeResult.Running;
            return NodeResult.Succes;
        }


    }
}
