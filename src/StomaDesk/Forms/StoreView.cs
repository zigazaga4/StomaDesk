using System;
using System.Windows.Forms;
using StomaDesk.Data;

namespace StomaDesk.Forms
{
    /// <summary>
    /// Base for the main window tabs. It holds the store and refreshes the view when data changes;
    /// a hidden tab only marks itself stale and refreshes when the user opens it.
    /// Not abstract, because the Visual Studio designer cannot open controls whose base class is abstract.
    /// </summary>
    public class StoreView : UserControl
    {
        private ClinicStore _store;
        private bool _stale;

        protected ClinicStore Store
        {
            get { return _store; }
        }

        public void Bind(ClinicStore store)
        {
            if (_store != null)
                _store.Changed -= Store_Changed;

            _store = store;
            _store.Changed += Store_Changed;
            OnBound();
            RefreshView();
        }

        /// <summary>Runs once the store is available (fill combo boxes, set default dates).</summary>
        protected virtual void OnBound()
        {
        }

        /// <summary>Reloads the view from the store.</summary>
        public virtual void RefreshView()
        {
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible && _stale && _store != null)
            {
                _stale = false;
                RefreshView();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _store != null)
                _store.Changed -= Store_Changed;
            base.Dispose(disposing);
        }

        private void Store_Changed(object sender, EventArgs e)
        {
            if (Visible)
                RefreshView();
            else
                _stale = true;
        }
    }
}
