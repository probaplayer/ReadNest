using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ReadNest_FE.Dictionaries;
using ReadNest_FE.Interfaces;
using ReadNest_Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReadNest_FE.Services.Features
{
    public class AuthService : IAuthService
    {
        protected readonly UiEventService _uiEventService;
        protected readonly NavigationManager _navigation;
        protected readonly HttpClient _httpClient;
        protected string? _url;
        protected readonly Store.Store _store;
        protected IJSRuntime _js;

        public AuthService(HttpClient httpClient, UiEventService uiEventService, Store.Store store, NavigationManager navigation, IJSRuntime js)
        {
            _httpClient = httpClient;
            _navigation = navigation;
            bool isInvalid = string.IsNullOrEmpty(store.Host) || string.IsNullOrEmpty(store.Token);
            if (isInvalid)
                _navigation.NavigateTo(RouterManager.GetRouter(Enums.RouterType.LOGIN)!);
            _store = store;
            _js = js;
            _uiEventService = uiEventService;
        }
        public async Task<Response<UserResponse>> Delete(string userId)
        {
            _uiEventService.SetLoading(true);
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentException("ID cannot be null or empty.", nameof(userId));
                }

                _url = $"{_store.Host}/api/Auth/Delete?userId={Uri.EscapeDataString(userId)}";

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _store.Token);

                var response = await _httpClient.DeleteAsync(_url);
                if (response.IsSuccessStatusCode)
                {
                    Response<UserResponse>? data = await response.Content.ReadFromJsonAsync<Response<UserResponse>>();
                    _uiEventService.ShowAlert(data?.Message!, "success");
                    return data!;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    _uiEventService.ShowAlert(errorMessage, "error");
                    return null!;
                }

            }
            catch (HttpRequestException ex)
            {
                _uiEventService.ShowAlert($"Delete request failed: {ex.Message}","error");
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }

        public async Task<Response<UserResponse>> Login(UserLogin userLogin)
        {
            _uiEventService.SetLoading(true);
            try
            {
                _url = $"{_store.Host}/api/Auth/login";
                var response = await _httpClient.PostAsJsonAsync(_url, userLogin);
                if (response.IsSuccessStatusCode)
                {
                    Response<UserResponse>? data = await response.Content.ReadFromJsonAsync<Response<UserResponse>>();
                    if (data!.Success)
                    {
                        _store.Token = data.Data!.Token;
                        _store.UserName = data.Data!.UserName;
                    }
                    _uiEventService.ShowAlert(data?.Message!, "success");
                    return data!;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    _uiEventService.ShowAlert(errorMessage, "error");
                    return null!;
                }
            }
            catch (HttpRequestException ex)
            {
                _uiEventService.ShowAlert(ex.Message, "error");
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }

        public async Task<Response<UserResponse>> Register(UserLogin userRegister)
        {
            _uiEventService.SetLoading(true);
            try
            {
                _url = $"{_store.Host}/api/Auth/signup";
                var response = await _httpClient.PostAsJsonAsync(_url, userRegister);

                if (response.IsSuccessStatusCode)
                {
                    Response<UserResponse>? data = await response.Content.ReadFromJsonAsync<Response<UserResponse>>();
                    if (data!.Success)
                    {
                        _store.Token = data.Data!.Token;
                        _store.UserName = data.Data!.UserName;
                    }
                    _uiEventService.ShowAlert(data?.Message!, "success");
                    return data!;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    _uiEventService.ShowAlert(errorMessage, "error");
                    return null!;
                }
            }
            catch (HttpRequestException ex)
            {
                _uiEventService.ShowAlert($"POST request failed: {ex.Message}", "error");
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }

        public async Task Logout()
        {
            _store.Token = string.Empty;
            _store.RefreshToken = null;
            _store.UserName = string.Empty;
            await _js.InvokeVoidAsync("localStorage.setItem", "authToken", string.Empty);
            await _js.InvokeVoidAsync("localStorage.removeItem", "readingHistories");
            _navigation.NavigateTo(RouterManager.GetRouter(Enums.RouterType.LOGIN)!);
        }

        public async Task<Response<UserResponse>> RefreshTokenAsync()
        {
            _uiEventService.SetLoading(true);
            try
            {
                var accessToken = _store.Token;
                var refreshToken = _store.RefreshToken;

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    return new Response<UserResponse>(null, "Tokens not found", false);
                }

                var response = await _httpClient.PostAsJsonAsync($"{_store.Host}/api/Auth/refresh-token", new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Response<UserResponse>>();
                    if (result?.Success == true && result.Data != null)
                    {
                        _store.Token = result.Data.Token;
                        _store.RefreshToken = result.Data.RefreshToken;
                        _store.HasContributePermission = result.Data.HasContributePermission;
                    }
                    _uiEventService.ShowAlert(result?.Message!, "success");
                    return result!;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    _uiEventService.ShowAlert(errorMessage, "error");
                    return null!;
                }
            }
            catch(Exception ex)
            {
                _uiEventService.ShowAlert(ex.Message, "error");
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }

        public async Task<Response<bool>> ChangePassword(UserChangePassword userLogin)
        {
            _uiEventService.SetLoading(true);
            try
            {
                _url = $"{_store.Host}/api/Auth/change-password";
                var response = await _httpClient.PostAsJsonAsync(_url, userLogin);
                if (response.IsSuccessStatusCode)
                {
                    Response<bool>? data = await response.Content.ReadFromJsonAsync<Response<bool>>();
                    _uiEventService.ShowAlert(data?.Message!, "success");
                    return data!;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    _uiEventService.ShowAlert(errorMessage, "error");
                    return null!;
                }
            }
            catch (HttpRequestException ex)
            {
                _uiEventService.ShowAlert(ex.Message, "error");
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }
    }
}
