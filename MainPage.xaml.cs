using CommunityToolkit.Maui.Views;
using Scrooge_app.Databases;
using Scrooge_app.Views;
using System.Diagnostics;


namespace Scrooge_app
{
    //podpinanie do bazy danych
    public partial class MainPage : ContentPage
    {
        private readonly LocalDbService _dbService;
        private bool _isFirstLoad = true;
        public MainPage( LocalDbService dbService )
        {
            InitializeComponent();
            _dbService = dbService;


        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if(_isFirstLoad)
            {
                _isFirstLoad = false;

                var popup = new LoadingPopupView();
                this.ShowPopup(popup);
                try
                {
                    await Refresh();
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                finally
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        popup.Close();
                    });
                }
            }


        }
        protected override async void OnNavigatedTo( NavigatedToEventArgs args )
        {
            if(!_isFirstLoad)
            {
                base.OnNavigatedTo(args);
                
                try
                {
                    await Refresh();
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                
            }

        }

        private async Task Refresh()
        {
            lv_people.IsVisible = true;
            img_empty.IsVisible = false;
            try
            {
                var people = await _dbService.GetPeople();
                if(people != null && people.Count() != 0)
                {
                    lv_people.ItemsSource = null;
                    lv_people.ItemsSource = people;
                }
                else
                {
                    lv_people.IsVisible = false;
                    img_empty.IsVisible = true;
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error" , $"Could not load people: {ex.Message}" , "OK");
            }
        }
        //przechodzi w peopleview i przesyła obiekt people odpowiadający klikniętemu przedmiotowi z listview
        private async void lv_people_ItemTapped( object sender , ItemTappedEventArgs e )
        {
            var person = (People)e.Item;
            var navParam = new Dictionary<string , object>{
                { "PersonData", person }
            };
            await Shell.Current.GoToAsync(nameof(PeopleView) , navParam);
            ( (ListView)sender ).SelectedItem = null;
        }
        //po kliknięciu na ikone śmietnika obok przedmiotu z listview usuwa odpowiadające dane z bazy danych odśiweża ui
        private async void OnDeleteInvoked( object sender , EventArgs e )
        {
            var listItem = sender as MenuItem;
            var person = listItem?.BindingContext as People;

            if(person != null)
            {
                bool ans = await DisplayAlert("Are you shure?" , $"You won't be able to get back the lost data from {person.Name}..." , "DELETE" , "GO BACK");

                if(ans)
                {
                    var popup = new LoadingPopupView();
                    this.ShowPopup(popup);

                    try
                    {
                        await _dbService.DeletePerson(person);
                    }
                    finally
                    {
                        popup.Close();
                    }
                    _isFirstLoad = true;
                    OnAppearing();

                }
            }
            else
            {
                await DisplayAlert("Error" , "Something went wrong!" , "OK");
            }
        }

        //https://colorhunt.co/palette/0000003d0000950101ff0000
        //lub
        //https://colorhunt.co/palette/f3f4f4853953612d532c2c2c
        //lub
        //https://colorhunt.co/palette/000000cf0f47ff0b55ffdede
        //
        //Create a hyperlink
        //https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/label?view=net-maui-10.0#create-a-hyperlink
        //

    }
}
