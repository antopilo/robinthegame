using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class TextNode : IDialogNode
{
    public DialogNodeType Type { get; } = DialogNodeType.Text;

    public IDialogNode Next { get; } = null;

    public string Text { get; set; } = "EMPTY_TEXT_NODE";

    public List<IDialogNode> Choices { get; set; } = null;
    public List<string> ChoicesLabel { get; set; } = null;

    public TextNode(string message, IDialogNode Next)
    {
        this.Text = message;
        this.Next = Next;
    }
}

