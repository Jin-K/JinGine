using System.Linq;
using JinGine.Domain.Models;

namespace JinGine.Domain.Services;

public class Editor2DTextReader
{
    private readonly Editor2DText _model;

    public Editor2DTextReader(Editor2DText model) => _model = model;

    public string[] ReadLines() => _model.Select(ls => ls.Content).ToArray();
}