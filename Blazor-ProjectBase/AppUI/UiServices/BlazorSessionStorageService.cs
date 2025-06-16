using Domain.Enums;
using Domain.Extensions;
using Domain.Interfaces.BlazorUiInterfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace AppUI.UiServices
{
    public class BlazorSessionStorageService : IBlazorStorageService
    {
        private readonly ProtectedSessionStorage _session;
        private readonly ProtectedLocalStorage _storage;
        private readonly NavigationManager _nav;
        public BlazorSessionStorageService(ProtectedSessionStorage session,
                                           ProtectedLocalStorage storage,
                                           NavigationManager nav)
        {
            _storage = storage;
            _session = session;
            _nav = nav;
        }

        public async Task SetToProtectedStorage(ProtectedStorageVariable key, string value)
        {
            await _storage.SetAsync(key.ToString(), value.Encrypt());
        }

        public async Task<string?> GetFromProtectedStorage(ProtectedStorageVariable key)
        {
            if (string.IsNullOrEmpty(_nav.Uri))
            {
                return null;
            }

            var result = await _storage.GetAsync<string>(key.ToString());
            if (result.Success && result.Value != null)
            {
                return result.Value.Decrypt();
            }
            else
            {
                throw new InvalidDataException($"Failed to retrieve value for key: {key}");
            }
        }

        public async Task RemoveFromProtectedStorage(ProtectedStorageVariable key)
        {
            await _storage.DeleteAsync(key.ToString());
        }

        public async Task SetToProtectedSession(ProtectedSessionVariable key, string value)
        {
            await _session.SetAsync(key.ToString(), value.Encrypt());
        }

        public async Task<string?> GetFromProtectedSession(ProtectedSessionVariable key)
        {
            if (string.IsNullOrEmpty(_nav.Uri))
            {
                return null;
            }
            var result = await _session.GetAsync<string>(key.ToString());
            if (result.Success && result.Value != null)
            {
                return result.Value.Decrypt();
            }
            else
            {
                throw new InvalidDataException($"Failed to retrieve value for key: {key}");
            }
        }

        public async Task RemoveFromProtectedSession(ProtectedSessionVariable key)
        {
            await _session.DeleteAsync(key.ToString());
        }
    }
}
