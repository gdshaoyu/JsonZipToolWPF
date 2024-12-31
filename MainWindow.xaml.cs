using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JsonZipToolWPF.Data;
using JsonZipToolWPF.Models;
using Microsoft.EntityFrameworkCore;
using MaterialDesignThemes.Wpf;

namespace JsonZipToolWPF
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _dbContext;

        public MainWindow()
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
            InitializeEvents();
            InitializeDatabase();
            
            // 初始化MessageQueue
            TipsBar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        }

        private void InitializeDatabase()
        {
            _dbContext.Database.EnsureCreated();
        }

        private void InitializeEvents()
        {
            CompressButton.Click += CompressButton_Click;
            DecompressButton.Click += DecompressButton_Click;
            FormatJsonButton.Click += FormatJsonButton_Click;
            ClearButton.Click += ClearButton_Click;
            AboutButton.Click += AboutButton_Click;
            HistoryButton.Click += HistoryButton_Click;
            CloseButton.Click += CloseButton_Click;
        }

        private void SaveConversionRecord(string operationType, string input, string output)
        {
            var record = new ConversionRecord
            {
                OperationType = operationType,
                InputText = input,
                OutputText = output,
                ConversionTime = DateTime.Now
            };

            _dbContext.ConversionRecords.Add(record);
            _dbContext.SaveChanges();
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = InputTextBox.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    ShowTips("请输入要压缩的数据");
                    return;
                }

                string compressed = GZipCompress(input);
                SetOutputText(compressed);
                SaveConversionRecord("压缩", input, compressed);
                ShowTips("压缩成功！");
            }
            catch (Exception ex)
            {
                ShowTips($"压缩失败：{ex.Message}");
            }
        }

        private void DecompressButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = InputTextBox.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    ShowTips("请输入要解压的数据");
                    return;
                }

                string decompressed = GZipDecompress(input);
                SetOutputText(decompressed);
                SaveConversionRecord("解压", input, decompressed);
                ShowTips("解压成功！");
            }
            catch (Exception ex)
            {
                ShowTips($"解压失败：{ex.Message}");
            }
        }

        private void FormatJsonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = InputTextBox.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    ShowTips("请输入要格式化的JSON数据");
                    return;
                }

                JToken jToken = JToken.Parse(input);
                string formatted = jToken.ToString(Formatting.Indented);
                SetOutputText(formatted);
                SaveConversionRecord("格式化", input, formatted);
                ShowTips("JSON格式化成功！");
            }
            catch (Exception ex)
            {
                ShowTips($"JSON格式化失败：{ex.Message}");
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new HistoryWindow
            {
                Owner = this,
                ShowInTaskbar = false
            };
            historyWindow.Show();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
            OutputTextBox.Clear();
        }

        private void SetOutputText(string text)
        {
            OutputTextBox.Text = text;
        }

        private string GZipCompress(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            byte[] buffer = Encoding.UTF8.GetBytes(text);
            using var ms = new MemoryStream();
            using (var gzip = new GZipStream(ms, CompressionMode.Compress))
            {
                gzip.Write(buffer, 0, buffer.Length);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        private string GZipDecompress(string compressedText)
        {
            if (string.IsNullOrEmpty(compressedText)) return compressedText;

            try
            {
                byte[] gzipBuffer = Convert.FromBase64String(compressedText);
                using var ms = new MemoryStream(gzipBuffer);
                using var gzip = new GZipStream(ms, CompressionMode.Decompress);
                using var resultMs = new MemoryStream();
                gzip.CopyTo(resultMs);
                return Encoding.UTF8.GetString(resultMs.ToArray());
            }
            catch
            {
                return compressedText;
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow
            {
                Owner = this
            };
            aboutWindow.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _dbContext.Dispose();
        }

        private void ShowTips(string message)
        {
            TipsBar.MessageQueue?.Enqueue(message);
        }

        private void ColorZone_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Normal)
                    WindowState = WindowState.Maximized;
                else
                    WindowState = WindowState.Normal;
            }
            else
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
} 