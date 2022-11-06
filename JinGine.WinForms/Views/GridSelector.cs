namespace JinGine.WinForms.Views;

// TODO has more than 1 responsibility :'(
internal class GridSelector
{
    private readonly Control _control;
    private readonly CharsGrid _grid;
    private Point _gridStartLoc;
    private Point _gridEndLoc;
    private Point? _mouseDownLoc;

    internal GridSelector(Control control, CharsGrid grid)
    {
        _control = control;
        _grid = grid;
        _gridStartLoc = Point.Empty;
        _gridEndLoc = Point.Empty;
        _mouseDownLoc = null;
        State = Status.Unselected;

        control.MouseDown += OnMouseDown;
        control.MouseMove += OnMouseMove;
        control.MouseUp += OnMouseUp;
    }

    internal Status State { get; private set; }

    internal Rectangle Selection {
        get {
            return Rectangle.Union(
                _grid.CharPointToScreenRect(_gridStartLoc),
                _grid.CharPointToScreenRect(_gridEndLoc)
            );
            // return _projector.GridLocationsToScreenRect(_gridStartLoc, _gridEndLoc);
        }
    }

    internal (Point start, Point end)? Selection2 => State is Status.Unselected ? null : (_gridStartLoc, _gridEndLoc);

    internal void StartSelect(Point startPoint)
    {
        _gridStartLoc = _gridEndLoc = _projector.ScreenToGridLocation(startPoint);
        _mouseDownLoc = null;
        State = Status.Selecting;
    }

    internal void EndSelect(Point endPoint)
    {
        _gridEndLoc = _projector.ScreenToGridLocation(endPoint);
        _mouseDownLoc = null;
        State = Status.Selected;
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        _mouseDownLoc = e.Location;
        _gridStartLoc = _projector.ScreenToGridLocation(e.Location);
        if (State is Status.Unselected) return;
        State = Status.Unselected;
        _control.Invalidate();
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (_mouseDownLoc is null) return;
        _gridEndLoc = _projector.ScreenToGridLocation(e.Location);
        State = Status.Selecting;
        _control.Invalidate();
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        _mouseDownLoc = null;
        if (State is not Status.Selecting) return;
        State = Status.Selected;
        _control.Invalidate();
    }

    internal enum Status
    {
        Unselected,
        Selecting,
        Selected,
    }
}