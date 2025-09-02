using Piranha.Cache;
using PiranhaCMS.Common.Extensions;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.PublicWeb.Models.ViewModelFactories.Base;
using PiranhaCMS.PublicWeb.Models.ViewModels;
using PiranhaCMS.Search.Engine;
using PiranhaCMS.Search.Extensions;
using PiranhaCMS.Search.Models;
using PiranhaCMS.Search.Models.Constants;
using PiranhaCMS.Search.Models.Dto;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Models.Internal;
using PiranhaCMS.Search.Models.Requests;
using System.Text;

namespace PiranhaCMS.PublicWeb.Models.ViewModelFactories;

public class MusicSearchPageViewModelFactory : IPageViewModelFactory<MusicSearchPage, MusicSearchPageViewModel>
{
    public const int PageSize = 20;
    private readonly ISearchIndexEngine<MusicLibraryDocument> _engine;
    private readonly ICache _cache;
    private readonly IHttpContextAccessor _contextAccessor;

    public MusicSearchPageViewModelFactory(
        ISearchIndexEngine<MusicLibraryDocument> engine,
        ICache cache,
        IHttpContextAccessor contextAccessor)
    {
        _engine = engine;
        _cache = cache;
        _contextAccessor = contextAccessor;
    }

    public MusicSearchPageViewModel Create(MusicSearchPage page)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(_contextAccessor.HttpContext);

        var model = new MusicSearchPageViewModel(page);
        var request = _contextAccessor.HttpContext.Request;
        var searchText = request.Query["q"].ToString().SanitizeSearchString();
        var artist = request.Query[_engine.GetFieldName(x => x.Artist)].ToString();
        var release = request.Query[_engine.GetFieldName(x => x.Release)].ToString();
        var genre = request.Query[_engine.GetFieldName(x => x.Genre)].ToString();
        var year = request.Query[_engine.GetFieldName(x => x.Year)].ToString();
        var paginationQueryString = new StringBuilder();
        _ = int.TryParse(request.Query["page"], out int pageIndex);

        if (!string.IsNullOrEmpty(searchText) && string.IsNullOrEmpty(artist) && string.IsNullOrEmpty(release) && string.IsNullOrEmpty(genre) && string.IsNullOrEmpty(year))
        {
            paginationQueryString.Append("?q=");
            paginationQueryString.Append(searchText);

            model.SearchResult = FullTextSearch(
                _engine.GetFieldName(x => x.Text),
                searchText,
                new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
                new Dictionary<string, IEnumerable<string?>?> { { _engine.GetFieldName(x => x.Text), [] } });
        }
        else if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(release))
        {
            paginationQueryString.Append("?q=");
            paginationQueryString.Append(searchText);
            paginationQueryString.Append($"&{_engine.GetFieldName(x => x.Artist)}=");
            paginationQueryString.Append(artist);
            paginationQueryString.Append($"&{_engine.GetFieldName(x => x.Release)}=");
            paginationQueryString.Append(release);

            model.SearchResult = MultiTermSearch(
                [
                    new()
                    {
                        Name = _engine.GetFieldName(x => x.Text),
                        Value = searchText,
                        SearchType = SearchType.QueryMatch,
                        Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Text))
                    },
                    new()
                    {
                        Name = _engine.GetFieldName(x => x.Artist),
                        Value = artist,
                        SearchType = SearchType.ExactMatch,
                        Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Artist))
                    },
                    new()
                    {
                        Name = _engine.GetFieldName(x => x.Release),
                        Value = release,
                        SearchType = SearchType.ExactMatch,
                        Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Release))
                    }
                ],
                new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
                new Dictionary<string, IEnumerable<string?>?> { { _engine.GetFieldName(x => x.Text), [] }, { _engine.GetFieldName(x => x.Artist), [] } });
        }
        else if (!string.IsNullOrEmpty(genre) && !string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(release))
        {
            paginationQueryString.Append($"?{_engine.GetFieldName(x => x.Genre)}=");
            paginationQueryString.Append(genre);
            paginationQueryString.Append($"&{_engine.GetFieldName(x => x.Artist)}=");
            paginationQueryString.Append(artist);
            paginationQueryString.Append($"&{_engine.GetFieldName(x => x.Release)}=");
            paginationQueryString.Append(release);

            model.SearchResult = MultiTermSearch(
                [
                    new()
                    {
                        Name = _engine.GetFieldName(x => x.Genre),
                        Value = genre,
                        SearchType = SearchType.ExactMatch,
                        Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Genre))
                    },
                    new()
                    {
                        Name = _engine.GetFieldName(x => x.Artist),
                        Value = artist,
                        SearchType = SearchType.ExactMatch,
                        Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Artist))
                    },
                    new()
                    {
                        Name = _engine.GetFieldName(x => x.Release),
                        Value = release,
                        SearchType = SearchType.ExactMatch,
                        Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Release))
                    }
                ],
                new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
                new Dictionary<string, IEnumerable<string?>?> { { _engine.GetFieldName(x => x.Genre), [] }, { _engine.GetFieldName(x => x.Artist), [] } });
        }
        else if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(artist))
        {
            paginationQueryString.Append("?q=");
            paginationQueryString.Append(searchText);
            paginationQueryString.Append($"&{_engine.GetFieldName(x => x.Artist)}=");
            paginationQueryString.Append(artist);

            model.SearchResult = MultiTermSearch(
                [
                    new()
                    {
                        Name = _engine.GetFieldName(x => x.Text),
                        Value = searchText,
                        SearchType = SearchType.QueryMatch,
                        Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Text))
                    },
                    new()
                    {
                        Name = _engine.GetFieldName(x => x.Artist),
                        Value = artist,
                        SearchType = SearchType.ExactMatch,
                        Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Artist))
                    }
                ],
                new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
                new Dictionary<string, IEnumerable<string?>?> { { _engine.GetFieldName(x => x.Text), [] }, { _engine.GetFieldName(x => x.Artist), [] } });
        }
        else if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(artist))
        {
            paginationQueryString.Append($"?{_engine.GetFieldName(x => x.Year)}=");
            paginationQueryString.Append(year);
            paginationQueryString.Append($"&{_engine.GetFieldName(x => x.Artist)}=");
            paginationQueryString.Append(artist);

            model.SearchResult = MultiTermSearch(
                [
                    new()
                    {
                        Name = _engine.GetFieldName(x => x.Year),
                        Value = year,
                        SearchType = SearchType.ExactMatch,
                        Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Year))
                    },
                    new()
                    {
                        Name = _engine.GetFieldName(x => x.Artist),
                        Value = artist,
                        SearchType = SearchType.ExactMatch,
                        Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Artist))
                    }
                ],
                new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
                new Dictionary<string, IEnumerable<string?>?> { { _engine.GetFieldName(x => x.Year), [] }, { _engine.GetFieldName(x => x.Artist), [] } });
        }
        else if (!string.IsNullOrEmpty(release) && !string.IsNullOrEmpty(artist))
        {
            paginationQueryString.Append($"?{_engine.GetFieldName(x => x.Artist)}=");
            paginationQueryString.Append(artist);
            paginationQueryString.Append($"&{_engine.GetFieldName(x => x.Release)}=");
            paginationQueryString.Append(release);

            model.SearchResult = MultiTermSearch(
            [
                new()
                {
                    Name = _engine.GetFieldName(x => x.Artist),
                    Value = artist,
                    SearchType = SearchType.ExactMatch,
                    Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Artist))
                },
                new()
                {
                    Name = _engine.GetFieldName(x => x.Release),
                    Value = release,
                    SearchType = SearchType.ExactMatch,
                    Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Release))
                }
            ],
            new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
            new Dictionary<string, IEnumerable<string?>?> { { _engine.GetFieldName(x => x.Artist), [] } });
        }
        else if (!string.IsNullOrEmpty(genre) && !string.IsNullOrEmpty(artist))
        {
            paginationQueryString.Append($"?{_engine.GetFieldName(x => x.Genre)}=");
            paginationQueryString.Append(genre);
            paginationQueryString.Append($"&{_engine.GetFieldName(x => x.Artist)}=");
            paginationQueryString.Append(artist);

            model.SearchResult = MultiTermSearch(
            [
                new()
                {
                    Name = _engine.GetFieldName(x => x.Genre),
                    Value = genre,
                    SearchType = SearchType.ExactMatch,
                    Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Genre))
                },
                new()
                {
                    Name = _engine.GetFieldName(x => x.Artist),
                    Value = artist,
                    SearchType = SearchType.ExactMatch,
                    Properties = DocumentFields<MusicLibraryDocument>.GetField(_engine.GetFieldName(x => x.Artist))
                }
            ],
            new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
            new Dictionary<string, IEnumerable<string?>?> { { _engine.GetFieldName(x => x.Artist), [] }, { _engine.GetFieldName(x => x.Genre), [] } });
        }
        else if (!string.IsNullOrEmpty(genre))
        {
            paginationQueryString.Append($"?{_engine.GetFieldName(x => x.Genre)}=");
            paginationQueryString.Append(genre);

            model.SearchResult = SingleTermSearch(
                _engine.GetFieldName(x => x.Genre),
                genre,
                new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
                new Dictionary<string, IEnumerable<string?>?> { { _engine.GetFieldName(x => x.Genre), [] } });
        }
        else if (!string.IsNullOrEmpty(year))
        {
            paginationQueryString.Append($"?{_engine.GetFieldName(x => x.Year)}=");
            paginationQueryString.Append(year);

            model.SearchResult = SingleTermSearch(
                _engine.GetFieldName(x => x.Year),
                year,
                new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
                new Dictionary<string, IEnumerable<string?>?> { { _engine.GetFieldName(x => x.Year), [] } },
                true);
        }
        else if (!string.IsNullOrEmpty(artist))
        {
            paginationQueryString.Append($"?{_engine.GetFieldName(x => x.Artist)}=");
            paginationQueryString.Append(artist);

            model.SearchResult = SingleTermSearch(
                _engine.GetFieldName(x => x.Artist),
                artist,
                new PaginationRequest(PageSize, pageIndex, paginationQueryString.ToString()),
                new Dictionary<string, IEnumerable<string?>?> { { _engine.GetFieldName(x => x.Artist), [] } });
        }
        else
        {
            var cachedObject = _cache.GetAsync<MusicIndexCounts>(CacheKeys.MusicIndexCount).GetAwaiter().GetResult();
            if (cachedObject != null)
            {
                model.IndexCounts = cachedObject;
            }
            else
            {
                var indexCounts = GetIndexCounts();
                _cache.SetAsync(CacheKeys.MusicIndexCount, indexCounts);
                model.IndexCounts = indexCounts;
            }
        }

        return model;
    }

    private MusicIndexCounts GetIndexCounts()
    {
        if (_engine.IndexNotExistsOrEmpty())
            return MusicIndexCounts.Empty;

        var totalFilesTask = Task.Run(() => _engine.CountDocuments(null));
        var totalFilesByExtensionTask = Task.Run(() => _engine.CountDocuments(new CounterRequest
        {
            Field = _engine.GetFieldName(x => x.Extension)
        }));
        var releaseYearsTask = Task.Run(() => _engine.CountDocuments(new CounterRequest
        {
            Field = _engine.GetFieldName(x => x.Year),
            IsNumeric = true
        }));
        var genreCountTask = Task.Run(() => _engine.CountDocuments(new CounterRequest
        {
            Field = _engine.GetFieldName(x => x.Genre)
        }));
        var latestAdditionsTask = Task.Run(() => _engine.GetLatestAddedItems(new CounterRequest
        {
            Field = _engine.GetFieldName(x => x.Release),
            AdditionalField = _engine.GetFieldName(x => x.Artist),
            SortByField = _engine.GetFieldName(x => x.ModifiedDate),
            IsNumeric = false,
            Top = 10
        }));

        Task.WhenAll(
            totalFilesTask,
            totalFilesByExtensionTask,
            releaseYearsTask,
            genreCountTask,
            latestAdditionsTask)
            .GetAwaiter()
            .GetResult();

        return new MusicIndexCounts
        {
            TotalFiles = totalFilesTask.Result.First().Value,
            TotalFilesByExtension = totalFilesByExtensionTask.Result,
            //TotalHiResFiles = FullTextSearch(
            //    searchIndexEngine.GetFieldName(x => x.Text), 
            //    "hr flac",
            //    new PaginationRequest(int.MaxValue, 0),
            //    null)
            //}).TotalHits,
            ReleaseYears = releaseYearsTask.Result,
            GenreCount = genreCountTask.Result,
            LatestAdditions = latestAdditionsTask.Result
        };
    }

    private SearchResultDto<MusicLibraryDocument> FullTextSearch(
        string field,
        string value,
        PaginationRequest paginationRequest,
        IDictionary<string, IEnumerable<string?>?>? facets = null)
    {
        var searchRequest = new SearchRequestInternal
        {
            SearchFields =
            [
                new()
                {
                    Name = field,
                    Value = value,
                    SearchType = SearchType.QueryMatch,
                    Properties = DocumentFields<MusicLibraryDocument>.GetField(field)
                }
            ],
            QueryType = QueryTypesEnum.Text,
            Pagination = paginationRequest,
            Facets = facets,
        };
        var res = _engine.Search(searchRequest);

        return res;
    }

    private SearchResultDto<MusicLibraryDocument> SingleTermSearch(
        string field,
        string value,
        PaginationRequest paginationRequest,
        IDictionary<string, IEnumerable<string?>?>? facets = null,
        bool isNumeric = false)
    {
        var searchRequest = new SearchRequestInternal
        {
            SearchFields =
            [
                new()
                {
                    Name = field,
                    Value = value,
                    SearchType = SearchType.ExactMatch
                }
            ],
            QueryType = isNumeric ? QueryTypesEnum.Numeric : QueryTypesEnum.Term,
            Pagination = paginationRequest,
            Facets = facets,
        };
        var res = _engine.Search(searchRequest);

        return res;
    }

    private SearchResultDto<MusicLibraryDocument> MultiTermSearch(
        IEnumerable<SearchField> searchFields,
        PaginationRequest paginationRequest,
        IDictionary<string, IEnumerable<string?>?>? facets = null)
    {
        var searchRequest = new SearchRequestInternal
        {
            SearchFields = searchFields,
            QueryType = QueryTypesEnum.MultiTerm,
            Pagination = paginationRequest,
            Facets = facets,
        };
        var res = _engine.Search(searchRequest);

        return res;
    }
}
