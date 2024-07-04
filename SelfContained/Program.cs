﻿// This is a simple example application for a self-contained single file.

using System.Diagnostics.CodeAnalysis;
using Terminal.Gui;

namespace SelfContained;

public static class Program
{
    [RequiresUnreferencedCode ("Calls Terminal.Gui.Application.Run<T>(Func<Exception, Boolean>, ConsoleDriver)")]
    private static void Main (string [] args)
    {
        Application.Run<ExampleWindow> ().Dispose ();

        // Before the application exits, reset Terminal.Gui for clean shutdown
        Application.Shutdown ();

        Console.WriteLine ($@"Username: {ExampleWindow.UserName}");
    }
}

// Defines a top-level window with border and title
public class ExampleWindow : Window
{
    public static string? UserName;

    public ExampleWindow ()
    {
        Title = $"Example App ({Application.QuitKey} to quit)";

        // Create input components and labels
        var usernameLabel = new Label { Text = "Username:" };

        var usernameText = new TextField
        {
            // Position text field adjacent to the label
            X = Pos.Right (usernameLabel) + 1,

            // Fill remaining horizontal space
            Width = Dim.Fill ()
        };

        var passwordLabel = new Label
        {
            Text = "Password:", X = Pos.Left (usernameLabel), Y = Pos.Bottom (usernameLabel) + 1
        };

        var passwordText = new TextField
        {
            Secret = true,

            // align with the text box above
            X = Pos.Left (usernameText),
            Y = Pos.Top (passwordLabel),
            Width = Dim.Fill ()
        };

        // Create login button
        var btnLogin = new Button
        {
            Text = "Login",
            Y = Pos.Bottom (passwordLabel) + 1,

            // center the login button horizontally
            X = Pos.Center (),
            IsDefault = true
        };

        // When login button is clicked display a message popup
        btnLogin.Accept += (s, e) =>
        {
            if (usernameText.Text == "admin" && passwordText.Text == "password")
            {
                MessageBox.Query ("Logging In", "Login Successful", "Ok");
                UserName = usernameText.Text;
                Application.RequestStop ();
            }
            else
            {
                MessageBox.ErrorQuery ("Logging In", "Incorrect username or password", "Ok");
            }
        };

        // Add the views to the Window
        Add (usernameLabel, usernameText, passwordLabel, passwordText, btnLogin);
    }
}
