using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Dialogs
{
    private IDialogNode m_entryNode;
    private IDialogNode m_currentNode;

    public bool Next()
    {
        // If it reaches the end of the dialog.
        if (m_currentNode.Next is null)
            return false;

        return true;
    }

}

