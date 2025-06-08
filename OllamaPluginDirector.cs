using MemoQ.Addins.Common.Framework;
using MemoQ.MTInterfaces;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;

[assembly: MemoQ.Addins.Common.Framework.Module(
    ModuleName = "Ollama MT",
    ClassName = "MemoQOllamaPlugin.OllamaPluginDirector")]

namespace MemoQOllamaPlugin
{
    [Serializable]
    public class OllamaPluginSettings
    {
        public bool IsEnabled { get; set; } = true;
        public string ApiEndpoint { get; set; } = "http://localhost:11434";
        public string Model { get; set; } = "llama2";
    }

    public class OllamaPluginDirector : PluginDirectorBase, IModule
    {
        private IEnvironment _environment;
        private readonly Image _displayIcon;
        private OllamaPluginSettings _settings = new OllamaPluginSettings();
        private string _settingsDirectory;

        public OllamaPluginDirector()
        {
            _displayIcon = CreateDefaultIcon();
        }

        public override IEnvironment Environment
        {
            set { _environment = value; }
        }

        public override bool BatchSupported => true;
        public override string CopyrightText => "© 2025 hwzieba";
        public override Image DisplayIcon => _displayIcon;
        public override string FriendlyName => "Ollama Machine Translation";
        public override bool InteractiveSupported => true;
        public override string PluginID => "OllamaMTPlugin";
        public override bool StoringTranslationSupported => false;

        public bool IsActivated => _settings.IsEnabled;

        public void Initialize(IModuleEnvironment env)
        {
            try
            {
                _settingsDirectory = Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                    "MemoQ",
                    "OllamaPlugin");
                LoadSettings();
                Log("Plugin initialized");
            }
            catch (Exception ex)
            {
                LogError($"Initialization failed: {ex}");
                ShowError($"Initialization failed: {ex.Message}");
            }
        }

        public void Cleanup()
        {
            SaveSettings();
            _displayIcon?.Dispose();
        }

        public override IEngine2 CreateEngine(CreateEngineParams args)
        {
            // Przekazanie ustawień do konstruktora OllamaEngine
            return new OllamaEngine(_settings.ApiEndpoint, _settings.Model);
        }

        public override bool IsLanguagePairSupported(LanguagePairSupportedParams args)
        {
            return true;
        }

        public override MemoQ.MTInterfaces.PluginSettings EditOptions(IWin32Window parentForm, MemoQ.MTInterfaces.PluginSettings settings)
        {
            try
            {
                using (var form = new Form())
                {
                    form.Text = "Ollama Plugin Settings";
                    form.Size = new Size(500, 300);
                    form.StartPosition = FormStartPosition.CenterParent;

                    var chkEnabled = new CheckBox
                    {
                        Text = "Enable Ollama MT",
                        Checked = _settings.IsEnabled,
                        Location = new Point(20, 20),
                        AutoSize = true
                    };

                    var lblEndpoint = new Label { Text = "API Endpoint:", Location = new Point(20, 60) };
                    var txtEndpoint = new TextBox { Text = _settings.ApiEndpoint, Location = new Point(120, 60), Width = 300 };

                    var lblModel = new Label { Text = "Model:", Location = new Point(20, 100) };
                    var txtModel = new TextBox { Text = _settings.Model, Location = new Point(120, 100), Width = 300 };

                    var btnSave = new Button
                    {
                        Text = "Save",
                        DialogResult = DialogResult.OK,
                        Location = new Point(20, 140)
                    };

                    form.Controls.AddRange(new Control[] { chkEnabled, lblEndpoint, txtEndpoint, lblModel, txtModel, btnSave });

                    if (form.ShowDialog(parentForm) == DialogResult.OK)
                    {
                        _settings.IsEnabled = chkEnabled.Checked;
                        _settings.ApiEndpoint = txtEndpoint.Text;
                        _settings.Model = txtModel.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"EditOptions failed: {ex}");
                ShowError($"Settings error: {ex.Message}");
            }

            // Zwróć nową instancję podstawowych ustawień memoQ
            return new MemoQ.MTInterfaces.PluginSettings(string.Empty);
        }

        private Image CreateDefaultIcon()
        {
            var bmp = new Bitmap(256, 256);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font("Arial", 48))
            using (var brush = new SolidBrush(Color.Blue))
            {
                g.Clear(Color.White);
                g.DrawString("O", font, brush, new PointF(80, 80));
            }
            return bmp;
        }

        private void LoadSettings()
        {
            try
            {
                var path = Path.Combine(_settingsDirectory, "settings.bin");
                if (File.Exists(path))
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        _settings = (OllamaPluginSettings)serializer.Deserialize(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to load settings: {ex}");
            }
        }

        private void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(_settingsDirectory);
                var path = Path.Combine(_settingsDirectory, "settings.bin");

                using (var stream = File.Create(path))
                {
                    var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    serializer.Serialize(stream, _settings);
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to save settings: {ex}");
            }
        }

        private void Log(string message)
        {
            try
            {
                File.AppendAllText(Path.Combine(_settingsDirectory, "ollama_log.txt"),
                    $"{DateTime.Now}: {message}\n");
            }
            catch { }
        }

        private void LogError(string message)
        {
            Log($"ERROR: {message}");
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ollama Plugin Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}