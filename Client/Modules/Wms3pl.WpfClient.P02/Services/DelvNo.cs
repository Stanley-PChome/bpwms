using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace Wms3pl.WpfClient.P02.Services
{
    public class DelvNo : INotifyPropertyChanged
    {
        #region Id

        private string _id;

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        #endregion

        #region ParentId
        private string _parentId;

        public string ParentId
        {
          get { return _parentId; }
          set
          {
            _parentId = value;
            NotifyPropertyChanged("ParentId");
          }
        }
        #endregion

        #region Name

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        #endregion

        //樹狀結構階層
        #region Level

        private int _level;

        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                NotifyPropertyChanged("Level");
            }
        }

        #endregion

        //樹狀節點背景色
        #region BgColor
        private Brush _bgColor = Brushes.Transparent;
        public Brush BgColor
        {
          get { return _bgColor; }
          set
          {
            _bgColor = value;
            NotifyPropertyChanged("BgColor");
          }
        }
        #endregion

        public DelvNo Parent { get; set; }

        #region IsChecked

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void SetIsChecked(bool value, bool updateChildren, bool updateParent)
        {
            //沒改
            if (value == _isChecked) return;

            _isChecked = value;

            //本項目改變後，子項目也全部要變
            if (updateChildren)
                DelvNos.ForEach(c => c.SetIsChecked(value, true, false));


            if (Parent != null && updateParent)
            {
                if (!_isChecked)
                {
                    //本項目是未checked。改變後，如果同一階層都未check，則父項目也要一起改成未 check
                    if (this.Level <= 2 && !Parent.DelvNos.Any(i => i.IsChecked))
                        Parent.SetIsChecked(false, false, true);
                }
                else
                {
                    //本項目是 checked，改變後，所有的父項目也要改成 checked
                    Parent.SetIsChecked(true, false, true);
                }
            }
            NotifyPropertyChanged("IsChecked");
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Id, Name);
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #region DelvNo
        private List<DelvNo> _delvNos = new List<DelvNo>();

        public List<DelvNo> DelvNos
        {
            get { return _delvNos; }
            set { _delvNos = value; }
        }
        #endregion    }
    }
}
