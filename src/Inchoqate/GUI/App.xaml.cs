﻿using System.Windows;
using Inchoqate.GUI.View.Editors;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.ViewModel.Editors;
using Inchoqate.GUI.ViewModel.Editors.StackEditor;

namespace Inchoqate.GUI;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Startup += delegate
        {
            MainWindow!.Loaded += delegate
            {
                // Only initiate project data if the main
                // window and OpenGL context is loaded.
                DataContext.Project = new();
            };
        };
    }

    public AppViewModel DataContext { get; } = new();

    public ResourceDictionary ThemeDictionary => Current.Resources.MergedDictionaries.First();

    public void ChangeTheme(Uri uri)
    {
        ThemeDictionary.MergedDictionaries.Clear();
        ThemeDictionary.MergedDictionaries.Add(new() { Source = uri });
    }
}