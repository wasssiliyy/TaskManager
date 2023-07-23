using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TaskManager.Commands;
using TaskManager.Models;

namespace TaskManager.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {

        private ObservableCollection<Info> _runningPrograms = new ObservableCollection<Info>();

        public ObservableCollection<Info> RunningPrograms
        {
            get { return _runningPrograms; }
            set { _runningPrograms = value; OnPropertyChanged(); }
        }

        private Info _info;

        public Info Info
        {
            get { return _info; }
            set { _info = value; OnPropertyChanged(); }
        }

        private string _createOrEndProgramName;

        public string CreateOrEndProgramName
        {
            get { return _createOrEndProgramName; }
            set { _createOrEndProgramName = value; OnPropertyChanged(); }
        }

        private string _blockProgramName;

        public string BlockProgramName
        {
            get { return _blockProgramName; }
            set { _blockProgramName = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Info> _blockedPrograms = new ObservableCollection<Info>();

        public ObservableCollection<Info> BlockedPrograms
        {
            get { return _blockedPrograms; }
            set { _blockedPrograms = value; OnPropertyChanged(); }
        }


        public RelayCommand SelectionChanged { get; set; }
        public RelayCommand End { get; set; }
        public RelayCommand Create { get; set; }
        public RelayCommand AddBlockBoxButton { get; set; }

        public MainWindowViewModel()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            Info process = null;

            SelectionChanged = new RelayCommand((obj) =>
            {
                if (RunningPrograms.Count > 0)
                {
                    process = Info;
                }
            });

            End = new RelayCommand((obj) =>
            {
                if (process != null)
                {
                    var allProcess = Process.GetProcesses();
                    for (int i = 0; i < allProcess.Length; i++)
                    {
                        if (allProcess[i].ProcessName == process.Name)
                        {
                            allProcess[i].Kill();
                        }
                    }
                }
            });

            Create = new RelayCommand((obj) =>
            {
                if (BlockedPrograms.Count > 0)
                {
                    for (int i = 0; i < BlockedPrograms.Count; i++)
                    {
                        if (!CreateOrEndProgramName.Contains(BlockedPrograms[i].Name.ToLower()))
                        {
                            Process.Start($"{CreateOrEndProgramName}");
                        }
                        else
                        {
                            MessageBox.Show("This program is blocked");
                        }
                    }
                }
                else
                {
                    Process.Start($"{CreateOrEndProgramName}");
                }

                CreateOrEndProgramName = string.Empty;
            });

            AddBlockBoxButton = new RelayCommand((obj) =>
            {
                bool result = false;
                for (int i = 0; i < BlockedPrograms.Count; i++)
                {
                    if (BlockedPrograms[i].Name.ToLower() == BlockProgramName.ToLower())
                    {
                        result = true;
                        break;
                    }
                }

                if (!result)
                {
                    var programInfo = new Info();
                    programInfo.Name = BlockProgramName;
                    BlockedPrograms.Add(programInfo);

                    var allProcess = Process.GetProcesses();

                    for (int i = 0; i < allProcess.Length; i++)
                    {
                        for (int k = 0; k < BlockedPrograms.Count; k++)
                        {
                            if (allProcess[i].ProcessName.ToLower() == BlockedPrograms[k].Name.ToLower())
                            {
                                allProcess[i].Kill();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You can't add the program once again !");
                }

                BlockProgramName = string.Empty;
            });
        }

        private PerformanceCounter cpuCounter;
        private float GetCpuUsage(Process process)
        {
            cpuCounter.NextValue();
            //System.Threading.Thread.Sleep(10);
            return cpuCounter.NextValue() / Environment.ProcessorCount;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            RunningPrograms.Clear();
            var allProcess = Process.GetProcesses();
            for (int i = 0; i < allProcess.Length; i++)
            {
                if (BlockedPrograms.Count > 0)
                {
                    for (int k = 0; k < BlockedPrograms.Count; k++)
                    {
                        if (allProcess[i].ProcessName.ToLower() == BlockedPrograms[k].Name.ToLower())
                        {
                            allProcess[i].Kill();
                        }
                        else
                        {
                            var pragram = new Info();
                            pragram.Name = allProcess[i].ProcessName;
                            var cpu = GetCpuUsage(allProcess[i]);
                            pragram.CPU = cpu;
                            RunningPrograms.Add(pragram);
                        }
                    }
                }
                else
                {
                    var pragram = new Info();
                    pragram.Name = allProcess[i].ProcessName;
                    var cpu = GetCpuUsage(allProcess[i]);
                    pragram.CPU = cpu;
                    RunningPrograms.Add(pragram);
                }
            }

        }
    }
}
