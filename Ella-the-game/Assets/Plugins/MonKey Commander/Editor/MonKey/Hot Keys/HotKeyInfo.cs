using System.Collections.Generic;
using System.Linq;
using MonKey.Internal;

namespace MonKey.Editor.Internal
{

    internal class HotKeyInfo
    {

        public readonly KeyCombination Code;

        public readonly List<CommandInfo> AssociatedCommands;

        public bool IsSomeCommandsValid
        {
            get { return AssociatedCommands.Count(_ => _.IsValid) > 0; }
        }

        public bool IsConflictual
        {
            get
            {
                return AssociatedCommands != null
                       && (AssociatedCommands.Count(_ => _.IsValid) > 1);
            }
        }

        public HotKeyInfo(KeyCombination code, params CommandInfo[] associatedCommands)
        {
            Code = code;
            AssociatedCommands = new List<CommandInfo>(associatedCommands);
        }

    }
}
