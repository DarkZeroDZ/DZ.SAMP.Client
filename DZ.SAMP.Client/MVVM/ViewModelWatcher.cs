using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace DZ.SAMP.Client.MVVM
{
    public static class ViewModelWatcher
    {
        public static bool IsActive { get; private set; }

        private static readonly List<WeakReference> ViewModels = new List<WeakReference>();

        private static readonly object Lock = new object();

        public static void Start(int time = 30)
        {
            lock (Lock)
            {
                if (IsActive) return;

                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(time) };
                timer.Tick += (_, __) => Print();
                timer.Start();

                IsActive = true;
            }
        }

        public static void Add(ViewModelBase vm)
        {
            lock (Lock)
            {
                var reference = new WeakReference(vm, false);

                ViewModels.Add(reference);
            }
        }

        public static void Print()
        {
            lock (Lock)
            {
                var sb = new StringBuilder();

                var dead = ViewModels.Where(m => !m.IsAlive).ToList();

                sb.AppendLine();
                if (dead.Count > 0)
                {
                    sb.AppendLine("Removed ViewModels:");

                    foreach (var vm in dead)
                    {
                        sb.AppendLine("   " + vm.Target);
                    }

                    sb.AppendLine();
                }

                ViewModels.RemoveAll(m => !m.IsAlive);

                if (ViewModels.Count > 0)
                {
                    sb.AppendLine("Active ViewModels:");

                    foreach (var vm in ViewModels)
                    {
                        sb.AppendLine("   " + vm.Target);
                    }
                    sb.AppendLine();
                }

                Console.Write(sb.ToString());
            }
        }
    }
}
