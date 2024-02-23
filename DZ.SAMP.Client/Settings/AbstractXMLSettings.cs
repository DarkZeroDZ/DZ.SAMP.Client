using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows;
using System.Xml.Linq;

namespace DZ.SAMP.Client.Settings
{
    public abstract class AbstractXMLSettings : AbstractXMLItem
    {
        #region Properties
        public virtual int Version { get; }
        public bool FirstStart { get; }

        public virtual string FullFileName => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SA-MP Launcher", this.Name + ".xml");

        public virtual string Name => "SA-MP Launcher_Settings";
        #endregion

        #region Events
        public event HandledEventHandler BeforSaving;
        public event EventHandler Saved;
        public event EventHandler Loaded;
        public event EventHandler Reseted;
        public event EventHandler Imported;
        #endregion

        #region Fields
        private readonly object _lock = new object();
        #endregion

        #region Constructor
        public AbstractXMLSettings(bool load = true)
        {
            this.FirstStart = !File.Exists(this.FullFileName);

            if (!load) return;

            if (!this.FirstStart)
                this.Load();
            else
            {
                this.LoadDefault();
                this.Save();
            }
        }

        public AbstractXMLSettings(XElement e) : base(e) { }
        #endregion

        #region Methods
        public virtual void LoadDefault() { }

        #region Load
        public void Load() => this.Load(this.FullFileName);

        public void Load(string fileName)
        {
            try
            {
                XElement e;

                lock (this._lock)
                    e = XElement.Load(fileName);

                e.Elements().ToList().ForEach(x => this.TrySet(x));

                this.Loaded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                var res = MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + "Basiseinstellungen laden?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Error);

                if (res == MessageBoxResult.Yes)
                {
                    this.LoadDefault();
                }
                else
                {
                    Environment.Exit(-1);
                }
            }
        }
        #endregion

        #region Save
        public virtual bool Save() { return this.Save(this.FullFileName); }
        public virtual bool Save(string fileName)
        {
            try
            {
                var e = new HandledEventArgs();
                this.BeforSaving?.Invoke(this, e);
                if (e.Handled) return false;

                var info = new FileInfo(fileName);
                if (!info.Directory.Exists)
                    info.Directory.Create();

                lock(this._lock)
                    this.ToXElement().Save(fileName);

                this.Saved?.Invoke(this, EventArgs.Empty);

                var dInfo2 = new DirectoryInfo(fileName);
                var dSecurity2 = dInfo2.GetAccessControl();
                dSecurity2.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    FileSystemRights.FullControl,
                    InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                    PropagationFlags.NoPropagateInherit,
                    AccessControlType.Allow));

                dInfo2.SetAccessControl(dSecurity2);

                return true;
            }
            catch (Exception er)
            {
                Debug.WriteLine(er.ToString());
                MessageBox.Show(er.Message);
                return false;
            }
        }
        #endregion

        
        #endregion
    }
}
