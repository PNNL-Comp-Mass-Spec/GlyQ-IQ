
using DeconTools.Backend.Core;
using Sipper.Model;

namespace Sipper.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region Constructors

        public MainWindowViewModel()
        {
            SipperProject = new Project();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public Project SipperProject { get; set; }

        public Run Run { get; set; }
        #endregion

    

    }
}
