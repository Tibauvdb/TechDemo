using System.Collections.Generic;

namespace Game.BehaviourTree
{
    public interface INode
    {
        IEnumerator<NodeResult> Tick();
    }
}
