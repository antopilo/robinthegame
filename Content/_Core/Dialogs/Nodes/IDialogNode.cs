using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum DialogNodeType
{
    Text, Choice, Quest, Redirect
}

public interface IDialogNode
{
    DialogNodeType Type { get; }

    IDialogNode Next { get; }

    string Text { get; set; }

    List<IDialogNode> Choices { get; set; }
    List<string> ChoicesLabel { get; set; }

    //TODO: ADD QUEST SUPPORT
    // Quest Quest { get; }
}

