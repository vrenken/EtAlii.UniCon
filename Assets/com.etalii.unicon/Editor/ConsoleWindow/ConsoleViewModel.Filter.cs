namespace EtAlii.UniCon.Editor
{
    using System;
    using UniRx;
    using UnityEngine;

    public partial class ConsoleViewModel
    {
        /// <summary>
        /// Gets raised when any property related to the filter has changed and the view needs to update itself accordingly.
        /// </summary>
        public event Action FilterChanged;

        public readonly ReactiveCommand<FilterMapping> OnAddExcludeFilterClicked = new();
        public readonly ReactiveCommand<FilterMapping> OnAddIncludeFilterClicked = new();

        private void SetupFilters()
        {
            OnAddIncludeFilterClicked.Subscribe(e => Debug.Log($"[FILTERS] Request for include filter: {e.Property.Key} = {e.Property.Value.ToString()}"));
            OnAddExcludeFilterClicked.Subscribe(e => Debug.Log($"[FILTERS] Request for exclude filter: {e.Property.Key} = {e.Property.Value.ToString()}"));
        }
    }    
}
