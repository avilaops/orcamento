using Roncav_Budget.ViewModels;

namespace Roncav_Budget.Views;

public partial class NovaContaPage : ContentPage
{
    public NovaContaPage(NovaContaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
