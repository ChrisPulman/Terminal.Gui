﻿namespace Terminal.Gui;

public partial class Toplevel
{
    /// <summary>Gets or sets if this Toplevel is in overlapped mode within a Toplevel container.</summary>
    public bool IsOverlapped => Application.OverlappedTop is { } && Application.OverlappedTop != this && !Modal;

    /// <summary>Gets or sets if this Toplevel is a container for overlapped children.</summary>
    public bool IsOverlappedContainer { get; set; }
}

public static partial class Application
{
    /// <summary>
    ///     Gets the list of the Overlapped children which are not modal <see cref="Toplevel"/> from the
    ///     <see cref="OverlappedTop"/>.
    /// </summary>
    public static List<Toplevel> OverlappedChildren
    {
        get
        {
            if (OverlappedTop is { })
            {
                List<Toplevel> _overlappedChildren = new ();

                foreach (Toplevel top in _topLevels)
                {
                    if (top != OverlappedTop && !top.Modal)
                    {
                        _overlappedChildren.Add (top);
                    }
                }

                return _overlappedChildren;
            }

            return null;
        }
    }

    #nullable enable
    /// <summary>
    ///     The <see cref="Toplevel"/> object used for the application on startup which
    ///     <see cref="Toplevel.IsOverlappedContainer"/> is true.
    /// </summary>
    public static Toplevel? OverlappedTop
    {
        get
        {
            if (Top is { IsOverlappedContainer: true })
            {
                return Top;
            }

            return null;
        }
    }
    #nullable restore

    /// <summary>Brings the superview of the most focused overlapped view is on front.</summary>
    public static void BringOverlappedTopToFront ()
    {
        if (OverlappedTop is { })
        {
            return;
        }

        View top = FindTopFromView (Top?.MostFocused);

        if (top is Toplevel && Top.Subviews.Count > 1 && Top.Subviews [^1] != top)
        {
            Top.BringSubviewToFront (top);
        }
    }

    /// <summary>Gets the current visible Toplevel overlapped child that matches the arguments pattern.</summary>
    /// <param name="type">The type.</param>
    /// <param name="exclude">The strings to exclude.</param>
    /// <returns>The matched view.</returns>
    public static Toplevel GetTopOverlappedChild (Type type = null, string [] exclude = null)
    {
        if (OverlappedTop is null)
        {
            return null;
        }

        foreach (Toplevel top in OverlappedChildren)
        {
            if (type is { } && top.GetType () == type && exclude?.Contains (top.Data.ToString ()) == false)
            {
                return top;
            }

            if ((type is { } && top.GetType () != type) || exclude?.Contains (top.Data.ToString ()) == true)
            {
                continue;
            }

            return top;
        }

        return null;
    }

    /// <summary>
    ///     Move to the next Overlapped child from the <see cref="OverlappedTop"/> and set it as the <see cref="Top"/> if
    ///     it is not already.
    /// </summary>
    /// <param name="top"></param>
    /// <returns></returns>
    public static bool MoveToOverlappedChild (Toplevel top)
    {
        if (top.Visible && OverlappedTop is { } && Current?.Modal == false)
        {
            lock (_topLevels)
            {
                _topLevels.MoveTo (top, 0, new ToplevelEqualityComparer ());
                Current = top;
            }

            return true;
        }

        return false;
    }

    /// <summary>Move to the next Overlapped child from the <see cref="OverlappedTop"/>.</summary>
    public static void OverlappedMoveNext ()
    {
        if (OverlappedTop is { } && !Current.Modal)
        {
            lock (_topLevels)
            {
                _topLevels.MoveNext ();
                var isOverlapped = false;

                while (_topLevels.Peek () == OverlappedTop || !_topLevels.Peek ().Visible)
                {
                    if (!isOverlapped && _topLevels.Peek () == OverlappedTop)
                    {
                        isOverlapped = true;
                    }
                    else if (isOverlapped && _topLevels.Peek () == OverlappedTop)
                    {
                        MoveCurrent (Top);

                        break;
                    }

                    _topLevels.MoveNext ();
                }

                Current = _topLevels.Peek ();
            }
        }
    }

    /// <summary>Move to the previous Overlapped child from the <see cref="OverlappedTop"/>.</summary>
    public static void OverlappedMovePrevious ()
    {
        if (OverlappedTop is { } && !Current.Modal)
        {
            lock (_topLevels)
            {
                _topLevels.MovePrevious ();
                var isOverlapped = false;

                while (_topLevels.Peek () == OverlappedTop || !_topLevels.Peek ().Visible)
                {
                    if (!isOverlapped && _topLevels.Peek () == OverlappedTop)
                    {
                        isOverlapped = true;
                    }
                    else if (isOverlapped && _topLevels.Peek () == OverlappedTop)
                    {
                        MoveCurrent (Top);

                        break;
                    }

                    _topLevels.MovePrevious ();
                }

                Current = _topLevels.Peek ();
            }
        }
    }

    private static bool OverlappedChildNeedsDisplay ()
    {
        if (OverlappedTop is null)
        {
            return false;
        }

        foreach (Toplevel top in _topLevels)
        {
            if (top != Current && top.Visible && (top.NeedsDisplay || top.SubViewNeedsDisplay || top.LayoutNeeded))
            {
                OverlappedTop.SetSubViewNeedsDisplay ();

                return true;
            }
        }

        return false;
    }

    private static bool SetCurrentOverlappedAsTop ()
    {
        if (OverlappedTop is null && Current != Top && Current?.SuperView is null && Current?.Modal == false)
        {
            Top = Current;

            return true;
        }

        return false;
    }
}
