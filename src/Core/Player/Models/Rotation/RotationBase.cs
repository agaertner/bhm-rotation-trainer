using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Nekres.RotationTrainer.Core.Player.Models {
    internal abstract class RotationBase<T> : IEnumerable<T> where T : ActionBase {

        protected const string DELIMITER = "╡\t╞";

        public event EventHandler<EventArgs> Changed;

        private ObservableCollection<T> _abilities;

        protected RotationBase(IEnumerable<T> abilities) {
            _abilities                   =  new ObservableCollection<T>(abilities);
            _abilities.CollectionChanged += OnAbilitiesChanged;
            this.RenewChangeHandlers();
        }

        private void OnAbilitiesChanged(object o, NotifyCollectionChangedEventArgs e) {
            Changed?.Invoke(this, EventArgs.Empty);
            this.RenewChangeHandlers();
        }

        private void RenewChangeHandlers() {
            foreach (var ability in _abilities) {
                ability.Changed -= OnChanged;
                ability.Changed += OnChanged;
            }
        }

        protected virtual void OnChanged(object o, EventArgs e) {
            Changed?.Invoke(o, e);
        }

        public IEnumerator<T> GetEnumerator() {
            return _abilities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public T this[int i] => _abilities[i];

        public bool Remove(T action) {
            return _abilities.Remove(action);
        }

        public void RemoveAt(int index) {
            _abilities.RemoveAt(index);
        }

        public void Insert(int index, T action) {
            _abilities.Insert(index, action);
        }

        public void Add(T action) {
            _abilities.Add(action);
        }

        public override string ToString() {
            return !_abilities.Any() ? string.Empty : string.Join(DELIMITER, _abilities.Select(x => x.ToString()));
        }
    }
}
