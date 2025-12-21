using Roncav_Budget.ViewModels;

namespace Roncav_Budget.Views;

public partial class RelatoriosPage : ContentPage
{
    private readonly RelatoriosViewModel _viewModel;

    public RelatoriosPage(RelatoriosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarDadosAsync();
    }
}
