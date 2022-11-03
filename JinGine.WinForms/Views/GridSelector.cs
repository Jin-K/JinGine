namespace JinGine.WinForms.Views;

// TODO has more than 1 responsibility :'(
internal class GridSelector
{
    private readonly Control _control;
    private readonly GridProjector _projector;
    private Point _gridStartLoc;
    private Point _gridEndLoc;
    private Point? _mouseDownLoc;

    internal GridSelector(Control control, GridProjector projector)
    {
        _control = control;
        _projector = projector;
        _gridStartLoc = Point.Empty;
        _gridEndLoc = Point.Empty;
        _mouseDownLoc = null;
        State = SelectionState.Unselected;

        control.MouseDown += OnMouseDown;
        control.MouseMove += OnMouseMove;
        control.MouseUp += OnMouseUp;
    }

    internal SelectionState State { get; private set; }

    internal Rectangle Selection => _projector.GridLocationsToScreenRect(_gridStartLoc, _gridEndLoc);

    internal void StartSelect(Point startPoint)
    {
        _gridStartLoc = _gridEndLoc = _projector.ScreenLocationToGridLocation(startPoint);
        _mouseDownLoc = null;
        State = SelectionState.Selecting;
    }

    internal void EndSelect(Point endPoint)
    {
        _gridEndLoc = _projector.ScreenLocationToGridLocation(endPoint);
        _mouseDownLoc = null;
        State = SelectionState.Selected;
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        _mouseDownLoc = e.Location;
        _gridStartLoc = _projector.ScreenLocationToGridLocation(e.Location);
        if (State is SelectionState.Unselected) return;
        State = SelectionState.Unselected;
        _control.Invalidate();
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (_mouseDownLoc is null) return;
        _gridEndLoc = _projector.ScreenLocationToGridLocation(e.Location);
        State = SelectionState.Selecting;
        _control.Invalidate();
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        _mouseDownLoc = null;
        if (State is not SelectionState.Selecting) return;
        State = SelectionState.Selected;
        _control.Invalidate();
    }
}