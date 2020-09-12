﻿using Prism.Services.Dialogs;
using RaceControl.Core.Mvvm;
using System;

namespace RaceControl.Views
{
    public partial class VideoDialogWindow : IDialogWindow
    {
        public IDialogResult Result { get; set; }

        public VideoDialogWindow()
        {
            InitializeComponent();
            Closed += VideoDialogWindow_Closed;
        }

        private void VideoDialogWindow_Closed(object sender, EventArgs e)
        {
            if (DataContext is IVideoDialogViewModel vm)
            {
                Result = new DialogResult(ButtonResult.None, new DialogParameters { { ParameterNames.IDENTIFIER, vm.UniqueIdentifier } });
            }

            Closed -= VideoDialogWindow_Closed;
        }
    }
}