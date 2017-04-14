﻿using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Maple.Core
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Maple.Core.ObservableObject" />
    /// <autogeneratedoc />
    public class FileSystemViewModel : ObservableObject
    {
        protected readonly BusyStack _busyStack;

        /// <summary>
        /// Gets the select command.
        /// </summary>
        /// <value>
        /// The select command.
        /// </value>
        /// <autogeneratedoc />
        public ICommand SelectCommand { get; private set; }

        private RangeObservableCollection<IFileSystemInfo> _selectedItems;
        /// <summary>
        /// Gets the selected items.
        /// </summary>
        /// <value>
        /// The selected items.
        /// </value>
        /// <autogeneratedoc />
        public RangeObservableCollection<IFileSystemInfo> SelectedItems
        {
            get { return _selectedItems; }
            private set { SetValue(ref _selectedItems, value); }
        }

        private RangeObservableCollection<MapleDrive> _drives;
        /// <summary>
        /// Gets the drives.
        /// </summary>
        /// <value>
        /// The drives.
        /// </value>
        /// <autogeneratedoc />
        public RangeObservableCollection<MapleDrive> Drives
        {
            get { return _drives; }
            private set { SetValue(ref _drives, value); }
        }

        private MapleFileSystemContainerBase _selectedItem;
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>
        /// The selected item.
        /// </value>
        /// <autogeneratedoc />
        public MapleFileSystemContainerBase SelectedItem
        {
            get { return _selectedItem; }
            set { SetValue(ref _selectedItem, value, OnChanged: OnSelectedItemChanged); }
        }

        private bool _isBusy;
        /// <summary>
        /// Gets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        /// <autogeneratedoc />
        public bool IsBusy
        {
            get { return _isBusy; }
            private set { SetValue(ref _isBusy, value); }
        }

        private string _filter;
        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        /// <autogeneratedoc />
        public string Filter
        {
            get { return _filter; }
            set { SetValue(ref _filter, value, OnChanged: () => SelectedItem.OnFilterChanged(Filter)); }
        }

        private bool _displayListView;
        /// <summary>
        /// Gets or sets a value indicating whether [display ListView].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [display ListView]; otherwise, <c>false</c>.
        /// </value>
        /// <autogeneratedoc />
        public bool DisplayListView
        {
            get { return _displayListView; }
            set { SetValue(ref _displayListView, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemViewModel"/> class.
        /// </summary>
        /// <autogeneratedoc />
        public FileSystemViewModel()
        {
            _busyStack = new BusyStack();
            _busyStack.OnChanged += (hasItems) => IsBusy = hasItems;
            SelectCommand = new RelayCommand<IFileSystemInfo>(SetSelectedItem, CanSetSelectedItem);

            using (_busyStack.GetToken())
            {
                DisplayListView = false;

                Drives = new RangeObservableCollection<MapleDrive>();
                SelectedItems = new RangeObservableCollection<IFileSystemInfo>();

                var drives = DriveInfo.GetDrives()
                                        .Where(p => p.IsReady && p.DriveType != DriveType.CDRom && p.DriveType != DriveType.Unknown)
                                        .Select(p => new MapleDrive(p, new FileSystemDepth(0)))
                                        .ToList();
                Drives.AddRange(drives);
            }
        }

        private void OnSelectedItemChanged()
        {
            SelectedItem.Load();
            SelectedItem.LoadMetaData();
        }

        /// <summary>
        /// Sets the selected item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <autogeneratedoc />
        public void SetSelectedItem(IFileSystemInfo item)
        {
            var value = item as MapleFileSystemContainerBase;

            if (value == null)
                return;

            SelectedItem = value;
            SelectedItem.ExpandPath();
            SelectedItem.Parent.IsSelected = true;
        }

        private bool CanSetSelectedItem(IFileSystemInfo item)
        {
            var value = item as MapleFileSystemContainerBase;

            return value != null && value != SelectedItem;
        }
    }
}
