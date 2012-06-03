﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SQLRepeater.Entities.Generators.Collections
{
    /// <summary>
    /// To enable binding to the NiceString property to the DataGrid.
    /// </summary>
    public class GeneratorParameterCollection : ObservableCollection<GeneratorParameter>
    {

        /// <summary>
        /// Returns a humanly readable string that describes the GeneratorParameters in the collection.
        /// Format: Name: Value
        /// </summary>
        /// <returns>readable string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var s in this.Items)
            {
                sb.AppendFormat("{{{0}: {1}}}; ", s.ParameterName, s.Value);
            }
            return sb.ToString();
        }


        /// <summary>
        /// Bindable ToString property
        /// </summary>
        public string NiceString
        {
            get { return this.ToString(); }
        }

        
        public GeneratorParameterCollection() 
            : base()
        {
            this.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(GeneratorParameterCollection_CollectionChanged);
            
        }

        /// <summary>
        /// To notify about updates of the NiceString property we need to hook up events of the internal collection and add event handler to all the added items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GeneratorParameterCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        ((GeneratorParameter)item).PropertyChanged += GeneratorParameterCollection_PropertyChanged;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        ((GeneratorParameter)item).PropertyChanged -= GeneratorParameterCollection_PropertyChanged;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    foreach (var item in e.OldItems)
                    {
                        ((GeneratorParameter)item).PropertyChanged -= GeneratorParameterCollection_PropertyChanged;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// If any of the properties of the Items in the collections change, then we will trigger the PropertyChanged event for the NiceString property.
        /// This way we can bind to NiceString in the GUI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GeneratorParameterCollection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("NiceString"));
        }
    }
}
