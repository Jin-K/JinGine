using System;
using System.IO;
using System.Linq;
using JinGine.Core.Models;

namespace JinGine.Core.BusinessLogic;

public class Editor2DTextReader : IEditor2DTextReader
{
    private readonly Editor2DText _model;

    public Editor2DTextReader(Editor2DText model)
    {
        _model = model;
    }

    public string[] ReadLines()
    {
        return _model.Select(ls => ls.Content).ToArray();
    }
}