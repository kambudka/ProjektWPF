using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Security;

namespace ProjektWPF
{
    public interface IViewModel { }

    public class AuthenticationViewModel : IViewModel, INotifyPropertyChanged
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly DelegateCommand _loginCommand;
        private readonly DelegateCommand _logoutCommand;
        private readonly DelegateCommand _showViewCommand;
        private string _username;
        private string _status;

        //Tworzenie usługi autoryzującej dla ViewModelu
        public AuthenticationViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _loginCommand = new DelegateCommand(Login, CanLogin);
            _logoutCommand = new DelegateCommand(Logout, CanLogout);
            _showViewCommand = new DelegateCommand(ShowView, null);
            
        }

        //Właściwości wykorzystane przy autoryzacji użytkowników. Takie jak nazwa/rola/status.
        #region Properties
        public string Username
        {
            get { return _username; }
            set { _username = value; NotifyPropertyChanged("Username"); }
        }

        //Zwraca nazwę obecnie zalogowanego użytkownika
        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                    return string.Format("Zalogowany jako {0}",
                          Thread.CurrentPrincipal.Identity.Name);

                return "Nie jesteś Zalogowany!";
            }
        }

        //Zwraca jaką role posiada obecnie zalogowany użytkownik.
        public string AuthenticatedRole
        {
            get
            {
                if (IsAuthenticated)
                    if (Thread.CurrentPrincipal.IsInRole("Administrators") == true) return "Administrator";
                    else if (Thread.CurrentPrincipal.IsInRole("Kurier") == true) return "Kurier";
                    else if (Thread.CurrentPrincipal.IsInRole("Ksiegowy") == true) return "Ksiegowy";
                return "Nie jesteś Zalogowany!";
            }
        }

        //String do zwracania wiadomości/błędów związanych z logowaniem.
        public string Status
        {
            get { return _status; }
            set { _status = value; NotifyPropertyChanged("Status"); }
        }
        #endregion

        // Commandsy wykorzystane przy logowaniu się użytkowików oraz sprawdzaniu dostępu do okna.
        #region Commands
        public DelegateCommand LoginCommand { get { return _loginCommand; } }

        public DelegateCommand LogoutCommand { get { return _logoutCommand; } }

        public DelegateCommand ShowViewCommand { get { return _showViewCommand; } }
        #endregion

        //Obsługa logowania się użytkownika
        private void Login(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            string clearTextPassword = passwordBox.Password;
            try
            {
                //Sprawdzanie użytkownika
                User user = _authenticationService.AuthenticateUser(Username, clearTextPassword);

                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");

                //Autoryzuj użytkownika
                customPrincipal.Identity = new CustomIdentity(user.Username, user.Email, user.Roles);

                //Powiadomienie o zmianie użytkownika
                NotifyPropertyChanged("AuthenticatedUser");
                NotifyPropertyChanged("AuthenticatedRole");
                NotifyPropertyChanged("IsAuthenticated");
                NotifyPropertyChanged("IsAdmin");
                NotifyPropertyChanged("IsKurier");
                NotifyPropertyChanged("IsKsiegowy");
                _loginCommand.RaiseCanExecuteChanged();
                _logoutCommand.RaiseCanExecuteChanged();
                Username = string.Empty; //reset
                passwordBox.Password = string.Empty; //reset
                Status = string.Empty;
            }
            catch (UnauthorizedAccessException) //Złapanie wyjątku jeśli login i hasło nie pasują do siebie.
            {
                Status = "Logowanie nie powiodlo się.";
            }
            catch (Exception ex)
            {
                Status = string.Format("ERROR: {0}", ex.Message);
            }
        }

        //Sprawdza czy jest już ktoś zalogowany jeśli nie to udostępnia przycisk Logowania.
        private bool CanLogin(object parameter)
        {
            return !IsAuthenticated;
        }

        //Po wylogowaniu zostaje przypisany anonimowy użytkownik oraz powiadomione wszystkie funkcjie o zmianie użytkownika.
        private void Logout(object parameter)
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
                NotifyPropertyChanged("AuthenticatedUser");
                NotifyPropertyChanged("AuthenticatedRole");
                NotifyPropertyChanged("IsAuthenticated");
                NotifyPropertyChanged("IsAdmin");
                NotifyPropertyChanged("IsKurier");
                NotifyPropertyChanged("IsKsiegowy");
                _loginCommand.RaiseCanExecuteChanged();
                _logoutCommand.RaiseCanExecuteChanged();
                Status = string.Empty;
            }
        }

        //Zwraca true przez co wyswietla się przycisk wyloguj
        private bool CanLogout(object parameter)    
        {
            return IsAuthenticated;
        }

        //Zwraca true jeżeli jakikolwiek użytkownik jest zalogowany
        public bool IsAuthenticated 
        {
            get { return Thread.CurrentPrincipal.Identity.IsAuthenticated; }
        }

        //Sprawdza czy nasz użytkownik jest administratorem
        public bool IsAdmin
        {
            get
            {   if (IsAuthenticated)
                    if (Thread.CurrentPrincipal.IsInRole("Administrators") == true)
                        return true;
                return false;
            }

        }

        //Sprawdza czy nasz użytkownik jest kurierem
        public bool IsKurier    
        {
            get
            {
                if (IsAuthenticated)
                    if (Thread.CurrentPrincipal.IsInRole("Kurier") == true)
                        return true;
                else if (Thread.CurrentPrincipal.IsInRole("Administrators") == true)
                        return true;
                return false;
            }
        }

        //Sprawdza czy nasz użytkownik jest ksiegowym
        public bool IsKsiegowy  
        {
            get
            {
                if (IsAuthenticated)
                    if (Thread.CurrentPrincipal.IsInRole("Ksiegowy") == true)
                        return true;
                    else if (Thread.CurrentPrincipal.IsInRole("Administrators") == true)
                        return true;
                return false;
            }
        }

        //Otwiera okno dodanai paczki, najpierw sprawdzając czy uztkownik ma uprawnienia.
        private void ShowView(object parameter) 
        {
            try
            {
                Status = string.Empty;
                IView view;
                view = new Okno_Dodaj();
                view.Show();
            }
            catch (SecurityException)
            {
                Status = "Nie jestes Zalogowany!";
            }
        }

        //Powiadamia gdy wartoś została zmieniona
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;   

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}