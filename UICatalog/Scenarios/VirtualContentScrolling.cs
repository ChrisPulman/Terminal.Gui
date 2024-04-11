﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Terminal.Gui;

namespace UICatalog.Scenarios;

[ScenarioMetadata ("_Virtual Content Scrolling Demo", "Demonstrates scrolling built-into View")]
[ScenarioCategory ("Layout")]
public class VirtualScrolling : Scenario
{
    private ViewDiagnosticFlags _diagnosticFlags;
    public class VirtualDemoView : FrameView
    {
        public VirtualDemoView ()
        {
            Width = Dim.Fill ();
            Height = Dim.Fill ();
            ColorScheme = Colors.ColorSchemes ["Base"];
            Text = "Virtual Demo View Text. This is long text.\nThe second line.\n3\n4\n5th line\nLine 6. This is a longer line that should wrap automatically.";
            CanFocus = true;
            BorderStyle = LineStyle.Rounded;
            Arrangement = ViewArrangement.Fixed;

            // TODO: Add a way to set the scroll settings in the Scenario
            ContentSize = new Size (60, 40);
            ViewportSettings |= ViewportSettings.ClearVisibleContentOnly;

            // Things this view knows how to do
            AddCommand (Command.ScrollDown, () => ScrollVertical (1));
            AddCommand (Command.ScrollUp, () => ScrollVertical (-1));

            AddCommand (Command.ScrollRight, () => ScrollHorizontal (1));
            AddCommand (Command.ScrollLeft, () => ScrollHorizontal (-1));

            //AddCommand (Command.PageUp, () => PageUp ());
            //AddCommand (Command.PageDown, () => PageDown ());
            //AddCommand (Command.TopHome, () => Home ());
            //AddCommand (Command.BottomEnd, () => End ());

            // Default keybindings for all ListViews
            KeyBindings.Add (Key.CursorUp, Command.ScrollUp);
            KeyBindings.Add (Key.CursorDown, Command.ScrollDown);
            KeyBindings.Add (Key.CursorLeft, Command.ScrollLeft);
            KeyBindings.Add (Key.CursorRight, Command.ScrollRight);

            //KeyBindings.Add (Key.PageUp, Command.PageUp);
            //KeyBindings.Add (Key.PageDown, Command.PageDown);
            //KeyBindings.Add (Key.Home, Command.TopHome);
            //KeyBindings.Add (Key.End, Command.BottomEnd);

            Border.Add (new Label () { X = 23 });
            LayoutComplete += VirtualDemoView_LayoutComplete;

            MouseEvent += VirtualDemoView_MouseEvent;
        }

        private void VirtualDemoView_MouseEvent (object sender, MouseEventEventArgs e)
        {
            if (e.MouseEvent.Flags == MouseFlags.WheeledDown)
            {
                ScrollVertical (1);
                return;
            }
            if (e.MouseEvent.Flags == MouseFlags.WheeledUp)
            {
                ScrollVertical (-1);

                return;
            }

            if (e.MouseEvent.Flags == MouseFlags.WheeledRight)
            {
                ScrollHorizontal (1);
                return;
            }
            if (e.MouseEvent.Flags == MouseFlags.WheeledLeft)
            {
                ScrollHorizontal (-1);

                return;
            }

        }

        private void VirtualDemoView_LayoutComplete (object sender, LayoutEventArgs e)
        {
            var status = Border.Subviews.OfType<Label> ().FirstOrDefault ();

            if (status is { })
            {
                status.Title = $"Frame: {Frame}\n\nViewport: {Viewport}, ContentSize = {ContentSize}";
            }

            SetNeedsDisplay ();
        }
    }

    public override void Main ()
    {
        Application.Init ();

        var view = new VirtualDemoView { Title = "Virtual Scrolling" };

        // Add Scroll Setting UI to Padding
        view.Padding.Thickness = new (0, 3, 0, 0);
        view.Padding.ColorScheme = Colors.ColorSchemes["Error"];

        var cbAllowNegativeX = new CheckBox ()
        {
            Title = "Allow _X < 0",
            Y = 0,
            CanFocus = false
        };
        cbAllowNegativeX.Checked = view.ViewportSettings.HasFlag (ViewportSettings.AllowNegativeX);
        cbAllowNegativeX.Toggled += AllowNegativeX_Toggled;

        void AllowNegativeX_Toggled (object sender, StateEventArgs<bool?> e)
        {
            if (e.NewValue == true)
            {
                view.ViewportSettings |= ViewportSettings.AllowNegativeX;
            }
            else
            {
                view.ViewportSettings &= ~ViewportSettings.AllowNegativeX;
            }
        }

        view.Padding.Add (cbAllowNegativeX);

        var cbAllowNegativeY = new CheckBox ()
        {
            Title = "Allow _Y < 0",
            X = Pos.Right (cbAllowNegativeX) + 1,
            Y = 0,
            CanFocus = false
        };
        cbAllowNegativeY.Checked = view.ViewportSettings.HasFlag (ViewportSettings.AllowNegativeY);
        cbAllowNegativeY.Toggled += AllowNegativeY_Toggled;

        void AllowNegativeY_Toggled (object sender, StateEventArgs<bool?> e)
        {
            if (e.NewValue == true)
            {
                view.ViewportSettings |= ViewportSettings.AllowNegativeY;
            }
            else
            {
                view.ViewportSettings &= ~ViewportSettings.AllowNegativeY;
            }
        }

        view.Padding.Add (cbAllowNegativeY);
        
        var cbAllowXGreaterThanContentWidth = new CheckBox ()
        {
            Title = "Allow X > Content",
            Y = Pos.Bottom(cbAllowNegativeX),
            CanFocus = false
        };
        cbAllowXGreaterThanContentWidth.Checked = view.ViewportSettings.HasFlag (ViewportSettings.AllowXGreaterThanContentWidth);
        cbAllowXGreaterThanContentWidth.Toggled += AllowXGreaterThanContentWidth_Toggled;

        void AllowXGreaterThanContentWidth_Toggled (object sender, StateEventArgs<bool?> e)
        {
            if (e.NewValue == true)
            {
                view.ViewportSettings |= ViewportSettings.AllowXGreaterThanContentWidth;
            }
            else
            {
                view.ViewportSettings &= ~ViewportSettings.AllowXGreaterThanContentWidth;
            }
        }

        view.Padding.Add (cbAllowXGreaterThanContentWidth);

        var cbAllowYGreaterThanContentHeight = new CheckBox ()
        {
            Title = "Allow Y > Content",
            X = Pos.Right (cbAllowXGreaterThanContentWidth) + 1,
            Y = Pos.Bottom (cbAllowNegativeX),
            CanFocus = false
        };
        cbAllowYGreaterThanContentHeight.Checked = view.ViewportSettings.HasFlag (ViewportSettings.AllowYGreaterThanContentHeight);
        cbAllowYGreaterThanContentHeight.Toggled += AllowYGreaterThanContentHeight_Toggled;

        void AllowYGreaterThanContentHeight_Toggled (object sender, StateEventArgs<bool?> e)
        {
            if (e.NewValue == true)
            {
                view.ViewportSettings |= ViewportSettings.AllowYGreaterThanContentHeight;
            }
            else
            {
                view.ViewportSettings &= ~ViewportSettings.AllowYGreaterThanContentHeight;
            }
        }

        view.Padding.Add (cbAllowYGreaterThanContentHeight);

        var labelContentSize = new Label ()
        {
            Title = "_ContentSize:",
            Y = Pos.Bottom(cbAllowYGreaterThanContentHeight),
        };

        var contentSizeWidth = new Buttons.NumericUpDown()
        {
            Value = view.ContentSize.Width,
            X = Pos.Right (labelContentSize) + 1,
            Y = Pos.Top (labelContentSize),
        };
        contentSizeWidth.ValueChanged += ContentSizeWidth_ValueChanged;

        void ContentSizeWidth_ValueChanged (object sender, PropertyChangedEventArgs e)
        {
           view.ContentSize = view.ContentSize with { Width = ((Buttons.NumericUpDown)sender).Value };
        }

        var labelComma = new Label ()
        {
            Title = ", ",
            X = Pos.Right (contentSizeWidth),
            Y = Pos.Top (labelContentSize),
        };

        var contentSizeHeight = new Buttons.NumericUpDown ()
        {
            Value = view.ContentSize.Height,
            X = Pos.Right (labelComma),
            Y = Pos.Top (labelContentSize),
            CanFocus =false
        };
        contentSizeHeight.ValueChanged += ContentSizeHeight_ValueChanged;

        void ContentSizeHeight_ValueChanged (object sender, PropertyChangedEventArgs e)
        {
            view.ContentSize = view.ContentSize with { Height = ((Buttons.NumericUpDown)sender).Value };
        }


        var cbClearOnlyVisible = new CheckBox ()
        {
            Title = "Clear only Visible Content",
            X = Pos.Right (contentSizeHeight) + 1,
            Y = Pos.Top (labelContentSize),
            CanFocus = false
        };
        cbClearOnlyVisible.Checked = view.ViewportSettings.HasFlag (ViewportSettings.ClearVisibleContentOnly);
        cbClearOnlyVisible.Toggled += ClearVisibleContentOnly_Toggled;

        void ClearVisibleContentOnly_Toggled (object sender, StateEventArgs<bool?> e)
        {
            if (e.NewValue == true)
            {
                view.ViewportSettings |= ViewportSettings.ClearVisibleContentOnly;
            }
            else
            {
                view.ViewportSettings &= ~ViewportSettings.ClearVisibleContentOnly;
            }
        }

        view.Padding.Add (labelContentSize, contentSizeWidth, labelComma, contentSizeHeight, cbClearOnlyVisible);


        // Add demo views to show that things work correctly
        var textField = new TextField { X = 20, Y = 7, Width = 15, Text = "Test TextField" };

        var colorPicker = new ColorPicker { Title = "BG", BoxHeight = 1, BoxWidth = 1, X = Pos.AnchorEnd (11), Y = 10 };
        colorPicker.BorderStyle = LineStyle.RoundedDotted;

        colorPicker.ColorChanged += (s, e) =>
                              {
                                  colorPicker.SuperView.ColorScheme = new (colorPicker.SuperView.ColorScheme)
                                  {
                                      Normal = new (
                                                    colorPicker.SuperView.ColorScheme.Normal.Foreground,
                                                    e.Color
                                                   )
                                  };
                              };

        var textView = new TextView
        {
            X = Pos.Center (),
            Y = 10,
            Title = "TextView",
            Text = "I have a 3 row top border.\nMy border inherits from the SuperView.\nI have 3 lines of text with room for 2.",
            AllowsTab = false,
            Width = 30,
            Height = 6 // TODO: Use Dim.Auto
        };
        textView.Border.Thickness = new (1, 3, 1, 1);

        var charMap = new Scenarios.CharMap ()
        {
            X = Pos.Center (),
            Y = Pos.Bottom (textView) + 1,
            Width = 30,
            Height = 10
        };

        charMap.Accept += (s, e) =>
                              MessageBox.Query (20, 7, "Hi", $"Am I a {view.GetType ().Name}?", "Yes", "No");

        var buttonAnchoredRight = new Button { X = Pos.AnchorEnd (10), Y = 0, Text = "Button" };

        var labelAnchoredBottomLeft = new Label
        {
            AutoSize = false,
            Y = Pos.AnchorEnd (3),
            Width = 25,
            Height = Dim.Fill (),
            Text = "Label\nY=AnchorEnd(3),Height=Dim.Fill()"
        };

        view.Margin.Data = "Margin";
        view.Margin.Thickness = new (0);

        view.Border.Data = "Border";
        view.Border.Thickness = new (3);

        view.Padding.Data = "Padding";

        view.Add (buttonAnchoredRight, textField, colorPicker, charMap, textView, labelAnchoredBottomLeft);

        var longLabel = new Label
        {
            Id = "label2",
            X = 0,
            Y = 30,
            Text = "This label is long. It should clip to the Viewport (but not ContentArea). This is a virtual scrolling demo. Use the arrow keys and/or mouse wheel to scroll the content.",
        };
        longLabel.TextFormatter.WordWrap = true;
        view.Add (longLabel);

        var editor = new Adornments.AdornmentsEditor
        {
            Title = $"{Application.QuitKey} to Quit - Scenario: {GetName ()}",
            ColorScheme = Colors.ColorSchemes ["Dialog"]
        };

        editor.Initialized += (s, e) => { editor.ViewToEdit = view; };

        editor.Closed += (s, e) => View.Diagnostics = _diagnosticFlags;

        Application.Run (editor);
        editor.Dispose ();
        Application.Shutdown ();
    }
}
