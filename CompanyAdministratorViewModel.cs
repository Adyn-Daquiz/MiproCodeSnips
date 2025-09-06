using MyProfessionals.Core.Services;
using MyProfessionalss.Data.Model;
using MyProfessionalss.Data.Model.DTO;
using MyProfessionalss.Data.Model.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyProfessionals.ViewModels
{
    public class CompanyAdministratorViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly ICompanyAdminService _companyAdminService;
        private readonly int _companyId;

        private string _userCodeInput;
        public string UserCodeInput
        {
            get => _userCodeInput;
            set => SetProperty(ref _userCodeInput, value);
        }

        private UserDto _selectedUser;
        public UserDto SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        private CompanyEmployeeType _selectedEmployeeType;
        public CompanyEmployeeType SelectedEmployeeType
        {
            get => _selectedEmployeeType;
            set => SetProperty(ref _selectedEmployeeType, value);
        }

        public ObservableCollection<CompanyEmployeeType> CompanyEmployeeTypes { get; }
        public ObservableCollection<UserDto> CompanyAdmins { get; } = new();

        public ICommand SearchCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand RefreshCommand { get; }

        public CompanyAdministratorViewModel(
            int companyId,
            IUserService userService,
            ICompanyAdminService companyAdminService)
        {
            _companyId = companyId;
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _companyAdminService = companyAdminService ?? throw new ArgumentNullException(nameof(companyAdminService));

            CompanyEmployeeTypes = new ObservableCollection<CompanyEmployeeType>(
                (CompanyEmployeeType[])Enum.GetValues(typeof(CompanyEmployeeType))
            );

            SearchCommand = new Command(async () => await SearchUserAsync());
            SaveCommand = new Command(async () => await SaveUserAsync(), () => SelectedUser != null);
            RemoveCommand = new Command<int>(async (id) => await RemoveUserAsync(id));
            RefreshCommand = new Command(async () => await LoadAssignedEmployeesAsync());

            Task.Run(async () => await LoadAssignedEmployeesAsync());
        }

        private async Task LoadAssignedEmployeesAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                CompanyAdmins.Clear();
                var employees = await _companyAdminService.GetAssignedEmployeesAsync(_companyId);

                foreach (var emp in employees)
                    CompanyAdmins.Add(emp);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Failed to load employees: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchUserAsync()
        {
            if (string.IsNullOrWhiteSpace(UserCodeInput))
                return;

            IsBusy = true;
            try
            {
                var user = await _userService.GetUserByUserCodeAsync(UserCodeInput);
                SelectedUser = user;

                if (user == null)
                    await App.Current.MainPage.DisplayAlert("Not Found", "No user found with that code.", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Unable to find user: {ex.Message}", "OK");
                SelectedUser = null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveUserAsync()
        {
            if (SelectedUser == null)
            {
                await App.Current.MainPage.DisplayAlert("Missing User", "There is no User to assign.", "OK");
                return;
            }

            if (SelectedEmployeeType == 0)
            {
                await App.Current.MainPage.DisplayAlert("Missing Employee Type", "Please select an Employee Type.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var result = await _userService.AssignEmployeeToCompanyAsync(
                    SelectedUser?.Id ?? 0,
                    _companyId,
                    SelectedEmployeeType
                );

                if (result)
                {
                    await App.Current.MainPage.DisplayAlert(
                        "Success",
                        $"{SelectedUser.FullName} assigned as {SelectedEmployeeType}",
                        "OK"
                    );

                    await LoadAssignedEmployeesAsync();
                    UserCodeInput = string.Empty;
                    SelectedUser = null;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Failed to assign user to company.", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Unable to save: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RemoveUserAsync(int userId)
        {
            bool confirm = await App.Current.MainPage.DisplayAlert(
                "Confirm",
                "Are you sure you want to remove this user?",
                "Yes", "No");

            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _companyAdminService.RemoveEmployeeFromCompanyAsync(userId, _companyId);
                if (result)
                {
                    var userToRemove = CompanyAdmins.FirstOrDefault(x => x.Id == userId);
                    if (userToRemove != null)
                        CompanyAdmins.Remove(userToRemove);
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Failed to remove employee.", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Unable to remove user: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
