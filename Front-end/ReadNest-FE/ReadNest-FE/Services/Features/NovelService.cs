using ReadNest_Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using ReadNest_FE.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net;
using ReadNest_FE.Dictionaries;

namespace ReadNest_FE.Services.Features
{
    public class NovelService : RequestHandler<Novel>, INovelService
    {
        public NovelService(HttpClient httpClient, Store.Store store, NavigationManager navigation, IAuthService authService, UiEventService uiEventService) : 
                base(httpClient, uiEventService, store, navigation, authService)
        {
        }

        public async Task<Response<DetailNovel>> DetailPost(DetailNovel detailNovel)
        {
            _uiEventService.SetLoading(true);
            try
            {
                _url = $"{_store.Host}/api/{typeof(Novel).Name.ToLower()}";
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _store.Token);
                var url = $"{_url}/detail";

                var response = await SendRequestWithRefreshAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _store.Token);
                    return await _httpClient.PostAsJsonAsync(url, detailNovel);
                });

                if (response.IsSuccessStatusCode)
                {
                    Response<DetailNovel>? data = await response.Content.ReadFromJsonAsync<Response<DetailNovel>>();
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
                handleOnExceptionRequest(ex);
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }

        public async Task<Response<DetailNovel>> DetailPut(DetailNovel detailNovel)
        {
            _uiEventService.SetLoading(true);
            try
            {
                _url = $"{_store.Host}/api/{typeof(Novel).Name.ToLower()}";
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _store.Token);
                var url = $"{_url}/detail";

                var response = await SendRequestWithRefreshAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _store.Token);
                    return await _httpClient.PutAsJsonAsync(url, detailNovel);
                });

                if (response.IsSuccessStatusCode)
                {
                    Response<DetailNovel>? data = await response.Content.ReadFromJsonAsync<Response<DetailNovel>>();
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
                handleOnExceptionRequest(ex);
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }

        public async Task<Response<PaginationData<List<NovelResponese>>>> GetNovelsFilter(NovelFilter novelFilter)
        {
            _uiEventService.SetLoading(true);
            try
            {
                _url = $"{_store.Host}/api/{typeof(Novel).Name.ToLower()}";
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _store.Token);
                var url = $"{_url}/filter";

                var response = await SendRequestWithRefreshAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _store.Token);
                    return await _httpClient.PostAsJsonAsync(url, novelFilter);
                });

                if (response.IsSuccessStatusCode)
                {
                    Response<PaginationData<List<NovelResponese>>>? data = await response.Content.ReadFromJsonAsync<Response<PaginationData<List<NovelResponese>>>>();
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
                handleOnExceptionRequest(ex);
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }

        public async Task<Response<NovelHomePage>> GetNovelsInHomePage(int page = 1, int pageSize = 5)
        {
            _uiEventService.SetLoading(true);
            try
            {
                _url = $"{_store.Host}/api/{typeof(Novel).Name.ToLower()}";
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _store.Token);

                var queryParams = new List<string>();

                queryParams.Add($"page={page}");
                queryParams.Add($"pageSize={pageSize}");

                var url = $"{_url}/home";
                if (queryParams.Any())
                {
                    url += "?" + string.Join("&", queryParams);
                }

                var response = await SendRequestWithRefreshAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _store.Token);
                    return await _httpClient.GetAsync(url);
                });

                if (response.IsSuccessStatusCode)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    Response<NovelHomePage>? data =
                        await response.Content.ReadFromJsonAsync<Response<NovelHomePage>>(jsonOptions);
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
                handleOnExceptionRequest(ex);
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }

        public new async Task<Response<PaginationData<List<NovelResponese>>>> GetValue(string? keyword, int page = 1, int pageSize = 5)
        {
            try
            {
                _url = $"{_store.Host}/api/{typeof(Novel).Name.ToLower()}";
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _store.Token);

                var queryParams = new List<string>();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
                }

                queryParams.Add($"page={page}");
                queryParams.Add($"pageSize={pageSize}");

                var url = _url;
                if (queryParams.Any())
                {
                    url += "?" + string.Join("&", queryParams);
                }

                var response = await SendRequestWithRefreshAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _store.Token);
                    return await _httpClient.GetAsync(url);
                });

                if (response.IsSuccessStatusCode)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    Response<PaginationData<List<NovelResponese>>>? data = 
                        await response.Content.ReadFromJsonAsync<Response<PaginationData<List<NovelResponese>>>>(jsonOptions);
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
                handleOnExceptionRequest(ex);
                return null!;
            }
        }

        public new async Task<Response<DetailNovel>> GetValueById(string id)
        {
            _uiEventService.SetLoading(true);
            try
            {
                _url = $"{_store.Host}/api/{typeof(Novel).Name.ToLower()}";
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _store.Token);
                var url = $"{_url}/{id}";

                var response = await SendRequestWithRefreshAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _store.Token);
                    return await _httpClient.GetAsync(url);
                });

                if (response.IsSuccessStatusCode)
                {
                    Response<DetailNovel>? data = await response.Content.ReadFromJsonAsync<Response<DetailNovel>>();
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
                handleOnExceptionRequest(ex);
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }

        public async Task<Response<bool>> ShareNovel(string novelId, string userId)
        {
            _uiEventService.SetLoading(true);
            try
            {
                _url = $"{_store.Host}/api/{typeof(Novel).Name.ToLower()}/{novelId}/share";
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _store.Token);

                var response = await _httpClient.PostAsJsonAsync(_url, new { UserId = userId });

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
                handleOnExceptionRequest(ex);
                return null!;
            }
            finally
            {
                _uiEventService.SetLoading(false);
            }
        }


    }
}
