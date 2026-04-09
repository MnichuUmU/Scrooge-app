using Scrooge_app.Views;
using static System.Reflection.Metadata.BlobBuilder;

namespace Scrooge_app
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AddPeoplePage), typeof(AddPeoplePage));
            Routing.RegisterRoute(nameof(AddLogPage), typeof(AddLogPage));
            Routing.RegisterRoute(nameof(PeopleView), typeof(PeopleView));
            
        }
    }
}
