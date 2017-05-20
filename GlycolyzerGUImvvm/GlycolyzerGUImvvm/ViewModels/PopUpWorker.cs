using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycolyzerGUImvvm.Views;

namespace GlycolyzerGUImvvm.ViewModels
{
    class PopUpWorker
    {
        private volatile bool _shouldStop;
        //private volatile LoadingPopUp loadingPopUp = new LoadingPopUp();
        //private volatile SavingPopUp savingPopUp = new SavingPopUp();

        public void DoLoadWork()
        {
            LoadingPopUp loadingPopUp = new LoadingPopUp();
            while (!_shouldStop)
            {
                loadingPopUp.Show();
            }
            loadingPopUp.FinishedLoading();
            App.Pause();
            loadingPopUp.Close();
        }

        public void DoSaveWork()
        {
            SavingPopUp savingPopUp = new SavingPopUp();
            while (!_shouldStop)
            {
                savingPopUp.Show();
            }
            savingPopUp.FinishedSaving();
            App.Pause();
            savingPopUp.Close();
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }
    }
}
