using System;
using System.Windows;
using System.Linq;
using JsonZipToolWPF.Data;
using JsonZipToolWPF.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;

namespace JsonZipToolWPF
{
    public partial class HistoryWindow : Window
    {
        private readonly AppDbContext _dbContext;
        private readonly DispatcherTimer _refreshTimer;

        public HistoryWindow()
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
            
            // 初始化MessageQueue
            TipsBar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
            
            // 创建定时器，每秒刷新一次历史记录
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            HistoryListView.SelectionChanged += HistoryListView_SelectionChanged;

            LoadHistory();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadHistory();
        }

        private void LoadHistory()
        {
            // 保存当前选中的记录ID
            int? selectedId = null;
            if (HistoryListView.SelectedItem is ConversionRecord selectedRecord)
            {
                selectedId = selectedRecord.Id;
            }

            // 获取最近20条记录
            var records = _dbContext.ConversionRecords
                .OrderByDescending(r => r.ConversionTime)
                .Take(20)
                .ToList();

            HistoryListView.ItemsSource = records;

            // 恢复选中状态
            if (selectedId.HasValue)
            {
                var recordToSelect = records.FirstOrDefault(r => r.Id == selectedId);
                if (recordToSelect != null)
                {
                    HistoryListView.SelectedItem = recordToSelect;
                }
            }
        }

        private void HistoryListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (HistoryListView.SelectedItem is ConversionRecord record)
            {
                DetailTextBoxIn.Text = record.InputText;
                DetailTextBoxOut.Text = record.OutputText;
            }
            else
            {
                DetailTextBoxIn.Clear();
                DetailTextBoxOut.Clear();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _refreshTimer.Stop();
            _dbContext.Dispose();
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

        private async void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("确定要清空所有历史记录吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _dbContext.ConversionRecords.RemoveRange(_dbContext.ConversionRecords);
                await _dbContext.SaveChangesAsync();
                DetailTextBoxIn.Clear();
                DetailTextBoxOut.Clear();
                LoadHistory();
                TipsBar.MessageQueue?.Enqueue("历史记录已清空");
            }
        }
    }
} 