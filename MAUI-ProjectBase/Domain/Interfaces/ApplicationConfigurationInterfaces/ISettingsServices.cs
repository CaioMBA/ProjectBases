﻿using Domain.Entities;
using Domain.Models.ApplicationConfigurationModels;
namespace Domain.Interfaces.ApplicationConfigurationInterfaces
{
    public interface ISettingsServices
    {
        void AddLog(string log);
        List<LogEntity> logEntities();

        AppLanguageModel _currentLanguage { get; }
        event Action? OnLanguageChanged;
        List<AppLanguageModel> AvailableLanguages();
        void ChangeLanguage(string languageCode);

        AppSkinModel _currentSkin { get; }
        event Action? OnSkinChanged;
        List<AppSkinModel> AvailableSkins();
        void ChangeSkin(string skin);
    }
}
