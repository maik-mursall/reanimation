using Aarthificial.Reanimation.Common;
using UnityEngine;

namespace Aarthificial.Reanimation.Nodes
{
    [CreateAssetMenu(fileName = "switch", menuName = "Reanimator/Switch", order = 400)]
    public class SwitchNode : ReanimatorNode
    {
        public static SwitchNode Create(
            ControlDriver driver = null,
            ReanimatorNode[] nodes = null
        )
        {
            var instance = CreateInstance<SwitchNode>();

            if (driver != null)
                instance.controlDriver = driver;
            if (nodes != null)
                instance.nodes = nodes;

            return instance;
        }
        
        [SerializeField] protected ReanimatorNode[] nodes;
        [SerializeField] protected ControlDriver controlDriver = new ControlDriver();
        [SerializeField] protected DriverDictionary drivers = new DriverDictionary();

        public override TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState)
        {
            AddTrace(nextState);
            nextState.Merge(drivers);
            return nodes[controlDriver.ResolveDriver(previousState, nextState, nodes.Length)]
                .Resolve(previousState, nextState);
        }
    }
}