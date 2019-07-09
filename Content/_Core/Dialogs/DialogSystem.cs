using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class DialogSystem
{
    private List<Dialogs> m_dialogs = new List<Dialogs>();
    private Dialogs m_currentDialog { get; set; }

    public void SetDialog(Dialogs dialog)
    {
        m_currentDialog = dialog;
    }

    public void Next()
    {
        // Display message here.
        //if (!m_currentDialog.Next())
        //    SetNextDialog();
    }


}

