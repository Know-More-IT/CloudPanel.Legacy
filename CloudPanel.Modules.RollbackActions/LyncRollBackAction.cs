using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.RollbackActions
{
    public class LyncRollBackAction
    {
        private readonly List<LyncRollBackAction> performedActions;

        public LyncRollBackAction()
        {
            performedActions = new List<LyncRollBackAction>();
        }
    }
}
