﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NVIDIASurroundToggle.Resources;

namespace NVIDIASurroundToggle
{
    internal partial class ToolsForm : Form
    {
        public ToolsForm()
        {
            InitializeComponent();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                Close();
            }
        }

        private void WaitingProcessCheckedChanged(object sender, EventArgs e)
        {
            nud_app_timeout.ReadOnly = txt_app_process.ReadOnly = !cb_app_process.Checked;
            if (txt_app_process.ReadOnly)
            {
                if (!string.IsNullOrWhiteSpace(txt_app_executable.Text)
                    && File.Exists(txt_app_executable.Text))
                {
                    txt_app_process.Text = Path.GetFileNameWithoutExtension(txt_app_executable.Text).ToLower();
                }
                else
                {
                    txt_app_process.Text = string.Empty;
                }
                nud_app_timeout.Value = 20;
            }
        }

        private void CommandShortcutClick(object sender, EventArgs e)
        {
            var description = string.Empty;
            var args = new List<string>();
            if (rb_com_toggle.Checked)
            {
                args.Add(@"-aToggleMode");
                description = Language.FrmTools_Toggles_the_display_mode_between_surround_and_extended;
            }
            else if (rb_com_surround.Checked)
            {
                args.Add(@"-aGoSurround");
                description = Language.FrmTools_Changes_the_display_mode_to_surround;
            }
            else if (rb_com_extended.Checked)
            {
                args.Add(@"-aGoExtended");
                description = Language.FrmTools_Changes_the_display_mode_to_extended;
            }
            else if (rb_com_options.Checked)
            {
                args.Add(@"-aOpenOptions");
                description = Language.FrmTools_Opens_the_program_s_configuration_screen;
            }
            else if (rb_com_tools.Checked)
            {
                args.Add(@"-aOpenTools");
                description = Language.FrmTools_Opens_the_program_s_tools_screen;
            }
            if (cb_com_noSLI.Enabled && cb_com_noSLI.Checked)
            {
                args.Add(@"--nosli true");
            }
            var argsString = string.Join(" ", args);

            if (dialog_save.ShowDialog(this) == DialogResult.OK)
            {
                var toolname = Application.ExecutablePath;
                if (Utility.CreateShortcut(dialog_save.FileName, toolname, argsString, null, description))
                {
                    MessageBox.Show(
                        Language.FrmTools_Shortcut_created_successfully,
                        Language.FrmTools_Shortcut,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        Language.FrmTools_Failed_to_create_the_shortcut__Unexpected_exception_occurred,
                        Language.FrmTools_Shortcut,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                dialog_save.FileName = string.Empty;
            }
        }

        private void ApplicationShortcutClick(object sender, EventArgs e)
        {
            while (string.IsNullOrWhiteSpace(txt_app_executable.Text) || !File.Exists(txt_app_executable.Text))
            {
                ApplicationExecutableClick(sender, e);
                if (DialogResult != DialogResult.OK)
                {
                    return;
                }
            }
            var programName = Path.GetFileNameWithoutExtension(txt_app_executable.Text);
            var icon = $"{txt_app_executable.Text},0";
            var description = string.Empty;
            var args = new List<string>();
            if (rb_app_surround.Checked)
            {
                args.Add(@"-aGoSurround");
                description = string.Format(Language.FrmTools_Open_in_surround_mode, programName);
            }
            else if (rb_app_extended.Checked)
            {
                args.Add(@"-aGoExtended");
                description = string.Format(Language.FrmTools_Open_in_extended_mode, programName);
            }
            if (cb_app_noSLI.Enabled && cb_app_noSLI.Checked)
            {
                args.Add(@"--nosli true");
            }
            args.Add($"-e \"{txt_app_executable.Text}\"");
            if (cb_app_args.Checked && !string.IsNullOrWhiteSpace(txt_app_args.Text))
            {
                args.Add($"--arguments=\"{txt_app_args.Text}\"");
            }
            if (cb_app_process.Checked && !string.IsNullOrWhiteSpace(txt_app_process.Text))
            {
                args.Add($"-w \"{txt_app_process.Text}\"");
                args.Add($"-t {nud_app_timeout.Value}");
            }
            var argsString = string.Join(" ", args);

            if (dialog_save.ShowDialog(this) == DialogResult.OK)
            {
                var toolname = Application.ExecutablePath;
                if (Utility.CreateShortcut(dialog_save.FileName, toolname, argsString, icon, description))
                {
                    MessageBox.Show(
                        Language.FrmTools_Shortcut_created_successfully,
                        Language.FrmTools_Shortcut,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        Language.FrmTools_Failed_to_create_the_shortcut__Unexpected_exception_occurred,
                        Language.FrmTools_Shortcut,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                dialog_save.FileName = string.Empty;
            }
        }

        private void ApplicationExecutableClick(object sender, EventArgs e)
        {
            if (dialog_open.ShowDialog(this) == DialogResult.OK)
            {
                if (File.Exists(dialog_open.FileName) && Path.GetExtension(dialog_open.FileName) == @".exe")
                {
                    txt_app_executable.Text = dialog_open.FileName;
                    txt_app_process.Text =
                        (Path.GetFileNameWithoutExtension(txt_app_executable.Text) ?? string.Empty).ToLower();
                    dialog_open.FileName = string.Empty;
                }
                else
                {
                    MessageBox.Show(
                        Language.FrmTools_Bad_file_selected__Please_select_an_executable_file,
                        Language.FrmTools_App_Select,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
        }

        private void ApplicationArgumentsCheckedChanged(object sender, EventArgs e)
        {
            txt_app_args.ReadOnly = !cb_app_args.Checked;
            if (txt_app_args.ReadOnly)
            {
                txt_app_args.Text = string.Empty;
            }
        }

        private void CommandOptionsCheckedChanged(object sender, EventArgs e)
        {
            cb_com_noSLI.Enabled = rb_com_extended.Checked || rb_com_toggle.Checked;
            cb_com_noSLI.Checked = cb_com_noSLI.Enabled && cb_com_noSLI.Checked;
        }

        private void ApplicationsOptionsCheckedChanged(object sender, EventArgs e)
        {
            cb_app_noSLI.Enabled = rb_app_extended.Checked;
            cb_app_noSLI.Checked = cb_app_noSLI.Enabled && cb_app_noSLI.Checked;
        }
    }
}