using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VRC_Favourite_Manager;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

public class WorldManager
{
    private readonly VRChatAPIService _vrChatAPIService;
    private readonly JsonManager _jsonManager;
    private HashSet<WorldModel> _favoriteWorlds;
    private HashSet<string> _existingWorldIds;

    public WorldManager(VRChatAPIService vrChatAPIService, JsonManager jsonManager)
    {
        _vrChatAPIService = vrChatAPIService;
        _jsonManager = jsonManager;
        _favoriteWorlds = new HashSet<WorldModel>();
        _existingWorldIds = new HashSet<string>();

        LoadWorldsAsync();
    }

    public IEnumerable<WorldModel> GetFavoriteWorlds() => _favoriteWorlds;

    private async void LoadWorldsAsync()
    {
        if (_jsonManager.WorldConfigExists())
        {
            var worlds = _jsonManager.LoadWorlds();
            foreach (var world in worlds)
            {
                _favoriteWorlds.Add(world);
                _existingWorldIds.Add(world.WorldId);
            }
            if(worlds.Count > 0)
            {
                await CheckForNewWorldsAsync();
            }
            else
            {
                await InitialScanAsync();
            }
        }
        else
        {
            await InitialScanAsync();
        }
    }

    private async Task InitialScanAsync()
    {
        int page = 0;
        bool hasMore = true;
        while (hasMore)
        {
            var worlds = await _vrChatAPIService.GetFavoriteWorldsAsync(100, page * 100);
            foreach (var world in worlds)
            {
                _favoriteWorlds.Add(world);
            }
            if (worlds.Count < 100)
            {
                hasMore = false;
            }
            page++;
        }
        SaveWorlds();
    }

    private async Task CheckForNewWorldsAsync()
    {
        if (_existingWorldIds == null)
        {
            _existingWorldIds = new HashSet<string>();
        }

        // Add existing worlds' IDs to the set
        foreach (var world in _favoriteWorlds)
        {
            _existingWorldIds.Add(world.WorldId);
        }

        var worlds = await _vrChatAPIService.GetFavoriteWorldsAsync(100, 0);
        foreach (var world in worlds)
        {
            if (!_existingWorldIds.Contains(world.WorldId))
            {
                _favoriteWorlds.Add(world);
                _existingWorldIds.Add(world.WorldId);
            }
        }
        SaveWorlds();
    }

    public void SaveWorlds()
    {
        _jsonManager.SaveWorlds(_favoriteWorlds);
    }

    public void RemoveWorld(WorldModel world)
    {
        if (_favoriteWorlds.Contains(world))
        {
            _favoriteWorlds.Remove(world);
            _existingWorldIds.Remove(world.WorldId);
            SaveWorlds();
        }
    }
}
