using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DZ.SAMP.Client.MVVM
{
    public class ViewModelBase : PropertyChangedObject, IDisposable
    {
        public static bool WriteDisposeToConsole = false;

        private bool _disposed = false;
        private readonly List<Action> _events = new List<Action>();

        public ViewModelBase()
        {
            ViewModelWatcher.Add(this);
        }


        protected void AddHandler(object source, string eventName, string methodName)
        {
            AddHandler<EventHandler>(source, eventName, methodName);
        }

        protected void AddHandler<TEventHandler>(object source, string eventName, string methodName)
        {
            try
            {
                var type = source.GetType();
                var info = type.GetEvent(eventName);

                var method = GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                var handler = Delegate.CreateDelegate(typeof(TEventHandler), this, method, true);

                info.AddEventHandler(source, handler);

                _events.Add(() => info.RemoveEventHandler(source, handler));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected void RemoveAllHandler()
        {
            try
            {
                while (_events.Count > 0)
                {
                    var action = _events[0];

                    action();

                    _events.RemoveAt(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                RemoveAllHandler();

                OnDispose();
            }

            if (WriteDisposeToConsole)
                Console.WriteLine(this + " disposed");

            _disposed = true;
        }
        protected virtual void OnDispose()
        {

        }

        ~ViewModelBase()
        {
            Dispose(false);
        }

        public virtual bool TryClose()
        {
            try
            {
                CleanUp();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return false;
            }
        }

        protected virtual void CleanUp()
        {

        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            var property = (MemberExpression)expression.Body;
            OnPropertyChanged(property.Member.Name);
        }

        protected void OnPropertyChanged(object sender, EventArgs e)
        {
            OnPropertyChanged();
        }
    }
}
