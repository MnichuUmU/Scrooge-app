using Scrooge_app.Databases;
using System.Diagnostics;
using System.Globalization;

namespace Scrooge_app;
public partial class AddLogPage : ContentPage, IQueryAttributable
{
    string _currentPhotoPath = "";

    //tworszy obiekt który przechwici przes³any obiekt people
    private People SelectedPerson { get; set; }
    // podpinanie pod baze danych
    private readonly LocalDbService _dbService;
    public AddLogPage( LocalDbService dbService )
    {
        InitializeComponent();
        _dbService = dbService;
    }
    //przypisuje obiektowy SelectedPerson przes³any obiekt people
    public void ApplyQueryAttributes( IDictionary<string , object> query )
    {
        if(query.ContainsKey("PersonData"))
        {
            SelectedPerson = query["PersonData"] as People;
        }
    }
    //Przy w³¹czeniu aplikacji pokazuje popup ³adowania i zamyka go po za³adowaniu informacji do ui
    protected override async void OnNavigatedTo( NavigatedToEventArgs args )
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

    private async Task Refresh()
    {
        var people = await _dbService.GetPeople();
        DateTime InAWeek = DateTime.Now.AddDays(7);
        due_picker.Date = InAWeek;
        pick_person.ItemsSource = people;

        if(SelectedPerson != null)
        {
            pick_person.SelectedItem = people.FirstOrDefault(p => p.Id == SelectedPerson.Id);
        }
        else
        {
            pick_person.SelectedItem = null;
        }

        ent_name.Text = null;
        edit_description.Text = null;
        ent_amount.Text = null;
    }
    //validuje wpisany balance i stwierdza czy to porzyczka czy zwrot
    private void ent_amount_TextChanged( object sender , TextChangedEventArgs e )
    {
        string text = ent_amount.Text;
        if(!string.IsNullOrWhiteSpace(text))
        {
            text.Replace("–" , "-").Replace("—" , "-").Replace(" " , "");
            if(decimal.TryParse(text , NumberStyles.Any , CultureInfo.InvariantCulture , out decimal money))
            {
                if(money > 0)
                {
                    lbl_status.Text = "LOAN / YOU REPAY";
                }
                else if(money < 0)
                {
                    lbl_status.Text = "REPAYMENT / YOU BORROW";
                }
                else
                {
                    lbl_status.Text = "INVALID VALUE";
                }
            }
        }
    }
    private async void ent_add_proof_Clicked( object sender , EventArgs e )
    {
        try
        {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();

            if(photo == null) return;

            string localFolder = FileSystem.AppDataDirectory;
            string fileName = $"pfp_{Guid.NewGuid()}.jpg";
            string targetPath = Path.Combine(localFolder , fileName);

            using(Stream stream = await photo.OpenReadAsync())
            using(FileStream targetSteam = File.Create(targetPath))
            {
                await stream.CopyToAsync(targetSteam);
            }

            _currentPhotoPath = targetPath;
            img_proof.Source = ImageSource.FromFile(_currentPhotoPath);
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error" , $"Picking photo failed: {ex.Message}" , "OK");
        }
    }
    // validuje wpisane dane i wprowadza odpowiedznie zmiany do bazy danych po czym wraca do mainpage
    private async void btn_add_log_Clicked( object sender , EventArgs e )
    {
        if(pick_person.SelectedItem == null)
        {
            await DisplayAlert("Error" , "Please select a person" , "OK");
            return;
        }
        if(due_picker.Date <= DateTime.Now)
        {
            await DisplayAlert("Error" , "Please select a correct date" , "OK");
            return;
        }
        if(string.IsNullOrWhiteSpace(ent_name.Text))
        {
            await DisplayAlert("Error" , "Please enter a log name" , "OK");
            return;
        }
        if(!decimal.TryParse(ent_amount.Text , out decimal value) && value == 0)
        {
            await DisplayAlert("Error" , "Please enter an acceptable number" , "OK");
            return;
        }
        if(string.IsNullOrWhiteSpace(edit_description.Text))
        {
            bool haveDescription = await DisplayAlert("Are you shure?" , "Do you want to leave the description empty?" , "YES" , "NO");
            if(!haveDescription) return;
        }

        var selected_person = (People)pick_person.SelectedItem;
        try
        {

            Logs newLog = new Logs()
            {
                Date = DateTime.Now ,
                LogName = ent_name.Text ,
                Description = edit_description.Text ,
                Amount = value ,
                Due = due_picker.Date ,
                PersonId = selected_person.Id ,
                Proof = _currentPhotoPath 
            };
            await _dbService.AddLog(newLog);
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error" , "An error has occured" , "OK");
        }

        await DisplayAlert("Sukces" , "New log appended!" , "OK");

        pick_person.SelectedItem = null;
        ent_name.Text = string.Empty;
        edit_description.Text = string.Empty;
        img_proof.Source = null;

        await Shell.Current.GoToAsync("//MainPage");
    }

}
