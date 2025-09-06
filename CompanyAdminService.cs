using System.Net.Http.Json;
using MyProfessionalss.Data.Model.DTO;

namespace MyProfessionals.Core.Services
{
    public class CompanyAdminService
    {
        private readonly HttpClient _httpClient;

        public CompanyAdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress ??= new Uri(ApiConfig.BaseUrl);
        }

        // Create
        public async Task<CompanyAdminDto?> AddAsync(CompanyAdminDto newAdmin, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("companyadmins", newAdmin, cancellationToken);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<CompanyAdminDto>(cancellationToken: cancellationToken)
                : null;
        }

        // Read all admins for a company
        public async Task<IReadOnlyList<CompanyAdminDto>> GetByCompanyAsync(int companyId, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<List<CompanyAdminDto>>(
                $"companyadmins/company/{companyId}", cancellationToken
            ) ?? [];
        }

        // Read one admin by ID
        public async Task<CompanyAdminDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<CompanyAdminDto>(
                $"companyadmins/{id}", cancellationToken
            );
        }

        // Delete by ID
        public async Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"companyadmins/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }

        // Check if a user is an admin of a company
        public async Task<bool> IsUserAdminAsync(int userId, int companyId, CancellationToken cancellationToken = default)
        {
            var admins = await GetByCompanyAsync(companyId, cancellationToken);
            return admins.Any(a => a.UserId == userId);
        }

        // Get all employees assigned to a company
        public async Task<IReadOnlyList<UserDto>> GetAssignedEmployeesAsync(int companyId, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<List<UserDto>>(
                $"companyadmins/company/{companyId}/employees", cancellationToken
            ) ?? [];
        }

        // Remove employee from company
        public async Task<bool> RemoveEmployeeAsync(int userId, int companyId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync(
                $"companyadmins/company/{companyId}/employees/{userId}", cancellationToken
            );
            return response.IsSuccessStatusCode;
        }
    }
}
