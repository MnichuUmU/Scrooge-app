using Scrooge_app.Databases;
using System.Text.RegularExpressions;

namespace Scrooge_app;

public partial class AddPeoplePage : ContentPage
{
    private readonly LocalDbService _dbService;
    private string _currentPhotoPath = "user.png";
    public AddPeoplePage( LocalDbService dbService )
    {
        InitializeComponent();
        _dbService = dbService;
    }

  
    private async void btn_imp_fb_Clicked( object sender , EventArgs e )
    {
        var uri = new Uri("fb://facewebmodal/f?href=https://www.facebook.com");
        try
        {
            if(await Launcher.Default.CanOpenAsync(uri))
            {
                await Launcher.Default.OpenAsync(uri);
            }
            else
            {
                await Launcher.Default.OpenAsync("https://www.facebook.com/");
            }

        }
        catch(Exception ex)
        {
            await DisplayAlert("Error" , "Could not open Instagram." , "OK");
        }

    }

    private async void btn_imp_insta_Clicked( object sender , EventArgs e )
    {
        var uri = new Uri("instagram://explore");
        try
        {
            if(await Launcher.Default.CanOpenAsync(uri))
            {
                await Launcher.Default.OpenAsync(uri);
            }
            else
            {
                await Launcher.Default.OpenAsync("https://www.instagram.com");
            }

        }
        catch(Exception ex)
        {
            await DisplayAlert("Error" , "Could not open Instagram." , "OK");
        }

    }

    private async void btn_imp_contact_Clicked( object sender , EventArgs e )
    {
        try
        {
            var contact = await Microsoft.Maui.ApplicationModel.Communication.Contacts.Default.PickContactAsync();

            if(contact == null) return;

            ent_name.Text = contact.GivenName;
            ent_surname.Text = contact.FamilyName;

            var firstPhone = contact.Phones?.FirstOrDefault();
            if(firstPhone != null)
            {
                ent_phone.Text = firstPhone.PhoneNumber;
            }

            var firstEmail = contact.Emails?.FirstOrDefault();
            if(firstEmail != null)
            {
                ent_mail.Text = firstEmail.EmailAddress;
            }

        }
        catch(Exception ex)
        {
            await DisplayAlert("Permission Error" , $"Could not access contacts: {ex.Message}" , "OK");
        }
    }

    private async void ent_profile_Clicked( object sender , EventArgs e )
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
            ent_profile.Source = ImageSource.FromFile(_currentPhotoPath);
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error" , $"Picking photo failed: {ex.Message}" , "OK");
        }
    }


    private async void btn_add_person_Clicked( object sender , EventArgs e )
    {

        if(string.IsNullOrWhiteSpace(ent_name.Text))
        {
            await DisplayAlert("Error" , "Please enter a name" , "OK");
            return;
        }
        if(string.IsNullOrWhiteSpace(ent_surname.Text))
        {
            await DisplayAlert("Error" , "Please enter a surname" , "OK");
            return;
        }
        if(string.IsNullOrWhiteSpace(ent_phone.Text))
        {
            bool haveNumber = await DisplayAlert("Are you shure" , "Do you want to leave phone number empty?" , "YES" , "NO");
            if(!haveNumber) return;
            else if(!string.IsNullOrWhiteSpace(ent_phone.Text) && !Regex.Match(ent_phone.Text , @"^(?:\+48)?[\s-]?\d{3}[\s-]?\d{3}[\s-]?\d{3}$").Success)
            {
                await DisplayAlert("Error" , "Please enter an acceptable phone number" , "OK");
                return;
            }
        }

        if(string.IsNullOrWhiteSpace(ent_mail.Text))
        {
            bool haveMail = await DisplayAlert("Are you shure?" , "Do you want to leave the mail empty?" , "YES" , "NO");
            if(!haveMail) return;
        }
        try
        {
            People newPerson = new People()
            {
                Name = ent_name.Text ,
                Surname = ent_surname.Text ,
                Balance = 0 ,
                Phone = ent_phone?.Text ?? "" ,
                Mail = ent_mail?.Text ?? "" ,
                Facebook = ent_fb?.Text ?? "" ,
                Instagram = ent_insta?.Text ?? "" ,
                Family = ent_family?.Text ?? "" ,
                ProfilePick = _currentPhotoPath
            };
            await _dbService.AddPerson(newPerson);
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error" , "An error has occured" , "OK");
        }

        await DisplayAlert("Sukces" , "A new person was added!" , "OK");

        ent_name.Text = string.Empty;
        ent_surname.Text = string.Empty;
        ent_phone.Text = string.Empty;
        ent_mail.Text = string.Empty;
        ent_fb.Text = string.Empty;
        ent_insta.Text = string.Empty;
        ent_family.Text = string.Empty;
        ent_profile.Source = ImageSource.FromFile("user.png");

        await Shell.Current.GoToAsync("//MainPage");
    }
}

