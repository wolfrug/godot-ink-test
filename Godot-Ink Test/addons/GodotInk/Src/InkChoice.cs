#nullable enable

using Godot;
using System.Collections.Generic;

namespace GodotInk;

#if TOOLS
[Tool]
#endif
public partial class InkChoice : GodotObject
{
    public string Text => inner.text;
    public string PathStringOnChoice => inner.pathStringOnChoice;
    public string SourcePath => inner.sourcePath;
    public Ink.Runtime.Path TargetPath => inner.targetPath;
    public int Index => inner.index;
    public List<string> Tags => inner.tags;

    private readonly Ink.Runtime.Choice inner;

    private InkChoice()
    {
        inner = new Ink.Runtime.Choice();
    }

    public InkChoice(Ink.Runtime.Choice inner)
    {
        this.inner = inner;
    }
}
