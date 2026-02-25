using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MeowTools.Modifiers;
using MeowTools.Modifiers.Fusion;
using MeowTools.Modifiers.Hybrid;

namespace MeowTools
{
    /// <summary>
    /// 植物大战僵尸修改器主窗口
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private sealed class GameVersion
        {
            
            public string Name { get; }
            public string ProcessName  { get; }
            public Func<Process, Modifier> Factory { get; }

            private GameVersion(string name, string processName, Func<Process, Modifier> factory)
            {
                Name = name;
                ProcessName = processName;
                Factory = factory;
            }

            private static readonly GameVersion Fusion = new GameVersion("融合版", "PlantsVsZombiesRH", process => new FusionModifier(process));
            
            private static readonly GameVersion Hybrid = new GameVersion("杂交版", "植物大战僵尸杂交版", process => new HybridModifier(process));

            public static readonly GameVersion[] Entries = { Fusion, Hybrid };
        }
        
        private Modifier _modifier;
        private bool _isGameRunning = false;
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeFeature();
        }
        
        /// <summary>
        /// 初始化功能
        /// </summary>
        private void InitializeFeature()
        {
            try
            {
                foreach (var version in GameVersion.Entries)
                {
                    var processes = Process.GetProcessesByName(version.ProcessName);
                    
                    if (processes.Length > 0)
                    {
                        ProcessStatusText.Text = version.Name;
                        ProcessStatusText.Foreground = System.Windows.Media.Brushes.Green;
                        StatusText.Text = "运行中";
                        
                        var process =  processes[0];
                        _modifier = version.Factory.Invoke(process);
                        _isGameRunning = true;
                        return;
                    }
                    
                    _isGameRunning = false;
                    ProcessStatusText.Text = "未检测到游戏进程";
                    ProcessStatusText.Foreground = System.Windows.Media.Brushes.Red;
                    StatusText.Text = "未检测到游戏进程";
                }
                
            }
            catch (Exception ex)
            {
                _isGameRunning = false;
                ProcessStatusText.Text = "检测失败";
                ProcessStatusText.Foreground = System.Windows.Media.Brushes.Orange;
                StatusText.Text = $"错误: {ex.Message}";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _isGameRunning = false;
            _modifier?.Close();
            base.OnClosed(e);
        }

        /// <summary>
        /// 游戏是否没有运行
        /// </summary>
        /// <returns>游戏没运行时为 true</returns>
        private bool AssertGameNotRunning()
        {
            if (_isGameRunning) return false;
            
            MessageBox.Show("请先启动游戏!", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return true;
        }
        
        /// <summary>
        /// 刷新检测按钮点击事件
        /// </summary>
        private void RefreshProcessButtonClick(object sender, RoutedEventArgs e)
        {
            InitializeFeature();
        }
        
        /// <summary>
        /// 设置阳光
        /// </summary>
        private void SetSunshineButtonClick(object sender, RoutedEventArgs e)
        {
            if (AssertGameNotRunning())
            {
                return;
            }

            if (long.TryParse(SunshineCountInput.Text, out var count))
            {
                // 这里添加实际的内存修改代码
                StatusText.Text = _modifier.ModifySunshine(count) ? $"阳光已设置为: {count}" : "阳光修改失败";
            }
            else
            {
                MessageBox.Show("请输入有效的数字!", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// 设置金币
        /// </summary>
        private void SetCoinButtonClick(object sender, RoutedEventArgs e)
        {
            if (AssertGameNotRunning())
            {
                return;
            }
            
            if (int.TryParse(CoinCountInput.Text, out var count))
            {
                // 这里添加实际的内存修改代码
                StatusText.Text = _modifier.ModifyCoin(count) ? $"金币已设置为: {count}" : "金币修改失败";
            }
            else
            {
                MessageBox.Show("请输入有效的数字!", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        /// <summary>
        /// 解锁所有植物
        /// </summary>
        private void PlantNoConsumeButtonClick(object sender, RoutedEventArgs e)
        {
            if (AssertGameNotRunning())
            {
                return;
            }
        }
        
        /// <summary>
        /// 无冷却时间
        /// </summary>
        private void NoCooldownButtonClick(object sender, RoutedEventArgs e)
        {
            if (AssertGameNotRunning())
            {
                return;
            }
            
            _modifier.PlantNoCooldown();
        }

        
    }
}