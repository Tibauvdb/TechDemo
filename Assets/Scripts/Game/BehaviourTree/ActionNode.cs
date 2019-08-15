using System.Collections.Generic;

namespace Game.BehaviourTree
{
    class ActionNode : INode
    {
        public delegate IEnumerator<NodeResult> Action();

        private readonly Action _action;

        public ActionNode(Action action)
        {
            _action = action;
        }

        public IEnumerator<NodeResult> Tick()
        {
            return _action();
        }
    }
}
