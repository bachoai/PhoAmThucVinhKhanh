using Quan4CulinaryTourism.Mobile.ViewModels;

namespace Quan4CulinaryTourism.Mobile.Views;

public partial class QrEntryPage : ContentPage
{
    public QrEntryPage(QrEntryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
