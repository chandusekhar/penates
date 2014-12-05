using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Penates.Utils.Objects {
    public class Nodo<T> {
        List<Nodo<T>> Children = new List<Nodo<T>>();

        T Item { get; set; }

        public Nodo() {
        }

        public Nodo(T item) {
            this.Item = item;
        }

        public void Add(T item) {
            this.Item = item;
        }

        public Nodo<T> AddChild(T item) {
            Nodo<T> nodeItem = new Nodo<T>(item);
            Children.Add(nodeItem);
            return nodeItem;
        }

        public bool AddChild(Nodo<T> nodo) {
            try {
                Children.Add(nodo);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public List<Nodo<T>> getChildren() {
            return this.Children;
        }

        public T getItem() {
            return this.Item;
        }

        public int countChildren() {
            if (this.Children == null) {
                return 0;
            }
            return this.Children.Count;
        }
    }
}