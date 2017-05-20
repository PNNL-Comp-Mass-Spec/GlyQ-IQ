using System;
using System.ComponentModel;

namespace YAFMS_DB.PNNLOmics
{
    [Serializable]
    public abstract class BaseData: INotifyPropertyChanged
    {
        int ID { get; set; }
        public abstract void Clear();        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
