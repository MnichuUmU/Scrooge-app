using CommunityToolkit.Maui.Views;
using Scrooge_app.Databases;

namespace Scrooge_app.Views;
public partial class PeopleView : ContentPage, IQueryAttributable
{

    private People SelectedPerson { get; set; }
    private readonly LocalDbService _dbService;
    public PeopleView( LocalDbService dbService )
    {
        InitializeComponent();
        _dbService = dbService;
    }
    public void ApplyQueryAttributes( IDictionary<string , object> query )
    {
        if(query.ContainsKey("PersonData"))
        {
            SelectedPerson = query["PersonData"] as People;
            SetUi();
        }
    }


    private void SetUi()
    {
        if(SelectedPerson != null)
        {
            img_pfp.Source = SelectedPerson.ProfilePick;
            lbl_name.Text = SelectedPerson.Name;
            lbl_surname.Text = SelectedPerson.Surname;
            lbl_info_number.Text = string.IsNullOrWhiteSpace(SelectedPerson.Phone) ? "no data" : SelectedPerson.Phone;
            lbl_info_mail.Text = string.IsNullOrWhiteSpace(SelectedPerson.Mail) ? "no data" : SelectedPerson.Mail;
            lbl_info_family.Text = string.IsNullOrWhiteSpace(SelectedPerson.Mail) ? "no data" : SelectedPerson.Family;
            lbl_info_fb.Text = string.IsNullOrWhiteSpace(SelectedPerson.Facebook) ? "no data" : "Go to profile";
            lbl_info_fb.GestureRecognizers.Clear();

            if(!string.IsNullOrWhiteSpace(SelectedPerson.Facebook))
            {
                lbl_info_fb.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        try { await Launcher.Default.OpenAsync(SelectedPerson.Facebook); }
                        catch { await DisplayAlert("Error" , "Invalid Facebook link format." , "OK"); }
                    })
                });
            }
            lbl_info_insta.Text = string.IsNullOrWhiteSpace(SelectedPerson.Instagram) ? "no data" : "Go to profile";
            lbl_info_insta.GestureRecognizers.Clear();

            if(!string.IsNullOrWhiteSpace(SelectedPerson.Instagram))
            {
                lbl_info_insta.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        try { await Launcher.Default.OpenAsync(SelectedPerson.Instagram); }
                        catch { await DisplayAlert("Error" , "Invalid Instagram link format." , "OK"); }
                    })
                });
            }
        }
    }
    protected override async void OnNavigatedTo( NavigatedToEventArgs args )
    {
        base.OnNavigatedTo(args);
        var popup = new LoadingPopupView();
        this.ShowPopup(popup);
        try
        {
            var logs = await _dbService.GetLogsList(SelectedPerson.Id);
            if(logs != null)
            {
                lv_logs.ItemsSource = logs;
                lbl_info_couter.Text = logs.Count.ToString();
            }
            else
            {
                throw new Exception("Database error, no data returned");
            }
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error" , $"Could not load recipes: {ex.Message}" , "OK");
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                popup.Close();
            });
        }
    }

    private async void UiUpdate()
    {
        try
        {
            var logs = await _dbService.GetLogsList(SelectedPerson.Id);
            if(logs != null)
            {
                lv_logs.ItemsSource = logs;
                lbl_info_couter.Text = logs.Count.ToString();
            }
            else
            {
                throw new Exception("Database error, no data returned");
            }
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error" , $"Could not load recipes: {ex.Message}" , "OK");
        }
    }

    private async void btn_addlog_Clicked( object sender , EventArgs e )
    {
        var navParam = new Dictionary<string , object>{
                { "PersonData", SelectedPerson }
            };
        await Shell.Current.GoToAsync(nameof(AddLogPage) , navParam);
    }
    private async void OnDeleteInvoked( object sender , EventArgs e )
    {
        var swipeItem = sender as SwipeItem;
        var logDisplay = swipeItem?.BindingContext as LogListDisplayModel;

        if(logDisplay != null)
        {
            bool ans = await DisplayAlert("Are you shure?" , $"You won't be able to get back the lost data from {logDisplay.LogName}..." , "DELETE" , "GO BACK");

            if(ans)
            {
                bool change_balance = await DisplayAlert("Do you want to change balance value?" , $"Do you want to change balance by: {logDisplay.Amount}..." , "CHANGE" , "DON'T CHANGE");
                var popup = new LoadingPopupView();
                this.ShowPopup(popup);

                try
                {
                    if(change_balance)
                    {
                        await _dbService.DeleteLogById(logDisplay.LogId , SelectedPerson.Id , logDisplay.Amount);
                    }
                    else
                    {
                        await _dbService.DeleteLogById(logDisplay.LogId , null , null);
                    }

                }
                finally
                {
                    popup.Close();
                }
                UiUpdate();

            }
        }
        else
        {
            await DisplayAlert("Error" , "Something went wrong!" , "OK");
        }
    }

}