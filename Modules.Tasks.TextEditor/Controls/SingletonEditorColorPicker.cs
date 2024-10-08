﻿using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.Views.Controls;
using Modules.Common.Views.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Modules.Tasks.TextEditor.Controls;

public class SingletonEditorColorPicker : Button
{
    public static readonly DependencyProperty SelectedColorStringProperty = 
        DependencyProperty.Register(nameof(SelectedColorString), typeof(string), typeof(SingletonEditorColorPicker), new PropertyMetadata(Constants.ColorName.Transparent));
    public static readonly DependencyProperty AppliedColorStringProperty = 
        DependencyProperty.Register(nameof(AppliedColorString), typeof(string), typeof(SingletonEditorColorPicker), new PropertyMetadata(Constants.ColorName.Transparent));

    private SingletonPopup? _popup;

    public ICommand OpenPopupCommand { get; }

    public string SelectedColorString
    {
        get => (string)GetValue(SelectedColorStringProperty);
        set => SetValue(SelectedColorStringProperty, value);
    }

    public string AppliedColorString
    {
        get => (string)GetValue(AppliedColorStringProperty);
        set => SetValue(AppliedColorStringProperty, value);
    }

    public SingletonEditorColorPicker()
    {
        OpenPopupCommand = new RelayCommand(OpenPopup);
    }

    private void OpenPopup()
    {
        _popup ??= PopupService.Popup;

        _popup.IsOpen = false;

        // First register to clear previous registration if there is any
        _popup.RegisterSelectedColorChangeEvent(OnPopupSelectedColorChanged);
        _popup.SelectedColor = SelectedColorString;

        _popup.Closed += OnPopupClosed;
        _popup.LostFocus += OnPopupLostFocus;

        _popup.PlacementTarget = this;
        _popup.IsOpen = true;
    }

    // Called when the left button has been clicked
    protected override void OnClick()
    {
        base.OnClick();

        ApplyDisplayColor();
    }

    private void ApplyDisplayColor()
    {
        if (AppliedColorString == SelectedColorString)
        {
            AppliedColorString = string.Empty;
        }

        AppliedColorString = SelectedColorString;
    }

    private void OnPopupLostFocus(object sender, RoutedEventArgs e)
    {
        OnPopupClosed(sender, EventArgs.Empty);
        
        if (_popup != null)
        {
            _popup.LostFocus -= OnPopupLostFocus;
        }
    }

    private void OnPopupSelectedColorChanged()
    {
        SelectedColorString = _popup.SelectedColor;
        _popup.IsOpen = false;

        ApplyDisplayColor();
    }

    private void OnPopupClosed(object? sender, EventArgs e)
    {
        IsEnabled = true;
        
        if (_popup != null)
        {
            _popup.Closed -= OnPopupClosed;
        }
    }
}