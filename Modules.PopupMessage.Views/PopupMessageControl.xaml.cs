using System.Windows.Controls;
using Modules.PopupMessage.Contracts;

namespace Modules.PopupMessage.Views
{
    /// <summary>
    /// Interaction logic for PopupMessageControl.xaml
    /// </summary>
    public partial class PopupMessageControl : UserControl, IPopupMessageControl
    {
        public PopupMessageControl()
        {
            InitializeComponent();
        }

        public PopupMessageControl(PopupMessageManager manager) : this()
        {
            ArgumentNullException.ThrowIfNull(manager);
            DataContext = manager;
        }
    }
}
